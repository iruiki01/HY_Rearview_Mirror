using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hWindowTool
{
    /// <summary>
    /// 涂抹数据
    /// </summary>
    [Serializable]
    public class SmearData
    {
        /// <summary>
        /// 涂抹数据--构造函数
        /// </summary>
        public SmearData()
        {
            //创建涂抹空区域
            hRegion.GenEmptyRegion();
        }

        /// <summary>
        /// 涂抹名称(键名)
        /// </summary>
        internal string SmearName { get; private set; }

        /// <summary>
        /// 涂抹区域
        /// </summary>
        internal HRegion hRegion = new HRegion();

        /// <summary>
        /// 涂抹区域显示颜色
        /// </summary>
        private HColorEnum hRegionColor = HColorEnum.red_25;
        /// <summary>
        /// 涂抹区域显示颜色
        /// </summary>
        public HColorEnum HRegionColor
        {
            get { return hRegionColor; }
            set
            {
                if (hRegionColor != value)
                {
                    hRegionColor = value;
                    //触发刷新显示事件
                    _RefreshDisplay?.Invoke();
                }
            }
        }

        /// <summary>
        /// 涂抹工具颜色
        /// </summary>
        public HColorEnum SmearToolColor = HColorEnum.blue_25;

        /// <summary>
        /// 擦除工具颜色
        /// </summary>
        public HColorEnum EraseToolColor = HColorEnum.yellow_25;

        /// <summary>
        /// 工具形状
        /// </summary>
        public ToolShapeEnum ToolShape;

        /// <summary>
        /// 工具大小
        /// </summary>
        private double toolSize = 10;
        /// <summary>
        /// 工具大小
        /// </summary>
        public double ToolSize
        {
            get { return toolSize; }
            set
            {
                toolSize = value > 1 ? value : 1;
            }
        }

        /// <summary>
        /// 获取工具大小
        /// </summary>
        /// <returns></returns>
        internal double GetToolSize()
        {
            return ToolSize / 2;
        }

        /// <summary>
        /// 涂抹区域可见
        /// </summary>
        private bool visible = true;
        /// <summary>
        /// 涂抹区域可见
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
            get { return text ?? SmearName; }
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
                ISmearAction.RefreshDisplay += value;
            }
            remove
            {
                _RefreshDisplay -= value;
                ISmearAction.RefreshDisplay -= value;
            }
        }

        /// <summary>
        /// 设置涂抹名称
        /// </summary>
        /// <param name="name">名称</param>
        internal void SetSmearName(string name)
        {
            //名称不为null
            if (name != null)
            {
                //更新涂抹名称
                SmearName = name;
            }
        }

        /// <summary>
        /// 涂抹行为统一接口
        /// </summary>
        [NonSerialized]
        internal ISmearAction iSmearAction;
        /// <summary>
        /// 涂抹行为统一接口
        /// </summary>
        public ISmearAction ISmearAction
        {
            get
            {
                //涂抹行为统一接口未实例
                if (iSmearAction == null)
                {
                    //实例涂抹行为统一接口
                    iSmearAction = new SmearAction(this);
                }
                //返回涂抹行为统一接口
                return iSmearAction;
            }
            private set { iSmearAction = value; }
        }
    }

    /// <summary>
    /// 涂抹类型枚举
    /// </summary>
    public enum SmearTypeEnum
    {
        /// <summary>
        /// 未选择
        /// </summary>
        None,
        /// <summary>
        /// 涂抹
        /// </summary>
        涂抹,
        /// <summary>
        /// 擦除
        /// </summary>
        擦除,
    }

    /// <summary>
    /// 工具形状枚举
    /// </summary>
    public enum ToolShapeEnum
    {
        /// <summary>
        /// 方形
        /// </summary>
        方形,
        /// <summary>
        /// 圆形
        /// </summary>
        圆形,
    }
}
