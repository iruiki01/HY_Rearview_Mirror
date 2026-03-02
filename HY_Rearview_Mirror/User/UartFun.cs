using HY_Rearview_Mirror.InterfaceTool;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HY_Rearview_Mirror.User
{
    public class UartFunParam : ITestParam
    {
        public List<string> dataList_Uart{ get; set; }
        /// <summary>
        /// 运行结果
        /// </summary>
        public bool RunResult { get; set; }
        /// <summary>
        /// 转换数据类型
        /// </summary>
        public string ChangeDataType { get; set; }
        /// <summary>
        /// 判断类型
        /// </summary>
        public string JudgeType { get; set; }
        /// <summary>
        /// 判断值
        /// </summary>
        public string JudgeValue { get; set; }

        public UartFunParam()
        {
            dataList_Uart = new List<string>();
            RunResult = false;
            ChangeDataType = "";
            JudgeType = "";
            JudgeValue = "";
        }
    }
    /// <summary>
    /// 产品串口协议
    /// </summary>
    public class UartFun : IPlugin
    {
        public string PluginId =>"串口通信";

        public string moduleName =>"通信";

        public string DescribeMessage => "用于串口通信";

        [JsonIgnore]
        public UartForm uartForm = null;  // 在基类中存储窗体

        public UartFunParam uartFunParam { get; set; }

        public string VersionStr1 = "";
        public string VersionStr => VersionStr1;

        private ManualResetEventSlim _flagEvent;

        private string g_JudgeValue = "";

        public Form CreateUI(ITestParam testParam, string VersionStr = "")
        {
            uartForm = new UartForm();
            uartForm.eventManager.AddListener("发送界面参数", new Action<List<object>>(SetDataForm_OrderCompleted));
            uartForm.eventManager.AddListener("关闭窗体", new Action(FormClosing));
            uartForm.eventManager.AddListener("运行", new Func<Dictionary<string, object>>(Run));

            uartForm.SetDataToForm(testParam, VersionStr);
            if (testParam != null)
            {
                uartFunParam = testParam as UartFunParam;
            }           
            uartFunParam = new UartFunParam();
            return uartForm;
        }

        private void FormClosing()
        {
            uartForm.eventManager.Clear();
        }

        private void SetDataForm_OrderCompleted(List<object> list)
        {

        }

        public void Dispose()
        {
           
        }

        public ITestParam GetData()
        {
            return uartFunParam;
        }

        public string GetVersion()
        {
            return VersionStr;
        }

        public void Initialize()
        {

        }

        public Dictionary<string, object> Run()
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();

            return keyValuePairs;
        }
        public Dictionary<string, object> Run(ITestParam testParam)
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();

            return keyValuePairs;
        }
    }
}
