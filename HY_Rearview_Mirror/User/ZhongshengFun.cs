using HY_Rearview_Mirror.InterfaceTool;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HY_Rearview_Mirror.User
{
    public class ZhongshengParam : ITestParam
    {
        public List<string > RunData { get; set; }// （0：名称）（1：地址）（2：操作）
        /// <summary>
        /// 运行结果
        /// </summary>
        public bool RunResult { get; set; }

        public ZhongshengParam()
        {
            RunResult = false;

            List<string> RunData = new List<string>();
        }
    }

    internal class ZhongshengFun : IPlugin
    {
        public string PluginId => "中盛继电器";

        public string moduleName => "控制";

        public string DescribeMessage => "用控制硬件";

        public string VersionStr1 = "";
        public string VersionStr => VersionStr1;


        [JsonIgnore]
        public ZhongshengForm zhongshengForm = null;

        public ZhongshengParam zhongshengParam = new ZhongshengParam();

        public Form CreateUI(ITestParam testParam, string VersionStr = "")
        {
            zhongshengForm = new ZhongshengForm();
            zhongshengForm.eventManager.AddListener("发送界面参数", new Action<List<object>>(SetDataForm_OrderCompleted));
            zhongshengForm.eventManager.AddListener("关闭窗体", new Action(FormClosing));
            zhongshengForm.eventManager.AddListener("运行", new Func<Dictionary<string, object>>(Run));
            zhongshengForm.SetDataToForm(testParam, VersionStr);

            if (testParam != null)
                zhongshengParam = testParam as ZhongshengParam;

            return zhongshengForm;
        }

        private void FormClosing()
        {
            zhongshengForm.eventManager.Clear();
        }

        private void SetDataForm_OrderCompleted(List<object> list)
        {
            VersionStr1 = (string)list[0];
            zhongshengParam.RunData = (List<string>)list[1];
        }

        public void Dispose()
        {
        }

        public ITestParam GetData()
        {
            return zhongshengParam;
        }

        public string GetVersion()
        {
            return VersionStr;
        }

        public void Initialize()
        {
           
        }

        public Dictionary<string, object> Run(ITestParam testParam)
        {
            var p = testParam as ZhongshengParam;

            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();

            string str = "s=1;";
            str = str + p.RunData[1];
            //global.ZS_ModbusRtuHelpe.Write("s=1;0", 2);
            ushort u = Convert.ToUInt16(p.RunData[2]);
            global.ZS_ModbusRtuHelpe.Write(str, u);

            keyValuePairs.Add("描述", "执行成功！");
            keyValuePairs.Add("结果", true);

            return keyValuePairs;
        }
        public Dictionary<string, object> Run()
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();

            string str = "s=1;";
            str = str + zhongshengParam.RunData[1];
            //global.ZS_ModbusRtuHelpe.Write("s=1;0", 2);
            ushort u = Convert.ToUInt16(zhongshengParam.RunData[2]);
            global.ZS_ModbusRtuHelpe.Write(str, u);

            keyValuePairs.Add("描述", "执行成功！");
            keyValuePairs.Add("结果", true);

            return keyValuePairs;
        }
    }
}
