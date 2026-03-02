using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hWindowTool
{
    /// <summary>
    /// 圆环数据
    /// </summary>
    [Serializable]
    public class RingData : HWROI
    {
        /// <summary>
        /// 圆环数据构造函数
        /// </summary>
        /// <param name="rOIData">ROI数据</param>
        public RingData(ROIData rOIData) : base(rOIData) { }

        /// <summary>
        /// 圆心Row
        /// </summary>
        public double Row = 50;

        /// <summary>
        /// 圆心Column
        /// </summary>
        public double Column = 50;

        /// <summary>
        /// 圆半径1
        /// </summary>
        public double Radius1 = 25;

        /// <summary>
        /// 圆半径2
        /// </summary>
        public double Radius2 = 50;
    }
}
