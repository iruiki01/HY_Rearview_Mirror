using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hWindowTool
{
    /// <summary>
    /// 二维直线数据
    /// </summary>
    [Serializable]
    public class Line2Data : HWROI
    {
        /// <summary>
        /// 二维直线数据构造函数
        /// </summary>
        /// <param name="rOIData">ROI数据</param>
        public Line2Data(ROIData rOIData) : base(rOIData) { }

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

        /// <summary>
        /// 宽度
        /// </summary>
        public double Width = 10;

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

        #region 获取ROI角点
        /// <summary>
        /// 获取ROI角度
        /// </summary>
        /// <param name="row">角点roe</param>
        /// <param name="col">角点col</param>
        public void GetROIAnglePoint(out HTuple row, out HTuple col)
        {
            row = new HTuple(AngleRow1, AngleRow2, AngleRow3, AngleRow4);
            col = new HTuple(AngleColumn1, AngleColumn2, AngleColumn3, AngleColumn4);
        }
        #endregion
    }
}
