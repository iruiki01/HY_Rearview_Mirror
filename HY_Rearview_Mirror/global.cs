using dataGridView1每行改变颜色;
using Helper;
using HY_Rearview_Mirror.CAN;
using HY_Rearview_Mirror.Functions;
using HY_Rearview_Mirror.Functions.Colors;
using HY_Rearview_Mirror.InterfaceTool;
using HY_Rearview_Mirror.TypeManager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HY_Rearview_Mirror
{
    #region 工作线程参数结构体
    /// <summary>
    /// 工作线程参数类
    /// </summary>
    public class WorkStation
    {
        public bool[] Run_Flag = new bool[10];
        public int[] Run_Step = new int[10];
        public Delay[] delay = new Delay[10];
        public Timeout[] timeout = new Timeout[10];
        public bool[] Alarm_Flag = new bool[10];
        public bool[] Result = new bool[10];
        public bool[] WorkOut_Flag = new bool[10];
        public string[] Message = new string[10];
        public void Init()
        {
            for (int i = 0; i < 10; i++)
            {
                Run_Step[i] = 0;
                Run_Flag[i] = false;
                Alarm_Flag[i] = false;
                Result[i] = false;
                WorkOut_Flag[i] = false;

                delay[i] = new Delay();
                timeout[i] = new Timeout();
            }
        }
    }
    #endregion
    internal static class global
    {
        // 全局数据存储
        public static List<RootNode3Data> RootNodeDataList = new List<RootNode3Data>();
        public static int indexSelect = -1;

        public static Dictionary<string, IPlugin> _loadedPlugins = new Dictionary<string, IPlugin>();
        public static TypeFactory factory = new TypeFactory();
        public static List<PluginDescribe> _PluginDescribeList = new List<PluginDescribe>();
        public static CanTool can1Service = new CanTool(CanChannel.CAN1);
        /// <summary>
        /// 相机管理类
        /// </summary>
        public static CameraController cameraController = new CameraController();
        /// <summary>
        /// 直流电源通讯类
        /// </summary>
        public static IT6722ATool iT6722ATool = null;
        /// <summary>
        /// 固纬万用表串口通信
        /// </summary>
        public static RobustSerialPort serialHelper_GW = null;
        /// <summary>
        /// 感光标定光源串口通信
        /// </summary>
        public static RobustSerialPort serialHelper_Light = null;
        /// <summary>
        /// 光研成像色度仪
        /// </summary>
        public static GYCamTool gYCamTool = null;

        public static readonly string FilePath = AppDomain.CurrentDomain.BaseDirectory + @"Resource\Data";
        public static readonly string UserFilePath = AppDomain.CurrentDomain.BaseDirectory + @"Resource\UserData";

        public static string ProjectName = "";


        // 先初始化为只有文件日志（null 表示没有 UI）
        public static ModuleLogger writeLogProduce = new ModuleLogger("DataProduce", "Test", null, 30);
        public static ModuleLogger writeLogSystem = new ModuleLogger("DataSystem", "Test", null, 30);

        // PLC通信
        public static InovanceH5UTcpTool inovanceH5UTcpTool = new InovanceH5UTcpTool("192.168.10.20");

        public static EventManager G_eventManager = new EventManager();

        public static RFIDHelper rFIDHelper = new RFIDHelper();

        /// <summary>
        /// 控制中盛继电器
        /// </summary>
        public static ModbusRtuHelper ZS_ModbusRtuHelpe = null;
        /// <summary>
        /// 光谱仪
        /// </summary>
        public static Use_EK2000Pro use_EK2000Pro = null;
        /// <summary>
        /// 获取颜色
        /// </summary>
        public static RGB rGB = new RGB();
        /// <summary>
        /// 颜色判断器
        /// </summary>
        public static RgbColorMatcher rgbColorMatcher = new RgbColorMatcher();
    }
    // 子节点数据类
    public class ChildNodeData
    {
        public string Name { get; set; }
        public ITestParam TestParam { get; set; }
        //[JsonIgnore]
        public IPlugin Plugin { get; set; }
    }

    // 根节点数据类
    public class RootNodeData
    {
        public List<ChildNodeData> Children { get; set; } = new List<ChildNodeData>();
    }
    // 3个根节点数据类
    public class RootNode3Data
    {
        public string Name { get; set; }
        public bool IsUse { get; set; }
        public RootNodeData RootData1 = new RootNodeData();
        public RootNodeData RootData2 = new RootNodeData();
        public RootNodeData RootData3 = new RootNodeData();
    }

    /// <summary>
    /// 插件描述类
    /// </summary>
    public class PluginDescribe
    {
        /// <summary>
        /// 模组名称
        /// </summary>
        public string 模组名称 { get; set; }
        /// <summary>
        /// 插件ID唯一标识（必须说明其功能），也是功能类型
        /// </summary>
        public string 功能类型 { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string 描述 { get; set; }
    }
}
