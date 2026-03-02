using HalconDotNet;
using Helper;
using hWindowTool;
using HY_Rearview_Mirror.InterfaceTool;
using Ivi.Visa.Interop;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HY_Rearview_Mirror.Functions.CameraController;

namespace HY_Rearview_Mirror.User
{
    public partial class CheckLightSetForm : UIForm
    {
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

        /// <summary>
        /// 轮廓数据
        /// </summary>
        public Dictionary<string, HObject> ContourDatas = new Dictionary<string, HObject>();

        // 声明带参数的事件
        public EventManager eventManager = new EventManager();
        ///// <summary>
        ///// 设置参数事件
        ///// </summary>
        //public event EventHandler<List<object>> OrderCompleted;
        ///// <summary>
        ///// 运行斑点分析事件
        ///// </summary>
        //public event Func<HImage, int, int, double, List<BlobResult>> AnalyzeBlobsHandler;
        ///// <summary>
        ///// 拉条事件
        ///// </summary>
        //public event Action<HImage, int, int> ScrollBarMinValueHandler;
        ///// <summary>
        ///// 测试运行事件
        ///// </summary>
        //public event Func<Dictionary<string, object>> RunHandler;
        ///// <summary>
        ///// 关闭窗体事件
        ///// </summary>
        //public event Action FormClosingHandler;
        int MinValue = 0;
        int MaxValue = 255;

       

        public CheckLightSetForm()
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
            eventManager.Trigger("关闭窗体");
            ContourDatas.Clear();
            MainForm.SendHImagetHandler -= GetImage0;

            if (g_hImage != null)
            {
                g_hImage.Dispose();  // 释放图片资源
                g_hImage = null;     // 清空引用
            }
            this.Dispose();
        }

        private void CheckLightSetForm_Load(object sender, EventArgs e)
        {
            //string path = AppDomain.CurrentDomain.BaseDirectory + "Resource\\Image\\O.bmp";
            // 读取图像
            //hImage = new HImage(path);
            hWindow1.ClearWindow();
            //显示到控件
            //hWindow1.HobjectToHimage(hImage.Clone());
            //Application.DoEvents(); // 处理所有消息队列
            //this.Refresh();        
        }
        public void Bitmap2HObjectBpp24(Bitmap bmp, out HObject image)
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData srcBmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                HOperatorSet.GenImageInterleaved(out image, srcBmpData.Scan0, "bgr", bmp.Width, bmp.Height, 0, "byte", 0, 0, 0, 0, -1, 0);
                bmp.UnlockBits(srcBmpData);
            }
            catch (Exception) { image = null; }
        }
        private void GetImage0(object sender, HImage img)
        {
            if(WaitNewCamMatFlag)
            {
                WaitNewCamMatFlag = false;
                g_hImage = img.Clone();
                hWindow1.HobjectToHimage(img);  // 显示
                img?.Dispose();
            }           
        }
        /// <summary>
        /// 设置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton4_Click(object sender, EventArgs e)
        {
            string str0 = uiTextBox3.Text;
            int value0 = uiHorScrollBar1.Value;
            int value1 = uiHorScrollBar2.Value;
            string str1 = uiTextBox1.Text;
            string str2 = uiTextBox2.Text;
            string str3 = uiComboBox1.Text;
            string str4 = uiComboBox2.Text;

            if(string.IsNullOrEmpty(str0) || string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2) || string.IsNullOrEmpty(str3))
            {
                UIMessageBox.ShowError("设置失败，请填写完整的数据！");
                return;
            }

            eventManager.Trigger("发送界面参数", new List<object>() { str0, value0, value1, str1, str2, str3, str4});

            UIMessageBox.ShowSuccess("参数已设置成功！");
        }
        public void SetDataToForm(ITestParam testParam, string VersionStr = "")
        {
            CheckLightTestParam pressureTestParam = testParam as CheckLightTestParam;
            if(pressureTestParam != null)
            {
                uiTextBox3.Text = VersionStr;
                uiHorScrollBar1.Value = pressureTestParam.minThreshold;
                uiHorScrollBar2.Value = pressureTestParam.maxThreshold;
                uiTextBox1.Text = pressureTestParam.minArea.ToString();
                uiTextBox2.Text = pressureTestParam.deviation.ToString();

                uiLabel5.Text = pressureTestParam.minThreshold.ToString();
                uiLabel6.Text = pressureTestParam.maxThreshold.ToString();

                if(!string.IsNullOrEmpty(pressureTestParam.CamId))
                {
                    uiComboBox1.Text = pressureTestParam.CamId.ToString();
                }
                uiComboBox2.Text = pressureTestParam.IsLightOutPut;
            }
            //HTuple row1 = null, col1 = null, row2 = null, col2 = null;
            //string serialized = "";
            //if (pressureTestParam.region != null)
            //{
            //    // 轴对齐矩形（最快）           
            //    HOperatorSet.SmallestRectangle1(pressureTestParam.region, out row1, out col1, out row2, out col2);
            //    // 返回：左上角(row1, col1) 和 右下角(row2, col2)
            //    serialized = row1 + " " + col1 + " " + row2 + " " + col2;
            //}        
        }
        /// <summary>
        /// 取图按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton5_Click(object sender, EventArgs e)
        {
            ContourDatas.Clear();
            string camName = uiComboBox1.Text;
            WaitNewCamMatFlag = true;
            global.cameraController.CamDeviceTriggerOnce(camName);
        }

        /// <summary>
        /// Blob分析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click_1(object sender, EventArgs e)
        {
            if (g_hImage == null || !g_hImage.IsInitialized()) return;
            MinValue = uiHorScrollBar1.Value;
            MaxValue = uiHorScrollBar2.Value;

            var results =eventManager.TriggerWithResult("Blob分析", g_hImage.Clone(), MinValue, MaxValue, 100);

            List<string> data = new List<string>();
            int idex = 0;

            if (results is List<BlobResult> dict)  // 类型转换
            {
                foreach (BlobResult result in dict)
                {
                    idex++;
                    data.Add(idex + "：面积" + result.Area);
                }
                uiListBox1.DataSource = data;
            }
            else if (results != null)
            {
                MessageBox.Show($"返回值不是字典类型，实际类型: {results.GetType()}");
            }        
        }

        private void uiHorScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            if (g_hImage == null || !g_hImage.IsInitialized()) return;
            MinValue = uiHorScrollBar1.Value;
            uiLabel5.Text = MinValue.ToString();
            eventManager.Trigger("拉条", g_hImage.Clone(), MinValue, MaxValue);
        }

        private void uiHorScrollBar2_ValueChanged(object sender, EventArgs e)
        {
            if (g_hImage == null || !g_hImage.IsInitialized()) return;
            MaxValue = uiHorScrollBar2.Value;
            uiLabel6.Text = MaxValue.ToString();
            eventManager.Trigger("拉条", g_hImage.Clone(), MinValue, MaxValue);
        }
        /// <summary>
        /// 运行按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton2_Click_1(object sender, EventArgs e)
        {
            var result = eventManager.TriggerWithResult("运行");

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
    }
}
