using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hWindowTool
{
    /// <summary>
    /// 实例ROI行为
    /// </summary>
    internal class InstanceROIAction
    {
        /// <summary>
        /// 实例ROI行为
        /// </summary>
        /// <param name="rOIData">ROI数据</param>
        public void Instance(ROIData rOIData)
        {
            switch (rOIData.ROIType)
            {
                case ROITypeEnum.点:
                    //实例化ROI点行为接口
                    rOIData.ROI.iROIAction = new PointAction(rOIData);
                    break;
                case ROITypeEnum.一维直线:
                    //实例化ROI一维直线行为接口
                    rOIData.ROI.iROIAction = new Line1Action(rOIData);
                    break;
                case ROITypeEnum.二维直线:
                    //实例化ROI二维直线行为接口
                    rOIData.ROI.iROIAction = new Line2Action(rOIData);
                    break;
                case ROITypeEnum.矩形1:
                    //实例化ROI矩形1行为接口
                    rOIData.ROI.iROIAction = new Rectangle1Action(rOIData);
                    break;
                case ROITypeEnum.矩形2:
                    //实例化ROI矩形2行为接口
                    rOIData.ROI.iROIAction = new Rectangle2Action(rOIData);
                    break;
                case ROITypeEnum.圆:
                    //实例化ROI圆行为接口
                    rOIData.ROI.iROIAction = new CircleAction(rOIData);
                    break;
                case ROITypeEnum.圆弧:
                    //实例化ROI圆弧行为接口
                    rOIData.ROI.iROIAction = new CircleArcAction(rOIData);
                    break;
                case ROITypeEnum.圆环:
                    //实例化ROI圆环行为接口
                    rOIData.ROI.iROIAction = new RingAction(rOIData);
                    break;
                case ROITypeEnum.扇形环:
                    //实例化ROI扇形环行为接口
                    rOIData.ROI.iROIAction = new FanShapedRingAction(rOIData);
                    break;
                case ROITypeEnum.椭圆:
                    //实例化ROI椭圆行为接口
                    rOIData.ROI.iROIAction = new EllipseAction(rOIData);
                    break;
            }
        }
    }
}
