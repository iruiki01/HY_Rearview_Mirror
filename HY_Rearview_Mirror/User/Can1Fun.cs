using HY_Rearview_Mirror.CAN;
using HY_Rearview_Mirror.InterfaceTool;
using Newtonsoft.Json;
using Sunny.UI.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HY_Rearview_Mirror.User
{
    public class CANFUNTestParam : ITestParam
    {
        public uint ID { get; set; }
        public List<string> dataList_Can{ get; set; }
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

        public CANFUNTestParam()
        {
            ID = 1;
            dataList_Can = new List<string>();
            RunResult = false;
            ChangeDataType = "";
            JudgeType = "";
            JudgeValue = "";
        }
    }
    public class Can1Fun : IPlugin
    {
        public string PluginId => "CAN1通信";

        public string moduleName => "CAN通道1";

        public string DescribeMessage => "用于CAN通信模块";

        [JsonIgnore]
        public Can1FunSetForm setDataForm = null;  // 在基类中存储窗体

        public CANFUNTestParam cANFUNTestParam { get; set; }

        public string VersionStr1 = "";
        public string VersionStr => VersionStr1;

        private ManualResetEventSlim _flagEvent;

        private string g_JudgeValue = "";

        public Can1Fun()
        {
            cANFUNTestParam = new CANFUNTestParam();
            _flagEvent = new ManualResetEventSlim(false);

            global.can1Service.MessageReceivedHandler += MessageReceived;
            //_canService = global.can1Service;
        }
        public ITestParam GetData()
        {
            return cANFUNTestParam;
        }
        public Form CreateUI(ITestParam testParam, string VersionStr = "")
        {      
            setDataForm = new Can1FunSetForm();

            setDataForm.eventManager.AddListener("发送界面参数", new Action<List<object>>(SetDataForm_OrderCompleted));
            setDataForm.eventManager.AddListener("关闭窗体", new Action(FormClosing));
            setDataForm.eventManager.AddListener("运行", new Func<Dictionary<string, object>>(Run));

            setDataForm.SetDataToForm(testParam, VersionStr);
            if(testParam != null)
            {
                cANFUNTestParam = testParam as CANFUNTestParam;
            }
            
            return setDataForm;
        }
        public void FormClosing()
        {
            setDataForm.eventManager.Clear();
        }
        public void Dispose()
        {
            global.can1Service.MessageReceivedHandler -= MessageReceived;
            GC.SuppressFinalize(this);
        }
        public void Initialize()
        {
            // 注册本插件的类型
            //SafeTestParamConverter.RegisterType("canfun", typeof(CANFUNTestParam));
        }

        public Dictionary<string, object> Run(ITestParam testParam)
        {
            string name = VersionStr;

            MessageReceivedStr = "";
          
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();
            var p = (CANFUNTestParam)testParam;

            // 方法1：使用 Split + Convert.ToByte
            byte[] bytes = p.dataList_Can[1].Split(' ')
                                    .Select(x => Convert.ToByte(x, 16))
                                    .ToArray();



            if (p.JudgeType == "获取前感光值(开光源)")
            {
                // 1：打开光源控制器
                string cmd = "$WAN#";
                global.serialHelper_Light.SendString(cmd);
                // 1：设置光源亮度
                //cmd = $"$SA{500:D3}#";
                //global.serialHelper_Light.SendString(cmd);
                Thread.Sleep(2000);

            }

            if (p.JudgeType == "获取后感光值(开光源)")
            {
                // 1：打开后光源控制器
                global.ZS_ModbusRtuHelpe.Write("s=1;6", (ushort)1);
                Thread.Sleep(2000);
            }




            if (p.JudgeType == "感光校准")
            {
                if (string.IsNullOrEmpty(Context.GetInstance().Photosensitive1) ||
                    string.IsNullOrEmpty(Context.GetInstance().Photosensitive2))
                {
                    keyValuePairs.Add("结果", false);
                    keyValuePairs.Add("描述", "感光校准数据为空，感光校正发送失败");
                    return keyValuePairs;
                }
                //Console.WriteLine("结果" + Context.GetInstance().Photosensitive1);
                //Console.WriteLine("结果" + Context.GetInstance().Photosensitive2);
                long data1 = Convert.ToInt64(Context.GetInstance().Photosensitive1);
                long data2 = Convert.ToInt64(Context.GetInstance().Photosensitive2);

                if(data1>99999 || data2>99999)
                {
                    keyValuePairs.Add("结果", false);
                    keyValuePairs.Add("描述", "感光校准数据异常，感光校正发送失败");
                    return keyValuePairs;
                }


                ushort value = Convert.ToUInt16(Context.GetInstance().Photosensitive1);
                string str = ToLittleEndianHex(value);

                string str1 = InsertDataString(cANFUNTestParam.dataList_Can[1], str);

                value = Convert.ToUInt16(Context.GetInstance().Photosensitive2);
                str = ToLittleEndianHex(value);

                string str2 = InsertDataString(str1, str, 4);

                bytes = str2.Split(' ')
                                    .Select(x => Convert.ToByte(x, 16))
                                    .ToArray();


                Context.GetInstance().Photosensitive1 = "";
                Context.GetInstance().Photosensitive2 = "";
            }

            // 重置事件
            _flagEvent.Reset();

            global.can1Service.Send(p.ID, bytes, true);

            // 等待信号（带超时）
            bool signaled = _flagEvent.Wait(5000); // 等待5秒

            

            if (p.ChangeDataType == "不判断")
            {
                if (signaled)
                {
                    keyValuePairs.Add("结果", true);
                    //keyValuePairs.Add("接收数据", MessageReceivedStr);
                }
                else
                {
                    keyValuePairs.Add("结果", false);
                    keyValuePairs.Add("状态", "超时");
                }
            }
            else
            {
                // 判断要执行的操作
                switch (p.JudgeType)
                {
                    case "大于":
                        break;
                    case "小于":
                        {
                            if (string.IsNullOrEmpty(g_JudgeValue))
                            {
                                keyValuePairs.Add("结果", false);
                                keyValuePairs.Add("状态", "没有获取到数据");
                                break;
                            }
                            double value1 = Convert.ToDouble(g_JudgeValue);

                            if (string.IsNullOrEmpty(p.JudgeValue))
                            {
                                if (signaled)
                                {
                                    keyValuePairs.Add("结果", "没有选择判断类型");
                                    keyValuePairs.Add("接收数据", MessageReceivedStr);
                                    keyValuePairs.Add("转换数据", g_JudgeValue);
                                    keyValuePairs.Add("判断阈值", "空");
                                }
                                else
                                {
                                    keyValuePairs.Add("结果", p.RunResult);
                                    keyValuePairs.Add("状态", "超时");
                                }
                                break;
                            }

                            double value2 = Convert.ToDouble(p.JudgeValue);
                            if (value1 < value2)
                            {
                                p.RunResult = true;
                            }
                            else
                            {
                                p.RunResult = false;
                            }

                            if (signaled)
                            {
                                keyValuePairs.Add("结果", p.RunResult);
                                keyValuePairs.Add("接收数据", MessageReceivedStr);
                                keyValuePairs.Add("转换数据", g_JudgeValue);
                                keyValuePairs.Add("判断阈值", p.JudgeValue);
                            }
                            else
                            {
                                keyValuePairs.Add("结果", false);
                                keyValuePairs.Add("状态", "超时");
                            }
                        }
                        break;
                    case "包含":
                        {
                            if (g_JudgeValue == p.JudgeValue)
                            {
                                p.RunResult = true;
                            }
                            else
                            {
                                p.RunResult = false;
                            }

                            if (signaled)
                            {
                                keyValuePairs.Add("结果", p.RunResult);
                                keyValuePairs.Add("接收数据", MessageReceivedStr);
                                keyValuePairs.Add("转换数据", g_JudgeValue);
                                keyValuePairs.Add("判断阈值", p.JudgeValue);

                            }
                            else
                            {
                                keyValuePairs.Add("结果", false);
                                keyValuePairs.Add("状态", "超时");
                            }
                        }
                        break;
                    case "感光校准":
                        {
                            keyValuePairs.Add("结果", true);
                            keyValuePairs.Add("描述", "感光校正发送成功");
                        }
                        break;
                    case "获取前感光值(开光源)":
                        {
                            if (signaled)
                            {
                                // 1：关闭光源控制器
                                string cmd = "$WAF#";
                                global.serialHelper_Light.SendString(cmd);

                                keyValuePairs.Add("结果", true);
                                keyValuePairs.Add("接收数据", MessageReceivedStr);
                                keyValuePairs.Add("转换数据", g_JudgeValue);

                                Context.GetInstance().Photosensitive1 = g_JudgeValue;
                                Context.GetInstance().LightValue2 = g_JudgeValue;

                                //global.writeLogSystem.Error("前感光值->" + g_JudgeValue);
                            }
                            else
                            {
                                // 1：关闭光源控制器
                                string cmd = "$WAF#";
                                global.serialHelper_Light.SendString(cmd);

                                keyValuePairs.Add("结果", false);
                                keyValuePairs.Add("状态", "超时");
                            }
                        }
                        break;
                    case "获取后感光值(开光源)":
                        {
                            if (signaled)
                            {
                                // 1：关闭后光源控制器
                                global.ZS_ModbusRtuHelpe.Write("s=1;6", (ushort)0);

                                keyValuePairs.Add("结果", true);
                                keyValuePairs.Add("接收数据", MessageReceivedStr);
                                keyValuePairs.Add("转换数据", g_JudgeValue);

                                //global.writeLogSystem.Error("后感光值->" + g_JudgeValue);
                                Context.GetInstance().Photosensitive2 = g_JudgeValue;
                                Context.GetInstance().LightValue2 = g_JudgeValue;
                            }
                            else
                            {
                                keyValuePairs.Add("结果", false);
                                keyValuePairs.Add("状态", "超时");
                            }
                        }
                        break;
                    case "获取前感光值(不开光源)":
                        {
                            if (signaled)
                            {
                                // 1：关闭光源控制器
                                string cmd = "$WAF#";
                                global.serialHelper_Light.SendString(cmd);

                                keyValuePairs.Add("结果", true);
                                keyValuePairs.Add("接收数据", MessageReceivedStr);
                                keyValuePairs.Add("转换数据", g_JudgeValue);

                                Context.GetInstance().Photosensitive1 = g_JudgeValue;

                                Context.GetInstance().LightValue1 = g_JudgeValue;

                                //global.writeLogSystem.Error("前感光值->" + g_JudgeValue);
                            }
                            else
                            {
                                // 1：关闭光源控制器
                                string cmd = "$WAF#";
                                global.serialHelper_Light.SendString(cmd);

                                keyValuePairs.Add("结果", false);
                                keyValuePairs.Add("状态", "超时");
                            }
                        }
                        break;
                    case "获取后感光值(不开光源)":
                        {
                            if (signaled)
                            {
                                // 1：关闭后光源控制器
                                global.ZS_ModbusRtuHelpe.Write("s=1;6", (ushort)0);

                                keyValuePairs.Add("结果", true);
                                keyValuePairs.Add("接收数据", MessageReceivedStr);
                                keyValuePairs.Add("转换数据", g_JudgeValue);

                                //global.writeLogSystem.Error("后感光值->" + g_JudgeValue);
                                Context.GetInstance().Photosensitive2 = g_JudgeValue;
                                Context.GetInstance().LightValue1 = g_JudgeValue;
                            }
                            else
                            {
                                keyValuePairs.Add("结果", false);
                                keyValuePairs.Add("状态", "超时");
                            }
                        }
                        break;
                    case "空":
                        {
                            if (g_JudgeValue == p.JudgeValue)
                            {
                                p.RunResult = true;
                            }
                            else
                            {
                                p.RunResult = false;
                            }

                            if (signaled)
                            {
                                keyValuePairs.Add("结果", p.RunResult);
                                keyValuePairs.Add("接收数据", MessageReceivedStr);
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
                            p.RunResult = false;
                            if (signaled)
                            {
                                keyValuePairs.Add("结果", p.RunResult);
                                keyValuePairs.Add("结果描述", "没有判断操作");
                            }
                            else
                            {
                                keyValuePairs.Add("结果", false);
                                keyValuePairs.Add("状态", "超时");
                            }
                        }
                        break;
                }
            }
            return keyValuePairs;
        }

        
        public  Dictionary<string, object> Run()
        {
            string name = VersionStr;
            MessageReceivedStr = "";
            // 重置事件
            _flagEvent.Reset();
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();

            // 方法1：使用 Split + Convert.ToByte
            byte[] bytes = cANFUNTestParam.dataList_Can[1].Split(' ')
                                    .Select(x => Convert.ToByte(x, 16))
                                    .ToArray();





            if (cANFUNTestParam.JudgeType == "获取前感光值(开光源)")
            {
                // 1：打开光源控制器
                string cmd = "$WAN#";
                global.serialHelper_Light.SendString(cmd);
                // 1：设置光源亮度
                //cmd = $"$SA{500:D3}#";
                //global.serialHelper_Light.SendString(cmd);
                Thread.Sleep(2000);

            }

            if (cANFUNTestParam.JudgeType == "获取后感光值(开光源)")
            {
                // 1：打开后光源控制器
                global.ZS_ModbusRtuHelpe.Write("s=1;6", (ushort)1);
                Thread.Sleep(2000);
            }

      


            if (cANFUNTestParam.JudgeType == "感光校准")
            {
                if(string.IsNullOrEmpty(Context.GetInstance().Photosensitive1) ||
                    string.IsNullOrEmpty(Context.GetInstance().Photosensitive2))
                {
                    keyValuePairs.Add("结果", false);
                    keyValuePairs.Add("描述", "感光校准数据为空，感光校正发送失败");
                    return keyValuePairs;
                }

                ushort value = Convert.ToUInt16(Context.GetInstance().Photosensitive1);
                string str = ToLittleEndianHex(value);

                string str1 = InsertDataString(cANFUNTestParam.dataList_Can[1],str);

                value = Convert.ToUInt16(Context.GetInstance().Photosensitive2);
                str = ToLittleEndianHex(value);

                string str2 = InsertDataString(str1, str,4);

                bytes = str2.Split(' ')
                                    .Select(x => Convert.ToByte(x, 16))
                                    .ToArray();
            }





            global.can1Service.Send(cANFUNTestParam.ID, bytes, true);

            // 等待信号（带超时）
            bool signaled = _flagEvent.Wait(5000); // 等待5秒

            if (cANFUNTestParam.ChangeDataType == "不判断")
            {
                if (signaled)
                {
                    keyValuePairs.Add("结果", true);
                    //keyValuePairs.Add("接收数据", MessageReceivedStr);
                }
                else
                {
                    keyValuePairs.Add("结果", false);
                    keyValuePairs.Add("状态", "超时");
                }
            }
            else
            {
                // 判断要执行的操作
                switch (cANFUNTestParam.JudgeType)
                {
                    case "大于":
                        break;
                    case "小于":
                        {
                            if (string.IsNullOrEmpty(g_JudgeValue))
                            {
                                keyValuePairs.Add("结果", false);
                                keyValuePairs.Add("状态", "没有获取到数据");
                                break;
                            }
                            double value1 = Convert.ToDouble(g_JudgeValue);

                            if (string.IsNullOrEmpty(cANFUNTestParam.JudgeValue))
                            {
                                if (signaled)
                                {
                                    keyValuePairs.Add("结果", false);
                                    keyValuePairs.Add("接收数据", MessageReceivedStr);
                                    keyValuePairs.Add("转换数据", g_JudgeValue);
                                    keyValuePairs.Add("判断阈值", "空");
                                    keyValuePairs.Add("描述", "没有选择判断类型");
                                }
                                else
                                {
                                    keyValuePairs.Add("结果", cANFUNTestParam.RunResult);
                                    keyValuePairs.Add("状态", "超时");
                                }
                                break;
                            }

                            double value2 = Convert.ToDouble(cANFUNTestParam.JudgeValue);
                            if (value1 < value2)
                            {
                                cANFUNTestParam.RunResult = true;
                            }
                            else
                            {
                                cANFUNTestParam.RunResult = false;
                            }

                            if (signaled)
                            {
                                keyValuePairs.Add("结果", cANFUNTestParam.RunResult);
                                keyValuePairs.Add("接收数据", MessageReceivedStr);
                                keyValuePairs.Add("转换数据", g_JudgeValue);
                                keyValuePairs.Add("判断阈值", cANFUNTestParam.JudgeValue);
                            }
                            else
                            {
                                keyValuePairs.Add("结果", cANFUNTestParam.RunResult);
                                keyValuePairs.Add("状态", "超时");
                            }
                        }
                        break;
                    case "包含":
                        {
                            if (g_JudgeValue == cANFUNTestParam.JudgeValue)
                            {
                                cANFUNTestParam.RunResult = true;
                            }
                            else
                            {
                                cANFUNTestParam.RunResult = false;
                            }

                            if (signaled)
                            {
                                keyValuePairs.Add("结果", cANFUNTestParam.RunResult);
                                keyValuePairs.Add("接收数据", MessageReceivedStr);
                                keyValuePairs.Add("转换数据", g_JudgeValue);
                            }
                            else
                            {
                                keyValuePairs.Add("结果", cANFUNTestParam.RunResult);
                                keyValuePairs.Add("状态", "超时");
                            }
                        }
                        break;
                    case "感光校准":
                        {
                            keyValuePairs.Add("结果", true);
                            keyValuePairs.Add("描述", "感光校正发送成功");
                        }
                        break;
                    case "获取前感光值(开光源)":
                        {
                            if (signaled)
                            {
                                // 1：关闭光源控制器
                                string cmd = "$WAF#";
                                global.serialHelper_Light.SendString(cmd);

                                keyValuePairs.Add("结果", true);
                                keyValuePairs.Add("接收数据", MessageReceivedStr);
                                keyValuePairs.Add("转换数据", g_JudgeValue);

                                Context.GetInstance().Photosensitive1 = g_JudgeValue;
                                Context.GetInstance().LightValue2 = g_JudgeValue;

                                //global.writeLogSystem.Error("前感光值->" + g_JudgeValue);
                            }
                            else
                            {
                                // 1：关闭光源控制器
                                string cmd = "$WAF#";
                                global.serialHelper_Light.SendString(cmd);

                                keyValuePairs.Add("结果", false);
                                keyValuePairs.Add("状态", "超时");
                            }
                        }
                        break;
                    case "获取后感光值(开光源)":
                        {
                            if (signaled)
                            {
                                // 1：关闭后光源控制器
                                global.ZS_ModbusRtuHelpe.Write("s=1;6", (ushort)0);

                                keyValuePairs.Add("结果", true);
                                keyValuePairs.Add("接收数据", MessageReceivedStr);
                                keyValuePairs.Add("转换数据", g_JudgeValue);

                                //global.writeLogSystem.Error("后感光值->" + g_JudgeValue);
                                Context.GetInstance().Photosensitive2 = g_JudgeValue;
                                Context.GetInstance().LightValue2 = g_JudgeValue;
                            }
                            else
                            {
                                keyValuePairs.Add("结果", false);
                                keyValuePairs.Add("状态", "超时");
                            }
                        }
                        break;
                    case "获取前感光值(不开光源)":
                        {
                            if (signaled)
                            {
                                // 1：关闭光源控制器
                                string cmd = "$WAF#";
                                global.serialHelper_Light.SendString(cmd);

                                keyValuePairs.Add("结果", true);
                                keyValuePairs.Add("接收数据", MessageReceivedStr);
                                keyValuePairs.Add("转换数据", g_JudgeValue);

                                Context.GetInstance().Photosensitive1 = g_JudgeValue;
                                Context.GetInstance().LightValue1 = g_JudgeValue;

                                //global.writeLogSystem.Error("前感光值->" + g_JudgeValue);
                            }
                            else
                            {
                                // 1：关闭光源控制器
                                string cmd = "$WAF#";
                                global.serialHelper_Light.SendString(cmd);

                                keyValuePairs.Add("结果", false);
                                keyValuePairs.Add("状态", "超时");
                            }
                        }
                        break;
                    case "获取后感光值(不开光源)":
                        {
                            if (signaled)
                            {
                                // 1：关闭后光源控制器
                                global.ZS_ModbusRtuHelpe.Write("s=1;6", (ushort)0);

                                keyValuePairs.Add("结果", true);
                                keyValuePairs.Add("接收数据", MessageReceivedStr);
                                keyValuePairs.Add("转换数据", g_JudgeValue);

                                //global.writeLogSystem.Error("后感光值->" + g_JudgeValue);
                                Context.GetInstance().Photosensitive2 = g_JudgeValue;
                                Context.GetInstance().LightValue1 = g_JudgeValue;
                            }
                            else
                            {
                                keyValuePairs.Add("结果", false);
                                keyValuePairs.Add("状态", "超时");
                            }
                        }
                        break;
                    case "空":
                        {
                            if (signaled)
                            {
                                keyValuePairs.Add("结果", true);
                                keyValuePairs.Add("接收数据", MessageReceivedStr);
                                keyValuePairs.Add("转换数据", g_JudgeValue);
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
                            cANFUNTestParam.RunResult = false;
                            if (signaled)
                            {
                                keyValuePairs.Add("结果", cANFUNTestParam.RunResult);
                                keyValuePairs.Add("结果描述", "没有判断操作");
                            }
                            else
                            {
                                keyValuePairs.Add("结果", cANFUNTestParam.RunResult);
                                keyValuePairs.Add("状态", "超时");
                            }
                        }
                        break;
                }
            }
            return keyValuePairs;
        }
        /****************************************************感光标定数据转换***********************************************************/
        /// <summary>
        /// 将数值转换为小端格式的2字节十六进制字符串
        /// 例如：948 -> "B403"
        /// </summary>
        public static string ToLittleEndianHex(ushort value)
        {
            byte lowByte = (byte)(value & 0xFF);      // 低字节
            byte highByte = (byte)(value >> 8);       // 高字节

            return $"{lowByte:X2}{highByte:X2}";      // 低位在前
        }

        /// <summary>
        /// 将小端格式的数据字符串插入到CAN报文指定位置
        /// 例如："0x02 0x01 0x00 0x00 0x00 0x00 0x00 0x00" + "B403" (位置2) 
        ///    -> "0x02 0x01 0xB4 0x03 0x00 0x00 0x00 0x00"
        /// </summary>
        /// <param name="frameString">带0x前缀和空格的CAN报文字符串</param>
        /// <param name="dataString">小端数据字符串（如"B403"表示0x03B4=948）</param>
        /// <param name="position">插入字节位置（默认2即Byte2）</param>
        /// <returns>修改后的格式化字符串</returns>
        public static string InsertDataString(string frameString, string dataString, int position = 2)
        {
            // 清理输入
            frameString = frameString.Replace(" ", "").Replace("0x", "").Replace("-", "");
            dataString = dataString.Replace(" ", "").Replace("0x", "").Replace("-", "");

            if (frameString.Length != 16)
                throw new ArgumentException("CAN报文需要16个字符（8字节）");

            if (dataString.Length != 4)
                throw new ArgumentException("数据需要4个字符（2字节）");

            if (position < 0 || position > 6)
                throw new ArgumentException("插入位置必须在0-6之间");

            // 直接替换指定位置
            int charPos = position * 2;
            string result = frameString.Substring(0, charPos)
                          + dataString
                          + frameString.Substring(charPos + 4);

            // 格式化为带0x前缀和空格的形式
            return FormatWithPrefix(result);
        }
        /// <summary>
        /// 格式化为带0x前缀和空格的字符串
        /// </summary>
        private static string FormatWithPrefix(string hexString)
        {
            hexString = hexString.Replace(" ", "").Replace("0x", "").Replace("-", "");

            if (hexString.Length != 16)
                throw new ArgumentException("需要16个字符");

            string result = "";
            for (int i = 0; i < 8; i++)
            {
                result += $"0x{hexString.Substring(i * 2, 2)} ";
            }
            return result.Trim();
        }
        /******************************************************************************************************/
        private void SetDataForm_OrderCompleted(List<object> e)
        {
            cANFUNTestParam.ID = SafeParseUint(e[0].ToString());
            cANFUNTestParam.dataList_Can = (List<string>)e[1];       
            VersionStr1 = e[2].ToString();
            cANFUNTestParam.ChangeDataType = e[3].ToString();
            cANFUNTestParam.JudgeType = e[4].ToString();
            cANFUNTestParam.JudgeValue = e[5].ToString();
        }

        /// <summary>
        /// 安全转换字符串到uint（支持十进制和十六进制）
        /// </summary>
        private uint SafeParseUint(string input)
        {
            input = input.Trim().ToUpper();

            // 检查十六进制格式（支持0x前缀或纯十六进制）
            if (input.StartsWith("0x") || (input.Length >= 2 && IsHexString(input)))
            {
                if (input.StartsWith("0x"))
                    input = input.Substring(2);

                if (uint.TryParse(input, System.Globalization.NumberStyles.HexNumber, null, out uint hexValue))
                    return hexValue;
            }

            // 尝试十进制
            if (uint.TryParse(input, out uint decimalValue))
                return decimalValue;

            throw new FormatException($"'{input}' 不是有效的十进制或十六进制数");
        }

        /// <summary>
        /// 判断字符串是否为十六进制格式
        /// </summary>
        private bool IsHexString(string s)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(s, @"^[0-9A-F]+$");
        }

        public string GetVersion()
        {
            return VersionStr;
        }

        private  string MessageReceivedStr = "";
        public void MessageReceived(object sender, CANMessage cANMessage)
        {
            if(cANFUNTestParam.ChangeDataType == "不判断")
            {
                _flagEvent.Set();                
            }
            else
            {        
                if (cANMessage.ID == 0X7FE)
                {
                    MessageReceivedStr = "";

                    List<byte> strings = new List<byte >();
                    strings.Clear();
                    foreach (var itm in cANMessage.Data)
                    {
                        strings.Add(itm);
                        //MessageReceivedStr += itm.ToString("X");
                    }

                    for(int i = 0;i < strings.Count;i++)
                    {
                        MessageReceivedStr += strings[i] + " ";
                    }

                    MessageReceivedStr = MessageReceivedStr.Trim();
                    // 转为十六进制字符串（小写）
                    MessageReceivedStr = string.Join(" ",
                        MessageReceivedStr.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(s => byte.Parse(s.Trim()).ToString("X2")) // X2 = 2位大写十六进制
                    );


                    // 判断数据转换的类型
                    switch (cANFUNTestParam.ChangeDataType)
                    {
                        case "INT":
                            break;
                        case "DOUBLE":
                            {
                                try
                                {
                                    double v1 = ConvertHexToVoltage(MessageReceivedStr);
                                    g_JudgeValue = Convert.ToString(v1);
                                    Console.WriteLine($"电压: {v1:F3}V");  // 电压: 4.514V
                                }
                                catch (Exception e) 
                                {
                                    Console.WriteLine(e.Message);
                                }
                            
                            }
                            break;
                        case "ASCII":
                            {
                                try
                                {
                                    g_JudgeValue = HexToAscii(MessageReceivedStr);
                                }
                                catch (ArgumentException a)
                                {
                                    Console.WriteLine(a.Message);
                                }
                            }
                            break;
                        case "感光值":
                            {
                                try
                                {
                                    if(!string.IsNullOrEmpty(MessageReceivedStr))
                                    {
                                        g_JudgeValue = ConvertAndParse(MessageReceivedStr).ToString();
                                    }
                                   
                                }
                                catch (ArgumentException a)
                                {
                                    Console.WriteLine(a.Message);
                                }
                          
                            }
                            break;
                    }
                    _flagEvent.Set();
                }
            }
        }

        /********************************************************感光值数据转换***************************************************************/
        /// <summary>
        /// 一步到位：编码输入并解析数据
        /// </summary>
        public long ConvertAndParse(string encodedData)
        {
            try
            {
                // 先按十六进制解析字符串
                byte[] original = encodedData.Split(' ')
                                           .Select(s => Convert.ToByte(s, 16)) // 按16进制解析！
                                           .ToArray();

                // 然后处理
                long result = original.Skip(2)
                                      .Take(6)
                                      .Reverse()
                                      .Aggregate(0L, (acc, b) => (acc << 8) | b);


                //byte[] original = encodedData
                //.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                //.Where(s => !string.IsNullOrWhiteSpace(s))  // 过滤空值
                //.Select(s =>
                //{
                //    // 处理 0x 前缀（如果有）
                //    string clean = s.Trim().Replace("0x", "").Replace("0X", "");
                //    return Convert.ToByte(clean, 16);
                //})
                //.ToArray();

                //// 然后处理
                //long result = original.Skip(2)
                //                      .Take(6)
                //                      .Reverse()
                //                      .Aggregate(0L, (acc, b) => (acc << 8) | b);


                return result;
            }
            catch(Exception e)
            {
                return 0;
            }     
        }
        /***********************************************************************************************************************************/
        /// <summary>
        /// 将十六进制字符串转换为 ASCII 文本
        /// </summary>
        /// <param name="hexString">十六进制字符串（例如："30 30 30 2E 30 30 31" 或 "7035303531353133"）</param>
        /// <returns>转换后的 ASCII 文本</returns>
        public string HexToAscii(string hexString)
        {
            // 移除所有空格
            string cleanHex = hexString.Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");

            // 验证十六进制字符串长度是否为偶数
            if (cleanHex.Length % 2 != 0)
            {
                throw new ArgumentException("十六进制字符串长度必须为偶数");
            }

            // 将十六进制字符串转换为字节数组
            byte[] bytes = new byte[cleanHex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                string hexByte = cleanHex.Substring(i * 2, 2);
                bytes[i] = Convert.ToByte(hexByte, 16);
            }

            // 将字节数组转换为 ASCII 字符串
            return Encoding.ASCII.GetString(bytes);
        }

        /// <summary>
        /// 将十六进制字符串转换为电压值（永远只处理2字节/4位十六进制）
        /// 多于4位的字符串，只取前4位；少于4位的左侧补零
        /// </summary>
        /// <param name="hexString">十六进制字符串（如"19E4C0000"、"11A2"）</param>
        /// <returns>电压值（单位：V），精度0.001V</returns>
        public double ConvertHexToVoltage(string hexString)
        {
            // 清理输入：移除0x前缀、空格、连字符
            string cleanHex = CleanHexString(hexString);

            // 只取前4位（2字节），忽略其余字符
            string twoBytes = cleanHex.Length >= 4
                ? cleanHex.Substring(0, 4)  // 取前4位
                : cleanHex.PadLeft(4, '0'); // 不足4位左侧补零

            // 转换为16位无符号整数
            ushort adValue = Convert.ToUInt16(twoBytes, 16);

            // 乘以精度系数得到电压值
            return adValue * 0.001;
        }

        /// <summary>
        /// 从完整报文（十六进制字符串）提取电压（2字节模式）
        /// </summary>
        /// <param name="messageHex">完整报文（如"0103A211000000"）</param>
        /// <returns>电压值（单位：V）</returns>
        public double ParseVoltageFromMessage(string messageHex)
        {
            string clean = CleanHexString(messageHex);

            if (clean.Length < 8)
                throw new ArgumentException("报文长度至少8字节（16位十六进制）");

            // 提取数据域（第3-4字节，跳过前2字节ID）
            string dataHex = clean.Substring(4, 4); // 取byte2和byte3
            return ConvertHexToVoltage(dataHex);
        }

        /// <summary>
        /// 清理十六进制字符串
        /// </summary>
        private string CleanHexString(string input)
        {
            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                input = input.Substring(2);

            input = input.Replace(" ", "").Replace("-", "");

            if (!System.Text.RegularExpressions.Regex.IsMatch(input, @"^[0-9A-Fa-f]+$"))
                throw new ArgumentException($"包含非法字符：{input}");

            return input;
        }

    }
}
