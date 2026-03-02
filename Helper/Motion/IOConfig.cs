using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Helper.Motion
{
    /// <summary>
    /// IO点配置信息
    /// </summary>
    public class IOConfig
    {
        /// <summary>
        /// IO点名称
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// IO点编号
        /// </summary>
        [JsonProperty("index")]
        public int Index { get; set; }

        /// <summary>
        /// IO类型：Input/Output
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 描述信息
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
