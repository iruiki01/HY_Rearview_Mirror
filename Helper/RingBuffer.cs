using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    /// <summary>
    /// 环形缓冲区
    /// </summary>
    public class RingBuffer<T>
    {
        private T[] buffer;
        private int readIndex;
        private int writeIndex;
        private int count;

        public RingBuffer(int size)
        {
            buffer = new T[size];
            readIndex = 0;
            writeIndex = 0;
            count = 0;
        }

        public int Count => count;
        public bool IsFull => count == buffer.Length;
        public bool IsEmpty => count == 0;
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            if (IsFull)
            {
                throw new InvalidOperationException("Buffer is full.");
            }
            buffer[writeIndex] = item;
            writeIndex = (writeIndex + 1) % buffer.Length;
            count++;
        }
        /// <summary>
        /// 取出数据
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Buffer is empty.");
            }
            T item = buffer[readIndex];
            readIndex = (readIndex + 1) % buffer.Length;
            count--;
            return item;
        }
        /// <summary>
        /// 查看头部元素
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Buffer is empty.");
            }
            return buffer[readIndex];
        }
    }
}
