using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hWindowTool
{
    /// <summary>
    /// 涂抹行为
    /// </summary>
    internal class SmearAction : ISmearAction
    {
        /// <summary>
        /// 涂抹行为构造函数
        /// </summary>
        /// <param name="smearData">涂抹数据</param>
        public SmearAction(SmearData smearData)
        {
            //更新涂抹数据
            this.smearData = smearData;
        }

        /// <summary>
        /// 涂抹数据
        /// </summary>
        private SmearData smearData;

        /// <summary>
        /// 工具区域
        /// </summary>
        private HRegion toolRegion = new HRegion();

        /// <summary>
        /// 涂抹类型
        /// </summary>
        private SmearTypeEnum smearType;

        /// <summary>
        /// 刷新显示事件
        /// </summary>
        public event Action RefreshDisplay;

        /// <summary>
        /// 涂抹工作
        /// </summary>
        /// <param name="mouseRow">鼠标当前位置Row</param>
        /// <param name="mouseColumn">鼠标当前位置Column</param>
        public void SmearWork(double mouseRow, double mouseColumn)
        {
            //创建工具区域
            CreateToolRegion(mouseRow, mouseColumn);
            //判断涂抹类型
            switch (smearType)
            {
                case SmearTypeEnum.涂抹:
                    //区域加
                    smearData.hRegion = smearData.hRegion.Union2(toolRegion);
                    break;
                case SmearTypeEnum.擦除:
                    //区域减
                    smearData.hRegion = smearData.hRegion.Difference(toolRegion);
                    break;
            }
        }

        /// <summary>
        /// 获取涂抹区域
        /// </summary>
        /// <param name="mouseRow">鼠标当前位置Row</param>
        /// <param name="mouseColumn">鼠标当前位置Column</param>
        /// <returns>涂抹区域集合</returns>
        public List<HObjData> GetSmearHRegion(double mouseRow, double mouseColumn)
        {
            //实例化涂抹区域集合
            List<HObjData> hObjDataList = new List<HObjData>();
            //添加涂抹区域到集合
            hObjDataList.Add(new HObjData(smearData.hRegion, smearData.HRegionColor, false));
            //判断涂抹类型
            switch (smearType)
            {
                case SmearTypeEnum.涂抹:
                    //创建工具区域
                    CreateToolRegion(mouseRow, mouseColumn);
                    //添加工具区域到集合
                    hObjDataList.Add(new HObjData(toolRegion, smearData.SmearToolColor, false));
                    break;
                case SmearTypeEnum.擦除:
                    //创建工具区域
                    CreateToolRegion(mouseRow, mouseColumn);
                    //添加工具区域到集合
                    hObjDataList.Add(new HObjData(toolRegion, smearData.EraseToolColor, false));
                    break;
            }
            //返回涂抹区域集合
            return hObjDataList;
        }

        /// <summary>
        /// 获取涂抹文本
        /// </summary>
        /// <returns>涂抹文本集合</returns>
        public List<HTextData> GetSmearHTextList()
        {
            //实例化文本集合
            List<HTextData> hTextDataList = new List<HTextData>();
            //获取区域中心坐标
            smearData.hRegion.AreaCenter(out double row, out double column);
            //添加到涂抹文本集合
            hTextDataList.Add(new HTextData(smearData.Text, true, row, column, HColorEnum.cyan, 20, false, false));
            //返回涂抹文本集合
            return hTextDataList;
        }

        /// <summary>
        /// 设置作业类型
        /// </summary>
        /// <param name="type">涂抹类型</param>
        public void SetJobType(SmearTypeEnum type)
        {
            //更新涂抹类型
            smearType = type;
        }

        /// <summary>
        /// 工作完成
        /// </summary>
        public void WorkDone()
        {
            //更新涂抹类型
            smearType = SmearTypeEnum.None;
        }

        /// <summary>
        /// 创建工具区域
        /// </summary>
        /// <returns></returns>
        private void CreateToolRegion(double mouseRow, double mouseColumn)
        {
            //获取工具大小
            double toolSize = smearData.GetToolSize();
            //判断工具形状
            switch (smearData.ToolShape)
            {
                case ToolShapeEnum.方形:
                    //创建工具区域
                    toolRegion.GenRectangle2(mouseRow, mouseColumn, 0, toolSize, toolSize);
                    break;
                case ToolShapeEnum.圆形:
                    //创建工具区域
                    toolRegion.GenCircle(mouseRow, mouseColumn, toolSize);
                    break;
            }
        }

        /// <summary>
        /// 设置涂抹数据
        /// </summary>
        /// <param name="hRegion">涂抹区域数据</param>
        /// <returns>true:设置成功 false:设置失败</returns>
        public bool SetSmearData(HRegion hRegion)
        {
            //涂抹区域数据未实例化
            if (hRegion == null)
            {
                //返回设置失败
                return false;
            }
            //涂抹区域未初始化
            if (!hRegion.IsInitialized())
            {
                //创建涂抹空区域
                smearData.hRegion.GenEmptyRegion();
            }
            else
            {
                //复制涂抹区域
                smearData.hRegion = hRegion.CopyObj(1, 1);
            }
            //触发刷新显示事件
            RefreshDisplay?.Invoke();
            //返回设置成功
            return true;
        }

        /// <summary>
        /// 获取涂抹区域
        /// </summary>
        /// <returns>涂抹区域</returns>
        public HRegion GetRegion()
        {
            //区域已实例化 && 区域已初始化 && 区域存在有效数据
            if (smearData.hRegion != null && smearData.hRegion.IsInitialized() && smearData.hRegion.CountObj() > 0)
            {
                //返回复制的区域对象
                return smearData.hRegion.CopyObj(1, -1);
            }
            else
            {
                //返回一个空区域
                HRegion hRegion = new HRegion();
                hRegion.GenEmptyObj();
                return hRegion;
            }
        }
    }
}
