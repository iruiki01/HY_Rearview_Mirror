using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    /// <summary>
    /// 消息状态
    /// </summary>
    public enum MessageStatus
    {
        Info,
        Warn,
        Error,
        Prompt
    }

    public class WriteLog
    {
        public ConcurrentQueue<string> ringBuffer = new ConcurrentQueue<string>();
        private object LockObject = new object();

        LogLib logLib = null;
        public WriteLog(string _folderPath, string _FileName)
        {
            logLib = new LogLib(_folderPath, _FileName);
            logLib.Create();
        }
        
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="slog"></param>
        /// <param name="type"></param>
         public void log(string slog, int type = 0)
        {
            lock (LockObject)
            {
                string logstr = "";
                switch (type)
                {
                    case 0:
                        logstr = "Info< " + DateTime.Now.ToString("HH:mm:ss") + " " + slog;
                        break;
                    case 1:
                        logstr = "Warn< " + DateTime.Now.ToString("HH:mm:ss") + " " + slog;
                        break;
                    case 2:
                        logstr = "Error< " + DateTime.Now.ToString("HH:mm:ss") + " " + slog;
                        break;
                    case 3:
                        logstr = "Prompt< " + DateTime.Now.ToString("HH:mm:ss") + " " + slog;
                        break;
                    default:
                        logstr = "Info< " + DateTime.Now.ToString("HH:mm:ss") + " " + slog;
                        break;
                }
                try
                {
                    ringBuffer.Enqueue(logstr);
                }
                catch { }


                if (!logstr.Contains("Prompt"))
                {
                    Task.Run(() => logLib.WriteLog(logstr));
                }
            }
        }
    }
    internal class LogLib
    {
        #region common define
        private static string APPLOGINFO = "<Application Start>";
        private static string APPLOGCLOSE = "<Application Close>";
        private static string APPLOGSEPARATOR = "||";
        private static string APPLOGDEBUG = " <BUG> ";
        private static string APPLOGFOOTER = "--------------------------------------------------";
        #endregion

        public readonly string folderPath = "";
        public readonly string FileName = "";
        public LogLib(string _folderPath, string _FileName)
        {
            folderPath = _folderPath;
            FileName = _FileName;
        }
        private string _fileName = "";
        private Dictionary<long, long> lockDic = new Dictionary<long, long>();
        /// <summary>  
        /// 创建文件  
        /// </summary>  
        /// <param name="fileName"></param>  
        public void Create()
        {
            string fileName = @"D:\" + folderPath + @"\"+ FileName + System.DateTime.Today.Date.ToString("yyyy-MM-dd") + @".log";
            FileCopyHelper.DirectoryCreate(@"D:\" + folderPath);
            _fileName = fileName;
            if (!System.IO.File.Exists(fileName))
            {
                using (System.IO.FileStream fs = System.IO.File.Create(fileName))
                {
                    fs.Close();
                }
            }
        }
        /// <summary>  
        /// 写入文本  
        /// </summary>  
        /// <param name="content">文本内容</param>  
         private void Write(string content, string newLine)
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                Create();
            }
            using (System.IO.FileStream fs = new System.IO.FileStream(_fileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite, 8, System.IO.FileOptions.Asynchronous))
            {
                Byte[] dataArray = System.Text.Encoding.Default.GetBytes(content + newLine);
                bool flag = true;
                long slen = dataArray.Length;
                long len = 0;
                while (flag)
                {
                    try
                    {
                        if (len >= fs.Length)
                        {
                            fs.Lock(len, slen);
                            lockDic[len] = slen;
                            flag = false;
                        }
                        else
                        {
                            len = fs.Length;
                        }
                    }
                    catch
                    {
                        //while (!lockDic.ContainsKey(len))
                        //{
                        //    len += lockDic[len];
                        //}
                    }
                }
                fs.Seek(len, System.IO.SeekOrigin.Begin);
                fs.Write(dataArray, 0, dataArray.Length);
                fs.Close();
            }
        }
        /// <summary>  
        /// 写入文件内容  
        /// </summary>  
        /// <param name="content"></param>  
         private void WriteLine(string content)
        {
            Write(content, System.Environment.NewLine);
        }
        //写log string
         public void WriteLog(string str, int msgLevel = 0)
        {
            str = str + "\r\n";
            WriteLine("[" + System.DateTime.Now + "]" + APPLOGSEPARATOR + str);
        }
    }
}
