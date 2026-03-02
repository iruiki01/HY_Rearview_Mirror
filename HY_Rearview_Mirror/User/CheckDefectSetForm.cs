using HalconDotNet;
using Helper;
using HY_Rearview_Mirror.Controls;
using HY_Rearview_Mirror.InterfaceTool;
using MvCameraControl;
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
    public partial class CheckDefectSetForm : UIForm
    {
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

        DataGridComboBox dataGridComboBox_CheckDefect = new DataGridComboBox();
        public CheckDefectSetForm()
        {
            InitializeComponent();
            g_hWindow1 = hWindow1;
            MainForm.SendHImagetHandler += GetImage0;

            var products = new List<CheckDefecData>
            {
                new CheckDefecData { 阈值 = "",上限 = "",下限 = ""},
            };
            dataGridComboBox_CheckDefect.DataSource = products;  // 绑定List;
            dataGridComboBox_CheckDefect.Dock = DockStyle.Fill;
            panel1.Controls.Add(dataGridComboBox_CheckDefect);
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            eventManager.Trigger("关闭窗体");

            MainForm.SendHImagetHandler -= GetImage0;

            if (g_hImage != null)
            {
                g_hImage.Dispose();  // 释放图片资源
                g_hImage = null;     // 清空引用
            }

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

        private void uiButton4_Click(object sender, EventArgs e)
        {
            string str1 = uiTextBox3.Text;                    // 描述
            string str2 = uiHorScrollBar1.Value.ToString();            // 拉条1
            string str3 = uiHorScrollBar2.Value.ToString();            // 拉条2
            string str4 = uiTextBox1.Text;                   // 最小面积
            string str5 = uiComboBox1.Text;              // 相机编号

            List<CheckDefecData> list = dataGridComboBox_CheckDefect.GetList<CheckDefecData>();
            List<string> list2 = new List<string>();
            list2.Clear();
            foreach (var itm in list)
            {
                list2.Add(itm.阈值);
                list2.Add(itm.上限);
                list2.Add(itm.下限);
            }

            eventManager.Trigger("发送界面参数", new List<object>() { str1, str2, str3, str4, str5, list2 });

            UIMessageBox.ShowSuccess("参数设置成功！");
        }
        /// <summary>
        /// 把参数设置到界面上
        /// </summary>
        /// <param name="testParam"></param>
        /// <param name="VersionStr"></param>
        public void SetDataToForm(ITestParam testParam, string VersionStr = "")
        {
            CheckDefectFunTestParam pressureTestParam = testParam as CheckDefectFunTestParam;
            if (pressureTestParam != null)
            {
                uiTextBox3.Text = VersionStr;
                uiHorScrollBar1.Value = pressureTestParam.minThreshold;
                uiHorScrollBar2.Value = pressureTestParam.maxThreshold;
                uiTextBox1.Text = pressureTestParam.minArea.ToString();

                uiLabel5.Text = pressureTestParam.minThreshold.ToString();
                uiLabel6.Text = pressureTestParam.maxThreshold.ToString();

                uiComboBox1.Text = pressureTestParam.CamId.ToString();

                if(pressureTestParam.Data_List.Count > 0)
                {
                    var products = new List<CheckDefecData>
                {
                    new CheckDefecData {  阈值 = pressureTestParam.Data_List[0], 上限 = pressureTestParam.Data_List[1], 下限 = pressureTestParam.Data_List[2]}
                };
                    dataGridComboBox_CheckDefect.DataSource = products;
                }        
            }     
        }

        private void uiButton5_Click(object sender, EventArgs e)
        {
            string camName = uiComboBox1.Text;
            WaitNewCamMatFlag = true;
            global.cameraController.CamDeviceTriggerOnce(camName);
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

        private void uiButton1_Click(object sender, EventArgs e)
        {
            if (g_hImage == null || !g_hImage.IsInitialized()) return;
            MinValue = uiHorScrollBar1.Value;
            MaxValue = uiHorScrollBar2.Value;

            var results = eventManager.TriggerWithResult("Blob分析", g_hImage.Clone(), MinValue, MaxValue, 100);

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
        public class CheckDefecData
        {          
            public string 阈值 { get; set; }
            public string 上限 { get; set; }
            public string 下限 { get; set; }
        }
    }
}
