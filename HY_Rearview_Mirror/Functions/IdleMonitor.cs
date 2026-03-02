using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HY_Rearview_Mirror.Functions
{
    /// <summary>
    /// 监控系统空闲时间（用户无操作时长）
    /// 基于Windows GetLastInputInfo API实现
    /// </summary>
    public static class IdleMonitor
    {
        #region Windows API 导入

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        private struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        #endregion

        /// <summary>
        /// 获取当前会话的空闲时间
        /// </summary>
        /// <returns>用户自上次输入以来经过的时间</returns>
        /// <exception cref="Win32Exception">当调用Windows API失败时抛出</exception>
        public static TimeSpan GetIdleTime()
        {
            var info = new LASTINPUTINFO
            {
                // 设置结构体大小（必须）
                cbSize = (uint)Marshal.SizeOf(typeof(LASTINPUTINFO))
            };

            // 调用Windows API并检查返回值
            if (!GetLastInputInfo(ref info))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(),
                    "无法获取最后输入信息");
            }

            // 计算空闲时间（Environment.TickCount会溢出，但差值仍然有效）
            unchecked
            {
                uint idleMilliseconds = (uint)Environment.TickCount - info.dwTime;
                return TimeSpan.FromMilliseconds(idleMilliseconds);
            }
        }

        /// <summary>
        /// 检查用户是否空闲超过指定时长
        /// </summary>
        /// <param name="threshold">空闲时间阈值</param>
        /// <returns>如果空闲时间超过阈值返回true</returns>
        public static bool IsIdleFor(TimeSpan threshold)
        {
            try
            {
                return GetIdleTime() >= threshold;
            }
            catch (Win32Exception Info) 
            {
                return false;
            }
            
        }

        /// <summary>
        /// 获取空闲时间并格式化为易读字符串
        /// </summary>
        public static string GetIdleTimeString()
        {
            try
            {
                var idle = GetIdleTime();
                return idle.Days > 0
                    ? $"{idle.Days}天 {idle:hh\\:mm\\:ss}"
                    : idle.ToString(@"hh\:mm\:ss");
            }
            catch (Win32Exception Info)
            {
               return null;
            }         
        }
    }
}
