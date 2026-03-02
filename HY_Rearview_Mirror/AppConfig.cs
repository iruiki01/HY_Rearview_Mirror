using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helper;
using Newtonsoft.Json;

namespace HY_Rearview_Mirror
{
    /// <summary>
    /// 应用程序配置
    /// </summary>
    public class AppConfig
    {
        #region 单例
        /// <summary>
        /// 单例
        /// </summary>
        private static AppConfig instance = null;
        private static readonly object syncRoot = new object();
        public static AppConfig GetInstance()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new AppConfig();
                }
            }
            return instance;
        }
        #endregion
        public ConfigData _config;
        private readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        // 加载配置
        public void LoadConfig()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    var json = File.ReadAllText(ConfigPath);
                    _config = JsonConvert.DeserializeObject<ConfigData>(json);
                }
                else
                {
                    // 使用默认配置
                    _config = new ConfigData();
                    SaveConfig();
                }
            }
            catch (Exception ex)
            {
                global.writeLogSystem.Error($"加载配置文件失败: {ex.Message}");
                _config = new ConfigData();
            }
        }

        // 保存配置
        public void SaveConfig()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_config, Formatting.Indented);
                File.WriteAllText(ConfigPath, json);
            }
            catch (Exception ex)
            {
                global.writeLogSystem.Error($"保存配置文件失败: {ex.Message}");
            }
        }
    }
    // 配置数据结构
    public class ConfigData : INotifyPropertyChanged
    {
        private SynchronizationContext _syncContext;
        
        public ConfigData()
        {
            // 尝试获取当前同步上下文
            _syncContext = SynchronizationContext.Current;
        }

        // 提供方法来设置同步上下文
        public void SetSynchronizationContext(SynchronizationContext context)
        {
            _syncContext = context;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                // 如果有同步上下文且当前不在UI线程
                if (_syncContext != null && _syncContext != SynchronizationContext.Current)
                {
                    _syncContext.Post(_ => handler(this, new PropertyChangedEventArgs(propertyName)), null);
                }
                else
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        public BindingList<UartData> uartData = new BindingList<UartData>();
        public BindingList<CanData> canData = new BindingList<CanData>();
        public BindingList<ZhongshengData> zhongshengData = new BindingList<ZhongshengData>();

        public int Station_No {  get; set; }
        public string _IsRunType { get; set; } = "不重测";
        public bool IsTestFlag { get; set; } = true;

        public string IsRunType
        {
            get => _IsRunType;
            set
            {
                _IsRunType = value;
                OnPropertyChanged();
            }
        }
        public string _SN { get; set; }

        public string SN
        {
            get => _SN;
            set
            {
                _SN = value;
                OnPropertyChanged();
            }
        }

        public string Station_Name { get; set; } = "性能测试";

    }

    public class UartData
    {
        public string 名称 { get; set; }
        public string 值 { get; set; }
    }
    public class CanData
    {
        public string 名称 { get; set; }
        public string 值 { get; set; }
    }
    public class ZhongshengData
    {
        public string 名称 { get; set; }
        public string 值 { get; set; }
        public string 操作 { get; set; }
    }
}

/***
          // 如果对象不是在UI线程中创建的，可以这样设置同步上下文
            _list.SetSynchronizationContext(SynchronizationContext.Current);
            // 3. 简单绑定
            textBox1.DataBindings.Add("Text", _list, "Name", true,DataSourceUpdateMode.OnPropertyChanged);          
 ***/

