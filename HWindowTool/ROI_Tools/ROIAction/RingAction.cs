using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hWindowTool
{
    /// <summary>
    /// 圆环行为
    /// </summary>
    internal class RingAction : IROIAction
    {
        /// <summary>
        /// 圆环行为构造函数
        /// </summary>
        /// <param name="rOIData">ROI数据</param>
        public RingAction(ROIData rOIData)
        {
            //更新ROI数据
            this.rOIData = rOIData;
            //更新圆数据
            this.ringData = (RingData)rOIData.ROI;
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
        /// 圆环数据
        /// </summary>
        private readonly RingData ringData;

        /// <summary>
        /// 工具
        /// </summary>
        private HWTools tools = new HWTools();

        /// <summary>
        /// 边缘小矩形1轮廓
        /// </summary>
        private HXLDCont hX1 = new HXLDCont();

        /// <summary>
        /// 边缘小矩形2轮廓
        /// </summary>
        private HXLDCont hX2 = new HXLDCont();

        /// <summary>
        /// 中心小矩形轮廓
        /// </summary>
        private HXLDCont hX3 = new HXLDCont();

        /// <summary>
        /// 边缘小矩形1坐标
        /// </summary>
        private double row1, column1;

        /// <summary>
        /// 边缘小矩形2坐标
        /// </summary>
        private double row2, column2;

        /// <summary>
        /// 选定:1边缘小矩形1 2边缘小矩形2 3中心小矩形
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
                hTextDataList.Add(new HTextData(rOIData.Text, true, ringData.Row, ringData.Column, color, 20, false, false));
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
                tools.TestRegionPoint(mouseRow, mouseColumn, ringData.Row, ringData.Column, testAreaSize))
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
                //鼠标在边缘小矩形1按下
                if (rOIData.ResizeFromUI && tools.TestRegionPoint(mouseDownRow, mouseDownColumn, row1, column1, testAreaSize))
                {
                    //选定边缘小矩形1
                    selected = 1;
                }
                //鼠标在边缘小矩形2按下
                else if (rOIData.ResizeFromUI && tools.TestRegionPoint(mouseDownRow, mouseDownColumn, row2, column2, testAreaSize))
                {
                    //选定边缘小矩形2
                    selected = 2;
                }
                else if (tools.TestRegionPoint(mouseDownRow, mouseDownColumn, ringData.Row, ringData.Column, testAreaSize))
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
            //移动的是边缘小矩形1
            if (selected == 1)
            {
                //更新边缘小矩形坐标
                row1 = mouseRow;
                column1 = mouseColumn;
                //更新圆半径
                ringData.Radius1 = HMisc.DistancePp(ringData.Row, ringData.Column, row1, column1);
                //计算中心到小矩形弧度
                double phi = HMisc.AngleLx(ringData.Row, ringData.Column, row1, column1);
                //更新边缘小矩形2坐标
                row2 = ringData.Row + ringData.Radius2 * Math.Sin(-phi);
                column2 = ringData.Column + ringData.Radius2 * Math.Cos(-phi);
            }
            //移动的是边缘小矩形2
            else if (selected == 2)
            {
                //更新边缘小矩形2坐标
                row2 = mouseRow;
                column2 = mouseColumn;
                //更新圆半径
                ringData.Radius2 = HMisc.DistancePp(ringData.Row, ringData.Column, row2, column2);
                //计算中心到小矩形弧度
                double phi = HMisc.AngleLx(ringData.Row, ringData.Column, row2, column2);
                //更新边缘小矩形1坐标
                row1 = ringData.Row + ringData.Radius1 * Math.Sin(-phi);
                column1 = ringData.Column + ringData.Radius1 * Math.Cos(-phi);
            }
            //移动的是中心小矩形
            else if (selected == 3)
            {
                //更新边缘小矩形1坐标
                row1 += mouseRow - ringData.Row;
                column1 += mouseColumn - ringData.Column;
                //更新边缘小矩形2坐标
                row2 += mouseRow - ringData.Row;
                column2 += mouseColumn - ringData.Column;
                //更新圆心坐标
                ringData.Row = mouseRow;
                ringData.Column = mouseColumn;
            }
            //创建ROI区域
            CreateROIRegion();
        }

        /// <summary>
        /// 更新小矩形数据
        /// </summary>
        private void UpdateMinRectangleData()
        {
            //更新边缘小矩形1坐标
            row1 = ringData.Row;
            column1 = ringData.Column + ringData.Radius1;
            //更新边缘小矩形2坐标
            row2 = ringData.Row;
            column2 = ringData.Column + ringData.Radius2;
        }

        /// <summary>
        /// 创建ROI轮廓
        /// </summary>
        private void CreateROIHXLDCont()
        {
            //创建ROI圆1
            rOIData.hXLDCont.GenCircleContourXld(ringData.Row, ringData.Column, ringData.Radius1, 0, 6.28318, "positive", 1);
            //创建ROI圆2
            HXLDCont hXLDCont2 = new HXLDCont();
            hXLDCont2.GenCircleContourXld(ringData.Row, ringData.Column, ringData.Radius2, 0, 6.28318, "positive", 1);
            //连接ROI圆2
            rOIData.hXLDCont = rOIData.hXLDCont.ConcatObj(hXLDCont2);
            //可从UI改变ROI大小
            if (rOIData.ResizeFromUI)
            {
                //创建边缘小矩形1
                hX1.GenRectangle2ContourXld(row1, column1, 0, HWTools.MIN_RECTANGLE_SIZE, HWTools.MIN_RECTANGLE_SIZE);
                //创建边缘小矩形2
                hX2.GenRectangle2ContourXld(row2, column2, 0, HWTools.MIN_RECTANGLE_SIZE, HWTools.MIN_RECTANGLE_SIZE);
            }
            else
            {
                //实例化边缘小矩形1
                hX1 = new HXLDCont();
                //实例化边缘小矩形2
                hX2 = new HXLDCont();
            }
            //创建中心小矩形
            hX3.GenRectangle2ContourXld(ringData.Row, ringData.Column, 0, HWTools.MIN_RECTANGLE_SIZE, HWTools.MIN_RECTANGLE_SIZE);
        }

        /// <summary>
        /// 设置ROI数据
        /// </summary>
        /// <param name="hTuple">ROI数据元组</param>
        /// <returns>执行状态 true:成功 false:失败</returns>
        public bool SetROIData(HTuple hTuple)
        {
            //ROI数据元组未实例化 || 数据长度和ROI不匹配
            if (hTuple == null || hTuple.Length != 4)
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
            ringData.Row = hTuple[0].D;
            ringData.Column = hTuple[1].D;
            ringData.Radius1 = hTuple[2].D > 0 ? hTuple[2].D : 0;
            ringData.Radius2 = hTuple[3].D > 0 ? hTuple[3].D : 0;
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
                //创建圆形1
                HRegion hRegion1 = new HRegion();
                hRegion1.GenCircle(ringData.Row, ringData.Column, ringData.Radius1);
                //创建圆形2
                HRegion hRegion2 = new HRegion();
                hRegion2.GenCircle(ringData.Row, ringData.Column, ringData.Radius2);

                //圆形1半径比圆形2半径大
                if (ringData.Radius1 > ringData.Radius2)
                {
                    //创建圆环区域
                    rOIData.hRegion = hRegion1.Difference(hRegion2);
                }
                else
                {
                    //创建圆环区域
                    rOIData.hRegion = hRegion2.Difference(hRegion1);
                }
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
            return new HTuple(ringData.Row, ringData.Column, ringData.Radius1, ringData.Radius2);
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
