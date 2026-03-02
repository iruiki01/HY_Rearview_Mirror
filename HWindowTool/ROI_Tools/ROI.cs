using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hWindowTool.ROI_Tools
{
    /// <summary>
    /// ROI基类
    /// </summary>
    [Serializable]
    public class ROI
    {
        /// <summary>
        /// ROI基类构造函数
        /// </summary>
        /// <param name="rOIData">ROI数据</param>
        public ROI(ROIData rOIData)
        {
            //更新ROI数据
            this.rOIData = rOIData;
        }

        /// <summary>
        /// ROI数据
        /// </summary>
        protected ROIData rOIData;

        /// <summary>
        /// ROI行为统一接口
        /// </summary>
        [NonSerialized]
        internal IROIAction iROIAction;
        /// <summary>
        /// ROI行为统一接口
        /// </summary>
        public IROIAction IROIAction
        {
            get
            {
                //ROI行为统一接口未实例化
                if (iROIAction == null)
                {
                    //实例化ROI行为统一接口
                    new InstanceROIAction().Instance(rOIData);
                }
                //返回ROI行为统一接口
                return iROIAction;
            }
            private set { iROIAction = value; }
        }
    }
}
