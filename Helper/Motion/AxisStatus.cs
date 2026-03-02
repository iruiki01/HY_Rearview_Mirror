using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Motion
{
    /// <summary>
    /// 轴状态信息
    /// </summary>
    public class AxisStatus
    {
        /// <summary>
        /// 轴是否在运动
        /// </summary>
        public bool IsMoving { get; set; }

        /// <summary>
        /// 正限位信号
        /// </summary>
        public bool PositiveLimit { get; set; }

        /// <summary>
        /// 负限位信号
        /// </summary>
        public bool NegativeLimit { get; set; }

        /// <summary>
        /// 原点信号
        /// </summary>
        public bool HomeSignal { get; set; }

        /// <summary>
        /// 报警状态
        /// </summary>
        public bool Alarm { get; set; }

        /// <summary>
        /// 轴使能状态
        /// </summary>
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// 控制卡类型枚举
    /// </summary>
    public enum CardType
    {
        /// <summary>
        /// 雷赛控制卡
        /// </summary>
        LeadShine,

        /// <summary>
        /// 正运动控制卡
        /// </summary>
        ZMotion
    }
}
