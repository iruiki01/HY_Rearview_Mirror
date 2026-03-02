using HalconDotNet;
using Helper;
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
    public partial class CheckRGBForm :  UIForm
    {
        /// <summary>
        /// 等待图标志
        /// </summary>
        private bool WaitNewCamMatFlag = false;
        /// <summary>
        /// 当前操作图像
        /// </summary>
        public HImage g_hImage;
        /// <summary>
        /// 轮廓数据
        /// </summary>
        public Dictionary<string, HObject> ContourDatas = new Dictionary<string, HObject>();
        public hWindowTool.HWindow g_hWindow1;

        // 声明带参数的事件
        public EventManager eventManager = new EventManager();
        int MinValue = 0;
        int MaxValue = 255;

        public CheckRGBForm()
        {
            InitializeComponent();

            g_hWindow1 = hWindow1;
            MainForm.SendHImagetHandler += GetImage0;
        }

        private void CheckRGBForm_Load(object sender, EventArgs e)
        {

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
        /// <summary>
        /// 取图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton5_Click(object sender, EventArgs e)
        {
            string camName = uiComboBox1.Text;
            WaitNewCamMatFlag = true;
            global.cameraController.CamDeviceTriggerOnce(camName);
        }
        /// <summary>
        /// 斑点分析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
            if (g_hImage == null || !g_hImage.IsInitialized()) return;
            MinValue = uiHorScrollBar1.Value;
            MaxValue = uiHorScrollBar2.Value;

            var results = eventManager.TriggerWithResult("Blob分析", g_hImage.Clone(), MinValue, MaxValue, 100, false);

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
        /// <summary>
        /// 运行按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton2_Click(object sender, EventArgs e)
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
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton4_Click(object sender, EventArgs e)
        {
            string str0 = uiTextBox3.Text;                  // 描述
            int value0 = uiHorScrollBar1.Value;         // 拉条1
            int value1 = uiHorScrollBar2.Value;         // 拉条2
            string str1 = uiTextBox1.Text;                  // 最小面积
            string str3 = uiComboBox1.Text;             // 选择相机
            string str4 = uiComboBox3.Text;             // 选择相机

            if (string.IsNullOrEmpty(str0) || string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str3))
            {
                UIMessageBox.ShowError("设置失败，请填写完整的数据！");
                return;
            }

            eventManager.Trigger("发送界面参数", new List<object>() { str0, value0, value1, str1, str3, str4 });

            UIMessageBox.ShowSuccess("参数已设置成功！");
        }
        public void SetDataToForm(ITestParam testParam, string VersionStr = "")
        {
            CheckRGBTestParam muraFunTestParam = testParam as CheckRGBTestParam;
            if (muraFunTestParam != null)
            {
                uiTextBox3.Text = VersionStr;
                uiHorScrollBar1.Value = muraFunTestParam.minThreshold;
                uiHorScrollBar2.Value = muraFunTestParam.maxThreshold;
                uiTextBox1.Text = muraFunTestParam.minArea.ToString();

                uiLabel5.Text = muraFunTestParam.minThreshold.ToString();
                uiLabel6.Text = muraFunTestParam.maxThreshold.ToString();

                uiComboBox3.Text = muraFunTestParam.Color.ToString();
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
    }
}
