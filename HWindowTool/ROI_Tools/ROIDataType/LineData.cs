using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalconControl.ROIDataType
{
    /// <summary>
    /// 直线数据
    /// </summary>
    [Serializable]
    internal class LineData
    {
        /// <summary>
        /// 起始ROW
        /// </summary>
        public double Row1 = 50;

        /// <summary>
        /// 起始Column1
        /// </summary>
        public double Column1 = 0;

        /// <summary>
        /// 结束Row2
        /// </summary>
        public double Row2 = 50;

        /// <summary>
        /// 结束Column2
        /// </summary>
        public double Column2 = 100;
    }
}
