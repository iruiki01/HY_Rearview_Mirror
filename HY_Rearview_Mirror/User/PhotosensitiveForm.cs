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
    public partial class PhotosensitiveForm : UIForm
    {
        public EventManager eventManager = new EventManager();
        public PhotosensitiveForm()
        {
            InitializeComponent();
        }

        internal void SetDataToForm(ITestParam testParam, string versionStr)
        {
            if (testParam == null) return;
            var p = testParam as PhotosensitiveParam;

            string str = uiTextBox2.Text = versionStr;
            string str1 = uiTextBox5.Text = p.Photosensitive1Value;
            string str2 = uiTextBox6.Text = p.Photosensitive2Value;
            string str3 = uiComboBox1.Text = p.Photosensitive1Type;
            string str4 = uiComboBox2.Text = p.Photosensitive2Type;
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            var value = global.ZS_ModbusRtuHelpe.ReadInt16("s=3;0");
            ushort lux = (ushort)value.Content;
            uiTextBox1.Text = lux.ToString() + "  Lux";
        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            uiTextBox3.Text = Context.GetInstance().Photosensitive1 + "  Lux";
        }

        private void uiButton4_Click(object sender, EventArgs e)
        {
            uiTextBox4.Text = Context.GetInstance().Photosensitive2 + "  Lux";
        }
        /// <summary>
        /// 发送界面参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
            string str = uiTextBox2.Text;
            string str1 = uiTextBox5.Text;
            string str2 = uiTextBox6.Text;
            string str3 = uiComboBox1.Text;
            string str4 = uiComboBox2.Text;
            eventManager.Trigger("发送界面参数",new List<object> { str , str1 , str2 , str3 , str4 });

            this.Close();
        }

        private void uiButton5_Click(object sender, EventArgs e)
        {
            eventManager.Trigger("运行");
        }
    }
}
