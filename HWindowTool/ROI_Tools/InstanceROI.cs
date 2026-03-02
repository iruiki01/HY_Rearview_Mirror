using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static hWindowTool.ROIData;

namespace hWindowTool
{
    /// <summary>
    /// 实例ROI
    /// </summary>
    internal class InstanceROI
    {
        /// <summary>
        /// 实例ROI
        /// </summary>
        /// <param name="rOIData">ROI数据</param>
        public void Instance(ROIData rOIData)
        {
            //判断ROI类型
            switch (rOIData.ROIType)
            {
                case ROITypeEnum.点:
                    //实例化点数据
                    rOIData.rOI = new PointData(rOIData);
                    break;
                case ROITypeEnum.一维直线:
                    //实例化一维直线数据
                    rOIData.rOI = new Line1Data(rOIData);
                    break;
                case ROITypeEnum.二维直线:
                    //实例化一维直线数据
                    rOIData.rOI = new Line2Data(rOIData);
                    break;
                case ROITypeEnum.矩形1:
                    //实例化矩形1数据
                    rOIData.rOI = new Rectangle1Data(rOIData);
                    break;
                case ROITypeEnum.矩形2:
                    //实例化矩形2数据
                    rOIData.rOI = new Rectangle2Data(rOIData);
                    break;
                case ROITypeEnum.圆:
                    //实例化圆数据
                    rOIData.rOI = new CircleData(rOIData);
                    break;
                case ROITypeEnum.圆弧:
                    //实例化圆弧数据
                    rOIData.rOI = new CircleArcData(rOIData);
                    break;
                case ROITypeEnum.圆环:
                    //实例化圆环数据
                    rOIData.rOI = new RingData(rOIData);
                    break;
                case ROITypeEnum.扇形环:
                    //实例化扇形环数据
                    rOIData.rOI = new FanShapedRingData(rOIData);
                    break;
                case ROITypeEnum.椭圆:
                    //实例化椭圆数据
                    rOIData.rOI = new EllipseData(rOIData);
                    break;
            }
        }
    }
}
