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
    public class IT6722AParam : ITestParam
    {
        /// <summary>
        /// 运行结果
        /// </summary>
        public bool RunResult { get; set; }
        /// <summary>
        /// 转换数据类型
        /// </summary>
        //public string ChangeDataType { get; set; }
        /// <summary>
        /// 判断类型
        /// </summary>
        public string JudgeType { get; set; }
        /// <summary>
        /// 判断值
        /// </summary>
        public string JudgeValue { get; set; }

        public IT6722AParam()
        {
            RunResult = false;
            //ChangeDataType = "";
            JudgeType = "";
            JudgeValue = "";
        }
    }
    /// <summary>
    /// IT6722A直流电源
    /// </summary>
    public class IT6722AFun : IPlugin
    {
        public string PluginId => "IT6722A直流电源";

        public string moduleName => "仪器";

        public string DescribeMessage => "用于直流电源IT6722A通信模块";

        public string VersionStr1 = "";
        public string VersionStr => VersionStr1;

        private static string g_JudgeValue1 = "";
        private static string g_JudgeValue2 = "";
        [JsonIgnore]
        public IT6722AForm iT6722AForm = null;  // 在基类中存储窗体
        public IT6722AParam iT6722AParam { get; set; }

        public Form CreateUI(ITestParam testParam, string VersionStr = "")
        {
            iT6722AForm = new IT6722AForm();
            iT6722AParam = new IT6722AParam();

            iT6722AForm.eventManager.AddListener("发送界面参数", new Action<List<string>>(SetDataForm_OrderCompleted));
            iT6722AForm.eventManager.AddListener("关闭窗体", new Action(FormClosing));
            iT6722AForm.eventManager.AddListener("运行", new Func<Dictionary<string, object>>(Run));
            iT6722AForm.SetDataToForm(testParam, VersionStr);

            if (testParam != null)
            {
                iT6722AParam = testParam as IT6722AParam;
            }
            return iT6722AForm;
        }
        private void SetDataForm_OrderCompleted(List<string> e)
        {
            VersionStr1 = e[0];
            iT6722AParam.JudgeType = e[1];
            iT6722AParam.JudgeValue = e[2];
        }
        public void FormClosing()
        {
            iT6722AForm.eventManager.Clear();
        }
        public void Dispose()
        {

        }

        public ITestParam GetData()
        {
            return iT6722AParam;
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
            var p = (IT6722AParam)testParam;

            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();

            // 判断要执行的操作
            switch (p.JudgeType)
            {
                case "电压判断":
                    {
                        var value = global.iT6722ATool.read();

                        double d = Convert.ToDouble(value.Item1);// 电压值

                        if (string.IsNullOrEmpty(p.JudgeValue))
                        {
                            keyValuePairs.Add("描述", "判断值不能为空");
                            keyValuePairs.Add("结果", false);
                            break;
                        }
                        if (Math.Abs(d - Convert.ToDouble(p.JudgeValue)) < 3)
                        {
                            keyValuePairs.Add("电压值", value.Item1);
                            keyValuePairs.Add("结果", true);
                        }
                        else
                        {
                            keyValuePairs.Add("电压值", value.Item1);
                            keyValuePairs.Add("结果", false);
                        }
                     
                    }
                    break;
                case "电流判断":
                    {
                        var value = global.iT6722ATool.read();

                        if (string.IsNullOrEmpty(p.JudgeValue))
                        {
                            keyValuePairs.Add("描述", "判断值不能为空");
                            keyValuePairs.Add("结果", false);
                            break;
                        }
                        double d = Convert.ToDouble(value.Item2);// 电流值
                        if (Math.Abs(d - Convert.ToDouble(p.JudgeValue)) < 3)
                        {
                            keyValuePairs.Add("电流值", value.Item2);
                            keyValuePairs.Add("结果", true);
                        }
                        else
                        {
                            keyValuePairs.Add("电流值", value.Item2);
                            keyValuePairs.Add("结果", false);
                        }
                    
                    }
                    break;
                case "上电":
                    {
                        //global.iT6722ATool.open();
                        global.iT6722ATool.set();
                        keyValuePairs.Add("结果", true);
                    }
                    break;
                case "读取":
                    {
                        var value = global.iT6722ATool.read();
                        keyValuePairs.Add("电压值", value.Item1);
                        keyValuePairs.Add("电流值", value.Item2);
                        keyValuePairs.Add("结果", true);
                        g_JudgeValue1 = value.Item1.ToString();
                        g_JudgeValue2 = value.Item2.ToString();
                    }
                    break;
                case "下电":
                    {
                        global.iT6722ATool.close();
                        keyValuePairs.Add("结果", true);
                    }
                    break;
                default:
                    {
                        keyValuePairs.Add("结果", false);
                        keyValuePairs.Add("结果描述", "没有判断操作");
                    }
                    break;
            }
            return keyValuePairs;
        }
        public Dictionary<string, object> Run()
        {         
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();

            // 判断要执行的操作
            switch (iT6722AParam.JudgeType)
            {
                case "电压判断":
                    {
                        var value = global.iT6722ATool.read();

                        double d = Convert.ToDouble(value.Item1);// 电压值

                        if(string.IsNullOrEmpty(iT6722AParam.JudgeValue))
                        {
                            keyValuePairs.Add("描述", "判断值不能为空");
                            keyValuePairs.Add("结果", false);
                            break;
                        }
                        if (Math.Abs(d - Convert.ToDouble(iT6722AParam.JudgeValue)) < 3)
                        {
                            keyValuePairs.Add("电压值", value.Item1);
                            keyValuePairs.Add("结果", true);
                        }
                        else
                        {
                            keyValuePairs.Add("电压值", value.Item1);
                            keyValuePairs.Add("结果", false);
                        }
                    
                    }
                    break;
                case "电流判断":
                    {
                        var value = global.iT6722ATool.read();

                        if (string.IsNullOrEmpty(iT6722AParam.JudgeValue))
                        {
                            keyValuePairs.Add("描述", "判断值不能为空");
                            keyValuePairs.Add("结果", false);
                            break;
                        }
                        double d = Convert.ToDouble(value.Item2);// 电流值
                        if (Math.Abs(d - Convert.ToDouble(iT6722AParam.JudgeValue)) < 3)
                        {
                            keyValuePairs.Add("电流值", value.Item2);
                            keyValuePairs.Add("结果", true);
                        }
                        else
                        {
                            keyValuePairs.Add("电流值", value.Item2);
                            keyValuePairs.Add("结果", false);
                        }
                    
                    }
                    break;
                case "上电":
                    {
                        //global.iT6722ATool.open();
                        global.iT6722ATool.set();
                        keyValuePairs.Add("结果", true);
                    }
                    break;
                case "读取":
                    {
                        var value = global.iT6722ATool.read();
                        keyValuePairs.Add("电压值", value.Item1);
                        keyValuePairs.Add("电流值", value.Item2);
                        keyValuePairs.Add("结果", true);
                        g_JudgeValue1 = value.Item1.ToString();
                        g_JudgeValue2 = value.Item2.ToString();
                    }
                    break;
                case "下电":
                    {
                        global.iT6722ATool.close();
                        keyValuePairs.Add("结果", true);
                    }
                    break;
                default:
                    {
                        iT6722AParam.RunResult = false;                      
                        keyValuePairs.Add("结果", iT6722AParam.RunResult);
                        keyValuePairs.Add("结果描述", "没有判断操作");                                            
                    }
                    break;
            }
            return keyValuePairs;
        }
    }
}
