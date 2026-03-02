using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    // 1. 通用线程类（接受动态委托）
    public class DynamicThreadData : IDisposable
    {
        public volatile bool StartThreadFlag = false;
        public Thread thread = null;
        private Action _handler;  // 关键：存储动态传入的逻辑

        public DynamicThreadData(Action handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            thread = new Thread(new ThreadStart(run));
            thread.IsBackground = true;
        }

        public void run()
        {
            while (StartThreadFlag)
            {
                _handler.Invoke();  // 执行动态委托
            }
        }

        public void Dispose() => StartThreadFlag = false;
    }

    // 2. 动态工厂
    public class DynamicThreadFactory
    {
        private List<DynamicThreadData> threads = new List<DynamicThreadData>();

        // 关键方法：动态添加不同逻辑的线程
        public void AddThread(Action handler)
        {
            threads.Add(new DynamicThreadData(handler));            
        }

        public void Start()
        {
            threads.ForEach(t => { t.StartThreadFlag = true; t.thread.Start(); });
        }

        public void Stop()
        {
            threads.ForEach(t => t.Dispose());
            threads.ForEach(t => t.thread.Join(1000));
        }
    }
}
