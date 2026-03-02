using HY_Rearview_Mirror.Functions;
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
    public class GW_MultimeterParam : ITestParam
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

        public GW_MultimeterParam()
        {
            RunResult = false;
            //ChangeDataType = "";
            JudgeType = "";
            JudgeValue = "";
        }
    }
    public class GW_MultimeterFun : IPlugin
    {
        public string PluginId => "测试电压电流";

        public string moduleName => "仪器";

        public string DescribeMessage => "用于测试电压电流";

        public string VersionStr1 = "";
        public string VersionStr => VersionStr1;

        private static string g_JudgeValue1 = "";
        private static string g_JudgeValue2 = "";

        private ManualResetEventSlim _flagEvent;

        [JsonIgnore]
        public GW_MultimeterForm gW_MultimeterForm = null;  // 在基类中存储窗体
        public GW_MultimeterParam gW_MultimeterParam { get; set; }
        public GW_MultimeterFun()
        {
            global.serialHelper_GW.DataReceived += OnDataReceived;
            _flagEvent = new ManualResetEventSlim(false);
        }
        public Form CreateUI(ITestParam testParam, string VersionStr = "")
        {
            gW_MultimeterForm = new GW_MultimeterForm();
            gW_MultimeterParam = new GW_MultimeterParam();

            gW_MultimeterForm.eventManager.AddListener("发送界面参数", new Action<List<string>>(SetDataForm_OrderCompleted));
            gW_MultimeterForm.eventManager.AddListener("关闭窗体", new Action(FormClosing));
            gW_MultimeterForm.eventManager.AddListener("运行", new Func<Dictionary<string, object>>(Run));
            gW_MultimeterForm.SetDataToForm(testParam, VersionStr);

            if (testParam != null)
            {
                gW_MultimeterParam = testParam as GW_MultimeterParam;
            }
            return gW_MultimeterForm;
        }
        private void SetDataForm_OrderCompleted(List<string> e)
        {
            VersionStr1 = e[0].ToString();
            gW_MultimeterParam.JudgeType = e[1].ToString();
            gW_MultimeterParam.JudgeValue = e[2].ToString();
        }
        public void FormClosing()
        {
            gW_MultimeterForm.eventManager.Clear();
        }
        public void Dispose()
        {
            global.serialHelper_GW.DataReceived -= OnDataReceived;
        }

        public ITestParam GetData()
        {
            return gW_MultimeterParam;
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
            var p = (GW_MultimeterParam)testParam;

            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();

            // 重置事件
            _flagEvent.Reset();
            // 判断要执行的操作
            switch (p.JudgeType)
            {
                case "读取电压":
                    {
                        string str = "*RST\r\n:FUNC VOLT:DC\r\n:VOLT:DC:RANG:AUTO ON \r\n:VOLT:DC:NPLC 10\r\n:SENS:AVER OFF\r\n:READ?";
                        global.serialHelper_GW.SendString(str);

                        bool signaled = _flagEvent.Wait(5000); // 等待5秒

                        if (signaled)
                        {
                            //string str = "*RST :FUNC VOLT:DC :VOLT: DC: RANG: AUTO ON :VOLT: DC: NPLC 10 :SENS:AVER OFF :READ?";

                            double v = Convert.ToDouble(g_JudgeValue1) * 1000;
                            string volts = Convert.ToString(v);

                            keyValuePairs.Add("结果", true);
                            keyValuePairs.Add("电压", volts);
                        }
                        else
                        {
                            keyValuePairs.Add("结果", false);
                            keyValuePairs.Add("状态", "超时");
                        }


                    }
                    break;
                case "读取电流":
                    {
                        string str = "*RST\r\n:FUNC CURR:DC\r\n:INPut:JACK 3A \r\n:CURR:DC:RANG:AUTO ON \r\n:CURR:DC:NPLC 1\r\n:SENS:AVER OFF\r\n:READ?";
                        global.serialHelper_GW.SendString(str);

                        bool signaled = _flagEvent.Wait(5000); // 等待5秒

                        if (signaled)
                        {
                            //string str = "*RST :FUNC CURR:DC :INPut: JACK 3A :CURR: DC: RANG: AUTO ON :CURR: DC: NPLC 1 :SENS: AVER OFF :READ?";

                            double v = Convert.ToDouble(g_JudgeValue2) * 1000;
                            string volts = Convert.ToString(v);

                            keyValuePairs.Add("结果", true);
                            keyValuePairs.Add("电流", volts);
                        }
                        else
                        {
                            keyValuePairs.Add("结果", false);
                            keyValuePairs.Add("状态", "超时");
                        }


                    }
                    break;
                case "判断电压":
                    {
                        string str = "*RST\r\n:FUNC VOLT:DC\r\n:VOLT:DC:RANG:AUTO ON \r\n:VOLT:DC:NPLC 10\r\n:SENS:AVER OFF\r\n:READ?";
                        global.serialHelper_GW.SendString(str);

                        bool signaled = _flagEvent.Wait(5000); // 等待5秒

                        if (signaled)
                        {

                            double v = Convert.ToDouble(g_JudgeValue1) * 1000;
                            string volts = Convert.ToString(v);

                            if (string.IsNullOrEmpty(gW_MultimeterParam.JudgeValue))
                            {
                                keyValuePairs.Add("描述", "判断值不能为空");
                                keyValuePairs.Add("结果", false);
                                break;
                            }

                            if (Math.Abs(v - Convert.ToDouble(p.JudgeValue)) < 3)
                            {
                                keyValuePairs.Add("结果", true);
                                keyValuePairs.Add("电压", volts);
                            }
                            else
                            {
                                keyValuePairs.Add("结果", false);
                                keyValuePairs.Add("电压", volts);
                            }
                        }
                        else
                        {
                            keyValuePairs.Add("结果", false);
                            keyValuePairs.Add("状态", "超时");
                        }


                    }
                    break;
                case "判断电流":
                    {
                        string str = "*RST\r\n:FUNC CURR:DC\r\n:INPut:JACK 3A \r\n:CURR:DC:RANG:AUTO ON \r\n:CURR:DC:NPLC 1\r\n:SENS:AVER OFF\r\n:READ?";
                        global.serialHelper_GW.SendString(str);

                        bool signaled = _flagEvent.Wait(5000); // 等待5秒

                        if (signaled)
                        {
                            double v = Convert.ToDouble(g_JudgeValue2) * 1000;
                            string volts = Convert.ToString(v);

                            if (string.IsNullOrEmpty(gW_MultimeterParam.JudgeValue))
                            {
                                keyValuePairs.Add("描述", "判断值不能为空");
                                keyValuePairs.Add("结果", false);
                                break;
                            }


                            if (Math.Abs(v - Convert.ToDouble(p.JudgeValue)) < 3)
                            {
                                keyValuePairs.Add("结果", true);
                                keyValuePairs.Add("电压", volts);
                            }
                            else
                            {
                                keyValuePairs.Add("结果", true);
                                keyValuePairs.Add("电压", g_JudgeValue2);
                            }
                        }
                        else
                        {
                            keyValuePairs.Add("结果", false);
                            keyValuePairs.Add("状态", "超时");
                        }
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

            // 重置事件
            _flagEvent.Reset();
            // 判断要执行的操作
            switch (gW_MultimeterParam.JudgeType)
            {
                case "读取电压":
                    {
                        string str = "*RST\r\n:FUNC VOLT:DC\r\n:VOLT:DC:RANG:AUTO ON \r\n:VOLT:DC:NPLC 10\r\n:SENS:AVER OFF\r\n:READ?";
                        global.serialHelper_GW.SendString(str);

                        bool signaled = _flagEvent.Wait(5000); // 等待5秒

                        if (signaled)
                        {
                            //string str = "*RST :FUNC VOLT:DC :VOLT: DC: RANG: AUTO ON :VOLT: DC: NPLC 10 :SENS:AVER OFF :READ?";

                            double v = Convert.ToDouble(g_JudgeValue1) * 1000;
                            string volts = Convert.ToString(v);

                            keyValuePairs.Add("结果", true);
                            keyValuePairs.Add("电压", volts);
                        }
                        else
                        {
                            keyValuePairs.Add("结果", false);
                            keyValuePairs.Add("状态", "超时");
                        }

                 
                    }
                    break;
                case "读取电流":
                    {
                        string str = "*RST\r\n:FUNC CURR:DC\r\n:INPut:JACK 3A \r\n:CURR:DC:RANG:AUTO ON \r\n:CURR:DC:NPLC 1\r\n:SENS:AVER OFF\r\n:READ?";
                        global.serialHelper_GW.SendString(str);

                        bool signaled = _flagEvent.Wait(5000); // 等待5秒

                        if (signaled)
                        {
                            //string str = "*RST :FUNC CURR:DC :INPut: JACK 3A :CURR: DC: RANG: AUTO ON :CURR: DC: NPLC 1 :SENS: AVER OFF :READ?";

                            double v = Convert.ToDouble(g_JudgeValue2) * 1000;
                            string volts = Convert.ToString(v);

                            keyValuePairs.Add("结果", true);
                            keyValuePairs.Add("电流", volts);
                        }
                        else
                        {
                            keyValuePairs.Add("结果", false);
                            keyValuePairs.Add("状态", "超时");
                        }

               
                    }
                    break;
                case "判断电压":
                    {
                        string str = "*RST\r\n:FUNC VOLT:DC\r\n:VOLT:DC:RANG:AUTO ON \r\n:VOLT:DC:NPLC 10\r\n:SENS:AVER OFF\r\n:READ?";
                        global.serialHelper_GW.SendString(str);

                        bool signaled = _flagEvent.Wait(5000); // 等待5秒

                        if (signaled)
                        {

                            double v = Convert.ToDouble(g_JudgeValue1) * 1000;
                            string volts = Convert.ToString(v);


                            if (string.IsNullOrEmpty(gW_MultimeterParam.JudgeValue))
                            {
                                keyValuePairs.Add("描述", "判断值不能为空");
                                keyValuePairs.Add("结果", false);
                                break;
                            }


                            if (Math.Abs(v - Convert.ToDouble(gW_MultimeterParam.JudgeValue)) < 3)
                            {
                                keyValuePairs.Add("结果", true);
                                keyValuePairs.Add("电压", volts);
                            }
                            else
                            {
                                keyValuePairs.Add("结果", false);
                                keyValuePairs.Add("电压", volts);
                            }
                        }
                        else
                        {
                            keyValuePairs.Add("结果", false);
                            keyValuePairs.Add("状态", "超时");
                        }

                     
                    }
                    break;
                case "判断电流":
                    {
                        string str = "*RST\r\n:FUNC CURR:DC\r\n:INPut:JACK 3A \r\n:CURR:DC:RANG:AUTO ON \r\n:CURR:DC:NPLC 1\r\n:SENS:AVER OFF\r\n:READ?";
                        global.serialHelper_GW.SendString(str);

                        bool signaled = _flagEvent.Wait(5000); // 等待5秒

                        if (signaled)
                        {
                            double v = Convert.ToDouble(g_JudgeValue2) * 1000;
                            string volts = Convert.ToString(v);

                            if (string.IsNullOrEmpty(gW_MultimeterParam.JudgeValue))
                            {
                                keyValuePairs.Add("描述", "判断值不能为空");
                                keyValuePairs.Add("结果", false);
                                break;
                            }

                            if (Math.Abs(v - Convert.ToDouble(gW_MultimeterParam.JudgeValue)) < 3)
                            {
                                keyValuePairs.Add("结果", true);
                                keyValuePairs.Add("电压", volts);
                            }
                            else
                            {
                                keyValuePairs.Add("结果", true);
                                keyValuePairs.Add("电压", g_JudgeValue2);
                            }
                        }
                        else
                        {
                            keyValuePairs.Add("结果", false);
                            keyValuePairs.Add("状态", "超时");
                        }                   
                    }
                    break;
                default:
                    {
                        gW_MultimeterParam.RunResult = false;
                        keyValuePairs.Add("结果", gW_MultimeterParam.RunResult);
                        keyValuePairs.Add("结果描述", "没有判断操作");
                    }
                    break;
            }
            return keyValuePairs;
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine($"接收到数据 - 字符串: {e.DataString}");
            Console.WriteLine($"接收到数据 - 十六进制: {e.HexString}");
            g_JudgeValue1 = e.DataString;
            g_JudgeValue2 = e.DataString;

            _flagEvent.Set();
        }
    }
}
