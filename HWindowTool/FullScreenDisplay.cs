using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hWindowTool
{
    /// <summary>
    /// 全屏显示
    /// </summary>
    public partial class FullScreenDisplay : Form
    {
        /// <summary>
        /// halcon控件
        /// </summary>
        private hWindowTool.HWindow hWindow;

        /// <summary>
        /// halcon控件相对位置
        /// </summary>
        private Point hLocation;

        /// <summary>
        /// halcon控件宽度
        /// </summary>
        private int hWidth;

        /// <summary>
        /// halcon控件高度
        /// </summary>
        private int hHeight;

        /// <summary>
        /// halcon控件父容器
        /// </summary>
        private Control hBaseControl;

        /// <summary>
        /// 全屏显示
        /// </summary>
        /// <param name="hWindow">halcon控件</param>
        public FullScreenDisplay(hWindowTool.HWindow hWindow)
        {
            InitializeComponent();
            //设置窗体最大化满屏
            this.MaximizedBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            //更新halcon控件
            this.hWindow = hWindow;
            //更新halcon控件相对位置
            this.hLocation = hWindow.Location;
            //更新halcon控件宽度
            this.hWidth = hWindow.Width;
            //更新halcon控件高度
            this.hHeight = hWindow.Height;
            //更新halcon控件父容器
            this.hBaseControl = hWindow.Parent;
            //订阅在第一次显示窗体前发生事件
            this.Load += FullScreenDisplay_Load;
        }

        /// <summary>
        /// 在第一次显示窗体前发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FullScreenDisplay_Load(object sender, EventArgs e)
        {
            //halcon控件存在父容器
            if (hBaseControl != null)
            {
                //从halcon控件父容器控件集合中移除halcon控件
                hBaseControl.Controls.Remove(hWindow);
            }
            //将halcon控件添加到本窗体
            this.Controls.Add(hWindow);
            //设置halcon控件相对位置和宽高
            hWindow.Location = new Point(0, 0);
            hWindow.Width = this.Width;
            hWindow.Height = this.Height;
        }

        /// <summary>
        /// 退出全屏
        /// </summary>
        public void ExitFullScreen()
        {
            //从本窗体移除halcon控件
            this.Controls.Remove(hWindow);
            //halcon控件存在父容器
            if (hBaseControl != null)
            {
                //将halcon控件添加回父容器
                hBaseControl.Controls.Add(hWindow);
            }
            //设置halcon控件相对位置和宽高
            hWindow.Location = hLocation;
            hWindow.Width = hWidth;
            hWindow.Height = hHeight;
            //关闭窗体
            this.Close();
        }
    }
}
