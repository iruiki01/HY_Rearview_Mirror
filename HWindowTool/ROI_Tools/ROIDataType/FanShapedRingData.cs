using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hWindowTool
{
    /// <summary>
    /// 扇形环数据
    /// </summary>
    [Serializable]
    public class FanShapedRingData : HWROI
    {
        /// <summary>
        /// 扇形环数据构造函数
        /// </summary>
        /// <param name="rOIData">ROI数据</param>
        public FanShapedRingData(ROIData rOIData) : base(rOIData) { }

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

        /// <summary>
        /// 起始角度(弧度)
        /// </summary>
        private double startAngle = 0;
        /// <summary>
        /// 起始角度(弧度)
        /// </summary>
        public double StartAngle
        {
            get { return startAngle; }
            set
            {
                if (value < 0)
                {
                    startAngle = value + 6.28318;
                }
                else if (value > 6.28318)
                {
                    startAngle = value - 6.28318;
                }
                else
                {
                    startAngle = value;
                }
            }
        }

        /// <summary>
        /// 结束角度(弧度)
        /// </summary>
        private double endAngle = 3.14159;
        /// <summary>
        /// 结束角度(弧度)
        /// </summary>
        public double EndAngle
        {
            get { return endAngle; }
            set
            {
                if (value < 0)
                {
                    endAngle = value + 6.28318;
                }
                else if (value > 6.28318)
                {
                    endAngle = value - 6.28318;
                }
                else
                {
                    endAngle = value;
                }
            }
        }
    }
}
