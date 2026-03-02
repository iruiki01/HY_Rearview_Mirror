using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hWindowTool
{
    /// <summary>
    /// 圆弧行为
    /// </summary>
    internal class CircleArcAction : IROIAction
    {
        /// <summary>
        /// 圆弧行为构造函数
        /// </summary>
        /// <param name="rOIData">ROI数据</param>
        public CircleArcAction(ROIData rOIData)
        {
            //更新ROI数据
            this.rOIData = rOIData;
            //更新圆数据
            this.circleArcData = (CircleArcData)rOIData.ROI;
            //ROI轮廓未实例化
            if (rOIData.hXLDCont == null)
            {
                //实例化ROI轮廓
                rOIData.hXLDCont = new HXLDCont();
            }
            //更新小矩形数据
            UpdateMinRectangleData();
            //创建ROI区域
            CreateROIRegion();
            //创建ROI轮廓
            CreateROIHXLDCont();
        }

        /// <summary>
        /// ROI数据
        /// </summary>
        private readonly ROIData rOIData;

        /// <summary>
        /// 圆弧数据
        /// </summary>
        private readonly CircleArcData circleArcData;

        /// <summary>
        /// 工具
        /// </summary>
        private HWTools tools = new HWTools();

        /// <summary>
        /// 边缘起始小矩形1轮廓
        /// </summary>
        private HXLDCont hX1 = new HXLDCont();

        /// <summary>
        /// 边缘结束小矩形2轮廓
        /// </summary>
        private HXLDCont hX2 = new HXLDCont();

        /// <summary>
        /// 中心小矩形轮廓
        /// </summary>
        private HXLDCont hX3 = new HXLDCont();

        /// <summary>
        /// 边缘起始小矩形1坐标
        /// </summary>
        private double row1, column1;

        /// <summary>
        /// 边缘结束小矩形2坐标
        /// </summary>
        private double row2, column2;

        /// <summary>
        /// 选定
        /// 1:边缘起始小矩形1
        /// 2:边缘结束小矩形2
        /// 3:中心小矩形
        /// </summary>
        private int selected;

        /// <summary>
        /// 测试区域大小
        /// </summary>
        private double testAreaSize;

        /// <summary>
        /// 刷新显示事件
        /// </summary>
        public event Action RefreshDisplay;

        /// <summary>
        /// ROI激活状态
        /// </summary>
        public bool Activate { get; set; }

        /// <summary>
        /// 起始角度(弧度)
        /// </summary>
        private double startAngle;
        /// <summary>
        /// 起始角度(弧度)
        /// </summary>
        protected double StartAngle
        {
            get { return startAngle; }
            set
            {
                startAngle = value;
                //计算圆弧起始角度
                circleArcData.StartAngle = value >= 0 ? value : value + tools.ATR(360);
            }
        }

        /// <summary>
        /// 结束角度(弧度)
        /// </summary>
        private double endAngle;
        /// <summary>
        /// 结束角度(弧度)
        /// </summary>
        public double EndAngle
        {
            get { return endAngle; }
            set
            {
                endAngle = value;
                //计算圆弧结束角度
                circleArcData.EndAngle = value >= 0 ? value : value + tools.ATR(360);
            }
        }

        /// <summary>
        /// 获取ROI轮廓数据
        /// </summary>
        /// <param name="hObjDataList">ROI轮廓对象</param>
        /// <param name="imageShortEdge">当前缩放图像短边</param>
        public void GetROIHXLDContData(ref List<HObjData> hObjDataList, double imageShortEdge)
        {
            //更新测试区域(测试区域占当前显示图像1%)
            testAreaSize = imageShortEdge * 0.01 < 0.01 ? 0.01 : imageShortEdge * 0.01;
            //创建ROI轮廓
            CreateROIHXLDCont();
            //当前是激活状态
            if (Activate)
            {
                //添加ROI圆轮廓到ROI对象集合
                hObjDataList.Add(new HObjData(rOIData.hXLDCont, rOIData.ActivateColor, true, false, rOIData.LineWidth));
                //添加小矩形轮廓到ROI对象集合
                hObjDataList.Add(new HObjData(hX1, rOIData.ActivateColor, true, false, HWTools.MIN_RECTANGLE_BORDER_SIZE));
                hObjDataList.Add(new HObjData(hX2, rOIData.ActivateColor, true, false, HWTools.MIN_RECTANGLE_BORDER_SIZE));
                hObjDataList.Add(new HObjData(hX3, rOIData.ActivateColor, true, false, HWTools.MIN_RECTANGLE_BORDER_SIZE));
            }
            else
            {
                //添加ROI圆轮廓到ROI对象集合
                hObjDataList.Add(new HObjData(rOIData.hXLDCont, rOIData.DefaultColor, true, false, rOIData.LineWidth));
                //添加小矩形轮廓到ROI对象集合
                hObjDataList.Add(new HObjData(hX1, rOIData.DefaultColor, true, false, HWTools.MIN_RECTANGLE_BORDER_SIZE));
                hObjDataList.Add(new HObjData(hX2, rOIData.DefaultColor, true, false, HWTools.MIN_RECTANGLE_BORDER_SIZE));
                hObjDataList.Add(new HObjData(hX3, rOIData.DefaultColor, true, false, HWTools.MIN_RECTANGLE_BORDER_SIZE));
            }
            //实例化小矩形填充集合
            List<HObjData> minRectangleFill = new List<HObjData>
            {
                //添加小矩形填充
                new HObjData(hX1, rOIData.DefaultColorMin, true, false, HWTools.MIN_RECTANGLE_FILL_SIZE),
                new HObjData(hX2, rOIData.DefaultColorMin, true, false, HWTools.MIN_RECTANGLE_FILL_SIZE),
                new HObjData(hX3, rOIData.DefaultColorMin, true, false, HWTools.MIN_RECTANGLE_FILL_SIZE)
            };
            //当前是激活状态 && 已选定小矩形
            if (Activate && selected != 0)
            {
                //设置选定小矩形颜色
                minRectangleFill[selected - 1].HColor = rOIData.SelectedColor;
            }
            //添加小矩形填充
            hObjDataList.AddRange(minRectangleFill);
        }

        /// <summary>
        /// 获取ROI文本集合
        /// </summary>
        /// <returns>ROI文本集合</returns>
        public List<HTextData> GetROIHTextList()
        {
            //文本可见
            if (rOIData.VisibleText)
            {
                //实例化ROI文本集合
                List<HTextData> hTextDataList = new List<HTextData>();
                //获取文本颜色
                HColorEnum color = Activate ? rOIData.ActivateColor : rOIData.DefaultColor;
                //添加到ROI文本集合
                hTextDataList.Add(new HTextData(rOIData.Text, true, circleArcData.Row, circleArcData.Column, color, 20, false, false));
                //返回ROI文本集合
                return hTextDataList;
            }
            //返回null
            return null;
        }

        /// <summary>
        /// 是否进入小矩形
        /// </summary>
        /// <param name="mouseRow">鼠标当前位置Row</param>
        /// <param name="mouseColumn">鼠标当前位置Column</param>
        /// <returns>true:进入 false:未进入</returns>
        public bool IsEnterMinRectangle(double mouseRow, double mouseColumn)
        {
            //光标进入小矩形
            if ((rOIData.ResizeFromUI && tools.TestRegionPoint(mouseRow, mouseColumn, row1, column1, testAreaSize)) ||
                (rOIData.ResizeFromUI && tools.TestRegionPoint(mouseRow, mouseColumn, row2, column2, testAreaSize)) ||
                tools.TestRegionPoint(mouseRow, mouseColumn, circleArcData.Row, circleArcData.Column, testAreaSize))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查ROI是否行动
        /// </summary>
        /// <param name="mouseDownRow">鼠标按下时位置Row</param>
        /// <param name="mouseDownColumn">鼠标按下时位置Column</param>
        /// <returns>在行动返回ROI名称,否则返回null</returns>
        public string IsAction(double mouseDownRow, double mouseDownColumn)
        {
            //ROI可见
            if (rOIData.Visible)
            {
                //更新小矩形选择状态
                selected = 0;
                //鼠标在边缘起始小矩形1按下
                if (rOIData.ResizeFromUI && tools.TestRegionPoint(mouseDownRow, mouseDownColumn, row1, column1, testAreaSize))
                {
                    //选定边缘起始小矩形1
                    selected = 1;
                }
                //鼠标在边缘结束小矩形2按下
                else if (rOIData.ResizeFromUI && tools.TestRegionPoint(mouseDownRow, mouseDownColumn, row2, column2, testAreaSize))
                {
                    //选定边缘结束小矩形2
                    selected = 2;
                }
                //鼠标在中心小矩形按下
                else if (tools.TestRegionPoint(mouseDownRow, mouseDownColumn, circleArcData.Row, circleArcData.Column, testAreaSize))
                {
                    //选定中心小矩形
                    selected = 3;
                }
                //选定成功
                if (selected != 0)
                {
                    //打开ROI激活状态
                    Activate = true;
                    //返回ROI名称
                    return rOIData.ROIName;
                }
                //关闭ROI激活状态
                Activate = false;
            }
            //返回null
            return null;
        }

        /// <summary>
        /// ROI移动
        /// </summary>
        /// <param name="mouseRow">鼠标当前位置Row</param>
        /// <param name="mouseColumn">鼠标当前位置Column</param>
        public void ROIMove(double mouseRow, double mouseColumn)
        {
            //移动的是边缘起始小矩形1
            if (selected == 1)
            {
                //更新边缘起始小矩形1坐标
                row1 = mouseRow;
                column1 = mouseColumn;
                //更新圆半径
                circleArcData.Radius = HMisc.DistancePp(circleArcData.Row, circleArcData.Column, row1, column1);
                //计算圆弧起始角度
                StartAngle = HMisc.AngleLx(circleArcData.Row, circleArcData.Column, row1, column1);
                //更新边缘结束小矩形2坐标
                row2 = circleArcData.Row + circleArcData.Radius * Math.Sin(-EndAngle);
                column2 = circleArcData.Column + circleArcData.Radius * Math.Cos(-EndAngle);
            }
            //移动的是边缘结束小矩形2
            else if (selected == 2)
            {
                //更新边缘结束小矩形2坐标
                row2 = mouseRow;
                column2 = mouseColumn;
                //更新圆半径
                circleArcData.Radius = HMisc.DistancePp(circleArcData.Row, circleArcData.Column, row2, column2);
                //计算圆弧结束角度
                EndAngle = HMisc.AngleLx(circleArcData.Row, circleArcData.Column, row2, column2);
                //更新边缘起始小矩形1坐标
                row1 = circleArcData.Row + circleArcData.Radius * Math.Sin(-StartAngle);
                column1 = circleArcData.Column + circleArcData.Radius * Math.Cos(-StartAngle);
            }
            //移动的是中心小矩形
            else if (selected == 3)
            {
                //更新边缘起始小矩形1坐标
                row1 += mouseRow - circleArcData.Row;
                column1 += mouseColumn - circleArcData.Column;
                //更新边缘结束小矩形2坐标
                row2 += mouseRow - circleArcData.Row;
                column2 += mouseColumn - circleArcData.Column;
                //更新圆心坐标
                circleArcData.Row = mouseRow;
                circleArcData.Column = mouseColumn;
            }
            //创建ROI区域
            CreateROIRegion();
        }

        /// <summary>
        /// 更新小矩形数据
        /// </summary>
        private void UpdateMinRectangleData()
        {
            //计算操作使用的起始弧度
            StartAngle = circleArcData.StartAngle <= tools.ATR(180) ? circleArcData.StartAngle : circleArcData.StartAngle - tools.ATR(360);
            //计算操作使用的结束弧度
            EndAngle = circleArcData.EndAngle <= tools.ATR(180) ? circleArcData.EndAngle : circleArcData.EndAngle - tools.ATR(360);
            //更新边缘起始小矩形1坐标
            row1 = circleArcData.Row + circleArcData.Radius * Math.Sin(-StartAngle);
            column1 = circleArcData.Column + circleArcData.Radius * Math.Cos(-StartAngle);
            //更新边缘结束小矩形2坐标
            row2 = circleArcData.Row + circleArcData.Radius * Math.Sin(-EndAngle);
            column2 = circleArcData.Column + circleArcData.Radius * Math.Cos(-EndAngle);
        }

        /// <summary>
        /// 创建ROI轮廓
        /// </summary>
        private void CreateROIHXLDCont()
        {
            //创建ROI圆1
            rOIData.hXLDCont.GenCircleContourXld(circleArcData.Row, circleArcData.Column, circleArcData.Radius, StartAngle, EndAngle, "positive", 1);
            //创建直线1
            HXLDCont line1 = new HXLDCont(new HTuple(circleArcData.Row, row1), new HTuple(circleArcData.Column, column1));
            //创建直线2
            HXLDCont line2 = new HXLDCont(new HTuple(circleArcData.Row, row2), new HTuple(circleArcData.Column, column2));
            //连接直线1
            rOIData.hXLDCont = rOIData.hXLDCont.ConcatObj(line1);
            //连接直线2
            rOIData.hXLDCont = rOIData.hXLDCont.ConcatObj(line2);
            //可从UI改变ROI大小
            if (rOIData.ResizeFromUI)
            {
                //创建边缘起始小矩形1
                hX1.GenRectangle2ContourXld(row1, column1, 0, HWTools.MIN_RECTANGLE_SIZE, HWTools.MIN_RECTANGLE_SIZE);
                //创建边缘结束小矩形2
                hX2.GenRectangle2ContourXld(row2, column2, 0, HWTools.MIN_RECTANGLE_SIZE, HWTools.MIN_RECTANGLE_SIZE);

            }
            else
            {
                //实例化边缘起始小矩形1
                hX1 = new HXLDCont();
                //实例化边缘结束小矩形2
                hX2 = new HXLDCont();
            }
            //创建中心小矩形
            hX3.GenRectangle2ContourXld(circleArcData.Row, circleArcData.Column, 0, HWTools.MIN_RECTANGLE_SIZE, HWTools.MIN_RECTANGLE_SIZE);
        }

        /// <summary>
        /// 设置ROI数据
        /// </summary>
        /// <param name="hTuple">ROI数据元组</param>
        /// <returns>执行状态 true:成功 false:失败</returns>
        public bool SetROIData(HTuple hTuple)
        {
            //ROI数据元组未实例化 || 数据长度和ROI不匹配
            if (hTuple == null || hTuple.Length != 5)
            {
                //结束方法
                return false;
            }
            //判断设置的元组和当前的元组值是否一致
            bool equal = hTuple.TupleEqual(GetROIData());
            //一致,不需要重新设置
            if (equal)
            {
                return true;
            }
            //更新ROI数据
            circleArcData.Row = hTuple[0].D;
            circleArcData.Column = hTuple[1].D;
            circleArcData.Radius = hTuple[2].D > 0 ? hTuple[2].D : 0;
            circleArcData.StartAngle = hTuple[3].D;
            circleArcData.EndAngle = hTuple[4].D;
            //更新小矩形数据
            UpdateMinRectangleData();
            //创建ROI区域
            CreateROIRegion();
            //创建ROI轮廓
            CreateROIHXLDCont();
            //触发刷新显示事件
            RefreshDisplay?.Invoke();
            //执行成功
            return true;
        }

        /// <summary>
        /// 创建ROI区域
        /// </summary>
        private void CreateROIRegion()
        {
            try
            {
                //创建扇形
                rOIData.hRegion.GenCircleSector(circleArcData.Row, circleArcData.Column, circleArcData.Radius, circleArcData.StartAngle, circleArcData.EndAngle);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 获取ROI数据
        /// </summary>
        /// <returns>ROI数据</returns>
        public HTuple GetROIData()
        {
            return new HTuple(circleArcData.Row, circleArcData.Column, circleArcData.Radius, circleArcData.StartAngle, circleArcData.EndAngle);
        }

        #region 获取ROI区域
        /// <summary>
        /// 获取ROI区域
        /// </summary>
        /// <returns>ROI区域</returns>
        public HRegion GetROIHRegion()
        {
            //区域已实例化 && 区域已初始化 && 区域存在有效数据
            if (rOIData.hRegion != null && rOIData.hRegion.IsInitialized() && rOIData.hRegion.CountObj() > 0)
            {
                //返回复制的区域对象
                return rOIData.hRegion.CopyObj(1, -1);
            }
            else
            {
                //返回一个空区域
                HRegion hRegion = new HRegion();
                hRegion.GenEmptyObj();
                return hRegion;
            }
        }
        #endregion

        #region 获取ROI轮廓
        /// <summary>
        /// 获取ROI轮廓
        /// </summary>
        /// <returns>ROI轮廓</returns>
        public HXLDCont GetROIHXLDCont()
        {
            //轮廓已实例化 && 轮廓已初始化 && 轮廓存在有效数据
            if (rOIData.hXLDCont != null && rOIData.hXLDCont.IsInitialized() && rOIData.hXLDCont.CountObj() > 0)
            {
                //返回复制轮廓数据
                return rOIData.hXLDCont.CopyObj(1, -1);
            }
            else
            {
                //返回一个空轮廓
                HXLDCont hXLDCont = new HXLDCont();
                hXLDCont.GenEmptyObj();
                return hXLDCont;
            }
        }
        #endregion

        #region 获取ROI全部数据
        /// <summary>
        /// 获取ROI全部数据
        /// </summary>
        /// <param name="hTuple">ROI数据元组</param>
        /// <param name="hRegion">ROI区域</param>
        /// <param name="hXLDCont">ROI轮廓</param>
        public void GetAllROIData(out HTuple hTuple, out HRegion hRegion, out HXLDCont hXLDCont)
        {
            //获取ROI数据
            hTuple = GetROIData();
            //获取ROI区域
            hRegion = GetROIHRegion();
            //获取ROI轮廓
            hXLDCont = GetROIHXLDCont();
        }
        #endregion
    }
}
