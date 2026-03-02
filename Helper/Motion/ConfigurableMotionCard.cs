using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Motion
{
    /// <summary>
    /// 配置化的运动控制卡管理器
    /// 结合配置文件和运动控制卡功能
    /// </summary>
    public class ConfigurableMotionCard
    {
        private readonly IMotionControlCard _motionCard;
        private readonly ConfigManager _configManager;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configManager">配置管理器</param>
        public ConfigurableMotionCard(ConfigManager configManager)
        {
            _configManager = configManager;

            // 根据配置创建控制卡实例
            var config = _configManager.Config;
            _motionCard = MotionCardFactory.CreateCard(config.CardType, config.CardId);
        }

        /// <summary>
        /// 初始化控制卡
        /// </summary>
        /// <returns>初始化成功返回true，失败返回false</returns>
        public bool Initialize()
        {
            if (!_motionCard.Initialize())
                return false;

            // 根据配置初始化各轴参数
            var axisConfigs = _configManager.GetAllAxisConfigs();
            foreach (var config in axisConfigs.Values)
            {
                _motionCard.SetPulseMode(config.Index, 0);
                _motionCard.SetVelocity(config.Index, config.MaxVelocity * 0.5); // 使用最大速度的一半作为默认速度
                _motionCard.SetAcceleration(config.Index, config.MaxAcceleration);
                _motionCard.SetDeceleration(config.Index, config.MaxAcceleration);
            }

            return true;
        }

        /// <summary>
        /// 关闭控制卡
        /// </summary>
        /// <returns>关闭成功返回true，失败返回false</returns>
        public bool Close()
        {
            return _motionCard.Close();
        }

        /// <summary>
        /// 根据轴名称进行相对运动
        /// </summary>
        /// <param name="axisName">轴名称</param>
        /// <param name="distance">移动距离（物理单位）</param>
        /// <returns>启动运动成功返回true，失败返回false</returns>
        public bool MoveRelativeByName(string axisName, double distance)
        {
            int axisIndex = _configManager.GetAxisIndex(axisName);
            if (axisIndex == -1)
            {
                Console.WriteLine($"未找到轴: {axisName}");
                return false;
            }

            var axisConfig = _configManager.GetAllAxisConfigs()[axisName];
            int pulseDistance = (int)(distance / axisConfig.PulseEquivalent);

            return _motionCard.MoveRelative(axisIndex, pulseDistance);
        }

        /// <summary>
        /// 根据轴名称进行绝对运动
        /// </summary>
        /// <param name="axisName">轴名称</param>
        /// <param name="position">目标位置（物理单位）</param>
        /// <returns>启动运动成功返回true，失败返回false</returns>
        public bool MoveAbsoluteByName(string axisName, double position)
        {
            int axisIndex = _configManager.GetAxisIndex(axisName);
            if (axisIndex == -1)
            {
                Console.WriteLine($"未找到轴: {axisName}");
                return false;
            }

            var axisConfig = _configManager.GetAllAxisConfigs()[axisName];
            int pulsePosition = (int)(position / axisConfig.PulseEquivalent);

            return _motionCard.MoveAbsolute(axisIndex, pulsePosition);
        }

        /// <summary>
        /// 根据轴名称停止运动
        /// </summary>
        /// <param name="axisName">轴名称</param>
        /// <param name="immediate">是否立即停止</param>
        /// <returns>停止成功返回true，失败返回false</returns>
        public bool StopByName(string axisName, bool immediate = true)
        {
            int axisIndex = _configManager.GetAxisIndex(axisName);
            if (axisIndex == -1)
            {
                Console.WriteLine($"未找到轴: {axisName}");
                return false;
            }

            return _motionCard.Stop(axisIndex, immediate);
        }

        /// <summary>
        /// 根据轴名称获取当前位置（物理单位）
        /// </summary>
        /// <param name="axisName">轴名称</param>
        /// <returns>当前位置（物理单位）</returns>
        public double GetCurrentPositionByName(string axisName)
        {
            int axisIndex = _configManager.GetAxisIndex(axisName);
            if (axisIndex == -1)
            {
                Console.WriteLine($"未找到轴: {axisName}");
                return 0;
            }

            var axisConfig = _configManager.GetAllAxisConfigs()[axisName];
            int pulsePosition = _motionCard.GetCurrentPosition(axisIndex);

            return pulsePosition * axisConfig.PulseEquivalent;
        }

        /// <summary>
        /// 根据IO名称读取输入状态
        /// </summary>
        /// <param name="ioName">IO名称</param>
        /// <returns>输入状态：true-高电平，false-低电平</returns>
        public bool ReadInputByName(string ioName)
        {
            int ioIndex = _configManager.GetIOIndex(ioName);
            if (ioIndex == -1)
            {
                Console.WriteLine($"未找到IO: {ioName}");
                return false;
            }

            var ioConfig = _configManager.GetAllIOConfigs()[ioName];
            if (ioConfig.Type != "Input")
            {
                Console.WriteLine($"IO点 {ioName} 不是输入类型");
                return false;
            }

            return _motionCard.ReadInput(ioIndex);
        }

        /// <summary>
        /// 根据IO名称写入输出状态
        /// </summary>
        /// <param name="ioName">IO名称</param>
        /// <param name="state">输出状态：true-高电平，false-低电平</param>
        /// <returns>写入成功返回true，失败返回false</returns>
        public bool WriteOutputByName(string ioName, bool state)
        {
            int ioIndex = _configManager.GetIOIndex(ioName);
            if (ioIndex == -1)
            {
                Console.WriteLine($"未找到IO: {ioName}");
                return false;
            }

            var ioConfig = _configManager.GetAllIOConfigs()[ioName];
            if (ioConfig.Type != "Output")
            {
                Console.WriteLine($"IO点 {ioName} 不是输出类型");
                return false;
            }

            return _motionCard.WriteOutput(ioIndex, state);
        }

        /// <summary>
        /// 获取所有轴名称
        /// </summary>
        /// <returns>轴名称数组</returns>
        public string[] GetAxisNames()
        {
            return _configManager.AxisNames;
        }

        /// <summary>
        /// 获取所有IO名称
        /// </summary>
        /// <returns>IO名称数组</returns>
        public string[] GetIONames()
        {
            return _configManager.IONames;
        }

        /// <summary>
        /// 获取输入IO名称
        /// </summary>
        /// <returns>输入IO名称数组</returns>
        public string[] GetInputNames()
        {
            return _configManager.GetAllIOConfigs()
                .Where(kv => kv.Value.Type == "Input" && kv.Value.Enabled)
                .Select(kv => kv.Key)
                .ToArray();
        }

        /// <summary>
        /// 获取输出IO名称
        /// </summary>
        /// <returns>输出IO名称数组</returns>
        public string[] GetOutputNames()
        {
            return _configManager.GetAllIOConfigs()
                .Where(kv => kv.Value.Type == "Output" && kv.Value.Enabled)
                .Select(kv => kv.Key)
                .ToArray();
        }
    }
}
