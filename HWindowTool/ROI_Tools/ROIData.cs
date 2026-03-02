using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hWindowTool
{
    /// <summary>
    /// ROI数据
    /// </summary>
    [Serializable]
    public class ROIData
    {
        /// <summary>
        /// ROI数据--构造函数
        /// </summary>
        /// <param name="rOIType">ROI类型</param>
        public ROIData(ROITypeEnum rOIType)
        {
            //创建空区域(防止序列化异常)
            hRegion.GenEmptyRegion();
            //更新ROI类型
            ROIType = rOIType;
        }

        /// <summary>
        /// ROI类型
        /// </summary>
        public readonly ROITypeEnum ROIType;

        /// <summary>
        /// ROI名称(键名)
        /// </summary>
        public string ROIName { get;  set; }
        /// <summary>
        /// 使用OCR文字识别
        /// </summary>
        public bool IsUseOcr { get; set; }
        /// <summary>
        /// OCR文字识别内容
        /// </summary>
        public string OcrStr { get; set; }

        /// <summary>
        /// ROI默认颜色
        /// </summary>
        private HColorEnum defaultColor = HColorEnum.blue;
        /// <summary>
        /// ROI默认颜色
        /// </summary>
        public HColorEnum DefaultColor
        {
            get { return defaultColor; }
            set
            {
                if (defaultColor != value)
                {
                    defaultColor = value;
                    //触发刷新显示事件
                    _RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// ROI激活时颜色
        /// </summary>
        private HColorEnum activateColor = HColorEnum.green;
        /// <summary>
        /// ROI激活时颜色
        /// </summary>
        public HColorEnum ActivateColor
        {
            get { return activateColor; }
            set
            {
                if (activateColor != value)
                {
                    activateColor = value;
                    //触发刷新显示事件
                    _RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// 选定颜色(小矩形)
        /// </summary>
        private HColorEnum selectedColor = HColorEnum.white;
        /// <summary>
        /// 选定颜色(小矩形)
        /// </summary>
        public HColorEnum SelectedColor
        {
            get { return selectedColor; }
            set
            {
                if (selectedColor != value)
                {
                    selectedColor = value;
                    //触发刷新显示事件
                    _RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// 默认颜色(小矩形)
        /// </summary>
        private HColorEnum defaultColorMin = HColorEnum.white;
        /// <summary>
        /// 默认颜色(小矩形)
        /// </summary>
        public HColorEnum DefaultColorMin
        {
            get { return defaultColorMin; }
            set
            {
                if (defaultColorMin != value)
                {
                    defaultColorMin = value;
                    //触发刷新显示事件
                    _RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// ROI可见
        /// </summary>
        private bool visible = true;
        /// <summary>
        /// ROI可见
        /// </summary>
        public bool Visible
        {
            get { return visible; }
            set
            {
                if (visible != value)
                {
                    visible = value;
                    //触发刷新显示事件
                    _RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// 文本可见
        /// </summary>
        private bool visibleText = true;
        /// <summary>
        /// 文本可见
        /// </summary>
        public bool VisibleText
        {
            get { return visibleText; }
            set
            {
                if (visibleText != value)
                {
                    visibleText = value;
                    //触发刷新显示事件
                    _RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// 文本(null默认显示键名)
        /// </summary>
        private string text;
        /// <summary>
        /// 文本(null默认显示键名)
        /// </summary>
        public string Text
        {
            get { return text ?? ROIName; }
            set
            {
                if (text != value)
                {
                    text = value;
                    //触发刷新显示事件
                    _RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// 显示线宽(边缘显示模式生效)
        /// </summary>
        private double lineWidth = 2;
        /// <summary>
        /// 显示线宽(边缘显示模式生效)
        /// </summary>
        public double LineWidth
        {
            get { return lineWidth; }
            set
            {
                if (value < 1)
                {
                    lineWidth = 1;
                }
                else if (value > 2000)
                {
                    lineWidth = 2000;
                }
                else
                {
                    lineWidth = value;
                }
                //触发刷新显示事件
                _RefreshDisplay?.Invoke();
            }
        }

        /// <summary>
        /// 可从UI改变ROI大小
        /// </summary>
        private bool resizeFromUI = true;
        /// <summary>
        /// 可从UI改变ROI大小
        /// </summary>
        public bool ResizeFromUI
        {
            get { return resizeFromUI; }
            set
            {
                if (resizeFromUI != value)
                {
                    resizeFromUI = value;
                    //触发刷新显示事件
                    _RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// 初始化标志
        /// 可以判断状态是否已经初始值,初始化完成后自己改成true,不需要判断是否已初始值的不用管
        /// </summary>
        public bool Initialize;

        /// <summary>
        /// ROI区域
        /// </summary>
        public HRegion hRegion = new HRegion();

        /// <summary>
        /// ROI轮廓
        /// </summary>
        [NonSerialized]
        internal HXLDCont hXLDCont = new HXLDCont();

        /// <summary>
        /// 刷新显示事件
        /// </summary>
        [field: NonSerialized]
        internal event Action _RefreshDisplay;
        /// <summary>
        /// 刷新显示事件
        /// </summary>
        internal event Action RefreshDisplay
        {
            add
            {
                _RefreshDisplay += value;
                ROI.IROIAction.RefreshDisplay += value;
            }
            remove
            {
                _RefreshDisplay -= value;
                ROI.IROIAction.RefreshDisplay -= value;
            }
        }

        /// <summary>
        /// 设置ROI名称
        /// </summary>
        /// <param name="name">名称</param>
        internal void SetROIName(string name)
        {
            //名称不为null
            if (name != null)
            {
                //更新ROI名称
                ROIName = name;
            }
        }

        /// <summary>
        /// ROI基类
        /// </summary>
        internal HWROI rOI;
        /// <summary>
        /// ROI基类
        /// </summary>
        public HWROI ROI
        {
            get
            {
                //ROI基类未实例
                if (rOI == null)
                {
                    //实例ROI基类
                    new InstanceROI().Instance(this);
                }
                //返回ROI数据
                return rOI;
            }
            private set { rOI = value; }
        }
    }

    /// <summary>
    /// ROI类型枚举
    /// </summary>
    public enum ROITypeEnum
    {
        /// <summary>
        /// 点
        /// </summary>
        点,
        /// <summary>
        /// 一维直线
        /// </summary>
        一维直线,
        /// <summary>
        /// 矩形1
        /// </summary>
        矩形1,
        /// <summary>
        /// 矩形2
        /// </summary>
        矩形2,
        /// <summary>
        /// 圆
        /// </summary>
        圆,
        /// <summary>
        /// 圆弧
        /// </summary>
        圆弧,
        /// <summary>
        /// 二维直线
        /// </summary>
        二维直线,
        /// <summary>
        /// 圆环
        /// </summary>
        圆环,
        /// <summary>
        /// 扇形环
        /// </summary>
        扇形环,
        /// <summary>
        /// 椭圆
        /// </summary>
        椭圆,
    }
}
