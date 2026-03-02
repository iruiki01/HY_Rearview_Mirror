using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Helper.Motion
{
    /// <summary>
    /// 运动控制卡配置
    /// </summary>
    public class MotionCardConfig
    {
        /// <summary>
        /// 控制卡类型
        /// </summary>
        [JsonProperty("cardType")]
        public CardType CardType { get; set; }

        /// <summary>
        /// 控制卡ID
        /// </summary>
        [JsonProperty("cardId")]
        public int CardId { get; set; }

        /// <summary>
        /// 轴配置列表
        /// </summary>
        [JsonProperty("axes")]
        public List<AxisConfig> Axes { get; set; } = new List<AxisConfig>();

        /// <summary>
        /// IO配置列表
        /// </summary>
        [JsonProperty("ios")]
        public List<IOConfig> IOs { get; set; } = new List<IOConfig>();

        /// <summary>
        /// 配置版本
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; } = "1.0";
    }
}
