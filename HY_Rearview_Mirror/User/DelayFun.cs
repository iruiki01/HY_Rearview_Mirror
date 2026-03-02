using Helper;
using HY_Rearview_Mirror.InterfaceTool;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HY_Rearview_Mirror.User
{
    public class DelayFunTestParam : ITestParam
    {
        /// <summary>
        /// 延时值
        /// </summary>
        public double seconds { get; set; }
        /// <summary>
        /// 运行结果
        /// </summary>
        public bool RunResult { get; set; }

        public DelayFunTestParam()
        {
            seconds = 1000.0;
            RunResult = false;
        }
    }
    public class DelayFun : IPlugin
    {
        public string PluginId => "延时功能";

        public string moduleName => "延时";

        public string DescribeMessage => "用于运行延时";

        public string VersionStr1 = "";
        public string VersionStr => VersionStr1;

        [JsonIgnore]
        public DelayFunSetForm delayFunSetForm = null;

        public DelayFunTestParam delayFunParam = new DelayFunTestParam();
        public Stopwatch stopwatch = new Stopwatch();
        public DelayFun()
        {
        }
        public Form CreateUI(ITestParam testParam, string VersionStr = "")
        {
            delayFunSetForm = new DelayFunSetForm();
            delayFunSetForm.eventManager.AddListener("发送界面参数", new Action<List<object>>(SetDataForm_OrderCompleted));
            delayFunSetForm.eventManager.AddListener("关闭窗体", new Action(FormClosing));
            delayFunSetForm.SetDataToForm(testParam, VersionStr);

            if(testParam != null)
                delayFunParam = testParam as DelayFunTestParam;

            return delayFunSetForm;
        }

        public void FormClosing()
        {
            delayFunSetForm.eventManager.Clear();
        }

        public void Dispose()
        {
            //GC.SuppressFinalize(this);
        }

        public ITestParam GetData()
        {
            return delayFunParam;
        }

        public void Initialize()
        {
        }

        public Dictionary<string, object> Run(ITestParam testParam)
        {
            var p = (DelayFunTestParam)testParam;

            stopwatch = Stopwatch.StartNew(); // 开始计时
            while (stopwatch.ElapsedMilliseconds < p.seconds)
            {
                //执行耗时
            }
            stopwatch.Stop();

            p.RunResult = true;
            return new Dictionary<string, object>
            {
                { "延时时间", p.seconds },
            };
        }
        public string GetVersion()
        {
            return VersionStr;
        }
        private void SetDataForm_OrderCompleted(List<object> e)
        {
            delayFunParam.seconds = Convert.ToDouble(e[0]);
            VersionStr1 = Convert.ToString(e[1]);
        }
    }
}
