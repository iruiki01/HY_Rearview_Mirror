using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hWindowTool
{
    /// <summary>
    /// halcon文本数据
    /// </summary>
    [Serializable]
    public class HTextData
    {
        /// <summary>
        /// halcon文本数据构造函数
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <param name="image">true:使用图像坐标 false:使用窗口坐标</param>
        /// <param name="row">显示位置row</param>
        /// <param name="col">显示位置col</param>
        /// <param name="hColor">字体颜色</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="box">盒子(字体背景)</param>
        /// <param name="bold">粗体</param>
        /// <param name="visibleText">文本可见</param>
        public HTextData(string text, bool image, double row, double col, HColorEnum hColor, int fontSize, bool box, bool bold, bool visibleText = true)
        {
            //更新用户文本数据
            this.text = text;
            this.image = image;
            this.row = row;
            this.col = col;
            this.hColor = hColor;
            this.fontSize = fontSize;
            this.box = box;
            this.bold = bold;
            this.visibleText = visibleText;
        }

        /// <summary>
        /// 文本内容
        /// </summary>
        private string text;
        /// <summary>
        /// 文本内容
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                if (text != value)
                {
                    text = value;
                    //触发刷新显示事件
                    RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// true:使用图像坐标 false:使用窗口坐标
        /// </summary>
        private bool image;
        /// <summary>
        /// true:使用图像坐标 false:使用窗口坐标
        /// </summary>
        public bool Image
        {
            get { return image; }
            set
            {
                if (image != value)
                {
                    image = value;
                    //触发刷新显示事件
                    RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// 显示位置x
        /// </summary>
        private double row;
        /// <summary>
        /// 显示位置x
        /// </summary>
        public double Row
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
        /// 显示位置y
        /// </summary>
        private double col;
        /// <summary>
        /// 显示位置y
        /// </summary>
        public double Col
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
        /// 字体颜色
        /// </summary>
        private HColorEnum hColor;
        /// <summary>
        /// 字体颜色
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
        /// 字体大小
        /// </summary>
        private int fontSize;
        /// <summary>
        /// 字体大小
        /// </summary>
        public int FontSize
        {
            get
            {
                return fontSize < 10 ? 10 : fontSize;
            }
            set
            {
                if (fontSize != value)
                {
                    fontSize = value;
                    //触发刷新显示事件
                    RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// 盒子(字体背景)
        /// </summary>
        private bool box;
        /// <summary>
        /// 盒子(字体背景)
        /// </summary>
        public bool Box
        {
            get { return box; }
            set
            {
                if (box != value)
                {
                    box = value;
                    //触发刷新显示事件
                    RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// 粗体
        /// </summary>
        private bool bold;
        /// <summary>
        /// 粗体
        /// </summary>
        public bool Bold
        {
            get { return bold; }
            set
            {
                if (bold != value)
                {
                    bold = value;
                    //触发刷新显示事件
                    RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// 文本可见
        /// </summary>
        private bool visibleText;
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
                    RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// 背景色
        /// </summary>
        private HColorEnum backgroundColor = HColorEnum.black_50;
        /// <summary>
        /// 背景色
        /// </summary>
        public HColorEnum BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                if (backgroundColor != value)
                {
                    backgroundColor = value;
                    //触发刷新显示事件
                    RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// 刷新显示事件
        /// </summary>
        [field: NonSerialized]
        internal event Action RefreshDisplay;
    }
}
