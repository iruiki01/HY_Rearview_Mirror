using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hWindowTool
{
    /// <summary>
    /// 圆数据
    /// </summary>
    [Serializable]
    public class CircleData : HWROI
    {
        /// <summary>
        /// 圆数据构造函数
        /// </summary>
        /// <param name="rOIData">ROI数据</param>
        public CircleData(ROIData rOIData) : base(rOIData) { }

        /// <summary>
        /// 圆心Row
        /// </summary>
        public double Row = 50;

        /// <summary>
        /// 圆心Column
        /// </summary>
        public double Column = 50;

        /// <summary>
        /// 圆半径
        /// </summary>
        public double Radius = 50;
    }
}
