using HY_Rearview_Mirror.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HY_Rearview_Mirror
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 确保只有一个实例运行
            using (var mutex = new Mutex(true, "MyUniqueAppName", out bool createdNew))
            {
                if (!createdNew)
                {
                    MessageBox.Show("程序已在运行中。", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 使用自定义上下文，允许动态切换主窗体
                var context = new ApplicationContext();
                var splash = new SplashForm(context);
                context.MainForm = splash;

                Application.Run(context);

                //Application.Run(new MainForm());
            }
        }
    }
}
