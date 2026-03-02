using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hWindowTool
{
    /// <summary>
    /// halcon对象数据
    /// </summary>
    [Serializable]
    public class HObjData
    {
        /// <summary>
        /// halcon对象数据
        /// </summary>
        /// <param name="hObject">要显示的HObject</param>
        /// <param name="hColor">显示颜色</param>
        /// <param name="margin">边缘模式</param>
        /// <param name="dottedLine">显示虚线</param>
        /// <param name="lineWidth">显示线宽(边缘显示模式生效)</param>
        public HObjData(HObject hObject, HColorEnum hColor, bool margin = true, bool dottedLine = false, double lineWidth = 2)
        {
            //更新要显示的HObject
            this.hObject = hObject;
            //更新显示颜色
            this.hColor = hColor;
            //更新边缘模式
            this.margin = margin;
            //更新显示虚线
            this.dottedLine = dottedLine;
            //更新显示线宽
            this.lineWidth = lineWidth;
        }

        /// <summary>
        /// halcon对象数据
        /// 创建Mark点(十字架,默认跟随缩放)
        /// </summary>
        /// <param name="row">Mark点位置row</param>
        /// <param name="col">Mark点位置col</param>
        /// <param name="angle">Mark点角度(注意不是弧度)</param>
        /// <param name="hColor">显示颜色</param>
        /// <param name="lineWidth">显示线宽</param>
        public HObjData(HTuple row, HTuple col, double angle, HColorEnum hColor, double lineWidth = 2)
        {
            //更新Mark点位置x
            this.row = row;
            //更新Mark点位置y
            this.col = col;
            //更新Mark点角度
            this.angle = angle;
            //更新显示颜色
            this.hColor = hColor;
            //更新显示线宽
            this.lineWidth = lineWidth;
            //更新边缘模式
            this.margin = true;
            //打开跟随缩放
            this.followZoom = true;
        }

        /// <summary>
        /// 要显示的HObject
        /// </summary>
        private HObject hObject;
        /// <summary>
        /// 要显示的HObject
        /// </summary>
        internal HObject HObject
        {
            get
            {
                //未实例化 || 未初始化
                if (hObject == null || !hObject.IsInitialized())
                {
                    hObject = new HObject();
                    hObject.GenEmptyObj();
                }
                return hObject;
            }
            set { hObject = value; }
        }

        /// <summary>
        /// 显示颜色
        /// </summary>
        private HColorEnum hColor;
        /// <summary>
        /// 显示颜色
        /// </summary>
        public HColorEnum HColor
        {
            get { return hColor; }
            set
            {
                if (hColor != value)
                {
                    hColor = value;
                    //触发刷新显示事件
                    RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// 显示颜色为auto时,没有多个对象时指定的颜色
        /// </summary>
        private HColorEnum hColor1 = HColorEnum.red;
        /// <summary>
        /// 显示颜色为auto时,没有多个对象时指定的颜色
        /// </summary>
        public HColorEnum HColor1
        {
            get { return hColor1; }
            set
            {
                //设置值与当前值不一致
                if (hColor1 != value)
                {
                    hColor1 = value;
                    //触发刷新显示事件
                    RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// 边缘显示(false:填充 true:边缘)
        /// </summary>
        private bool margin;
        /// <summary>
        /// 边缘显示(false:填充 true:边缘)
        /// </summary>
        public bool Margin
        {
            get { return margin; }
            set
            {
                if (margin != value)
                {
                    margin = value;
                    //触发刷新显示事件
                    RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// 显示虚线(false:实线 true:虚线)
        /// </summary>
        private bool dottedLine;
        /// <summary>
        /// 显示虚线(false:实线 true:虚线)
        /// </summary>
        public bool DottedLine
        {
            get { return dottedLine; }
            set
            {
                if (dottedLine != value)
                {
                    dottedLine = value;
                    //触发刷新显示事件
                    RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// 显示线宽(边缘显示模式生效)
        /// </summary>
        private double lineWidth;
        /// <summary>
        /// 显示线宽(边缘显示模式生效)
        /// </summary>
        public double LineWidth
        {
            get
            {
                if (lineWidth < 1)
                {
                    return 1;
                }
                else if (lineWidth > 2000)
                {
                    return 2000;
                }
                return lineWidth;
            }
            set
            {
                if (lineWidth != value)
                {
                    lineWidth = value;
                    //触发刷新显示事件
                    RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// Mark点位置row
        /// </summary>
        private HTuple row;
        /// <summary>
        /// Mark点位置row
        /// </summary>
        public HTuple Row
        {
            get { return row; }
            set
            {
                if (row != value)
                {
                    row = value;
                    //触发刷新显示事件
                    RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// Mark点位置col
        /// </summary>
        private HTuple col;
        /// <summary>
        /// Mark点位置col
        /// </summary>
        public HTuple Col
        {
            get { return col; }
            set
            {
                if (col != value)
                {
                    col = value;
                    //触发刷新显示事件
                    RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// Mark点角度(注意不是弧度)
        /// </summary>
        private double angle;
        /// <summary>
        /// Mark点角度(注意不是弧度)
        /// </summary>
        public double Angle
        {
            get { return angle; }
            set
            {
                if (angle != value)
                {
                    angle = value;
                    //触发刷新显示事件
                    RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// 跟随缩放
        /// </summary>
        private bool followZoom;
        /// <summary>
        /// 跟随缩放
        /// </summary>
        internal bool FollowZoom
        {
            get { return followZoom; }
            set { followZoom = value; }
        }

        /// <summary>
        /// 刷新显示事件
        /// </summary>
        [field: NonSerialized]
        internal event Action RefreshDisplay;
    }
}
