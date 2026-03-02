using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hWindowTool
{
    /// <summary>
    /// 矩形1数据
    /// </summary>
    [Serializable]
    public class Rectangle1Data : HWROI
    {
        /// <summary>
        /// 矩形1数据构造函数
        /// </summary>
        /// <param name="rOIData">ROI数据</param>
        public Rectangle1Data(ROIData rOIData) : base(rOIData) { }

        /// <summary>
        /// 左上角ROW
        /// </summary>
        public double Row1;

        /// <summary>
        /// 左上角Column1
        /// </summary>
        public double Column1;

        /// <summary>
        /// 右下角Row2
        /// </summary>
        public double Row2 = 100;

        /// <summary>
        /// 右下角Column2
        /// </summary>
        public double Column2 = 100;

        /// <summary>
        /// 角点2坐标
        /// </summary>
        public double AngleRow2, AngleColumn2;

        /// <summary>
        /// 角点3坐标
        /// </summary>
        public double AngleRow3, AngleColumn3;

        #region 获取ROI角点
        /// <summary>
        /// 获取ROI角度
        /// </summary>
        /// <param name="row">角点roe</param>
        /// <param name="col">角点col</param>
        public void GetROIAnglePoint(out HTuple row, out HTuple col)
        {
            row = new HTuple(Row1, AngleRow2, AngleRow3, Row2);
            col = new HTuple(Column1, AngleColumn2, AngleColumn3, Column2);
        }
        #endregion
    }
}
