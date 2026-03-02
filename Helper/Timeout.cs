using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class Timeout
    {
        public Stopwatch stopwatch = new Stopwatch(); // 开始计时   
        public void StartTimedOut()
        {
            stopwatch = Stopwatch.StartNew(); // 开始计时   
        }
        public bool IsTimedOut(int timeout)
        {
            if ((stopwatch.ElapsedMilliseconds < timeout))
            {
                return false;
            }
            else
            {
                stopwatch.Stop();
                return true;
            }
        }
    }
}
