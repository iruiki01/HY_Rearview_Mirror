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
    public class EK2000ProParam : ITestParam
    {
        ///// <summary>
        ///// 判断类型
        ///// </summary>
        //public string JudgeType { get; set; }
        ///// <summary>
        ///// 判断值
        ///// </summary>
        //public string JudgeValue { get; set; }
        
        /// <summary>
        /// 积分时间
        /// </summary>
        public string IntegralTime { get; set; }
        /// <summary>
        /// 高反值
        /// </summary>
        public string ReflectivityValue_Hight {  get; set; }
        /// <summary>
        /// 低反值
        /// </summary>
        public string ReflectivityValue_Low { get; set; }
        /// <summary>
        /// 运行结果
        /// </summary>
        public bool RunResult { get; set; }

        public EK2000ProParam()
        {
            IntegralTime = "";
            ReflectivityValue_Hight = "";
            ReflectivityValue_Low = "";
            RunResult = false;
        }
    }
    public class EK2000ProFun : IPlugin
    {
        public string PluginId => "反射率测试";

        public string moduleName => "仪器";

        public string DescribeMessage => "用于反射率测试";

        public string VersionStr1 = "";
        public string VersionStr => VersionStr1;
        [JsonIgnore]
        public EK2000ProForm eK2000ProForm = null;
        public EK2000ProParam eK2000ProParam = new EK2000ProParam();

        public Form CreateUI(ITestParam testParam, string VersionStr = "")
        {
            eK2000ProForm = new EK2000ProForm();

            eK2000ProForm.eventManager.AddListener("发送界面参数", new Action<List<object>>(SetDataForm_OrderCompleted));
            eK2000ProForm.eventManager.AddListener("关闭窗体", new Action(FormClosing));
            eK2000ProForm.eventManager.AddListener("运行", new Func<Dictionary<string, object>>(Run));

            eK2000ProForm.SetDataToForm(testParam, VersionStr);
            if (testParam != null)
                eK2000ProParam = testParam as EK2000ProParam;

            return eK2000ProForm;
        }

        private void FormClosing()
        {
            eK2000ProForm.eventManager.Clear();
        }

        private void SetDataForm_OrderCompleted(List<object> list)
        {
            VersionStr1 = list[0].ToString();
            eK2000ProParam.IntegralTime = list[1].ToString();
            eK2000ProParam.ReflectivityValue_Hight = list[2].ToString();
            eK2000ProParam.ReflectivityValue_Low = list[3].ToString();
        }

        public void Dispose()
        {

        }

        public ITestParam GetData()
        {
            return eK2000ProParam;
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
            var p = (EK2000ProParam)testParam;

            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();

            return keyValuePairs;
        }

        public Dictionary<string, object> Run()
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();

            return keyValuePairs;
        }
    }
}
