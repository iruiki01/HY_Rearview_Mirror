using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hWindowTool
{
    /// <summary>
    /// 涂抹行为统一接口
    /// </summary>
    public interface ISmearAction
    {
        /// <summary>
        /// 刷新显示事件
        /// </summary>
        event Action RefreshDisplay;

        /// <summary>
        /// 涂抹工作
        /// </summary>
        /// <param name="mouseRow">鼠标当前位置Row</param>
        /// <param name="mouseColumn">鼠标当前位置Column</param>
        void SmearWork(double mouseRow, double mouseColumn);

        /// <summary>
        /// 获取涂抹区域
        /// </summary>
        /// <param name="mouseRow">鼠标当前位置Row</param>
        /// <param name="mouseColumn">鼠标当前位置Column</param>
        /// <returns>涂抹区域集合</returns>
        List<HObjData> GetSmearHRegion(double mouseRow, double mouseColumn);

        /// <summary>
        /// 获取涂抹文本
        /// </summary>
        /// <returns>涂抹文本集合</returns>
        List<HTextData> GetSmearHTextList();

        /// <summary>
        /// 设置作业类型
        /// </summary>
        /// <param name="type">涂抹类型</param>
        void SetJobType(SmearTypeEnum type);

        /// <summary>
        /// 工作完成
        /// </summary>
        void WorkDone();

        /// <summary>
        /// 设置涂抹数据
        /// </summary>
        /// <param name="hRegion">涂抹区域数据</param>
        /// <returns>true:设置成功 false:设置失败</returns>
        bool SetSmearData(HRegion hRegion);

        /// <summary>
        /// 获取涂抹区域
        /// </summary>
        /// <returns>涂抹区域</returns>
        HRegion GetRegion();
    }
}
