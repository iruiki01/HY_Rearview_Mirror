using HalconDotNet;
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
using static HY_Rearview_Mirror.User.TemplateMatchingSetForm;

namespace HY_Rearview_Mirror.User
{
    public class TemplateMatchingParam : ITestParam
    {
        /// <summary>
        /// ROI图像
        /// </summary>
        public HObject regionImage { get; set; }
        /// <summary>
        /// 运行结果
        /// </summary>
        public bool RunResult { get; set; }
        /// <summary>
        /// 模板匹配方法
        /// </summary>
        public HalconShapeMatcher halconShapeMatcher { get; set; }
        /// <summary>
        /// 相机编号
        /// </summary>
        public string CamId {  get; set; }
        public TemplateMatchingParam()
        {
            RunResult = false;
            CamId = "相机1";
        }     
    }
    /// <summary>
    /// 模板匹配
    /// </summary>
    public class TemplateMatchingFun : IPlugin
    {
        public string PluginId => "模板匹配";

        public string moduleName => "视觉";

        public string DescribeMessage => "用于模板匹配";

        public string VersionStr1 = "";
        public string VersionStr => VersionStr1;
        [JsonIgnore]
        public TemplateMatchingSetForm templateMatchingSetForm = null;
        public TemplateMatchingParam templateMatchingParam = null;
        private bool WaitNewCamMatFlag = false;

        [JsonIgnore]
        /// <summary>
        /// 当前操作图像
        /// </summary>
        private HImage g_hImage;

        private ManualResetEventSlim _flagEvent;
        public TemplateMatchingFun()
        {
            templateMatchingParam = new TemplateMatchingParam();
            templateMatchingParam.halconShapeMatcher = new HalconShapeMatcher();
            MainForm.SendHImagetHandler += GetImage0;
            _flagEvent = new ManualResetEventSlim(false);          
        }
        public Form CreateUI(ITestParam testParam, string VersionStr = "")
        {
            templateMatchingSetForm = new TemplateMatchingSetForm();

            templateMatchingSetForm.eventManager.AddListener(StatusCode.制作模板.ToString(), new Action<HImage, HRegion, double, double>(CreateModel));
            templateMatchingSetForm.eventManager.AddListener(StatusCode.发送界面参数.ToString(), new Action<List<object>>(SetDataForm_OrderCompleted));
            templateMatchingSetForm.eventManager.AddListener(StatusCode.关闭窗体.ToString(), new Action(FormClosing));
            templateMatchingSetForm.eventManager.AddListener(StatusCode.运行.ToString(), new Func<Dictionary<string, object>>(Run));

            templateMatchingSetForm.SetDataToForm(testParam, VersionStr);

            if (testParam != null)
                templateMatchingParam = testParam as TemplateMatchingParam;
            return templateMatchingSetForm;
        }

        public void FormClosing()
        {
            templateMatchingSetForm.eventManager.Clear();
        }
        public void Dispose()
        {
            MainForm.SendHImagetHandler -= GetImage0;
            GC.SuppressFinalize(this);
        }

        public ITestParam GetData()
        {
            return templateMatchingParam;
        }

        public string GetVersion()
        {
            return VersionStr;
        }

        public void Initialize()
        {

        }

        private void GetImage0(object sender, HImage img)
        {
            if (WaitNewCamMatFlag)
            {
                WaitNewCamMatFlag = false;
                g_hImage = img.Clone();
                img?.Dispose();

                _flagEvent.Set();
            }
        }

        public Dictionary<string, object> Run(ITestParam testParam)
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();

            // 重置事件
            _flagEvent.Reset();

            WaitNewCamMatFlag = true;
            global.cameraController.CamDeviceTriggerOnce(templateMatchingParam.CamId);

            // 等待信号（带超时）
            bool signaled = _flagEvent.Wait(5000); // 等待5秒
            if(signaled)
            {
                templateMatchingParam.halconShapeMatcher.inputImage = g_hImage;
                templateMatchingParam.halconShapeMatcher.FindShape();

                if(templateMatchingParam.halconShapeMatcher.matchResult1.Result)
                {
                    keyValuePairs.Add("描述", "匹配成功！");
                    keyValuePairs.Add("结果", true);
                }
                else
                {
                    keyValuePairs.Add("描述", "匹配失败！");
                    keyValuePairs.Add("结果", false);
                }
                //templateMatchingSetForm.g_hWindow1.BindingContourDatas(templateMatchingParam.halconShapeMatcher.ContourDatas);
            }
            else
            {
                keyValuePairs.Add("描述", "采图超时！");
                keyValuePairs.Add("结果", false);
            }
        
            return keyValuePairs;
        }
        public Dictionary<string, object> Run()
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();

            // 重置事件
            _flagEvent.Reset();

            WaitNewCamMatFlag = true;
            global.cameraController.CamDeviceTriggerOnce(templateMatchingParam.CamId);

            // 等待信号（带超时）
            bool signaled = _flagEvent.Wait(5000); // 等待5秒
            if (signaled)
            {
                templateMatchingParam.halconShapeMatcher.inputImage = g_hImage;
                templateMatchingParam.halconShapeMatcher.FindShape();

                if (templateMatchingParam.halconShapeMatcher.matchResult1.Result)
                {
                    keyValuePairs.Add("描述", "匹配成功！");
                    keyValuePairs.Add("结果", true);
                }
                else
                {
                    keyValuePairs.Add("描述", "匹配失败！");
                    keyValuePairs.Add("结果", false);
                }
                templateMatchingSetForm.g_hWindow1.BindingContourDatas(templateMatchingParam.halconShapeMatcher.ContourDatas);
            }
            else
            {
                keyValuePairs.Add("描述", "采图超时！");
                keyValuePairs.Add("结果", false);
            }

            return keyValuePairs;
        }
        /// <summary>
        /// 制作模板
        /// </summary>
        /// <param name="hRegion"></param>
        /// <param name="scaleMin"></param>
        /// <param name="scaleMax"></param>
        private void CreateModel(HImage _hImage,HRegion hRegion, double scaleMin = 0.8, double scaleMax = 1.2)
        {
            templateMatchingParam.halconShapeMatcher.inputImage = _hImage;
            templateMatchingParam.halconShapeMatcher.CreateModel(hRegion, scaleMin, scaleMax);
            templateMatchingSetForm.g_hWindow1.BindingContourDatas(templateMatchingParam.halconShapeMatcher.RoiContourDatas);
        }
        /// <summary>
        /// 模板匹配
        /// </summary>
        private void FindShape()
        {
            templateMatchingParam.halconShapeMatcher.inputImage = g_hImage;
            templateMatchingParam.halconShapeMatcher.FindShape();
            templateMatchingSetForm.g_hWindow1.BindingContourDatas(templateMatchingParam.halconShapeMatcher.ContourDatas);
        }

        private void SetDataForm_OrderCompleted(List<object> e)
        {
            VersionStr1 = e[0].ToString();
            templateMatchingParam.regionImage = e[1] as HObject;
            templateMatchingParam.CamId = e[2].ToString();
        }
    }
}
