using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hWindowTool
{
    /// <summary>
    /// 二维直线行为
    /// </summary>
    internal class Line2Action : IROIAction
    {
        /// <summary>
        /// 二维直线行为构造函数
        /// </summary>
        /// <param name="rOIData">ROI数据</param>
        public Line2Action(ROIData rOIData)
        {
            //更新ROI数据
            this.rOIData = rOIData;
            //更新二维直线数据
            this.line2Data = (Line2Data)rOIData.ROI;
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
        /// 二维直线数据
        /// </summary>
        private readonly Line2Data line2Data;

        /// <summary>
        /// 工具
        /// </summary>
        private HWTools tools = new HWTools();

        /// <summary>
        /// 起始小矩形轮廓
        /// </summary>
        private HXLDCont hX1 = new HXLDCont();

        /// <summary>
        /// 中心小矩形轮廓
        /// </summary>
        private HXLDCont hX2 = new HXLDCont();

        /// <summary>
        /// 结束小矩形轮廓
        /// </summary>
        private HXLDCont hX3 = new HXLDCont();

        /// <summary>
        /// 正极小矩形轮廓
        /// </summary>
        private HXLDCont hX4 = new HXLDCont();

        /// <summary>
        /// 中心小矩形坐标
        /// </summary>
        private double row2, column2;

        /// <summary>
        /// 正极小矩形坐标
        /// </summary>
        private double row4, column4;

        /// <summary>
        /// 负极小矩形坐标
        /// </summary>
        private double row5, column5;

        /// <summary>
        /// 弧度,长度1,长度2
        /// </summary>
        private double phi, length1, length2;

        /// <summary>
        /// 选定: 1起始小矩形 2中间小矩形 3结束小矩形 4正极小矩形
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
                //添加直线ROI轮廓
                hObjDataList.Add(new HObjData(rOIData.hXLDCont, rOIData.ActivateColor, true, false, rOIData.LineWidth));
                //添加小矩形轮廓到ROI对象集合
                hObjDataList.Add(new HObjData(hX1, rOIData.ActivateColor, true, false, HWTools.MIN_RECTANGLE_BORDER_SIZE));
                hObjDataList.Add(new HObjData(hX2, rOIData.ActivateColor, true, false, HWTools.MIN_RECTANGLE_BORDER_SIZE));
                hObjDataList.Add(new HObjData(hX3, rOIData.ActivateColor, true, false, HWTools.MIN_RECTANGLE_BORDER_SIZE));
                hObjDataList.Add(new HObjData(hX4, rOIData.ActivateColor, true, false, HWTools.MIN_RECTANGLE_BORDER_SIZE));
            }
            else
            {
                //添加直线ROI轮廓
                hObjDataList.Add(new HObjData(rOIData.hXLDCont, rOIData.DefaultColor, true, false, rOIData.LineWidth));
                //添加小矩形轮廓到ROI对象集合
                hObjDataList.Add(new HObjData(hX1, rOIData.DefaultColor, true, false, HWTools.MIN_RECTANGLE_BORDER_SIZE));
                hObjDataList.Add(new HObjData(hX2, rOIData.DefaultColor, true, false, HWTools.MIN_RECTANGLE_BORDER_SIZE));
                hObjDataList.Add(new HObjData(hX3, rOIData.DefaultColor, true, false, HWTools.MIN_RECTANGLE_BORDER_SIZE));
                hObjDataList.Add(new HObjData(hX4, rOIData.DefaultColor, true, false, HWTools.MIN_RECTANGLE_BORDER_SIZE));
            }
            //实例化小矩形填充集合
            List<HObjData> minRectangleFill = new List<HObjData>
            {
                //添加小矩形填充
                new HObjData(hX1, rOIData.DefaultColorMin, true, false, HWTools.MIN_RECTANGLE_FILL_SIZE),
                new HObjData(hX2, rOIData.DefaultColorMin, true, false, HWTools.MIN_RECTANGLE_FILL_SIZE),
                new HObjData(hX3, rOIData.DefaultColorMin, true, false, HWTools.MIN_RECTANGLE_FILL_SIZE),
                new HObjData(hX4, rOIData.DefaultColorMin, true, false, HWTools.MIN_RECTANGLE_FILL_SIZE)
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
                hTextDataList.Add(new HTextData(rOIData.Text, true, row2, column2, color, 20, false, false));
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
            if ((rOIData.ResizeFromUI && tools.TestRegionPoint(mouseRow, mouseColumn, line2Data.Row2, line2Data.Column2, testAreaSize)) ||
                (rOIData.ResizeFromUI && tools.TestRegionPoint(mouseRow, mouseColumn, line2Data.Row1, line2Data.Column1, testAreaSize)) ||
                (rOIData.ResizeFromUI && tools.TestRegionPoint(mouseRow, mouseColumn, row4, column4, testAreaSize)) ||
                (tools.TestRegionPoint(mouseRow, mouseColumn, row2, column2, testAreaSize)))
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
                //鼠标在结束小矩形按下
                if (rOIData.ResizeFromUI && tools.TestRegionPoint(mouseDownRow, mouseDownColumn, line2Data.Row2, line2Data.Column2, testAreaSize))
                {
                    //选定结束小矩形
                    selected = 3;
                }
                //鼠标在起始小矩形按下
                else if (rOIData.ResizeFromUI && tools.TestRegionPoint(mouseDownRow, mouseDownColumn, line2Data.Row1, line2Data.Column1, testAreaSize))
                {
                    //选定起始小矩形
                    selected = 1;
                }
                //鼠标在正极小矩形按下
                else if (rOIData.ResizeFromUI && tools.TestRegionPoint(mouseDownRow, mouseDownColumn, row4, column4, testAreaSize))
                {
                    //选定正极小矩形
                    selected = 4;
                }
                //鼠标在中心小矩形按下
                else if (tools.TestRegionPoint(mouseDownRow, mouseDownColumn, row2, column2, testAreaSize))
                {
                    //选定中心小矩形
                    selected = 2;
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
            //移动的是起始小矩形
            if (selected == 1)
            {
                //更新直线起始坐标
                line2Data.Row1 = mouseRow;
                line2Data.Column1 = mouseColumn;
                //更新小矩形数据
                UpdateMinRectangleData();
            }
            //移动的是中心小矩形
            else if (selected == 2)
            {
                //更新直线起始和结束坐标
                line2Data.Row1 += mouseRow - row2;
                line2Data.Column1 += mouseColumn - column2;
                line2Data.Row2 += mouseRow - row2;
                line2Data.Column2 += mouseColumn - column2;
                //更新小矩形数据
                UpdateMinRectangleData();
            }
            //移动的是结束小矩形
            else if (selected == 3)
            {
                //更新直线结束坐标
                line2Data.Row2 = mouseRow;
                line2Data.Column2 = mouseColumn;
                //更新小矩形数据
                UpdateMinRectangleData();
            }
            //移动的是正极小矩形
            else if (selected == 4)
            {
                //计算鼠标点到线垂足点
                HMisc.ProjectionPl(mouseRow, mouseColumn, row2, column2, row4, column4, out row4, out column4);
                //更新宽度
                line2Data.Width = HMisc.DistancePp(row2, column2, row4, column4);
                line2Data.Width = line2Data.Width < 0.1 ? 0.1 : line2Data.Width;
                //更新小矩形数据
                UpdateMinRectangleData();
                //计算鼠标点到正极距离
                double positiveLength = HMisc.DistancePp(mouseRow, mouseColumn, row4, column4);
                //计算鼠标点到负极距离
                double negativeLength = HMisc.DistancePp(mouseRow, mouseColumn, row5, column5);
                //负极距离鼠标点更近(更换极向)
                if (negativeLength < positiveLength)
                {
                    //暂存起始坐标
                    double row = line2Data.Row1;
                    double column = line2Data.Column1;
                    //互调起始结束点
                    line2Data.Row1 = line2Data.Row2;
                    line2Data.Column1 = line2Data.Column2;
                    line2Data.Row2 = row;
                    line2Data.Column2 = column;
                }
                //更新小矩形数据
                UpdateMinRectangleData();
            }
            //创建ROI区域
            CreateROIRegion();
        }

        /// <summary>
        /// 更新小矩形数据
        /// </summary>
        private void UpdateMinRectangleData()
        {
            //更新直线弧度
            phi = HMisc.AngleLx(line2Data.Row1, line2Data.Column1, line2Data.Row2, line2Data.Column2);
            //更新长度1
            length1 = HMisc.DistancePp(line2Data.Row1, line2Data.Column1, line2Data.Row2, line2Data.Column2) / 2;
            //更新长度2
            length2 = line2Data.Width;
            //更新中心小矩形坐标
            row2 = line2Data.Row1 + (line2Data.Row2 - line2Data.Row1) / 2;
            column2 = line2Data.Column1 + (line2Data.Column2 - line2Data.Column1) / 2;
            //更新角点1坐标
            line2Data.AngleRow1 = row2 - (length1 * Math.Sin(-phi) + length2 * Math.Cos(-phi));
            line2Data.AngleColumn1 = column2 - (length1 * Math.Cos(-phi) - length2 * Math.Sin(-phi));
            //更新角点2坐标
            line2Data.AngleRow2 = row2 - (length1 * Math.Sin(phi) + length2 * Math.Cos(phi));
            line2Data.AngleColumn2 = column2 + (length1 * Math.Cos(phi) - length2 * Math.Sin(phi));
            //更新角点3坐标
            line2Data.AngleRow3 = row2 + (length1 * Math.Sin(phi) + length2 * Math.Cos(phi));
            line2Data.AngleColumn3 = column2 - (length1 * Math.Cos(phi) - length2 * Math.Sin(phi));
            //更新角点4坐标
            line2Data.AngleRow4 = row2 + (length1 * Math.Sin(-phi) + length2 * Math.Cos(-phi));
            line2Data.AngleColumn4 = column2 + (length1 * Math.Cos(-phi) - length2 * Math.Sin(-phi));
            //更新正极小矩形坐标
            row4 = line2Data.AngleRow1 + (line2Data.AngleRow2 - line2Data.AngleRow1) / 2;
            column4 = line2Data.AngleColumn1 + (line2Data.AngleColumn2 - line2Data.AngleColumn1) / 2;
            //更新负极小矩形坐标
            row5 = line2Data.AngleRow3 + (line2Data.AngleRow4 - line2Data.AngleRow3) / 2;
            column5 = line2Data.AngleColumn3 + (line2Data.AngleColumn4 - line2Data.AngleColumn3) / 2;
        }

        /// <summary>
        /// 创建ROI轮廓
        /// </summary>
        private void CreateROIHXLDCont()
        {
            //创建二维直线轮廓
            rOIData.hXLDCont.GenRectangle2ContourXld(row2, column2, phi, length1, length2);
            //添加直线起点到终点连接线
            rOIData.hXLDCont = rOIData.hXLDCont.ConcatObj(new HXLDCont(new HTuple(new double[] { line2Data.Row1, line2Data.Row2 }), new HTuple(new double[] { line2Data.Column1, line2Data.Column2 })));
            //计算直线箭头
            {
                //计算起始到结束弧度
                double arrowPhi = HMisc.AngleLx(line2Data.Row1, line2Data.Column1, line2Data.Row2, line2Data.Column2);
                //循环创建箭头直线
                for (int i = 0; i < 2; i++)
                {
                    //创建围绕直线1结束点旋转仿射变换矩阵
                    HHomMat2D hHomMat2D1 = new HHomMat2D();
                    if (i == 0)
                    {
                        hHomMat2D1.VectorAngleToRigid(line2Data.Row2, line2Data.Column2, 0, line2Data.Row2, line2Data.Column2, arrowPhi + 0.2);
                    }
                    else if (i == 1)
                    {
                        hHomMat2D1.VectorAngleToRigid(line2Data.Row2, line2Data.Column2, 0, line2Data.Row2, line2Data.Column2, arrowPhi + -0.2);
                    }
                    //进行仿射变换
                    double arrowRow = hHomMat2D1.AffineTransPoint2d(line2Data.Row2, line2Data.Column2 - 4, out double arrowCol);
                    //添加直线1箭头直线
                    rOIData.hXLDCont = rOIData.hXLDCont.ConcatObj(new HXLDCont(new HTuple(new double[] { arrowRow, line2Data.Row2 }), new HTuple(new double[] { arrowCol, line2Data.Column2 })));
                }
            }
            //计算正负极箭头
            {
                //计算正极到负极弧度
                double arrowPhi = HMisc.AngleLx(row4, column4, row5, column5);
                //定义正极负极延申长度
                double arrowLength = 10;
                //计算延申后的箭头中心
                double arrowCentreRow = row5 + arrowLength * Math.Sin(-arrowPhi);
                double arrowCentreColumn = column5 + arrowLength * Math.Cos(-arrowPhi);
                //添加正极到负极延申后的连接线
                rOIData.hXLDCont = rOIData.hXLDCont.ConcatObj(new HXLDCont(new HTuple(new double[] { row4, arrowCentreRow }), new HTuple(new double[] { column4, arrowCentreColumn })));
                //创建正负极箭头
                {
                    //循环创建箭头直线
                    for (int i = 0; i < 2; i++)
                    {
                        //创建围绕直线1结束点旋转仿射变换矩阵
                        HHomMat2D hHomMat2D1 = new HHomMat2D();
                        if (i == 0)
                        {
                            hHomMat2D1.VectorAngleToRigid(arrowCentreRow, arrowCentreColumn, 0, arrowCentreRow, arrowCentreColumn, arrowPhi + 0.2);
                        }
                        else if (i == 1)
                        {
                            hHomMat2D1.VectorAngleToRigid(arrowCentreRow, arrowCentreColumn, 0, arrowCentreRow, arrowCentreColumn, arrowPhi + -0.2);
                        }
                        //进行仿射变换
                        double arrowRow = hHomMat2D1.AffineTransPoint2d(arrowCentreRow, arrowCentreColumn - 4, out double arrowCol);
                        //添加直线1箭头直线
                        rOIData.hXLDCont = rOIData.hXLDCont.ConcatObj(new HXLDCont(new HTuple(new double[] { arrowRow, arrowCentreRow }), new HTuple(new double[] { arrowCol, arrowCentreColumn })));
                    }
                }
            }
            //可从UI改变ROI大小
            if (rOIData.ResizeFromUI)
            {
                //创建起始小矩形
                hX1.GenRectangle2ContourXld(line2Data.Row1, line2Data.Column1, phi, HWTools.MIN_RECTANGLE_SIZE, HWTools.MIN_RECTANGLE_SIZE);
                //创建结束小矩形
                hX3.GenRectangle2ContourXld(line2Data.Row2, line2Data.Column2, phi, HWTools.MIN_RECTANGLE_SIZE, HWTools.MIN_RECTANGLE_SIZE);
                //创建正极小矩形
                hX4.GenRectangle2ContourXld(row4, column4, phi, HWTools.MIN_RECTANGLE_SIZE, HWTools.MIN_RECTANGLE_SIZE);
            }
            else
            {
                //实例化起始小矩形
                hX1 = new HXLDCont();
                //实例化结束小矩形
                hX3 = new HXLDCont();
                //实例化正极小矩形
                hX4 = new HXLDCont();
            }
            //创建中心小矩形
            hX2.GenRectangle2ContourXld(row2, column2, phi, HWTools.MIN_RECTANGLE_SIZE, HWTools.MIN_RECTANGLE_SIZE);
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
            line2Data.Row1 = hTuple[0].D;
            line2Data.Column1 = hTuple[1].D;
            line2Data.Row2 = hTuple[2].D;
            line2Data.Column2 = hTuple[3].D;
            line2Data.Width = hTuple[4].D < 0.1 ? 0.1 : hTuple[4].D;
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
                //创建矩形2区域
                rOIData.hRegion.GenRectangle2(row2, column2, phi, length1, length2);
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
            return new HTuple(line2Data.Row1, line2Data.Column1, line2Data.Row2, line2Data.Column2, line2Data.Width);
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
