using HY_Rearview_Mirror.InterfaceTool;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HY_Rearview_Mirror.User
{
    public class GYCamTestParam : ITestParam
    {
        /// <summary>
        /// 九点坐标
        /// </summary>
        public List<Point> Pos9List{ get; set; }
        /// <summary>
        /// 曝光
        /// </summary>
        public string exposure { get; set; }
        /// <summary>
        /// 增益
        /// </summary>
        public string Gain { get; set; }
        /// <summary>
        /// X曝光
        /// </summary>
        public string X_exposure { get; set; }
        /// <summary>
        /// Y曝光
        /// </summary>
        public string Y_exposure { get; set; }
        /// <summary>
        /// Z曝光
        /// </summary>
        public string Z_exposure { get; set; }
        /// <summary>
        /// 转换数据类型
        /// </summary>
        public string ChangeDataType { get; set; }
        /// <summary>
        /// 判断类型
        /// </summary>
        public string JudgeType { get; set; }
        /// <summary>
        /// 判断值数组
        /// </summary>
        public List<string> JudgeValueList { get; set; }
        /// <summary>
        ///// 对比度判断值
        ///// </summary>
        //public string JudgeContrastValue { get; set; }
        ///// <summary>
        ///// <summary>
        ///// X色坐标判断值
        ///// </summary>
        //public string JudgeXChromaticValue { get; set; }
        ///// Y色坐标判断值
        ///// </summary>
        //public string JudgeYChromaticValue { get; set; }
        /// <summary>
        /// 运行结果
        /// </summary>
        public bool RunResult { get; set; }

        public GYCamTestParam()
        {
            Pos9List = new List<Point>();
            exposure = "50000";
            Gain = "0";
            X_exposure = "50000";
            Y_exposure = "50000";
            Z_exposure = "50000";

            RunResult = false;
            ChangeDataType = "";
            JudgeType = "";

            JudgeValueList = new List<string>();
        }
    }
    /// <summary>
    /// 光研成像色度仪
    /// </summary>
    public class GYCamFun : IPlugin
    {
        public string PluginId => "亮色度检测";

        public string moduleName => "成像色度仪";

        public string DescribeMessage => "用于亮色度检测";

        public string VersionStr1 = "";
        public string VersionStr => VersionStr1;

        [JsonIgnore]
        public GYCamSetForm gYCamSetForm = null;  // 在基类中存储窗体
        public GYCamTestParam gYCamTestParam { get; set; }

        public GYCamFun()
        {
            gYCamTestParam = new GYCamTestParam();
        }

        public Form CreateUI(ITestParam testParam, string VersionStr = "")
        {
            gYCamSetForm = new GYCamSetForm();

            gYCamSetForm.eventManager.AddListener("发送界面参数", new Action<List<object>>(SetDataForm_OrderCompleted));
            gYCamSetForm.eventManager.AddListener("关闭窗体", new Action(FormClosing));
            gYCamSetForm.eventManager.AddListener("运行", new Func<Dictionary<string, object>>(Run));

            gYCamSetForm.SetDataToForm(testParam, VersionStr);
            if (testParam != null)
            {
                gYCamTestParam = testParam as GYCamTestParam;
            }
            return gYCamSetForm;
        }
        public void FormClosing()
        {
            gYCamSetForm.eventManager.Clear();
        }
        public Dictionary<string, object> Run()
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();

            switch(gYCamTestParam.ChangeDataType)
            {
                case "亮度":
                    {
                        global.gYCamTool.SetExpoGain(gYCamTestParam.exposure, gYCamTestParam.Gain, gYCamTestParam.X_exposure, gYCamTestParam.Y_exposure, gYCamTestParam.Z_exposure);

                        var list = global.gYCamTool.GetDeviceSingleFrameAutoExp(gYCamTestParam.Pos9List);

                        foreach (var item in list)
                        {
                            keyValuePairs.Add("亮度", item.亮度);
                            keyValuePairs.Add("X色坐标", item.X色坐标);
                            keyValuePairs.Add("Y色坐标", item.Y色坐标);
                            keyValuePairs.Add("图片", item.disBMP);
                        }

                        if (gYCamTestParam.JudgeValueList[0] == "") break;
                        double value = Convert.ToDouble(list[0].亮度);
                        double value1 = Convert.ToDouble(gYCamTestParam.JudgeValueList[0]);

                        // 判断要执行的操作
                        switch (gYCamTestParam.JudgeType)
                        {
                            case "大于":
                                {                   
                                    if (value > (value1 - Convert.ToDouble(gYCamTestParam.JudgeValueList[1])))
                                    {
                                        keyValuePairs.Add("亮度结果", true);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("亮度结果", false);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试不合格");
                                    }
                                }
                                break;
                            case "小于":
                                {
                                    if (value < (value1 + Convert.ToDouble(gYCamTestParam.JudgeValueList[1])))
                                    {
                                        keyValuePairs.Add("亮度结果", true);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("亮度结果", false);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试不合格");
                                    }
                                }
                                break;
                            case "大于小于":
                                {
                                    if (value > (value1 - Convert.ToDouble(gYCamTestParam.JudgeValueList[1])) && value < (value1 + Convert.ToDouble(gYCamTestParam.JudgeValueList[1])))
                                    {
                                        keyValuePairs.Add("亮度结果", true);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("亮度结果", false);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试不合格");
                                    }
                                }
                                break;
                            case "空":
                                {
                                    keyValuePairs.Add("亮度结果", true);
                                    keyValuePairs.Add("亮度结果描述", "没选择判断类型！");
                                }
                                break;
                            default:
                                {
                                    gYCamTestParam.RunResult = false;
                                    keyValuePairs.Add("亮度结果", gYCamTestParam.RunResult);
                                    keyValuePairs.Add("亮度结果描述", "没有判断操作");
                                }
                                break;
                        }
                    }
                    break;
                case "色度":
                    {
                        global.gYCamTool.SetExpoGain(gYCamTestParam.exposure, gYCamTestParam.Gain, gYCamTestParam.X_exposure, gYCamTestParam.Y_exposure, gYCamTestParam.Z_exposure);

                        var list = global.gYCamTool.GetDeviceSingleFrameAutoExp(gYCamTestParam.Pos9List);

                        foreach (var item in list)
                        {
                            keyValuePairs.Add("亮度", item.亮度);
                            keyValuePairs.Add("X色坐标", item.X色坐标);
                            keyValuePairs.Add("Y色坐标", item.Y色坐标);
                            keyValuePairs.Add("图片", item.disBMP);
                        }

                        if (gYCamTestParam.JudgeValueList.Count <= 0) break;

                        double JudgeValueX = Convert.ToDouble(list[0].X色坐标);
                        double JudgeValueY = Convert.ToDouble(list[0].Y色坐标);

                        double ValueX = Convert.ToDouble(gYCamTestParam.JudgeValueList[0]);
                        double ValueY = Convert.ToDouble(gYCamTestParam.JudgeValueList[1]);

                        // 判断要执行的操作
                        switch (gYCamTestParam.JudgeType)
                        {
                            case "大于":
                                {
                                    if (JudgeValueX > (ValueX - Convert.ToDouble(gYCamTestParam.JudgeValueList[2])) &&(JudgeValueY > (ValueY - Convert.ToDouble(gYCamTestParam.JudgeValueList[3]))))
                                    {
                                        keyValuePairs.Add("色度结果", true);
                                        keyValuePairs.Add("色度结果描述", "色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("色度结果", false);
                                        keyValuePairs.Add("色度结果描述", "色度测试不合格");
                                    }
                                }
                                break;
                            case "小于":
                                {
                                    if (JudgeValueX < (ValueX + Convert.ToDouble(gYCamTestParam.JudgeValueList[2])) && (JudgeValueY < (ValueY + Convert.ToDouble(gYCamTestParam.JudgeValueList[3]))))
                                    {
                                        keyValuePairs.Add("色度结果", true);
                                        keyValuePairs.Add("色度结果描述", "色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("色度结果", false);
                                        keyValuePairs.Add("色度结果描述", "色度测试不合格");
                                    }
                                }
                                break;
                            case "大于小于":
                                {
                                    if (JudgeValueX > (ValueX - Convert.ToDouble(gYCamTestParam.JudgeValueList[2])) && (JudgeValueY > (ValueY - Convert.ToDouble(gYCamTestParam.JudgeValueList[3])))
                                        &&JudgeValueX < (ValueX + Convert.ToDouble(gYCamTestParam.JudgeValueList[2])) && (JudgeValueY < (ValueY + Convert.ToDouble(gYCamTestParam.JudgeValueList[3]))))
                                    {
                                        keyValuePairs.Add("色度结果", true);
                                        keyValuePairs.Add("色度结果描述", "色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("色度结果", false);
                                        keyValuePairs.Add("色度结果描述", "色度测试不合格");
                                    }
                                }
                                break;
                            case "空":
                                {
                                    keyValuePairs.Add("色度结果", true);
                                    keyValuePairs.Add("色度结果描述", "没选择判断类型！");
                                }
                                break;
                            default:
                                {
                                    gYCamTestParam.RunResult = false;
                                    keyValuePairs.Add("色度结果", gYCamTestParam.RunResult);
                                    keyValuePairs.Add("色度结果描述", "没有判断操作");
                                }
                                break;
                        }
                    }
                    break;
                case "对比度":
                    {
                        // 1:切白图
                        global.can1Service.Send(0x7FF, new byte[] { 3, 3, 0, 0, 0, 0, 0, 0 }, true);
                        Thread.Sleep(100);
                        // 2:测试白图亮色度
                        global.gYCamTool.SetExpoGain(gYCamTestParam.exposure, gYCamTestParam.Gain, gYCamTestParam.X_exposure, gYCamTestParam.Y_exposure, gYCamTestParam.Z_exposure);
                        var list = global.gYCamTool.GetDeviceSingleFrameAutoExp(gYCamTestParam.Pos9List);
                        foreach (var item in list)
                        {
                            keyValuePairs.Add("白场亮度", item.亮度);
                        }
                        // 3：切黑图
                        global.can1Service.Send(0x7FF, new byte[] { 3, 2, 0, 0, 0, 0, 0, 0 }, true);
                        Thread.Sleep(100);
                        // 4：测试黑图亮色度
                        var list1 = global.gYCamTool.GetDeviceSingleFrameAutoExp(gYCamTestParam.Pos9List);
                        foreach (var item in list1)
                        {
                            keyValuePairs.Add("黑场亮度", item.亮度);
                        }
                        // 5：计算对比度（白图亮度除以黑图亮度）
                        //var value = Convert.ToDouble(list[0].亮度) / Convert.ToDouble(list1[0].亮度);

                        double[] results = new double[1];
                        GYCam_Mini.LCRTest(new double[] { Convert.ToDouble(list[0].亮度) }, new double[] { Convert.ToDouble(list1[0].亮度) }, 1, results);

                        keyValuePairs.Add("结果", true);
                        keyValuePairs.Add("对比度值", results[0]);
                        keyValuePairs.Add("结果描述", "对比度测试合格");

                        if (gYCamTestParam.JudgeValueList.Count <= 0) break;

                        double value = Convert.ToDouble(results[0]);
                        double value1 = Convert.ToDouble(gYCamTestParam.JudgeValueList[0]);

                        // 判断要执行的操作
                        switch (gYCamTestParam.JudgeType)
                        {
                            case "大于":
                                {
                                    if (value > (value1 - Convert.ToDouble(gYCamTestParam.JudgeValueList[1])))
                                    {
                                        keyValuePairs.Add("对比度结果", true);
                                        keyValuePairs.Add("对比度结果描述", "对比度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("对比度结果", false);
                                        keyValuePairs.Add("对比度结果描述", "对比度测试不合格");
                                    }
                                }
                                break;
                            case "小于":
                                {
                                    if (value < (value1 + Convert.ToDouble(gYCamTestParam.JudgeValueList[1])))
                                    {
                                        keyValuePairs.Add("对比度结果", true);
                                        keyValuePairs.Add("对比度结果描述", "对比度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("对比度结果", false);
                                        keyValuePairs.Add("对比度结果描述", "对比度测试不合格");
                                    }
                                }
                                break;
                            case "大于小于":
                                {
                                    if (value > (value1 - Convert.ToDouble(gYCamTestParam.JudgeValueList[1])) && value < (value1 + Convert.ToDouble(gYCamTestParam.JudgeValueList[1])))
                                    {
                                        keyValuePairs.Add("结果", true);
                                        keyValuePairs.Add("结果描述", "对比度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("对比度结果", false);
                                        keyValuePairs.Add("对比度结果描述", "对比度测试不合格");
                                    }
                                }
                                break;
                            case "空":
                                {
                                    keyValuePairs.Add("对比度结果", true);
                                    keyValuePairs.Add("对比度结果描述", "没选择判断类型！");
                                }
                                break;
                            default:
                                {
                                    gYCamTestParam.RunResult = false;
                                    keyValuePairs.Add("对比度结果", gYCamTestParam.RunResult);
                                    keyValuePairs.Add("对比度结果描述", "没有判断操作");
                                }
                                break;
                        }
                    }
                    break;
                case "亮度、色度":
                    {
                        global.gYCamTool.SetExpoGain(gYCamTestParam.exposure, gYCamTestParam.Gain, gYCamTestParam.X_exposure, gYCamTestParam.Y_exposure, gYCamTestParam.Z_exposure);

                        var list = global.gYCamTool.GetDeviceSingleFrameAutoExp(gYCamTestParam.Pos9List);

                        foreach (var item in list)
                        {
                            keyValuePairs.Add("亮度", item.亮度);
                            keyValuePairs.Add("X色坐标", item.X色坐标);
                            keyValuePairs.Add("Y色坐标", item.Y色坐标);
                            keyValuePairs.Add("图片", item.disBMP);
                        }

                        if (gYCamTestParam.JudgeValueList.Count <= 0) break;

                        double value1 = Convert.ToDouble(gYCamTestParam.JudgeValueList[0]);// 亮度标准值
                        double value2 = Convert.ToDouble(gYCamTestParam.JudgeValueList[1]);// 亮度阈值
                        double value3 = Convert.ToDouble(gYCamTestParam.JudgeValueList[2]);// 色坐标X标准值
                        double value4 = Convert.ToDouble(gYCamTestParam.JudgeValueList[3]);// 色坐标X阈值
                        double value5 = Convert.ToDouble(gYCamTestParam.JudgeValueList[4]);// 色坐标Y标准值
                        double value6 = Convert.ToDouble(gYCamTestParam.JudgeValueList[5]);// 色坐标Y阈值

                        double value7 = Convert.ToDouble(list[0].亮度);
                        double value8 = Convert.ToDouble(list[0].X色坐标);
                        double value9 = Convert.ToDouble(list[0].Y色坐标);

                        // 亮度判断要执行的操作
                        switch (gYCamTestParam.JudgeType)
                        {
                            case "大于":
                                {
                                    if (value7 > (value1 - value2))
                                    {
                                        keyValuePairs.Add("亮度结果", true);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("亮度结果", false);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试不合格");
                                    }
                                }
                                break;
                            case "小于":
                                {
                                    if (value7 > (value1 + value2))
                                    {
                                        keyValuePairs.Add("亮度结果", true);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("亮度结果", false);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试不合格");
                                    }
                                }
                                break;
                            case "大于小于":
                                {
                                    if (value7 > (value1 - value2) && value7 > (value1 + value2))
                                    {
                                        keyValuePairs.Add("亮度结果", true);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("亮度结果", false);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试不合格");
                                    }
                                }
                                break;
                            case "空":
                                {
                                    keyValuePairs.Add("亮度结果", true);
                                    keyValuePairs.Add("亮度结果描述", "没选择判断类型！");
                                }
                                break;
                            default:
                                {
                                    gYCamTestParam.RunResult = false;
                                    keyValuePairs.Add("亮度结果", gYCamTestParam.RunResult);
                                    keyValuePairs.Add("亮度结果描述", "没有判断操作");
                                }
                                break;
                        }

                        // 色度判断要执行的操作
                        switch (gYCamTestParam.JudgeType)
                        {
                            case "大于":
                                {
                                    if (value8 > (value3 - value4) &&
                                         value9 > (value5 - value6))
                                    {
                                        keyValuePairs.Add("色度结果", true);
                                        keyValuePairs.Add("色度结果描述", "色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("色度结果", false);
                                        keyValuePairs.Add("色度结果描述", "色度测试不合格");
                                    }
                                }
                                break;
                            case "小于":
                                {
                                    if (value8 < (value3 + value4) &&
                                       value9 < (value5 + value6))
                                    {
                                        keyValuePairs.Add("色度结果", true);
                                        keyValuePairs.Add("色度结果描述", "色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("色度结果", false);
                                        keyValuePairs.Add("色度结果描述", "色度测试不合格");
                                    }
                                }
                                break;
                            case "大于小于":
                                {
                                    if (value8 > (value3 - value4) &&
                                         value9 > (value5 - value6) &&
                                         value8 < (value3 + value4) &&
                                         value9 < (value5 + value6))
                                    {
                                        keyValuePairs.Add("色度结果", true);
                                        keyValuePairs.Add("色度结果描述", "色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("色度结果", false);
                                        keyValuePairs.Add("色度结果描述", "色度测试不合格");
                                    }
                                }
                                break;
                            case "空":
                                {
                                    keyValuePairs.Add("色度结果", true);
                                    keyValuePairs.Add("色度结果描述", "没选择判断类型！");
                                }
                                break;
                            default:
                                {
                                    gYCamTestParam.RunResult = false;
                                    keyValuePairs.Add("色度结果", gYCamTestParam.RunResult);
                                    keyValuePairs.Add("色度结果描述", "没有判断操作");
                                }
                                break;
                        }

                    }
                    break;
                case "亮度、色度、对比度":
                    {

                        if (gYCamTestParam.JudgeValueList.Count <= 0) break;

                        double value1 = Convert.ToDouble(gYCamTestParam.JudgeValueList[0]);// 黑场亮度标准值
                        double value2 = Convert.ToDouble(gYCamTestParam.JudgeValueList[1]);// 黑场亮度阈值
                        double value3 = Convert.ToDouble(gYCamTestParam.JudgeValueList[2]);//  黑场色坐标X标准值
                        double value4 = Convert.ToDouble(gYCamTestParam.JudgeValueList[3]);//  黑场色坐标X阈值
                        double value5 = Convert.ToDouble(gYCamTestParam.JudgeValueList[4]);//  黑场色坐标Y标准值
                        double value6 = Convert.ToDouble(gYCamTestParam.JudgeValueList[5]);//  黑场色坐标Y阈值

                        double value7 = Convert.ToDouble(gYCamTestParam.JudgeValueList[6]);// 白场亮度标准值
                        double value8 = Convert.ToDouble(gYCamTestParam.JudgeValueList[7]);// 白场亮度阈值
                        double value9 = Convert.ToDouble(gYCamTestParam.JudgeValueList[8]);//  白场色坐标X标准值
                        double value10 = Convert.ToDouble(gYCamTestParam.JudgeValueList[9]);//  白场色坐标X阈值
                        double value11 = Convert.ToDouble(gYCamTestParam.JudgeValueList[10]);//  白场色坐标Y标准值
                        double value12 = Convert.ToDouble(gYCamTestParam.JudgeValueList[11]);//  白场色坐标Y阈值

                        double value13 = Convert.ToDouble(gYCamTestParam.JudgeValueList[12]);//  对比度标准值
                        double value14 = Convert.ToDouble(gYCamTestParam.JudgeValueList[13]);//  对比度阈值

                        // 1:切白图
                        global.can1Service.Send(0x7FF, new byte[] { 3, 3, 0, 0, 0, 0, 0, 0 }, true);
                        Thread.Sleep(100);
                        // 2:测试白图亮色度
                        global.gYCamTool.SetExpoGain(gYCamTestParam.exposure, gYCamTestParam.Gain, gYCamTestParam.X_exposure, gYCamTestParam.Y_exposure, gYCamTestParam.Z_exposure);
                        var list = global.gYCamTool.GetDeviceSingleFrameAutoExp(gYCamTestParam.Pos9List);
                        foreach (var item in list)
                        {
                            keyValuePairs.Add("白场亮度", item.亮度);
                            keyValuePairs.Add("白场X色坐标", item.X色坐标);
                            keyValuePairs.Add("白场Y色坐标", item.Y色坐标);
                        }

                        double value15 = Convert.ToDouble(list[0].亮度);
                        double value16 = Convert.ToDouble(list[0].X色坐标);
                        double value17 = Convert.ToDouble(list[0].Y色坐标);

                        // 亮度判断要执行的操作
                        switch (gYCamTestParam.JudgeType)
                        {
                            case "大于":
                                {
                                    if (value15 > (value7 - value8))
                                    {
                                        keyValuePairs.Add("白场亮度结果", true);
                                        keyValuePairs.Add("白场亮度结果描述", "白场亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("白场亮度结果", false);
                                        keyValuePairs.Add("白场亮度结果描述", "白场亮度测试不合格");
                                    }
                                }
                                break;
                            case "小于":
                                {
                                    if (value15 > (value7 + value8))
                                    {
                                        keyValuePairs.Add("白场亮度结果", true);
                                        keyValuePairs.Add("白场亮度结果描述", "白场亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("白场亮度结果", false);
                                        keyValuePairs.Add("白场亮度结果描述", "白场亮度测试不合格");
                                    }
                                }
                                break;
                            case "大于小于":
                                {
                                    if (value15 > (value7 - value8) && value15 < (value7 + value8))
                                    {
                                        keyValuePairs.Add("白场亮度结果", true);
                                        keyValuePairs.Add("白场亮度结果描述", "白场亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("白场亮度结果", false);
                                        keyValuePairs.Add("白场亮度结果描述", "白场亮度测试不合格");
                                    }
                                }
                                break;
                            case "空":
                                {
                                    keyValuePairs.Add("白场亮度结果", true);
                                    keyValuePairs.Add("白场亮度结果描述", "没选择判断类型！");
                                }
                                break;
                            default:
                                {
                                    gYCamTestParam.RunResult = false;
                                    keyValuePairs.Add("白场亮度结果", gYCamTestParam.RunResult);
                                    keyValuePairs.Add("白场亮度结果描述", "没有判断操作");
                                }
                                break;
                        }

                        // 色度判断要执行的操作
                        switch (gYCamTestParam.JudgeType)
                        {
                            case "大于":
                                {
                                    if (value16 > (value9 - value10) &&
                                         value17 > (value11 - value12))
                                    {
                                        keyValuePairs.Add("白场色度结果", true);
                                        keyValuePairs.Add("白场色度结果描述", "白场色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("白场色度结果", false);
                                        keyValuePairs.Add("白场色度结果描述", "白场色度测试不合格");
                                    }
                                }
                                break;
                            case "小于":
                                {
                                    if (value16 < (value9 + value10) &&
                                       value17 < (value11 + value12))
                                    {
                                        keyValuePairs.Add("白场色度结果", true);
                                        keyValuePairs.Add("白场色度结果描述", "白场色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("白场色度结果", false);
                                        keyValuePairs.Add("白场色度结果描述", "白场色度测试不合格");
                                    }
                                }
                                break;
                            case "大于小于":
                                {
                                    if (value16 > (value9 - value10) &&
                                         value17 > (value11 - value12) &&
                                         value16 < (value9 + value10) &&
                                         value17 < (value11 + value12))
                                    {
                                        keyValuePairs.Add("白场色度结果", true);
                                        keyValuePairs.Add("白场色度结果描述", "白场色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("白场色度结果", false);
                                        keyValuePairs.Add("白场色度结果描述", "白场色度测试不合格");
                                    }
                                }
                                break;
                            case "空":
                                {
                                    keyValuePairs.Add("白场色度结果", true);
                                    keyValuePairs.Add("白场色度结果描述", "没选择判断类型！");
                                }
                                break;
                            default:
                                {
                                    gYCamTestParam.RunResult = false;
                                    keyValuePairs.Add("白场色度结果", gYCamTestParam.RunResult);
                                    keyValuePairs.Add("白场色度结果描述", "没有判断操作");
                                }
                                break;
                        }


                        // 3：切黑场
                        global.can1Service.Send(0x7FF, new byte[] { 3, 2, 0, 0, 0, 0, 0, 0 }, true);
                        Thread.Sleep(100);
                        // 4：测试黑图亮色度
                        var list1 = global.gYCamTool.GetDeviceSingleFrameAutoExp(gYCamTestParam.Pos9List);
                        foreach (var item in list1)
                        {
                            keyValuePairs.Add("黑场亮度", item.亮度);
                            keyValuePairs.Add("黑场X色坐标", item.X色坐标);
                            keyValuePairs.Add("黑场Y色坐标", item.Y色坐标);
                            keyValuePairs.Add("图片", item.disBMP);
                        }

                        double value18 = Convert.ToDouble(list1[0].亮度);
                        double value19 = Convert.ToDouble(list1[0].X色坐标);
                        double value20 = Convert.ToDouble(list1[0].Y色坐标);

                        // 亮度判断要执行的操作
                        switch (gYCamTestParam.JudgeType)
                        {
                            case "大于":
                                {
                                    if (value18 > (value1 - value2))
                                    {
                                        keyValuePairs.Add("黑场亮度结果", true);
                                        keyValuePairs.Add("黑场亮度结果描述", "黑场亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("黑场亮度结果", false);
                                        keyValuePairs.Add("黑场亮度结果描述", "黑场亮度测试不合格");
                                    }
                                }
                                break;
                            case "小于":
                                {
                                    if (value18 > (value1 + value2))
                                    {
                                        keyValuePairs.Add("黑场亮度结果", true);
                                        keyValuePairs.Add("黑场亮度结果描述", "黑场亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("黑场亮度结果", false);
                                        keyValuePairs.Add("黑场亮度结果描述", "黑场亮度测试不合格");
                                    }
                                }
                                break;
                            case "大于小于":
                                {
                                    if (value18 > (value1 - value2) && value18 < (value1 + value2))
                                    {
                                        keyValuePairs.Add("黑场亮度结果", true);
                                        keyValuePairs.Add("黑场亮度结果描述", "黑场亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("黑场亮度结果", false);
                                        keyValuePairs.Add("黑场亮度结果描述", "黑场亮度测试不合格");
                                    }
                                }
                                break;
                            case "空":
                                {
                                    keyValuePairs.Add("黑场亮度结果", true);
                                    keyValuePairs.Add("黑场亮度结果描述", "没选择判断类型！");
                                }
                                break;
                            default:
                                {
                                    gYCamTestParam.RunResult = false;
                                    keyValuePairs.Add("黑场亮度结果", gYCamTestParam.RunResult);
                                    keyValuePairs.Add("黑场亮度结果描述", "没有判断操作");
                                }
                                break;
                        }

                        // 色度判断要执行的操作
                        switch (gYCamTestParam.JudgeType)
                        {
                            case "大于":
                                {
                                    if (value19 > (value3 - value4) &&
                                         value20 > (value5 - value6))
                                    {
                                        keyValuePairs.Add("黑场色度结果", true);
                                        keyValuePairs.Add("黑场色度结果描述", "黑场色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("黑场色度结果", false);
                                        keyValuePairs.Add("黑场色度结果描述", "黑场色度测试不合格");
                                    }
                                }
                                break;
                            case "小于":
                                {
                                    if (value19 < (value3 + value4) &&
                                       value20 < (value5 + value6))
                                    {
                                        keyValuePairs.Add("黑场色度结果", true);
                                        keyValuePairs.Add("黑场色度结果描述", "黑场色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("黑场色度结果", false);
                                        keyValuePairs.Add("黑场色度结果描述", "黑场色度测试不合格");
                                    }
                                }
                                break;
                            case "大于小于":
                                {
                                    if (value19 > (value3 - value4) &&
                                         value20 > (value5 - value6) &&
                                         value19 < (value3 + value4) &&
                                         value20 < (value5 + value6))
                                    {
                                        keyValuePairs.Add("黑场色度结果", true);
                                        keyValuePairs.Add("黑场色度结果描述", "黑场色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("黑场色度结果", false);
                                        keyValuePairs.Add("黑场色度结果描述", "黑场色度测试不合格");
                                    }
                                }
                                break;
                            case "空":
                                {
                                    keyValuePairs.Add("黑场色度结果", true);
                                    keyValuePairs.Add("黑场色度结果描述", "没选择判断类型！");
                                }
                                break;
                            default:
                                {
                                    gYCamTestParam.RunResult = false;
                                    keyValuePairs.Add("黑场色度结果", gYCamTestParam.RunResult);
                                    keyValuePairs.Add("黑场色度结果描述", "没有判断操作");
                                }
                                break;
                        }


                        // 5：计算对比度（白图亮度除以黑图亮度）
                        //var value = Convert.ToDouble(list[0].亮度) / Convert.ToDouble(list1[0].亮度);

                        double[] results = new double[1];
                        GYCam_Mini.LCRTest(new double[] { Convert.ToDouble(list[0].亮度) }, new double[] { Convert.ToDouble(list1[0].亮度) }, 1, results);

                        keyValuePairs.Add("结果", true);
                        keyValuePairs.Add("对比度值", results[0]);
                        keyValuePairs.Add("结果描述", "对比度测试合格");

                        double value21 = Convert.ToDouble(results[0]);

                        // 对比度判断要执行的操作
                        switch (gYCamTestParam.JudgeType)
                        {
                            case "大于":
                                {
                                    if (value21 > (value13 - value14))
                                    {
                                        keyValuePairs.Add("对比度结果", true);
                                        keyValuePairs.Add("对比度结果描述", "对比度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("对比度结果", false);
                                        keyValuePairs.Add("对比度结果描述", "对比度测试不合格");
                                    }
                                }
                                break;
                            case "小于":
                                {
                                    if (value21 > (value13 + value14))
                                    {
                                        keyValuePairs.Add("对比度结果", true);
                                        keyValuePairs.Add("对比度结果描述", "对比度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("对比度结果", false);
                                        keyValuePairs.Add("对比度结果描述", "对比度测试不合格");
                                    }
                                }
                                break;
                            case "大于小于":
                                {
                                    if (value21 > (value13 - value14) && value21 < (value13 + value14))
                                    {
                                        keyValuePairs.Add("对比度结果", true);
                                        keyValuePairs.Add("对比度结果描述", "对比度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("对比度结果", false);
                                        keyValuePairs.Add("对比度结果描述", "对比度测试不合格");
                                    }
                                }
                                break;
                            case "空":
                                {
                                    keyValuePairs.Add("对比度结果", true);
                                    keyValuePairs.Add("对比度结果描述", "没选择判断类型！");
                                }
                                break;
                            default:
                                {
                                    gYCamTestParam.RunResult = false;
                                    keyValuePairs.Add("对比度结果", gYCamTestParam.RunResult);
                                    keyValuePairs.Add("对比度结果描述", "没有判断操作");
                                }
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }

            return keyValuePairs;
        }
        private void SetDataForm_OrderCompleted(List<object> e)
        {
            gYCamTestParam.Pos9List = (List<Point>)e[0];
            VersionStr1 = Convert.ToString(e[1]);

            gYCamTestParam.exposure = Convert.ToString(e[2]);
            gYCamTestParam.Gain = Convert.ToString(e[3]);
            gYCamTestParam.X_exposure = Convert.ToString(e[4]);
            gYCamTestParam.Y_exposure = Convert.ToString(e[5]);
            gYCamTestParam.Z_exposure = Convert.ToString(e[6]);

            gYCamTestParam.ChangeDataType = Convert.ToString(e[7]);
            gYCamTestParam.JudgeType = Convert.ToString(e[8]);

            gYCamTestParam.JudgeValueList = e[9] as List<string>;        
        }

        public void Dispose()
        {

        }

        public ITestParam GetData()
        {
            return gYCamTestParam;
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
            var p = (GYCamTestParam)testParam;

            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();

            switch (p.ChangeDataType)
            {
                case "亮度":
                    {
                        global.gYCamTool.SetExpoGain(p.exposure, p.Gain, p.X_exposure, p.Y_exposure, p.Z_exposure);

                        var list = global.gYCamTool.GetDeviceSingleFrameAutoExp(p.Pos9List);

                        foreach (var item in list)
                        {
                            keyValuePairs.Add("亮度", item.亮度);
                            keyValuePairs.Add("X色坐标", item.X色坐标);
                            keyValuePairs.Add("Y色坐标", item.Y色坐标);
                            keyValuePairs.Add("图片", item.disBMP);
                        }

                        if (p.JudgeValueList[0] == "") break;
                        double value = Convert.ToDouble(list[0].亮度);
                        double value1 = Convert.ToDouble(p.JudgeValueList[0]);

                        // 判断要执行的操作
                        switch (p.JudgeType)
                        {
                            case "大于":
                                {
                                    if (value > (value1 - Convert.ToDouble(p.JudgeValueList[1])))
                                    {
                                        keyValuePairs.Add("亮度结果", true);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("亮度结果", false);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试不合格");
                                    }
                                }
                                break;
                            case "小于":
                                {
                                    if (value < (value1 + Convert.ToDouble(p.JudgeValueList[1])))
                                    {
                                        keyValuePairs.Add("亮度结果", true);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("亮度结果", false);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试不合格");
                                    }
                                }
                                break;
                            case "大于小于":
                                {
                                    if (value > (value1 - Convert.ToDouble(p.JudgeValueList[1])) && value < (value1 + Convert.ToDouble(p.JudgeValueList[1])))
                                    {
                                        keyValuePairs.Add("亮度结果", true);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("亮度结果", false);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试不合格");
                                    }
                                }
                                break;
                            case "空":
                                {
                                    keyValuePairs.Add("亮度结果", true);
                                    keyValuePairs.Add("亮度结果描述", "没选择判断类型！");
                                }
                                break;
                            default:
                                {
                                    p.RunResult = false;
                                    keyValuePairs.Add("亮度结果", p.RunResult);
                                    keyValuePairs.Add("亮度结果描述", "没有判断操作");
                                }
                                break;
                        }
                    }
                    break;
                case "色度":
                    {
                        global.gYCamTool.SetExpoGain(p.exposure, p.Gain, p.X_exposure, p.Y_exposure, p.Z_exposure);

                        var list = global.gYCamTool.GetDeviceSingleFrameAutoExp(p.Pos9List);

                        foreach (var item in list)
                        {
                            keyValuePairs.Add("亮度", item.亮度);
                            keyValuePairs.Add("X色坐标", item.X色坐标);
                            keyValuePairs.Add("Y色坐标", item.Y色坐标);
                            keyValuePairs.Add("图片", item.disBMP);
                        }

                        if (p.JudgeValueList.Count <= 0) break;

                        double JudgeValueX = Convert.ToDouble(list[0].X色坐标);
                        double JudgeValueY = Convert.ToDouble(list[0].Y色坐标);

                        double ValueX = Convert.ToDouble(p.JudgeValueList[0]);
                        double ValueY = Convert.ToDouble(p.JudgeValueList[1]);

                        // 判断要执行的操作
                        switch (p.JudgeType)
                        {
                            case "大于":
                                {
                                    if (JudgeValueX > (ValueX - Convert.ToDouble(p.JudgeValueList[2])) && (JudgeValueY > (ValueY - Convert.ToDouble(p.JudgeValueList[3]))))
                                    {
                                        keyValuePairs.Add("色度结果", true);
                                        keyValuePairs.Add("色度结果描述", "色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("色度结果", false);
                                        keyValuePairs.Add("色度结果描述", "色度测试不合格");
                                    }
                                }
                                break;
                            case "小于":
                                {
                                    if (JudgeValueX < (ValueX + Convert.ToDouble(p.JudgeValueList[2])) && (JudgeValueY < (ValueY + Convert.ToDouble(p.JudgeValueList[3]))))
                                    {
                                        keyValuePairs.Add("色度结果", true);
                                        keyValuePairs.Add("色度结果描述", "色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("色度结果", false);
                                        keyValuePairs.Add("色度结果描述", "色度测试不合格");
                                    }
                                }
                                break;
                            case "大于小于":
                                {
                                    if (JudgeValueX > (ValueX - Convert.ToDouble(p.JudgeValueList[2])) && (JudgeValueY > (ValueY - Convert.ToDouble(p.JudgeValueList[3])))
                                        && JudgeValueX < (ValueX + Convert.ToDouble(p.JudgeValueList[2])) && (JudgeValueY < (ValueY + Convert.ToDouble(p.JudgeValueList[3]))))
                                    {
                                        keyValuePairs.Add("色度结果", true);
                                        keyValuePairs.Add("色度结果描述", "色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("色度结果", false);
                                        keyValuePairs.Add("色度结果描述", "色度测试不合格");
                                    }
                                }
                                break;
                            case "空":
                                {
                                    keyValuePairs.Add("色度结果", true);
                                    keyValuePairs.Add("色度结果描述", "没选择判断类型！");
                                }
                                break;
                            default:
                                {
                                    p.RunResult = false;
                                    keyValuePairs.Add("色度结果", p.RunResult);
                                    keyValuePairs.Add("色度结果描述", "没有判断操作");
                                }
                                break;
                        }
                    }
                    break;
                case "对比度":
                    {
                        // 1:切白图
                        global.can1Service.Send(0x7FF, new byte[] { 3, 3, 0, 0, 0, 0, 0, 0 }, true);
                        Thread.Sleep(100);
                        // 2:测试白图亮色度
                        global.gYCamTool.SetExpoGain(p.exposure, p.Gain, p.X_exposure, p.Y_exposure, p.Z_exposure);
                        var list = global.gYCamTool.GetDeviceSingleFrameAutoExp(p.Pos9List);
                        foreach (var item in list)
                        {
                            keyValuePairs.Add("白场亮度", item.亮度);
                        }
                        // 3：切黑图
                        global.can1Service.Send(0x7FF, new byte[] { 3, 2, 0, 0, 0, 0, 0, 0 }, true);
                        Thread.Sleep(100);
                        // 4：测试黑图亮色度
                        var list1 = global.gYCamTool.GetDeviceSingleFrameAutoExp(p.Pos9List);
                        foreach (var item in list1)
                        {
                            keyValuePairs.Add("黑场亮度", item.亮度);
                        }
                        // 5：计算对比度（白图亮度除以黑图亮度）
                        //var value = Convert.ToDouble(list[0].亮度) / Convert.ToDouble(list1[0].亮度);

                        double[] results = new double[1];
                        GYCam_Mini.LCRTest(new double[] { Convert.ToDouble(list[0].亮度) }, new double[] { Convert.ToDouble(list1[0].亮度) }, 1, results);

                        keyValuePairs.Add("结果", true);
                        keyValuePairs.Add("对比度值", results[0]);
                        keyValuePairs.Add("结果描述", "对比度测试合格");

                        if (p.JudgeValueList.Count <= 0) break;

                        double value = Convert.ToDouble(results[0]);
                        double value1 = Convert.ToDouble(p.JudgeValueList[0]);

                        // 判断要执行的操作
                        switch (p.JudgeType)
                        {
                            case "大于":
                                {
                                    if (value > (value1 - Convert.ToDouble(p.JudgeValueList[1])))
                                    {
                                        keyValuePairs.Add("对比度结果", true);
                                        keyValuePairs.Add("对比度结果描述", "对比度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("对比度结果", false);
                                        keyValuePairs.Add("对比度结果描述", "对比度测试不合格");
                                    }
                                }
                                break;
                            case "小于":
                                {
                                    if (value < (value1 + Convert.ToDouble(p.JudgeValueList[1])))
                                    {
                                        keyValuePairs.Add("对比度结果", true);
                                        keyValuePairs.Add("对比度结果描述", "对比度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("对比度结果", false);
                                        keyValuePairs.Add("对比度结果描述", "对比度测试不合格");
                                    }
                                }
                                break;
                            case "大于小于":
                                {
                                    if (value > (value1 - Convert.ToDouble(p.JudgeValueList[1])) && value < (value1 + Convert.ToDouble(p.JudgeValueList[1])))
                                    {
                                        keyValuePairs.Add("对比度结果", true);
                                        keyValuePairs.Add("对比度结果描述", "对比度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("对比度结果", false);
                                        keyValuePairs.Add("对比度结果描述", "对比度测试不合格");
                                    }
                                }
                                break;
                            case "空":
                                {
                                    keyValuePairs.Add("对比度结果", true);
                                    keyValuePairs.Add("对比度结果描述", "没选择判断类型！");
                                }
                                break;
                            default:
                                {
                                    p.RunResult = false;
                                    keyValuePairs.Add("对比度结果", p.RunResult);
                                    keyValuePairs.Add("对比度结果描述", "没有判断操作");
                                }
                                break;
                        }
                    }
                    break;
                case "亮度、色度":
                    {
                        global.gYCamTool.SetExpoGain(p.exposure, p.Gain, p.X_exposure, p.Y_exposure, p.Z_exposure);

                        var list = global.gYCamTool.GetDeviceSingleFrameAutoExp(p.Pos9List);

                        foreach (var item in list)
                        {
                            keyValuePairs.Add("亮度", item.亮度);
                            keyValuePairs.Add("X色坐标", item.X色坐标);
                            keyValuePairs.Add("Y色坐标", item.Y色坐标);
                            keyValuePairs.Add("图片", item.disBMP);
                        }

                        if (p.JudgeValueList.Count <= 0) break;

                        double value1 = Convert.ToDouble(p.JudgeValueList[0]);// 亮度标准值
                        double value2 = Convert.ToDouble(p.JudgeValueList[1]);// 亮度阈值
                        double value3 = Convert.ToDouble(p.JudgeValueList[2]);// 色坐标X标准值
                        double value4 = Convert.ToDouble(p.JudgeValueList[3]);// 色坐标X阈值
                        double value5 = Convert.ToDouble(p.JudgeValueList[4]);// 色坐标Y标准值
                        double value6 = Convert.ToDouble(p.JudgeValueList[5]);// 色坐标Y阈值

                        double value7 = Convert.ToDouble(list[0].亮度);
                        double value8 = Convert.ToDouble(list[0].X色坐标);
                        double value9 = Convert.ToDouble(list[0].Y色坐标);

                        // 亮度判断要执行的操作
                        switch (p.JudgeType)
                        {
                            case "大于":
                                {
                                    if (value7 > (value1 - value2))
                                    {
                                        keyValuePairs.Add("亮度结果", true);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("亮度结果", false);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试不合格");
                                    }
                                }
                                break;
                            case "小于":
                                {
                                    if (value7 > (value1 + value2))
                                    {
                                        keyValuePairs.Add("亮度结果", true);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("亮度结果", false);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试不合格");
                                    }
                                }
                                break;
                            case "大于小于":
                                {
                                    if (value7 > (value1 - value2) && value7 > (value1 + value2))
                                    {
                                        keyValuePairs.Add("亮度结果", true);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("亮度结果", false);
                                        keyValuePairs.Add("亮度结果描述", "亮度测试不合格");
                                    }
                                }
                                break;
                            case "空":
                                {
                                    keyValuePairs.Add("亮度结果", true);
                                    keyValuePairs.Add("亮度结果描述", "没选择判断类型！");
                                }
                                break;
                            default:
                                {
                                    keyValuePairs.Add("亮度结果", false);
                                    keyValuePairs.Add("亮度结果描述", "没有判断操作");
                                }
                                break;
                        }

                        // 色度判断要执行的操作
                        switch (p.JudgeType)
                        {
                            case "大于":
                                {
                                    if (value8 > (value3 - value4) &&
                                         value9 > (value5 - value6))
                                    {
                                        keyValuePairs.Add("色度结果", true);
                                        keyValuePairs.Add("色度结果描述", "色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("色度结果", false);
                                        keyValuePairs.Add("色度结果描述", "色度测试不合格");
                                    }
                                }
                                break;
                            case "小于":
                                {
                                    if (value8 < (value3 + value4) &&
                                       value9 < (value5 + value6))
                                    {
                                        keyValuePairs.Add("色度结果", true);
                                        keyValuePairs.Add("色度结果描述", "色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("色度结果", false);
                                        keyValuePairs.Add("色度结果描述", "色度测试不合格");
                                    }
                                }
                                break;
                            case "大于小于":
                                {
                                    if (value8 > (value3 - value4) &&
                                         value9 > (value5 - value6) &&
                                         value8 < (value3 + value4) &&
                                         value9 < (value5 + value6))
                                    {
                                        keyValuePairs.Add("色度结果", true);
                                        keyValuePairs.Add("色度结果描述", "色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("色度结果", false);
                                        keyValuePairs.Add("色度结果描述", "色度测试不合格");
                                    }
                                }
                                break;
                            case "空":
                                {
                                    keyValuePairs.Add("色度结果", true);
                                    keyValuePairs.Add("色度结果描述", "没选择判断类型！");
                                }
                                break;
                            default:
                                {
                                    keyValuePairs.Add("色度结果", false);
                                    keyValuePairs.Add("色度结果描述", "没有判断操作");
                                }
                                break;
                        }

                    }
                    break;
                case "亮度、色度、对比度":
                    {

                        if (p.JudgeValueList.Count <= 0) break;

                        double value1 = Convert.ToDouble(p.JudgeValueList[0]);// 黑场亮度标准值
                        double value2 = Convert.ToDouble(p.JudgeValueList[1]);// 黑场亮度阈值
                        double value3 = Convert.ToDouble(p.JudgeValueList[2]);//  黑场色坐标X标准值
                        double value4 = Convert.ToDouble(p.JudgeValueList[3]);//  黑场色坐标X阈值
                        double value5 = Convert.ToDouble(p.JudgeValueList[4]);//  黑场色坐标Y标准值
                        double value6 = Convert.ToDouble(p.JudgeValueList[5]);//  黑场色坐标Y阈值

                        double value7 = Convert.ToDouble(p.JudgeValueList[6]);// 白场亮度标准值
                        double value8 = Convert.ToDouble(p.JudgeValueList[7]);// 白场亮度阈值
                        double value9 = Convert.ToDouble(p.JudgeValueList[8]);//  白场色坐标X标准值
                        double value10 = Convert.ToDouble(p.JudgeValueList[9]);//  白场色坐标X阈值
                        double value11 = Convert.ToDouble(p.JudgeValueList[10]);//  白场色坐标Y标准值
                        double value12 = Convert.ToDouble(p.JudgeValueList[11]);//  白场色坐标Y阈值

                        double value13 = Convert.ToDouble(p.JudgeValueList[12]);//  对比度标准值
                        double value14 = Convert.ToDouble(p.JudgeValueList[13]);//  对比度阈值

                        // 1:切白图
                        global.can1Service.Send(0x7FF, new byte[] { 3, 3, 0, 0, 0, 0, 0, 0 }, true);
                        Thread.Sleep(100);
                        // 2:测试白图亮色度
                        global.gYCamTool.SetExpoGain(p.exposure, p.Gain, p.X_exposure, p.Y_exposure, p.Z_exposure);
                        var list = global.gYCamTool.GetDeviceSingleFrameAutoExp(p.Pos9List);
                        foreach (var item in list)
                        {
                            keyValuePairs.Add("白场亮度", item.亮度);
                            keyValuePairs.Add("白场X色坐标", item.X色坐标);
                            keyValuePairs.Add("白场Y色坐标", item.Y色坐标);
                        }

                        double value15 = Convert.ToDouble(list[0].亮度);
                        double value16 = Convert.ToDouble(list[0].X色坐标);
                        double value17 = Convert.ToDouble(list[0].Y色坐标);

                        // 亮度判断要执行的操作
                        switch (p.JudgeType)
                        {
                            case "大于":
                                {
                                    if (value15 > (value7 - value8))
                                    {
                                        keyValuePairs.Add("白场亮度结果", true);
                                        keyValuePairs.Add("白场亮度描述", "白场亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("白场亮度结果", false);
                                        keyValuePairs.Add("白场亮度描述", "白场亮度测试不合格");
                                    }
                                }
                                break;
                            case "小于":
                                {
                                    if (value15 > (value7 + value8))
                                    {
                                        keyValuePairs.Add("白场亮度结果", true);
                                        keyValuePairs.Add("白场亮度描述", "白场亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("白场亮度结果", false);
                                        keyValuePairs.Add("白场亮度描述", "白场亮度测试不合格");
                                    }
                                }
                                break;
                            case "大于小于":
                                {
                                    if (value15 > (value7 - value8) && value15 < (value7 + value8))
                                    {
                                        keyValuePairs.Add("白场亮度结果", true);
                                        keyValuePairs.Add("白场亮度描述", "白场亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("白场亮度结果", false);
                                        keyValuePairs.Add("白场亮度描述", "白场亮度测试不合格");
                                    }
                                }
                                break;
                            case "空":
                                {
                                    keyValuePairs.Add("白场亮度结果", true);
                                    keyValuePairs.Add("白场亮度描述", "没选择判断类型！");
                                }
                                break;
                            default:
                                {
                                    keyValuePairs.Add("白场亮度结果", false);
                                    keyValuePairs.Add("白场亮度描述", "没有判断操作");
                                }
                                break;
                        }

                        // 色度判断要执行的操作
                        switch (p.JudgeType)
                        {
                            case "大于":
                                {
                                    if (value16 > (value9 - value10) &&
                                         value17 > (value11 - value12))
                                    {
                                        keyValuePairs.Add("白场色度结果", true);
                                        keyValuePairs.Add("白场色度描述", "白场色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("白场色度结果", false);
                                        keyValuePairs.Add("白场色度描述", "白场色度测试不合格");
                                    }
                                }
                                break;
                            case "小于":
                                {
                                    if (value16 < (value9 + value10) &&
                                       value17 < (value11 + value12))
                                    {
                                        keyValuePairs.Add("白场色度结果", true);
                                        keyValuePairs.Add("白场色度描述", "白场色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("白场色度结果", false);
                                        keyValuePairs.Add("白场色度描述", "白场色度测试不合格");
                                    }
                                }
                                break;
                            case "大于小于":
                                {
                                    if (value16 > (value9 - value10) &&
                                         value17 > (value11 - value12) &&
                                         value16 < (value9 + value10) &&
                                         value17 < (value11 + value12))
                                    {
                                        keyValuePairs.Add("白场色度结果", true);
                                        keyValuePairs.Add("白场色度描述", "白场色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("白场色度结果", false);
                                        keyValuePairs.Add("白场色度描述", "白场色度测试不合格");
                                    }
                                }
                                break;
                            case "空":
                                {
                                    keyValuePairs.Add("白场色度结果", true);
                                    keyValuePairs.Add("白场色度描述", "没选择判断类型！");
                                }
                                break;
                            default:
                                {
                                    keyValuePairs.Add("白场色度结果", false);
                                    keyValuePairs.Add("白场色度描述", "没有判断操作");
                                }
                                break;
                        }


                        // 3：切黑场
                        global.can1Service.Send(0x7FF, new byte[] { 3, 2, 0, 0, 0, 0, 0, 0 }, true);
                        Thread.Sleep(100);
                        // 4：测试黑图亮色度
                        var list1 = global.gYCamTool.GetDeviceSingleFrameAutoExp(p.Pos9List);
                        foreach (var item in list1)
                        {
                            keyValuePairs.Add("黑场亮度", item.亮度);
                            keyValuePairs.Add("黑场X色坐标", item.X色坐标);
                            keyValuePairs.Add("黑场Y色坐标", item.Y色坐标);   
                            keyValuePairs.Add("图片", item.disBMP);
                        }

                        double value18 = Convert.ToDouble(list1[0].亮度);
                        double value19 = Convert.ToDouble(list1[0].X色坐标);
                        double value20 = Convert.ToDouble(list1[0].Y色坐标);

                        // 亮度判断要执行的操作
                        switch (p.JudgeType)
                        {
                            case "大于":
                                {
                                    if (value18 > (value1 - value2))
                                    {
                                        keyValuePairs.Add("黑场亮度结果", true);
                                        keyValuePairs.Add("黑场亮度描述", "黑场亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("黑场亮度结果", false);
                                        keyValuePairs.Add("黑场亮度描述", "黑场亮度测试不合格");
                                    }
                                }
                                break;
                            case "小于":
                                {
                                    if (value18 > (value1 + value2))
                                    {
                                        keyValuePairs.Add("黑场亮度结果", true);
                                        keyValuePairs.Add("黑场亮度描述", "黑场亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("黑场亮度结果", false);
                                        keyValuePairs.Add("黑场亮度描述", "黑场亮度测试不合格");
                                    }
                                }
                                break;
                            case "大于小于":
                                {
                                    if (value18 > (value1 - value2) && value18 < (value1 + value2))
                                    {
                                        keyValuePairs.Add("黑场亮度结果", true);
                                        keyValuePairs.Add("黑场亮度描述", "黑场亮度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("黑场亮度结果", false);
                                        keyValuePairs.Add("黑场亮度描述", "黑场亮度测试不合格");
                                    }
                                }
                                break;
                            case "空":
                                {
                                    keyValuePairs.Add("黑场亮度结果", true);
                                    keyValuePairs.Add("黑场亮度描述", "没选择判断类型！");
                                }
                                break;
                            default:
                                {
                                    keyValuePairs.Add("黑场亮度结果", false);
                                    keyValuePairs.Add("黑场亮度描述", "没有判断操作");
                                }
                                break;
                        }

                        // 色度判断要执行的操作
                        switch (p.JudgeType)
                        {
                            case "大于":
                                {
                                    if (value19 > (value3 - value4) &&
                                         value20 > (value5 - value6))
                                    {
                                        keyValuePairs.Add("黑场色度结果", true);
                                        keyValuePairs.Add("黑场色度描述", "黑场色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("黑场色度结果", false);
                                        keyValuePairs.Add("黑场色度描述", "黑场色度测试不合格");
                                    }
                                }
                                break;
                            case "小于":
                                {
                                    if (value19 < (value3 + value4) &&
                                       value20 < (value5 + value6))
                                    {
                                        keyValuePairs.Add("黑场色度结果", true);
                                        keyValuePairs.Add("黑场色度描述", "黑场色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("黑场色度结果", false);
                                        keyValuePairs.Add("黑场色度描述", "黑场色度测试不合格");
                                    }
                                }
                                break;
                            case "大于小于":
                                {
                                    if (value19 > (value3 - value4) &&
                                         value20 > (value5 - value6) &&
                                         value19 < (value3 + value4) &&
                                         value20 < (value5 + value6))
                                    {
                                        keyValuePairs.Add("黑场色度结果", true);
                                        keyValuePairs.Add("黑场色度描述", "黑场色度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("黑场色度结果", false);
                                        keyValuePairs.Add("黑场色度描述", "黑场色度测试不合格");
                                    }
                                }
                                break;
                            case "空":
                                {
                                    keyValuePairs.Add("黑场色度结果", true);
                                    keyValuePairs.Add("黑场色度描述", "没选择判断类型！");
                                }
                                break;
                            default:
                                {
                                    keyValuePairs.Add("黑场色度结果", false);
                                    keyValuePairs.Add("黑场色度描述", "没有判断操作");
                                }
                                break;
                        }


                        // 5：计算对比度（白图亮度除以黑图亮度）
                        //var value = Convert.ToDouble(list[0].亮度) / Convert.ToDouble(list1[0].亮度);

                        double[] results = new double[1];
                        GYCam_Mini.LCRTest(new double[] { Convert.ToDouble(list[0].亮度) }, new double[] { Convert.ToDouble(list1[0].亮度) }, 1, results);

                        double value21 = Convert.ToDouble(results[0]);

                        keyValuePairs.Add("对比度值", results[0]);
                        // 对比度判断要执行的操作
                        switch (p.JudgeType)
                        {
                            case "大于":
                                {
                                    if (value21 > (value13 - value14))
                                    {
                                        keyValuePairs.Add("对比度结果", true);
                                        keyValuePairs.Add("对比度描述", "对比度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("对比度结果", false);
                                        keyValuePairs.Add("对比度描述", "对比度测试不合格");
                                    }
                                }
                                break;
                            case "小于":
                                {
                                    if (value21 > (value13 + value14))
                                    {
                                        keyValuePairs.Add("对比度结果", true);
                                        keyValuePairs.Add("对比度描述", "对比度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("对比度结果", false);
                                        keyValuePairs.Add("对比度描述", "对比度测试不合格");
                                    }
                                }
                                break;
                            case "大于小于":
                                {
                                    if (value21 > (value13 - value14) && value21 < (value13 + value14))
                                    {
                                        keyValuePairs.Add("对比度结果", true);
                                        keyValuePairs.Add("对比度描述", "对比度测试合格");
                                    }
                                    else
                                    {
                                        keyValuePairs.Add("对比度结果", false);
                                        keyValuePairs.Add("对比度描述", "对比度测试不合格");
                                    }
                                }
                                break;
                            case "空":
                                {
                                    keyValuePairs.Add("对比度结果", true);
                                    keyValuePairs.Add("对比度描述", "没选择判断类型！");
                                }
                                break;
                            default:
                                {
                                    keyValuePairs.Add("对比度结果", false);
                                    keyValuePairs.Add("对比度描述", "没有判断操作");
                                }
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }

            return keyValuePairs;
        }
    }
}
