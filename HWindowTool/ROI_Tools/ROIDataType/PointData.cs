using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hWindowTool
{
    /// <summary>
    /// 点数据
    /// </summary>
    [Serializable]
    public class PointData : HWROI
    {
        /// <summary>
        /// 点数据构造函数
        /// </summary>
        /// <param name="rOIData">ROI数据</param>
        public PointData(ROIData rOIData) : base(rOIData) { }

        /// <summary>
        /// 点Row
        /// </summary>
        public double Row = 50;

        /// <summary>
        /// 点Column
        /// </summary>
        public double Column = 50;
    }
}
