using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    /// <summary>
    /// 多线程状态机基类，每个状态由独立线程处理
    /// </summary>
    public abstract class ThreadedStateMachine : IDisposable
    {
        private readonly Dictionary<string, Thread> _stateThreads = new Dictionary<string, Thread>();
        private readonly Dictionary<string, CancellationTokenSource> _stateCancellationSources = new Dictionary<string, CancellationTokenSource>();
        private readonly Dictionary<string, ManualResetEvent> _statePauseEvents = new Dictionary<string, ManualResetEvent>();
        private readonly object _lock = new object();
        private bool _isDisposed;

        /// <summary>
        /// 获取状态机是否正在运行
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// 注册状态处理方法
        /// </summary>
        /// <param name="stateStr">状态标识符</param>
        /// <param name="stateProcessor">状态处理方法</param>
        protected void RegisterStateProcessor(string stateStr, Action<CancellationToken, ManualResetEvent> stateProcessor)
        {
            lock (_lock)
            {
                if (_stateThreads.ContainsKey(stateStr))
                {
                    throw new ArgumentException($"状态 {stateStr} 的处理方法已注册");
                }

                var cts = new CancellationTokenSource();
                var pauseEvent = new ManualResetEvent(true); // 初始为已启动状态

                // 创建线程来处理状态
                var thread = new Thread(() =>
                {
                    try
                    {
                        stateProcessor(cts.Token, pauseEvent);
                    }
                    catch (OperationCanceledException)
                    {
                        // 正常取消操作
                    }
                    catch (ThreadAbortException)
                    {
                        // 线程被中止
                        Thread.ResetAbort();
                    }
                    catch (Exception ex)
                    {
                        OnStateMachineError(ex, stateStr);
                    }
                })
                {
                    IsBackground = true,
                    Name = $"State_{stateStr}_Thread"
                };

                _stateThreads[stateStr] = thread;
                _stateCancellationSources[stateStr] = cts;
                _statePauseEvents[stateStr] = pauseEvent;
            }
        }

        /// <summary>
        /// 启动状态机
        /// </summary>
        public virtual void Start()
        {
            if (IsRunning)
                return;

            lock (_lock)
            {
                foreach (var thread in _stateThreads.Values)
                {
                    thread.Start();
                }
                IsRunning = true;
            }

            Console.WriteLine("状态机已启动");
        }

        /// <summary>
        /// 停止状态机
        /// </summary>
        public virtual void Stop()
        {
            if (!IsRunning)
                return;

            lock (_lock)
            {
                IsRunning = false;
            }

            // 取消所有状态任务
            foreach (var cts in _stateCancellationSources.Values)
            {
                cts.Cancel();
            }

            // 恢复所有暂停的线程，以便它们可以退出
            foreach (var pauseEvent in _statePauseEvents.Values)
            {
                pauseEvent.Set();
            }

            // 等待所有线程结束
            foreach (var thread in _stateThreads.Values)
            {
                if (thread.IsAlive)
                {
                    thread.Join(TimeSpan.FromSeconds(5));

                    // 如果线程仍然存活，尝试中止它
                    if (thread.IsAlive)
                    {
                        try
                        {
                            thread.Abort();
                        }
                        catch (PlatformNotSupportedException)
                        {
                            // .NET Core 不支持 Thread.Abort()
                            Console.WriteLine("警告: 平台不支持线程中止操作");
                        }
                    }
                }
            }

            Console.WriteLine("状态机已停止");
        }

        /// <summary>
        /// 暂停指定状态的处理
        /// </summary>
        /// <param name="stateStr">要暂停的状态</param>
        public virtual void PauseState(string stateStr)
        {
            lock (_lock)
            {
                if (_statePauseEvents.TryGetValue(stateStr, out var pauseEvent))
                {
                    pauseEvent.Reset();
                    Console.WriteLine($"状态 {stateStr} 已暂停");
                }
                else
                {
                    Console.WriteLine($"未找到状态 {stateStr}");
                }
            }
        }

        /// <summary>
        /// 恢复指定状态的处理
        /// </summary>
        /// <param name="stateStr">要恢复的状态</param>
        public virtual void ResumeState(string stateStr)
        {
            lock (_lock)
            {
                if (_statePauseEvents.TryGetValue(stateStr, out var pauseEvent))
                {
                    pauseEvent.Set();
                    Console.WriteLine($"状态 {stateStr} 已恢复");
                }
                else
                {
                    Console.WriteLine($"未找到状态 {stateStr}");
                }
            }
        }

        /// <summary>
        /// 状态机错误处理
        /// </summary>
        /// <param name="exception">异常信息</param>
        /// <param name="stateStr">发生错误时的状态</param>
        protected virtual void OnStateMachineError(Exception exception, string stateStr)
        {
            Console.WriteLine($"状态机错误 (状态 {stateStr}): {exception.Message}");
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // 停止状态机并清理资源
                    Stop();

                    foreach (var cts in _stateCancellationSources.Values)
                    {
                        cts.Dispose();
                    }

                    foreach (var pauseEvent in _statePauseEvents.Values)
                    {
                        pauseEvent.Dispose();
                    }

                    _stateThreads.Clear();
                    _stateCancellationSources.Clear();
                    _statePauseEvents.Clear();
                }

                _isDisposed = true;
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~ThreadedStateMachine()
        {
            Dispose();
        }
    }
}
