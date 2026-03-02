using Helper;
using HY_Rearview_Mirror.Functions;
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
    public partial class EK2000ProForm : UIForm
    {
        // 声明带参数的事件
        public EventManager eventManager = new EventManager();
        public EK2000ProForm()
        {
            InitializeComponent();
        }

        internal void SetDataToForm(ITestParam testParam, string versionStr)
        {
            var p = testParam as EK2000ProParam;
            string str = uiTextBox3.Text = versionStr;
            string str1 = p.IntegralTime;
            string str2 = p.ReflectivityValue_Hight;
            string str3 = p.ReflectivityValue_Low;
        }

        private void uiButton3_Click(object sender, EventArgs e)
        {

        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            string str = uiTextBox3.Text;
            string str1 = uiTextBox1.Text;
            string str2 = uiTextBox2.Text;
            string str3 = uiTextBox4.Text;
            eventManager.Trigger("发送界面参数", new List<object>() { str, str1, str2, str3});
            UIMessageBox.ShowSuccess("参数设置成功！");
            this.Close();
        }
        /// <summary>
        /// 设置积分时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton2_Click(object sender, EventArgs e)
        {
            string str1 = uiTextBox1.Text;
            global.use_EK2000Pro.eK2000ProHelper.SetIntegrationTimeToSpectrometer(Convert.ToInt32(str1));
            UIMessageBox.ShowSuccess("积分时间设置成功");
        }
    }
}
