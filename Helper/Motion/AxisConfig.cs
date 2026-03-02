using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Helper.Motion
{
    /// <summary>
    /// 轴配置信息
    /// </summary>
    public class AxisConfig
    {
        /// <summary>
        /// 轴名称
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 轴编号
        /// </summary>
        [JsonProperty("index")]
        public int Index { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 最大速度
        /// </summary>
        [JsonProperty("maxVelocity")]
        public double MaxVelocity { get; set; }

        /// <summary>
        /// 最大加速度
        /// </summary>
        [JsonProperty("maxAcceleration")]
        public double MaxAcceleration { get; set; }

        /// <summary>
        /// 脉冲当量（每个脉冲对应的移动距离）
        /// </summary>
        [JsonProperty("pulseEquivalent")]
        public double PulseEquivalent { get; set; } = 1.0;
    }
}
