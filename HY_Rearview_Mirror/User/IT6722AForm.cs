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
    public partial class IT6722AForm : UIForm
    {
        // 声明带参数的事件
        public EventManager eventManager = new EventManager();

        //public event EventHandler<List<string>> OrderCompleted;
        //public event Func<Dictionary<string, object>> RunHandler;
        //public event Action FormClosingHandler;

        public IT6722AForm()
        {
            InitializeComponent();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            eventManager.Trigger("关闭窗体");
            this.Dispose();
        }
        /// <summary>
        /// 设置参数按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
            eventManager.Trigger("发送界面参数", new List<string>() { uiTextBox1.Text, uiComboBox2.Text, uiTextBox2.Text});

            UIMessageBox.ShowSuccess("参数设置成功！");
        }
        /// <summary>
        /// 运行函数按钮
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
        public void SetDataToForm(ITestParam testParam, string VersionStr = "")
        {
            if (testParam == null) return;
            IT6722AParam pressureTestParam = testParam as IT6722AParam;

            uiTextBox1.Text = VersionStr;
            uiComboBox2.Text = pressureTestParam.JudgeType.ToString();
            uiTextBox2.Text = pressureTestParam.JudgeValue.ToString();
        }
    }
}
