using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hWindowTool
{
    /// <summary>
    /// 点行为
    /// </summary>
    public class PointAction : IROIAction
    {
        /// <summary>
        /// 点行为构造函数
        /// </summary>
        /// <param name="rOIData">ROI数据</param>
        internal PointAction(ROIData rOIData)
        {
            //更新ROI数据
            this.rOIData = rOIData;
            //更新点数据
            this.pointData = (PointData)rOIData.ROI;
            //ROI轮廓未实例化
            if (rOIData.hXLDCont == null)
            {
                //实例化ROI轮廓
                rOIData.hXLDCont = new HXLDCont();
            }
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
        /// 点数据
        /// </summary>
        private readonly PointData pointData;

        /// <summary>
        /// 工具
        /// </summary>
        private HWTools tools = new HWTools();

        /// <summary>
        /// 中心小矩形轮廓
        /// </summary>
        private HXLDCont hX1 = new HXLDCont();

        /// <summary>
        /// 选定:1中心小矩形
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
                //添加小矩形轮廓到ROI对象集合
                hObjDataList.Add(new HObjData(hX1, rOIData.ActivateColor, true, false, HWTools.MIN_RECTANGLE_BORDER_SIZE));
            }
            else
            {
                //添加小矩形轮廓到ROI对象集合
                hObjDataList.Add(new HObjData(hX1, rOIData.DefaultColor, true, false, HWTools.MIN_RECTANGLE_BORDER_SIZE));
            }
            //添加小矩形填充
            hObjDataList.Add(new HObjData(hX1, Activate ? rOIData.SelectedColor : rOIData.DefaultColorMin, true, false, HWTools.MIN_RECTANGLE_FILL_SIZE));
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
                hTextDataList.Add(new HTextData(rOIData.Text, true, pointData.Row, pointData.Column, color, 20, false, false));
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
            if (tools.TestRegionPoint(mouseRow, mouseColumn, pointData.Row, pointData.Column, testAreaSize))
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
                //鼠标在中心小矩形按下
                if (tools.TestRegionPoint(mouseDownRow, mouseDownColumn, pointData.Row, pointData.Column, testAreaSize))
                {
                    //选定边缘小矩形
                    selected = 1;
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
            //移动的是中心小矩形
            if (selected == 1)
            {
                //更新点坐标
                pointData.Row = mouseRow;
                pointData.Column = mouseColumn;
            }
            //创建ROI区域
            CreateROIRegion();
        }

        /// <summary>
        /// 创建ROI轮廓
        /// </summary>
        private void CreateROIHXLDCont()
        {
            //创建ROI点
            rOIData.hXLDCont.GenRectangle2ContourXld(pointData.Row, pointData.Column, 0, HWTools.MIN_RECTANGLE_SIZE, HWTools.MIN_RECTANGLE_SIZE);
            //创建中心小矩形
            hX1.GenRectangle2ContourXld(pointData.Row, pointData.Column, 0, HWTools.MIN_RECTANGLE_SIZE, HWTools.MIN_RECTANGLE_SIZE);
        }

        /// <summary>
        /// 设置ROI数据
        /// </summary>
        /// <param name="hTuple">ROI数据元组</param>
        /// <returns>执行状态 true:成功 false:失败</returns>
        public bool SetROIData(HTuple hTuple)
        {
            //ROI数据元组未实例化 || 数据长度和ROI不匹配
            if (hTuple == null || hTuple.Length != 2)
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
            pointData.Row = hTuple[0].D;
            pointData.Column = hTuple[1].D;
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
                //创建点区域
                rOIData.hRegion.GenRegionPoints(pointData.Row, pointData.Column);
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
            return new HTuple(pointData.Row, pointData.Column);
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
