using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    public class Delay
    {
        public Stopwatch stopwatch = new Stopwatch();
        public bool SleepForSeconds(double seconds)
        {
            stopwatch = Stopwatch.StartNew(); // 开始计时
            while (stopwatch.ElapsedMilliseconds < seconds)
            {
                //执行耗时
            }
            stopwatch.Stop();
            return true;
        }
    }
}
