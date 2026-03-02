using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hWindowTool
{
    /// <summary>
    /// 椭圆数据
    /// </summary>
    [Serializable]
    public class EllipseData : HWROI
    {
        /// <summary>
        /// 椭圆数据构造函数
        /// </summary>
        /// <param name="rOIData">ROI数据</param>
        public EllipseData(ROIData rOIData) : base(rOIData) { }

        /// <summary>
        /// 中心Row
        /// </summary>
        public double Row = 50;

        /// <summary>
        /// 中心Column
        /// </summary>
        public double Column = 50;

        /// <summary>
        /// 弧度(注意:不是角度)
        /// </summary>
        public double Phi;

        /// <summary>
        /// 长度1
        /// </summary>
        public double Length1 = 50;

        /// <summary>
        /// 长度2
        /// </summary>
        public double Length2 = 50;

        /// <summary>
        /// 角点1坐标
        /// </summary>
        public double AngleRow1, AngleColumn1;

        /// <summary>
        /// 角点2坐标
        /// </summary>
        public double AngleRow2, AngleColumn2;

        /// <summary>
        /// 角点3坐标
        /// </summary>
        public double AngleRow3, AngleColumn3;

        /// <summary>
        /// 角点4坐标
        /// </summary>
        public double AngleRow4, AngleColumn4;
    }
}
