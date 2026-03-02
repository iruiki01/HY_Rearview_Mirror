using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hWindowTool
{
    /// <summary>
    /// 圆弧数据
    /// </summary>
    [Serializable]
    public class CircleArcData : HWROI
    {
        /// <summary>
        /// 圆弧数据构造函数
        /// </summary>
        public CircleArcData(ROIData rOIData) : base(rOIData) { }

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
