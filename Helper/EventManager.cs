using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    /// <summary>
    /// 泛型事件管理器 - 通过名称管理任意签名的事件
    /// </summary>
    public class EventManager
    {
        private readonly Dictionary<string, Delegate> _eventTable = new Dictionary<string, Delegate>();
        private readonly object _lock = new object();

        /// <summary>
        /// 添加事件监听器
        /// </summary>
        public void AddListener(string eventName, Delegate callback)
        {
            lock (_lock)
            {
                if (_eventTable.ContainsKey(eventName))
                {
                    _eventTable[eventName] = Delegate.Combine(_eventTable[eventName], callback);
                }
                else
                {
                    _eventTable[eventName] = callback;
                }
            }
        }

        /// <summary>
        /// 移除事件监听器
        /// </summary>
        public void RemoveListener(string eventName, Delegate callback)
        {
            lock (_lock)
            {
                if (!_eventTable.ContainsKey(eventName)) return;

                _eventTable[eventName] = Delegate.Remove(_eventTable[eventName], callback);

                if (_eventTable[eventName] is null)
                {
                    _eventTable.Remove(eventName);
                }
            }
        }

        /// <summary>
        /// 触发事件 (无返回值)
        /// </summary>
        public void Trigger(string eventName, params object[] args)
        {
            Delegate action;
            lock (_lock)
            {
                if (!_eventTable.TryGetValue(eventName, out action)) return;
            }
            action.DynamicInvoke(args);
        }

        /// <summary>
        /// 触发事件并获取返回值
        /// </summary>
        public object TriggerWithResult(string eventName, params object[] args)
        {
            Delegate action;
            lock (_lock)
            {
                if (!_eventTable.TryGetValue(eventName, out action)) return null;
            }
            return action.DynamicInvoke(args);
        }

        /// <summary>
        /// 获取事件订阅者数量
        /// </summary>
        public int GetListenerCount(string eventName)
        {
            lock (_lock)
            {
                if (!_eventTable.TryGetValue(eventName, out var action)) return 0;
                return action.GetInvocationList().Length;
            }
        }

        /// <summary>
        /// 移除所有监听
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _eventTable.Clear();
            }
        }

        /// <summary>
        /// 移除指定事件的所有监听
        /// </summary>
        public void Clear(string eventName)
        {
            lock (_lock)
            {
                _eventTable.Remove(eventName);
            }
        }
    }
}
