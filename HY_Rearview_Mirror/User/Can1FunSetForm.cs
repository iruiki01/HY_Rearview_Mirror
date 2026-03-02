using Helper;
using HY_Rearview_Mirror.Controls;
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
    public partial class Can1FunSetForm : UIForm
    {
        // 声明带参数的事件
        public EventManager eventManager = new EventManager();

        DataGridComboBox dataGridComboBox_Can = new DataGridComboBox();
        List<string> dataList_Can = new List<string>();
        public Can1FunSetForm()
        {
            InitializeComponent();
            // 设置范围
            numericUpDown1.Maximum = 0xFFFFFFFF;
            numericUpDown1.Minimum = 0;

            dataGridComboBox_Can = new DataGridComboBox
            {
                Location = new Point(50, 50),
                Size = new Size(300, 25),
            };
            dataGridComboBox_Can.Dock = DockStyle.Fill;
            panel1.Controls.Add(dataGridComboBox_Can);

            dataGridComboBox_Can.SelectedValueChanged += SelectedValueChanged;

            // 绑定方式
            dataGridComboBox_Can.DataSource = AppConfig.GetInstance()._config.canData;
            dataGridComboBox_Can.SetupColumnsWidth(80); // 第一列120像素，其他等分
        }
        private void SelectedValueChanged(object sender, EventArgs e)
        {
            dataList_Can.Clear();
            string str = dataGridComboBox_Can.Text;

            foreach (var itme in AppConfig.GetInstance()._config.canData)
            {
                if (str == itme.名称)
                {
                    dataList_Can.Add(itme.名称);
                    dataList_Can.Add(itme.值);
                    break;
                }
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            eventManager.Trigger("关闭窗体");
            this.Dispose();
        }
        private void uiButton1_Click(object sender, EventArgs e)
        {
            SelectedValueChanged(sender,e);
            eventManager.Trigger("发送界面参数", new List<object>() { numericUpDown1.Text, dataList_Can, uiTextBox1.Text, uiComboBox1.Text, uiComboBox2.Text, uiTextBox2.Text});

            UIMessageBox.ShowSuccess("参数设置成功！");
        }
        public void SetDataToForm(ITestParam testParam,string VersionStr = "")
        {
            if (testParam == null) return;
            CANFUNTestParam pressureTestParam = testParam as CANFUNTestParam;

            numericUpDown1.Text = pressureTestParam.ID.ToString("X");

            if(pressureTestParam.dataList_Can.Count > 0)
            {
                dataGridComboBox_Can.Text = pressureTestParam.dataList_Can[0];
            }
           
            uiTextBox1.Text = VersionStr;
            uiComboBox1.Text = pressureTestParam.ChangeDataType.ToString();
            uiComboBox2.Text = pressureTestParam.JudgeType.ToString();
            uiTextBox2.Text = pressureTestParam.JudgeValue.ToString();
        }
        /// <summary>
        /// 运行按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton4_Click(object sender, EventArgs e)
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

        private void uiButton2_Click(object sender, EventArgs e)
        {
            AppConfig.GetInstance().SaveConfig();
        }
    }
}
