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
    public partial class GlareForm : UIForm
    {
        // 声明带参数的事件
        public EventManager eventManager = new EventManager();
        public GlareForm()
        {
            InitializeComponent();
        }

        internal void SetDataToForm(ITestParam testParam, string versionStr)
        {
            if(testParam != null)
            {
                var p = testParam as GlareParam;

                uiTextBox1.Text = versionStr;
                uiComboBox2.Text = p.JudgeType;
                uiTextBox2.Text = p.JudgeValue;
            }
 
        }
        /// <summary>
        /// 获取亮度值1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton2_Click(object sender, EventArgs e)
        {
            uiTextBox3.Text = Context.GetInstance().LightValue1;
        }
        /// <summary>
        /// 获取亮度值2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton3_Click(object sender, EventArgs e)
        {
            uiTextBox4.Text = Context.GetInstance().LightValue2;
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            eventManager.Trigger("关闭窗体");
            this.Dispose();
        }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
            string str = uiTextBox1.Text;
            string str1 = uiComboBox2.Text;
            string str2 = uiTextBox2.Text;
            //string str3 = uiTextBox5.Text;
            eventManager.Trigger("发送界面参数", new List<object>() { str, str1, str2 });
            UIMessageBox.ShowSuccess("参数设置成功！");
        }

        private void uiButton4_Click(object sender, EventArgs e)
        {
            eventManager.Trigger("运行"); var result = eventManager.TriggerWithResult("运行");

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
