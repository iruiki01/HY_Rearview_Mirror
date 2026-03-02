using Helper;
using HY_Rearview_Mirror.Controls;
using HY_Rearview_Mirror.InterfaceTool;
using Sunny.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HY_Rearview_Mirror.User
{
    /// <summary>
    /// 产品串口协议
    /// </summary>
    public partial class UartForm : UIForm
    {
        // 声明带参数的事件
        public EventManager eventManager = new EventManager();

        DataGridComboBox dataGridComboBox_Uart = new DataGridComboBox();

        List<string> dataList_Uart = new List<string>();
        public UartForm()
        {
            InitializeComponent();

            dataGridComboBox_Uart = new DataGridComboBox
            {
                Location = new Point(50, 50),
                Size = new Size(300, 25),
            };
            //var products = new List<UartData>
            //{
            //    new UartData { 名称 = "",值 = ""},
            //};
            //dataGridComboBox_Uart.DataSource = products;  // 绑定List;
            dataGridComboBox_Uart.Dock = DockStyle.Fill;
            panel1.Controls.Add(dataGridComboBox_Uart);

            dataGridComboBox_Uart.SelectedValueChanged += SelectedValueChanged;

            // 绑定方式
            dataGridComboBox_Uart.DataSource = AppConfig.GetInstance()._config.uartData;
            //dataGridComboBox_Uart.DataBindings.Add("Text", AppConfig.GetInstance()._config.uartData, "uartData", true, DataSourceUpdateMode.OnPropertyChanged);
            dataGridComboBox_Uart.SetupColumnsWidth(80); // 第一列120像素，其他等分
        }

        private void SelectedValueChanged(object sender, EventArgs e)
        {
            dataList_Uart.Clear();
            string str = dataGridComboBox_Uart.Text;

            foreach(var itme in AppConfig.GetInstance()._config.uartData)
            {
                if(str == itme.名称)
                {
                    dataList_Uart.Add(itme.名称);
                    dataList_Uart.Add(itme.值);
                    break;
                }
            }
        }
        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
            AppConfig.GetInstance().SaveConfig();
        }
        /// <summary>
        /// 设置参数按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton2_Click(object sender, EventArgs e)
        {
            string str1 = uiTextBox1.Text;
            string str2 = uiComboBox1.Text;
            string str3 = uiComboBox2.Text;
            string str4 = uiTextBox2.Text;
            eventManager.Trigger("发送界面参数", new List<object>() { str1, str2, str3, str4, dataList_Uart });

            UIMessageBox.ShowSuccess("参数设置成功！");
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
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            eventManager.Trigger("关闭窗体");
            this.Dispose();
        }

        internal void SetDataToForm(ITestParam testParam, string versionStr)
        {
            
        }
    }
}
