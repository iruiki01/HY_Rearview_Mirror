using HalconDotNet;
using Helper;
using hWindowTool;
using HY_Rearview_Mirror.InterfaceTool;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HY_Rearview_Mirror.User
{
    public partial class TemplateMatchingSetForm : UIForm
    {
        public enum StatusCode
        {
            关闭窗体,
            制作模板,
            运行,
            发送界面参数
        }

        /// <summary>
        /// 当前操作图像
        /// </summary>
        public HImage g_hImage;
        /// <summary>
        /// 鼠标位置绘制ROI类型
        /// </summary>
        private ROITypeEnum rOIType;
        /// <summary>
        /// 鼠标绘制状态
        /// </summary>
        private bool draw;
        /// <summary>
        /// ROI数据字典
        /// </summary>
        public static Dictionary<string, ROIData> rOIDatas = new Dictionary<string, ROIData>();
        private bool WaitNewCamMatFlag = false;
        public hWindowTool.HWindow g_hWindow1;
        public HObject g_ho_ImagePart = null;

        /// <summary>
        /// 声明事件
        /// </summary>
        public EventManager eventManager = new EventManager();
        //public event Action<HImage, HRegion, double, double> CreateModelHandler;
        //public event Action FindShapeHandler;
        //public event Action<List<object>> OrderCompleted;     
        //public event Action FormClosingHandler;
        //public event Func<Dictionary<string, object>> RunHandler;

        public TemplateMatchingSetForm()
        {
            InitializeComponent();

            g_hWindow1 = hWindow1;

            rOIDatas.Clear();
            //绑定ROI数据
            hWindow1.BindingROIDatas(rOIDatas);
            //订阅ROI激活事件
            hWindow1.ROIActivateDone += HWindow1_ROIActivateDone;
            //订阅ROI移动完成事件
            hWindow1.ROIMoveDone += HWindow1_ROIMoveDone;
            //订阅ROI正在移动事件
            hWindow1.ROIMoving += HWindow1_ROIMoving;
            //订阅halcon控件鼠标按下事件
            hWindow1.hWindowControl.HMouseDown += HWindowControl_HMouseDown;

            MainForm.SendHImagetHandler += GetImage0;
        }
        #region 事件回调
        /// <summary>
        /// halcon控件鼠标按下时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HWindowControl_HMouseDown(object sender, HalconDotNet.HMouseEventArgs e)
        {
            //需要鼠标绘制
            if (draw)
            {
                //光标鼠标绘制
                draw = false;
                //实例化对应ROI数据
                ROIData rOIData = new ROIData(rOIType);
                //添加ROI
                string rOIName = hWindow1.AddROI(null, rOIData);
                //判断鼠标绘制类型
                switch (rOIType)
                {
                    case ROITypeEnum.点:
                        rOIDatas[rOIName].ROI.IROIAction.SetROIData(new HTuple(e.Y, e.X));
                        break;
                    case ROITypeEnum.一维直线:
                        rOIDatas[rOIName].ROI.IROIAction.SetROIData(new HTuple(e.Y, e.X - 50, e.Y, e.X + 50));
                        break;
                    case ROITypeEnum.二维直线:
                        rOIDatas[rOIName].ROI.IROIAction.SetROIData(new HTuple(e.Y, e.X - 50, e.Y, e.X + 50, 10));
                        break;
                    case ROITypeEnum.矩形1:
                        rOIDatas[rOIName].ROI.IROIAction.SetROIData(new HTuple(e.Y - 50, e.X - 50, e.Y + 50, e.X + 50));
                        break;
                    case ROITypeEnum.矩形2:
                    case ROITypeEnum.椭圆:
                        rOIDatas[rOIName].ROI.IROIAction.SetROIData(new HTuple(e.Y, e.X, 0, 50, 50));
                        break;
                    case ROITypeEnum.圆:
                        rOIDatas[rOIName].ROI.IROIAction.SetROIData(new HTuple(e.Y, e.X, 50));
                        break;
                    case ROITypeEnum.圆弧:
                        rOIDatas[rOIName].ROI.IROIAction.SetROIData(new HTuple(e.Y, e.X, 50, 0, 3.14159));
                        break;
                    case ROITypeEnum.圆环:
                        rOIDatas[rOIName].ROI.IROIAction.SetROIData(new HTuple(e.Y, e.X, 25, 50));
                        break;
                    case ROITypeEnum.扇形环:
                        rOIDatas[rOIName].ROI.IROIAction.SetROIData(new HTuple(e.Y, e.X, 25, 50, 0, 3.14159));
                        break;
                }
                //刷新ROI列表
                //UpdateROIDataList(dgv_ROIDataList, rOIName);
            }
        }

        /// <summary>
        /// ROI正在移动时发生
        /// </summary>
        /// <param name="rOIName">ROI名称</param>
        /// <param name="rOIData">ROI数据</param>
        /// <param name="hRegion">ROI区域对象</param>
        private void HWindow1_ROIMoving(string rOIName, HalconDotNet.HTuple rOIData, HRegion hRegion)
        {
            //刷新ROI列表
            //UpdateROIDataList(dgv_ROIDataList, rOIName);


            //hWindow1.ClearUserDisplay();
            //hWindow1.DisplayObj(new HObjData(hRegion, HColorEnum.red, false));
        }

        /// <summary>
        /// ROI移动完成时发生
        /// </summary>
        /// <param name="rOIName">ROI名称</param>
        /// <param name="rOIData">ROI数据</param>
        /// <param name="hRegion">ROI区域对象</param>
        private void HWindow1_ROIMoveDone(string rOIName, HalconDotNet.HTuple rOIData, HRegion hRegion)
        {

        }

        /// <summary>
        /// ROI激活时发生
        /// </summary>
        /// <param name="rOIName">ROI名称</param>
        /// <param name="rOIData">ROI数据</param>
        /// <param name="hRegion">ROI区域对象</param>
        private void HWindow1_ROIActivateDone(string rOIName, HalconDotNet.HTuple rOIData, HRegion hRegion)
        {
            //刷新ROI列表
            //UpdateROIDataList(dgv_ROIDataList, rOIName);
        }
        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            eventManager.Trigger(StatusCode.关闭窗体.ToString());
            this.Dispose();
        }

        private void GetImage0(object sender, HImage img)
        {
            if (WaitNewCamMatFlag)
            {
                WaitNewCamMatFlag = false;
                g_hImage = img.Clone();
                hWindow1.HobjectToHimage(img);  // 显示
                img?.Dispose();
            }
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            rOIDatas.Clear();
            //绑定ROI数据
            hWindow1.BindingROIDatas(rOIDatas);

            //将字符串转换成等效枚举 --> 成功
            if (Enum.TryParse<ROITypeEnum>(uiButton1.Text, out ROITypeEnum enumValue))
            {
                //更新鼠标位置绘制ROI类型
                rOIType = enumValue;
                //打开鼠标绘制状态
                draw = true;
            }
        }

        private void uiButton5_Click(object sender, EventArgs e)
        {
            string camName = uiComboBox1.Text;
            WaitNewCamMatFlag = true;
            global.cameraController.CamDeviceTriggerOnce(camName);
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            rOIDatas.Clear();
            //绑定ROI数据
            hWindow1.BindingROIDatas(rOIDatas);
        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            if (rOIDatas.Count <= 0)
            {
                UIMessageBox.ShowError("请先绘制ROI区域！");
                return;
            }
            ////获取ROI数据
            //HTuple hTuple = rOIDatas["ROI0"].ROI.IROIAction.GetROIData();
            //List<double> rOIData = new List<double>();
            //foreach (var item1 in hTuple.DArr)
            //{
            //    rOIData.Add(Convert.ToDouble(item1.ToString("f3")));
            //}
            //string result = string.Join(",", rOIData);
            //uiTextBox1.Text = result;

            HObject ho_Mask = null;

            // 初始化Halcon对象
            HOperatorSet.GenEmptyObj(out ho_Mask);
            HOperatorSet.GenEmptyObj(out g_ho_ImagePart);
            using (HObject ho_Image = g_hImage.Clone())
            {
                // 从原图裁剪指定区域
                HOperatorSet.ReduceDomain(ho_Image, rOIDatas["ROI0"].hRegion, out ho_Mask);
            }
            HOperatorSet.CropDomain(ho_Mask, out g_ho_ImagePart);
            hWindow2.HobjectToHimage(g_ho_ImagePart.Clone());  // 显示
        }
        /// <summary>
        /// 制作模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton4_Click(object sender, EventArgs e)
        {
            if (rOIDatas.Count <= 0)
            {
                UIMessageBox.ShowError("请先绘制ROI区域！");
                return;
            }
            using (HImage ho_Image = g_hImage.Clone())
            {
                eventManager.Trigger(StatusCode.制作模板.ToString(), ho_Image, rOIDatas["ROI0"].hRegion, 0.8, 1.2);
            }          
        }
        /// <summary>
        /// 模板匹配
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton6_Click(object sender, EventArgs e)
        {
            var result = eventManager.TriggerWithResult(StatusCode.运行.ToString());

            if (result is Dictionary<string, object> dict)  // 类型转换
            {
                List<string> list = new List<string>();
                foreach (var kv in dict)
                {
                    list.Add(kv.Key);
                    list.Add(Convert.ToString(kv.Value));
                }
                uiListBox1.DataSource = list;
            }
            else if (result != null)
            {
                MessageBox.Show($"返回值不是字典类型，实际类型: {result.GetType()}");
            }

        }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton7_Click(object sender, EventArgs e)
        {
            //if (g_ho_ImagePart == null || !g_ho_ImagePart.IsInitialized())
            //{
            //    UIMessageBox.ShowError("请先绘制ROI！");
            //    return;
            //}
            eventManager.Trigger(StatusCode.发送界面参数.ToString(), new List<object>(){ uiTextBox3.Text, g_ho_ImagePart, uiComboBox1.Text});

            UIMessageBox.ShowSuccess("参数设置成功！");
        }

        public void SetDataToForm(ITestParam testParam, string VersionStr = "")
        {
            if (testParam == null) return;
            TemplateMatchingParam pressureTestParam = testParam as TemplateMatchingParam;
            uiTextBox3.Text = VersionStr;

            if(pressureTestParam.regionImage != null)
            {
                eventManager.Trigger(StatusCode.发送界面参数.ToString(), new List<object>() { uiTextBox3.Text, pressureTestParam.regionImage, uiComboBox1.Text });

                hWindow2.HobjectToHimage(pressureTestParam.regionImage.Clone());  // 显示
            }

            uiComboBox1.Text = pressureTestParam.CamId;
        }
    }
}
