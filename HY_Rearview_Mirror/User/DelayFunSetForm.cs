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
    public partial class DelayFunSetForm : UIForm
    {
        public EventManager eventManager = new EventManager();

        public DelayFunSetForm()
        {
            InitializeComponent();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            eventManager.Trigger("关闭窗体");
            this.Dispose();
        }
        public void SetDataToForm(ITestParam testParam, string VersionStr = "")
        {
            if (testParam == null) return;
            DelayFunTestParam pressureTestParam = testParam as DelayFunTestParam;
            uiTextBox1.Text = pressureTestParam.seconds.ToString();
            uiTextBox2.Text = VersionStr;
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {        
            eventManager.Trigger("发送界面参数", new List<object>() { uiTextBox1.Text, uiTextBox2.Text });
            UIMessageBox.ShowSuccess("参数设置成功！");
            this.Close();
        }
    }
}
