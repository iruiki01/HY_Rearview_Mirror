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
    public partial class ZhongshengForm : UIForm
    {
        public EventManager eventManager = new EventManager();
        DataGridComboBox dataGridComboBox_Zhongsheng = new DataGridComboBox(); // 亮度
        List<string> dataList_Zhongsheng = new List<string>();
        public ZhongshengForm()
        {
            InitializeComponent();

            dataGridComboBox_Zhongsheng = new DataGridComboBox
            {
                Location = new Point(50, 50),
                Size = new Size(300, 25),
            };
            dataGridComboBox_Zhongsheng.Dock = DockStyle.Fill;
            panel5.Controls.Add(dataGridComboBox_Zhongsheng);

            dataGridComboBox_Zhongsheng.SelectedValueChanged += SelectedValueChanged;

            // 绑定方式
            dataGridComboBox_Zhongsheng.DataSource = AppConfig.GetInstance()._config.zhongshengData;
            dataGridComboBox_Zhongsheng.SetupColumnsWidth(80); // 第一列120像素，其他等分
        }
        private void SelectedValueChanged(object sender, EventArgs e)
        {
            dataList_Zhongsheng.Clear();
            string str = dataGridComboBox_Zhongsheng.Text;

            foreach (var itme in AppConfig.GetInstance()._config.zhongshengData)
            {
                if (str == itme.名称)
                {
                    dataList_Zhongsheng.Add(itme.名称);
                    dataList_Zhongsheng.Add(itme.值);
                    dataList_Zhongsheng.Add(itme.操作);
                    break;
                }
            }
        }
        public void SetDataToForm(ITestParam testParam, string versionStr)
        {
            var value = testParam as ZhongshengParam;

            uiTextBox2.Text = versionStr;

            if (value.RunData != null)
            {
                dataGridComboBox_Zhongsheng.Text = value.RunData[0];
            }          
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            AppConfig.GetInstance().SaveConfig();
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            eventManager.Trigger("发送界面参数", new List<object>() {uiTextBox2.Text, dataList_Zhongsheng});
            UIMessageBox.ShowSuccess("参数设置成功！");
            this.Close();
        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            eventManager.Trigger("运行");
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            eventManager.Trigger("关闭窗体");
            this.Dispose();
        }
    }
}
