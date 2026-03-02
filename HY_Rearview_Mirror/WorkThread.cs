using Helper;
using Helper.Motion;
using hWindowTool;
using HY_Rearview_Mirror.Functions;
using Ivi.Visa.Interop;
using Newtonsoft.Json;
using Sunny.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HY_Rearview_Mirror
{
    public enum WorkThreadStep
    {
        测试
    }
    internal class WorkThread
    {
        #region 单例
        /// <summary>
        /// 单例
        /// </summary>
        private static WorkThread instance = null;
        private static readonly object syncRoot = new object();
        public static WorkThread GetInstance()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new WorkThread();
                }
            }
            return instance;
        }
        #endregion
        public DynamicThreadFactory factory = new DynamicThreadFactory();
        public WorkStation workStation = new WorkStation();
        public event Action<int, int, object> SetCellValueThreadSafeHandler;
        public event Action<int, int, bool> SetCellColorHandler;
        public event Action<int> RunProcessBarHandler;
        public event Action RestorePluginsHandler;
        public event Action<Bitmap> GYCamBitmapHandler;
        private Stopwatch stopwatch = new Stopwatch();
        private int ReadPlcValue = 0;
        /// <summary>
        /// 生产总数
        /// </summary>
        public int Count = 0;
        /// <summary>
        /// 生产NG统计
        /// </summary>
        public int OkCount = 0;

        public bool IsRunState = false;
        public int AutoRunCount = 0;

        public RFIDData m_rFIDData = new RFIDData();

        public WorkThread()
        {
            workStation.Init();
            var userLogic = GetUserLogic();
            factory.AddThread(userLogic);
        }
        ~WorkThread()
        {
            factory.Stop();
        }
        /// <summary>
        /// 运行线程函数
        /// </summary>
        /// <param name="StartFlag">true：开始运行；false：停止生产</param>
        /// <param name="RunOnFlag">false：开始生产；true：继续生产</param>
        public void Run(bool StartFlag, bool RunOnFlag = false)// 开始运行线程
        {
            if (StartFlag)// 开始
            {
                if (!RunOnFlag)// 开始生产
                {
                    workStation.Run_Step[(int)WorkThreadStep.测试] = 10;
                    workStation.Alarm_Flag[(int)WorkThreadStep.测试] = false;        
                    Thread.Sleep(500);
                    // 通用
                    workStation.Run_Flag[(int)WorkThreadStep.测试] = true;

                    global.writeLogSystem.Info("开始生产！");
                }
                else//继续生产
                {
                    // 通用
                    workStation.Alarm_Flag[(int)WorkThreadStep.测试] = false;
                }
            }
            else// 停止
            {
                // 通用
                workStation.Alarm_Flag[(int)WorkThreadStep.测试] = false;
                workStation.Run_Flag[(int)WorkThreadStep.测试] = false;
                workStation.Run_Step[(int)WorkThreadStep.测试] = 0;
                global.writeLogSystem.Info("停止生产！");
            }
        }
         Action GetUserLogic()
        {
            return () =>
            {
                #region 测试流程
                if (workStation.Run_Flag[(int)WorkThreadStep.测试])
                {
                    if (workStation.Alarm_Flag[(int)WorkThreadStep.测试])
                    {
                        // 报警中停止所有的动作
                        workStation.Alarm_Flag[(int)WorkThreadStep.测试] = false;
                        workStation.Run_Flag[(int)WorkThreadStep.测试] = false;
                    }
                    else
                    {
                        switch (workStation.Run_Step[(int)WorkThreadStep.测试])
                        {
                            case 0:
                                Thread.Sleep(10);
                                break;
                            case 10:
                                {
                                    global.writeLogProduce.Info("正在读取PLC测试信号！");
                                    workStation.Run_Step[(int)WorkThreadStep.测试]++;
                                }
                                break;
                            case 11:
                                {
                                    ReadPlcValue = global.inovanceH5UTcpTool.Read("D100", 1);
                                    workStation.Run_Step[(int)WorkThreadStep.测试]++;
                                }               
                                break;
                            case 12:
                                {
                                    if(ReadPlcValue == 1)
                                    {
                                        global.inovanceH5UTcpTool.Write("D102", 0);
                                        Thread.Sleep(10);
                                        global.inovanceH5UTcpTool.Write("D104", 0);
                                        Thread.Sleep(10);
                                        global.inovanceH5UTcpTool.Write("D110", 0);
                                        Thread.Sleep(10);
                                        global.inovanceH5UTcpTool.Write("D112", 0);

                                        if (AppConfig.GetInstance()._config.IsTestFlag)
                                        {
                                            // 测试时用
                                            m_rFIDData.Message = "";
                                            string json1 = JsonConvert.SerializeObject(m_rFIDData, Formatting.Indented);
                                            global.rFIDHelper.Write(json1);
                                        }
                                 
                                        IsRunState = false;
                                        global.writeLogProduce.Info("已收到PLC测试信号！");
                                        workStation.Run_Step[(int)WorkThreadStep.测试]++;
                                    }
                                    else
                                    {
                                        Thread.Sleep(50);
                                        workStation.Run_Step[(int)WorkThreadStep.测试]--;
                                    }
                   
                                }
                                break;
                            case 13:
                                try
                                {
                                    m_rFIDData.StationName = "";
                                    m_rFIDData.SN = "";
                                    m_rFIDData.Message = "";

                                    // 判断上个工位的RFID的数据
                                    var value = global.rFIDHelper.Read();

                                    global.writeLogProduce.Info("读取到上工位的RFID数据" + value.ToString());

                                    RFIDData rFIDData = JsonConvert.DeserializeObject<RFIDData>(value, new JsonSerializerSettings() { Formatting=Formatting.Indented});
                            
                                    if (rFIDData != null)
                                    {
                                        m_rFIDData.StationName = AppConfig.GetInstance()._config.Station_Name;
                                        m_rFIDData.SN = rFIDData.SN;
                                        AppConfig.GetInstance()._config.SN = rFIDData.SN;

                                        if (rFIDData.Message != "")// 存在NG
                                        {                                         
                                            // 给PLC发送OK
                                            global.inovanceH5UTcpTool.Write("D102", 1);
                                            global.writeLogProduce.Info("读取到上工位的RFID数据存在NG，D102写入1！");

                                            workStation.Run_Step[(int)WorkThreadStep.测试] = 10;
                                            break;
                                        }
                                        else
                                        {
                                            workStation.Run_Step[(int)WorkThreadStep.测试]++;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        workStation.Run_Step[(int)WorkThreadStep.测试]++;
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    global.writeLogSystem.Info("读取上工位RFID数据出错了->"+ex.ToString());
                                    workStation.Run_Step[(int)WorkThreadStep.测试] = 10;
                                    break;
                                }
                            case 14:
                                {
                                    global.G_eventManager.Trigger("发送运行时间","0ms");
                                    stopwatch.Restart();

                                    RestorePluginsHandler?.Invoke();
                                    RunProcessBarHandler?.Invoke(0);
                                    int id = 0;
                                    List<bool> _boolList = new List<bool>();
                                    //int count = global.RootNodeDataList.Count;
                                    int count = 0;
                                    foreach (var root3 in global.RootNodeDataList)// 表格内的每一行
                                    {
                                        if (root3.IsUse)
                                        {
                                            count++;
                                        }
                                    }

                                    foreach (var root3 in global.RootNodeDataList)// 表格内的每一行
                                    {
                                        _boolList.Clear();
                                        string ChildResult = "";

                                        id++;
                                        int id1 = 0;

                                        if (!root3.IsUse) continue;

                                        foreach (var root in new[] { root3.RootData1, root3.RootData2, root3.RootData3 })// 前提条件   操作步骤  期望结果
                                        {
                                            id1++;
                                            foreach (var child in root.Children)
                                            {                                               
                                                var param = child.TestParam;
                                                var result = child.Plugin?.Run(param);// 得到结果

                                                string name = child.Plugin.VersionStr;// 测试项名称
                                           
                                                if (id1 == 3)// 期望结果
                                                {                                                                                                    
                                                    int id2 = 0;
                                                    foreach (var kv in result)
                                                    {
                                                        id2++;
                                                        ChildResult += id2 + "：" + kv.Key +"："+ kv.Value + "\r\n";
                                                        global.writeLogProduce.Info(id2 + "：" + kv.Key + "：" + kv.Value + "\r\n");
                                                        SetCellValueThreadSafeHandler?.Invoke(id- 1, 5, ChildResult.Trim());
                                                        if(kv.Key.Contains("结果"))
                                                        {

                                                            if (kv.Value is bool)
                                                            {
                                                                if((!Convert.ToBoolean(kv.Value)) && (m_rFIDData.Message == ""))
                                                                {
                                                                    m_rFIDData.Message =   name + "：NG";
                                                                }
                                                                 _boolList.Add(Convert.ToBoolean(kv.Value));
                                                            }                                              
                                                        }
                                                        else if(kv.Key == "图片")
                                                        {
                                                            GYCamBitmapHandler?.Invoke((Bitmap)kv.Value);
                                                        }
                                                    }                                                                               
                                                }                                                
                                            }
                                            if(id1 == 3)
                                            {
                                                if (_boolList.All(x => x))
                                                {
                                                    SetCellValueThreadSafeHandler?.Invoke(id - 1, 6, "PASS");
                                                    SetCellColorHandler?.Invoke(id - 1, 6, true);
                                                }
                                                else
                                                {
                                                    IsRunState = true;
                                                    SetCellValueThreadSafeHandler?.Invoke(id - 1, 6, "FAIL");
                                                    SetCellColorHandler?.Invoke(id - 1, 6, false);
                                                }
                                                SetCellValueThreadSafeHandler?.Invoke(id - 1, 7, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                            }                                         
                                        }
                                        // 动态计算进度
                                        int progress = CalculateProgress(id, count);
                                        RunProcessBarHandler?.Invoke(progress);

                                        if (!workStation.Run_Flag[(int)WorkThreadStep.测试])// 检测到停止信号
                                        {
                                            workStation.Run_Step[(int)WorkThreadStep.测试] = 0;
                                            break;
                                        }
                                    }

                                    stopwatch.Stop();

                                    global.G_eventManager.Trigger("发送运行时间", stopwatch.ElapsedMilliseconds + "ms");

                                    workStation.Run_Step[(int)WorkThreadStep.测试] ++;
                                }
                                break;
                            case 15:
                                {
                                    if(IsRunState) // 存在NG
                                    {
                                        if(AppConfig.GetInstance()._config.IsRunType == "不重测")
                                        {
                                            workStation.Run_Step[(int)WorkThreadStep.测试]++;
                                            break;
                                        }
                                        else if (AppConfig.GetInstance()._config.IsRunType == "自动重测")
                                        {
                                            AutoRunCount++;
                                            if(AutoRunCount >=2)
                                            {
                                               
                                                AutoRunCount = 0;
                                                workStation.Run_Step[(int)WorkThreadStep.测试]++;
                                                break;
                                            }

                                            global.inovanceH5UTcpTool.Write("D104", 1);
                                            global.writeLogProduce.Info("给PLC发送NG, D104写入1！");
                                            Thread.Sleep(500);
                                            global.inovanceH5UTcpTool.Write("D112", 1);
                                            global.writeLogProduce.Info("给PLC发送重测, D112写入1！");
                                            workStation.Run_Step[(int)WorkThreadStep.测试] = 14;
                                            break;
                                        }
                                        else if (AppConfig.GetInstance()._config.IsRunType == "手动重测")
                                        {
                                            global.inovanceH5UTcpTool.Write("D104", 1);
                                            global.writeLogProduce.Info("给PLC发送NG, D104写入1！");
                                            workStation.Run_Step[(int)WorkThreadStep.测试] = 0;
                                            break;
                                        }
                                    }
                             
                                    workStation.Run_Step[(int)WorkThreadStep.测试]++;
                                }
                                break;
                            case 16:

                                string json = JsonConvert.SerializeObject(m_rFIDData, Formatting.Indented);
                                global.rFIDHelper.Write(json);
                                global.writeLogProduce.Info("写RFID->" + json);

                                if (!IsRunState) // OK
                                {
                                    Count++;
                                    OkCount++;

                                    // 给PLC发送OK
                                    global.inovanceH5UTcpTool.Write("D102", 1);
                                    global.writeLogProduce.Info("已向plc发送OK，D102地址写入1");
                                }
                                else // NG
                                {
                                    // 给PLC发送发行
                                    global.writeLogProduce.Info("已向plc发送发行，D110地址写入1");
                                    global.inovanceH5UTcpTool.Write("D110", 1);
                                    Count++;
                                }

                              
                                workStation.Run_Step[(int)WorkThreadStep.测试]++;
                                break;
                            case 17:
                                {

                                    if (AppConfig.GetInstance()._config.IsTestFlag)
                                    {
                                        // 测试时用
                                        m_rFIDData.Message = "";
                                        string json1 = JsonConvert.SerializeObject(m_rFIDData, Formatting.Indented);
                                        global.rFIDHelper.Write(json1);
                                    }


                                    workStation.Run_Step[(int)WorkThreadStep.测试]++;                                  
                                }
                                break;
                            case 18:
                                global.inovanceH5UTcpTool.Write("D100", 0); //暂不清除PLC信号
                                string str = SaveImage.CalculateYield(Count,OkCount);
                                global.G_eventManager.Trigger("发送良率", Count, OkCount, str);
                                workStation.Run_Step[(int)WorkThreadStep.测试] = 10;
                                break;
                            default:
                                global.writeLogSystem.Error("结果流程步骤出错了！");
                                break;
                        }
                    }
                }
                #endregion
            };
        }
        /// <summary>
        /// 计算进度百分比（测试次数转进度值）
        /// </summary>
        /// <param name="currentCount">当前已测试次数</param>
        /// <param name="totalCount">总测试次数</param>
        /// <returns>0-100的整数百分比</returns>
        public static int CalculateProgress(int currentCount, int totalCount)
        {
            if (totalCount <= 0) return 0; // 防止除零错误

            // 使用浮点计算避免整数除法
            double progress = (double)currentCount / totalCount * 100;

            // 四舍五入并限制范围
            return Math.Min(100, (int)Math.Round(progress));
        }
    }
    public class RFIDData
    {
        public string StationName { get; set; }
        public string SN { get; set; }
        public string Message { get; set; }
    }
}
