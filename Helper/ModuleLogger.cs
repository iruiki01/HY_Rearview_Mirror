using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dataGridView1每行改变颜色
{
    public class TrueAsyncRichTextBoxSink : ILogEventSink, IDisposable
    {
        private RichTextBox _rtb;
        private readonly ConcurrentQueue<LogEntry> _queue = new ConcurrentQueue<LogEntry>();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private Task _processorTask;

        private const int MAX_LINES = 1000;
        private const int CLEANUP_COUNT = 200;
        private const int BATCH_SIZE = 200;
        private const int FLUSH_MS = 100;

        private struct LogEntry
        {
            public string Text;
            public LogEventLevel Level;
            public DateTime Time;
        }

        public TrueAsyncRichTextBoxSink(RichTextBox richTextBox)
        {
            if (richTextBox != null)
            {
                Initialize(richTextBox);
            }
        }

        public void UpdateRichTextBox(RichTextBox richTextBox)
        {
            // 安全切换 RichTextBox
            if (_processorTask != null)
            {
                _cts.Cancel();
                try { _processorTask.Wait(1000); } catch { }
            }

            if (richTextBox != null && !richTextBox.IsDisposed)
            {
                Initialize(richTextBox);
            }
        }

        private void Initialize(RichTextBox richTextBox)
        {
            _rtb = richTextBox;
            EnableDoubleBuffer(_rtb);
            _rtb.HideSelection = false;

            _processorTask = Task.Factory.StartNew(
                ProcessLoop,
                _cts.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        private void EnableDoubleBuffer(Control ctrl)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.SetProperty,
                null, ctrl, new object[] { true });
        }

        public void Emit(LogEvent logEvent)
        {
            if (_rtb == null || _rtb.IsDisposed) return;

            _queue.Enqueue(new LogEntry
            {
                Text = logEvent.RenderMessage(),
                Level = logEvent.Level,
                Time = DateTime.Now
            });
        }

        private async Task ProcessLoop()
        {
            var batch = new List<LogEntry>(BATCH_SIZE);
            var lastFlush = DateTime.Now;

            while (!_cts.Token.IsCancellationRequested)
            {
                while (batch.Count < BATCH_SIZE && _queue.TryDequeue(out var entry))
                    batch.Add(entry);

                var timeElapsed = DateTime.Now - lastFlush;

                if (batch.Count >= BATCH_SIZE ||
                    (batch.Count > 0 && timeElapsed.TotalMilliseconds >= FLUSH_MS))
                {
                    var toSend = batch.ToList();
                    batch.Clear();
                    lastFlush = DateTime.Now;

                    if (_rtb != null && !_rtb.IsDisposed)
                    {
                        try
                        {
                            _rtb.BeginInvoke(new Action(() => RenderBatch(toSend)));
                        }
                        catch { /* 防止窗体关闭时异常 */ }
                    }
                }
                else if (batch.Count == 0)
                {
                    await Task.Delay(5, _cts.Token);
                }
            }
        }

        private void RenderBatch(List<LogEntry> entries)
        {
            if (_rtb == null || _rtb.IsDisposed || entries.Count == 0) return;

            SendMessage(_rtb.Handle, WM_SETREDRAW, (IntPtr)0, (IntPtr)0);
            try
            {
                int currentLines = _rtb.Lines.Length;
                if (currentLines + entries.Count > MAX_LINES)
                {
                    FastCleanup(currentLines);
                }

                string rtfContent = BuildColoredRtf(entries);
                _rtb.SelectionStart = _rtb.TextLength;
                _rtb.SelectionLength = 0;
                _rtb.SelectedRtf = rtfContent;

                if (_rtb.SelectionStart >= _rtb.TextLength - 100)
                {
                    _rtb.ScrollToCaret();
                }
            }
            finally
            {
                SendMessage(_rtb.Handle, WM_SETREDRAW, (IntPtr)1, (IntPtr)0);
                _rtb.Invalidate();
            }
        }

        private void FastCleanup(int currentLines)
        {
            Font originalFont = _rtb.Font;
            Color originalColor = _rtb.ForeColor;
            int keep = MAX_LINES - CLEANUP_COUNT;
            var lines = _rtb.Lines;

            if (lines.Length > keep)
            {
                var newLines = lines.Skip(lines.Length - keep).ToArray();
                _rtb.Clear();
                _rtb.Lines = newLines;
                _rtb.SelectAll();
                _rtb.SelectionFont = originalFont;
                _rtb.SelectionColor = originalColor;
                _rtb.Select(_rtb.TextLength, 0);
                InsertMarker($"... 已清理旧日志 ({CLEANUP_COUNT}条) ...");
            }
        }

        private void InsertMarker(string text)
        {
            int oldStart = _rtb.SelectionStart;
            _rtb.SelectionStart = 0;
            _rtb.SelectionLength = 0;
            _rtb.SelectedText = text + Environment.NewLine;
            _rtb.Select(0, text.Length);
            _rtb.SelectionColor = Color.Gray;
            _rtb.SelectionFont = new Font(_rtb.Font, FontStyle.Italic);
            _rtb.SelectionStart = oldStart + text.Length + 1;
        }

        private string BuildColoredRtf(List<LogEntry> entries)
        {
            var sb = new StringBuilder();
            string fontName = _rtb.Font.Name;
            int fontSizeHalfPoints = (int)(_rtb.Font.Size * 2);

            sb.Append(@"{\rtf1\ansi\ansicpg936\deff0\deflang1033\deflangfe2052");
            sb.Append(@"{\fonttbl{\f0\fnil\fcharset134 " + fontName + @";}}");
            sb.Append(@"{\colortbl ;");
            sb.Append(@"\red128\green128\blue128;");
            sb.Append(@"\red0\green0\blue0;");
            sb.Append(@"\red255\green140\blue0;");
            sb.Append(@"\red255\green0\blue0;");
            sb.Append(@"\red139\green0\blue0;");
            sb.Append(@"}");
            sb.Append(@"\f0\fs" + fontSizeHalfPoints + " ");

            foreach (var entry in entries)
            {
                int cf;
                switch (entry.Level)
                {
                    case LogEventLevel.Verbose:
                    case LogEventLevel.Debug: cf = 1; break;
                    case LogEventLevel.Warning: cf = 3; break;
                    case LogEventLevel.Error: cf = 4; break;
                    case LogEventLevel.Fatal: cf = 5; break;
                    default: cf = 2; break;
                }

                bool bold = entry.Level >= LogEventLevel.Error;
                string time = entry.Time.ToString("HH:mm:ss");
                string level = entry.Level.ToString().Substring(0, Math.Min(3, entry.Level.ToString().Length)).ToUpper();
                string msg = entry.Text.Replace("\\", "\\\\").Replace("{", "\\{").Replace("}", "\\}")
                    .Replace("\n", "\\line ").Replace("\r", "");

                sb.Append("\\cf" + cf);
                if (bold) sb.Append("\\b");
                sb.Append("\\fs" + fontSizeHalfPoints + " ");
                sb.Append("[" + time + "] [" + level + "] " + msg + "\\par");
                if (bold) sb.Append("\\b0");
            }
            sb.Append(@"}");
            return sb.ToString();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        private const int WM_SETREDRAW = 0x0B;

        public void Dispose()
        {
            _cts.Cancel();
            try { _processorTask?.Wait(500); } catch { }
            _cts.Dispose();
        }
    }

    public class ModuleLogger : IDisposable
    {
        private Logger _logger;
        private readonly string _moduleName;
        private readonly string _logDirectory;
        private readonly int _daysToRetain;
        private readonly System.Threading.Timer _cleanupTimer;
        private RichTextBox _currentRichTextBox;
        private bool _uiAttached = false;

        /// <summary>
        /// 创建模块日志记录器
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="uiTarget">可选：RichTextBox，如为 null 可后续调用 AttachRichTextBox 附加</param>
        /// <param name="daysToRetain">保留天数（默认30天，0或负数表示不清理）</param>
        /// <param name="enablePeriodicCleanup">是否每6小时自动清理一次</param>
        public ModuleLogger(string fileName,string moduleName, RichTextBox uiTarget = null, int daysToRetain = 30, bool enablePeriodicCleanup = false)
        {
            _moduleName = moduleName;
            _daysToRetain = daysToRetain;
            _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            _currentRichTextBox = uiTarget;

            Directory.CreateDirectory(_logDirectory);

            // 初始化（带或不带 UI）
            RebuildLogger(uiTarget);

            // 启动清理逻辑
            if (daysToRetain > 0)
            {
                Task.Run(() => ExecuteCleanup());
                if (enablePeriodicCleanup)
                {
                    _cleanupTimer = new System.Threading.Timer(state => ExecuteCleanup(), null,
                        TimeSpan.FromHours(6), TimeSpan.FromHours(6));
                }
            }
        }

        /// <summary>
        /// 动态附加 RichTextBox（可在 Form 初始化后调用，支持重复调用切换控件）
        /// </summary>
        public void AttachRichTextBox(RichTextBox richTextBox)
        {
            if (richTextBox == null || richTextBox.IsDisposed) return;

            // 避免重复附加相同控件
            if (_uiAttached && _currentRichTextBox == richTextBox) return;

            _currentRichTextBox = richTextBox;
            _uiAttached = true;

            // 重建 Logger 以包含新的 UI Sink
            RebuildLogger(richTextBox);

           // _logger?.Information("UI 日志输出已附加到窗体");
        }

        /// <summary>
        /// 分离 RichTextBox（切换回仅文件日志）
        /// </summary>
        public void DetachRichTextBox()
        {
            if (!_uiAttached) return;
            _currentRichTextBox = null;
            _uiAttached = false;
            RebuildLogger(null);
        }

        private void RebuildLogger(RichTextBox uiTarget)
        {
            // 安全释放旧 Logger（会刷新文件缓冲）
            var oldLogger = _logger;

            var path = Path.Combine(_logDirectory, $"{_moduleName}-.txt");
            var config = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                    path,
                    rollingInterval: RollingInterval.Day,
                    shared: true,  // 允许共享文件句柄（重建时重要）
                    retainedFileCountLimit: null,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message}{NewLine}{Exception}"
                );

            if (uiTarget != null && !uiTarget.IsDisposed)
            {
                config.WriteTo.Sink(new TrueAsyncRichTextBoxSink(uiTarget));
            }

            _logger = config.CreateLogger();

            // 延迟释放旧的，确保所有日志已写入
            Task.Run(() =>
            {
                try { oldLogger?.Dispose(); } catch { }
            });
        }

        private void ExecuteCleanup()
        {
            try
            {
                if (!Directory.Exists(_logDirectory)) return;
                var cutoffDate = DateTime.Now.AddDays(-_daysToRetain);
                int deletedCount = 0;

                var searchPattern = $"{_moduleName}-*.txt";
                var logFiles = Directory.GetFiles(_logDirectory, searchPattern, SearchOption.TopDirectoryOnly);

                foreach (var filePath in logFiles)
                {
                    try
                    {
                        var fileName = Path.GetFileName(filePath);
                        DateTime fileDate = DateTime.MinValue;
                        bool shouldDelete = false;

                        // 尝试从文件名解析日期（Serilog 格式：Name-20260128.txt）
                        var datePart = fileName.Replace($"{_moduleName}-", "").Replace(".txt", "");

                        if (datePart.Length == 8 && long.TryParse(datePart, out _))
                        {
                            if (DateTime.TryParseExact(datePart, "yyyyMMdd",
                                System.Globalization.CultureInfo.InvariantCulture,
                                System.Globalization.DateTimeStyles.None, out fileDate))
                            {
                                if (fileDate < cutoffDate) shouldDelete = true;
                            }
                        }
                        else
                        {
                            // 备用：使用文件修改时间
                            var fileInfo = new FileInfo(filePath);
                            if (fileInfo.LastWriteTime < cutoffDate)
                            {
                                shouldDelete = true;
                                fileDate = fileInfo.LastWriteTime;
                            }
                        }

                        if (shouldDelete)
                        {
                            File.Delete(filePath);
                            deletedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[日志清理] 删除失败 {filePath}: {ex.Message}");
                    }
                }

                if (deletedCount > 0)
                {
                    _logger?.Information("自动清理完成，已删除 {DeletedCount} 个过期日志文件（保留最近 {RetainDays} 天）",
                        deletedCount, _daysToRetain);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[日志清理] 清理过程出错: {ex.Message}");
            }
        }

        // 日志写入方法
        public void Debug(string message) => _logger.Debug(message);
        public void Info(string message) => _logger.Information(message);
        public void Info(string fmt, params object[] args) => _logger.Information(fmt, args);
        public void Warn(string message) => _logger.Warning(message);
        public void Error(string message) => _logger.Error(message);
        public void Error(Exception ex, string msg) => _logger.Error(ex, msg);
        public void Fatal(string message) => _logger.Fatal(message);

        public void Dispose()
        {
            _cleanupTimer?.Dispose();
            _logger?.Dispose();
        }
    }
}