using Sunny.UI;
using Sunny.UI.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HY_Rearview_Mirror.Controls
{
    public delegate void Send_Login_Delegate(string type);
    public partial class LoginForm : UIForm
    {
        private const string OperatorPassword = "123456";
        private const string ProgrammerPassword = "111111";
        private const string managePassword = "666666";
        public static Send_Login_Delegate send_Login_Delegate;
        public LoginForm()
        {
            InitializeComponent();

            this.FormClosing += LoginForm_FormClosing;
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
           this.Dispose();
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            string GetPassword = maskedTextBox1.Text;
            string str = uiComboBox1.Text;
            if (((GetPassword == null) || (GetPassword == "")) || ((str == null) || (str == ""))) { MessageBox.Show("请输入用户名或密码！"); return; }
            if ((str == "操作员") && (GetPassword == OperatorPassword ? true : false))
            {
                LoginPassword.LoginState = (int)UserState.操作员;
                send_Login_Delegate?.Invoke("操作员");
                this.Close();
            }
            else if ((str == "工程师") && (GetPassword == ProgrammerPassword ? true : false))
            {
                LoginPassword.LoginState = (int)UserState.工程师;
                send_Login_Delegate?.Invoke("工程师");
                this.Close();
            }
            else if((str == "管理员") && (GetPassword == managePassword ? true : false))
            {
                LoginPassword.LoginState = (int)UserState.管理员;
                send_Login_Delegate?.Invoke("管理员");
                this.Close();
            }
            else
            {
                UIMessageBox.ShowError("用户名或密码错误！");
            }
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            LoginPassword.LoginState = (int)UserState.操作员;
            send_Login_Delegate?.Invoke("空");
            maskedTextBox1.Text = "空";
            this.Dispose();
        }
    }
    public enum UserState
    {
        操作员,
        工程师,
        管理员,
        空
    }
    public class LoginPassword
    {
        public static int LoginState = (int)UserState.空;
    }
}
