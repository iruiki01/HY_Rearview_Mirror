using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Helper.Motion
{
    /// <summary>
    /// 配置管理器
    /// 负责加载和管理运动控制卡的配置信息
    /// </summary>
    public class ConfigManager
    {
        private static ConfigManager _instance;
        private MotionCardConfig _config;

        /// <summary>
        /// 单例实例
        /// </summary>
        //public static ConfigManager Instance => _instance ??= new ConfigManager();
        private static readonly Lazy<ConfigManager> _lazyInstance = new Lazy<ConfigManager>(() => new ConfigManager());
        public static ConfigManager Instance => _lazyInstance.Value;

        /// <summary>
        /// 当前配置
        /// </summary>
        public MotionCardConfig Config => _config;

        /// <summary>
        /// 所有轴名称数组
        /// </summary>
        public string[] AxisNames => _config?.Axes?.Where(a => a.Enabled).Select(a => a.Name).ToArray() ?? Array.Empty<string>();

        /// <summary>
        /// 所有IO名称数组
        /// </summary>
        public string[] IONames => _config?.IOs?.Where(io => io.Enabled).Select(io => io.Name).ToArray() ?? Array.Empty<string>();

        private ConfigManager() { }

        /// <summary>
        /// 从文件加载配置
        /// </summary>
        /// <param name="configFilePath">配置文件路径</param>
        /// <returns>加载成功返回true，失败返回false</returns>
        public bool LoadConfig(string configFilePath)
        {
            try
            {
                if (!File.Exists(configFilePath))
                {
                    Console.WriteLine($"配置文件不存在: {configFilePath}");
                    return false;
                }

                string jsonContent = File.ReadAllText(configFilePath);
                _config = JsonConvert.DeserializeObject<MotionCardConfig>(jsonContent);

                Console.WriteLine($"配置文件加载成功: {configFilePath}");
                Console.WriteLine($"控制卡类型: {_config.CardType}, 轴数量: {_config.Axes.Count}, IO数量: {_config.IOs.Count}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载配置文件失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 保存配置到文件
        /// </summary>
        /// <param name="configFilePath">配置文件路径</param>
        /// <returns>保存成功返回true，失败返回false</returns>
        public bool SaveConfig(string configFilePath)
        {
            try
            {
                string jsonContent = JsonConvert.SerializeObject(_config, Formatting.Indented);
                File.WriteAllText(configFilePath, jsonContent);

                Console.WriteLine($"配置文件保存成功: {configFilePath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存配置文件失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 根据轴名称获取轴编号
        /// </summary>
        /// <param name="axisName">轴名称</param>
        /// <returns>轴编号，未找到返回-1</returns>
        public int GetAxisIndex(string axisName)
        {
            var axis = _config?.Axes?.FirstOrDefault(a => a.Name == axisName && a.Enabled);
            return axis?.Index ?? -1;
        }

        /// <summary>
        /// 根据轴编号获取轴名称
        /// </summary>
        /// <param name="axisIndex">轴编号</param>
        /// <returns>轴名称，未找到返回空字符串</returns>
        public string GetAxisName(int axisIndex)
        {
            var axis = _config?.Axes?.FirstOrDefault(a => a.Index == axisIndex && a.Enabled);
            return axis?.Name ?? string.Empty;
        }

        /// <summary>
        /// 根据IO名称获取IO编号
        /// </summary>
        /// <param name="ioName">IO名称</param>
        /// <returns>IO编号，未找到返回-1</returns>
        public int GetIOIndex(string ioName)
        {
            var io = _config?.IOs?.FirstOrDefault(i => i.Name == ioName && i.Enabled);
            return io?.Index ?? -1;
        }

        /// <summary>
        /// 根据IO编号获取IO名称
        /// </summary>
        /// <param name="ioIndex">IO编号</param>
        /// <returns>IO名称，未找到返回空字符串</returns>
        public string GetIOName(int ioIndex)
        {
            var io = _config?.IOs?.FirstOrDefault(i => i.Index == ioIndex && i.Enabled);
            return io?.Name ?? string.Empty;
        }

        /// <summary>
        /// 获取所有轴的配置信息
        /// </summary>
        /// <returns>轴配置字典（名称->配置）</returns>
        public Dictionary<string, AxisConfig> GetAllAxisConfigs()
        {
            return _config?.Axes?.Where(a => a.Enabled).ToDictionary(a => a.Name, a => a) ?? new Dictionary<string, AxisConfig>();
        }

        /// <summary>
        /// 获取所有IO的配置信息
        /// </summary>
        /// <returns>IO配置字典（名称->配置）</returns>
        public Dictionary<string, IOConfig> GetAllIOConfigs()
        {
            return _config?.IOs?.Where(io => io.Enabled).ToDictionary(io => io.Name, io => io) ?? new Dictionary<string, IOConfig>();
        }

        /// <summary>
        /// 创建默认配置（用于示例或初始化）
        /// </summary>
        public void CreateDefaultConfig()
        {
            _config = new MotionCardConfig
            {
                CardType = CardType.LeadShine,
                CardId = 0,
                Axes = new List<AxisConfig>
                {
                    new AxisConfig { Name = "X轴", Index = 0, Enabled = true, MaxVelocity = 1000, MaxAcceleration = 100, PulseEquivalent = 0.001 },
                    new AxisConfig { Name = "Y轴", Index = 1, Enabled = true, MaxVelocity = 1000, MaxAcceleration = 100, PulseEquivalent = 0.001 },
                    new AxisConfig { Name = "Z轴", Index = 2, Enabled = true, MaxVelocity = 800, MaxAcceleration = 80, PulseEquivalent = 0.001 }
                },
                IOs = new List<IOConfig>
                {
                    new IOConfig { Name = "急停输入", Index = 0, Type = "Input", Enabled = true, Description = "急停按钮信号" },
                    new IOConfig { Name = "启动按钮", Index = 1, Type = "Input", Enabled = true, Description = "启动按钮信号" },
                    new IOConfig { Name = "停止按钮", Index = 2, Type = "Input", Enabled = true, Description = "停止按钮信号" },
                    new IOConfig { Name = "运行指示灯", Index = 0, Type = "Output", Enabled = true, Description = "设备运行指示灯" },
                    new IOConfig { Name = "报警指示灯", Index = 1, Type = "Output", Enabled = true, Description = "设备报警指示灯" }
                }
            };
        }
    }
}
