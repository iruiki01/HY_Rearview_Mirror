using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HY_Rearview_Mirror.Controls
{
    public partial class SplashForm : Form
    {
        private ApplicationContext context;
        private MainForm mainForm;
        private ProgressBar progressBar;
        private Label lblStatus;
        private Timer fadeTimer;
        private double fadeStep = 0.08;
        public SplashForm()
        {
            InitializeComponent();
        }
        public SplashForm(ApplicationContext ctx)
        {
            context = ctx;
            InitializeUI();
        }

        private void InitializeUI()
        {
            // 基础设置
            this.Size = new Size(600, 400);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.BackColor = Color.FromArgb(20, 20, 25);
            this.Opacity = 0; // 初始透明

            // 圆角窗体
            var path = new GraphicsPath();
            int radius = 20;
            path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
            path.AddArc(this.Width - radius * 2, 0, radius * 2, radius * 2, 270, 90);
            path.AddArc(this.Width - radius * 2, this.Height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(0, this.Height - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            this.Region = new Region(path);

            // 进度条
            progressBar = new ProgressBar
            {
                Location = new Point(100, 280),
                Size = new Size(400, 8),
                Style = ProgressBarStyle.Continuous,
                Maximum = 100,
                Value = 0
            };
            this.Controls.Add(progressBar);

            // 状态文字
            lblStatus = new Label
            {
                Location = new Point(100, 300),
                Size = new Size(400, 25),
                ForeColor = Color.White,
                Font = new Font("Microsoft YaHei", 10),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "准备启动..."
            };
            this.Controls.Add(lblStatus);

            // 淡入定时器
            var fadeIn = new Timer { Interval = 16 };
            fadeIn.Tick += (s, e) => {
                if (this.Opacity < 1) this.Opacity += 0.05;
                else fadeIn.Stop();
            };

            this.Shown += (s, e) => {
                fadeIn.Start();
                BeginLoading(); // 开始异步加载
            };
        }

        private async void BeginLoading()
        {
            var progress = new Progress<LoadingProgress>(p => {
                progressBar.Value = p.Percentage;
                lblStatus.Text = p.Message;
            });

            // 在后台线程初始化 MainForm（防止卡顿）
            mainForm = await Task.Run(() => InitializeMainForm(progress));

            // 加载完成，淡出并切换
            await FadeOut();

            // 切换 ApplicationContext 的主窗体
            context.MainForm = mainForm;
            mainForm.Show();
            this.Close(); // 关闭 Splash，但 Application.Run 继续运行
        }

        private MainForm InitializeMainForm(IProgress<LoadingProgress> progress)
        {
            MainForm form = null;

            // 阶段1-3：耗时操作（后台线程）
            progress.Report(new LoadingProgress(10, "加载配置文件..."));
            System.Threading.Thread.Sleep(200);

            progress.Report(new LoadingProgress(30, "初始化数据..."));
            System.Threading.Thread.Sleep(300);

            progress.Report(new LoadingProgress(60, "加载缓存数据..."));
            System.Threading.Thread.Sleep(200);

            // 阶段4：在主线程创建窗体（关键修复）
            this.Invoke(new Action(() => {
                progress.Report(new LoadingProgress(80, "构建主界面..."));

                form = new MainForm();

                // 强制创建句柄但不显示（替代 Show/Hide）
                // 访问 Handle 属性会强制创建 Win32 窗口句柄和子控件
                var forceCreate = form.Handle;

                // 强制计算布局（解决 Dock/Anchor 控件不显示）
                form.PerformLayout();

                // 预加载子控件资源（遍历创建句柄）
                CreateControlsHandle(form);

                progress.Report(new LoadingProgress(100, "就绪"));
            }));

            System.Threading.Thread.Sleep(300);
            return form;
        }

        // 递归创建所有子控件句柄
        private void CreateControlsHandle(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (!ctrl.IsHandleCreated)
                {
                    var dummy = ctrl.Handle; // 强制创建句柄
                }

                // 递归处理容器（Panel, GroupBox, TabControl 等）
                if (ctrl.HasChildren)
                    CreateControlsHandle(ctrl);
            }
        }

        private async Task FadeOut()
        {
            while (this.Opacity > 0)
            {
                this.Opacity -= fadeStep;
                await Task.Delay(16);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 绘制背景
            using (var brush = new LinearGradientBrush(
                this.ClientRectangle,
                Color.FromArgb(80, 160, 255),
                Color.FromArgb(20, 20, 25),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, this.ClientRectangle);
            }

            // 绘制 Logo/标题
            using (var font = new Font("Microsoft YaHei", 22, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.White))
            {
                var text = "华阳流媒体测试程序";
                var size = g.MeasureString(text, font);
                g.DrawString(text, font, brush, (this.Width - size.Width) / 2, 120);
            }

            // 绘制版本
            using (var font = new Font("Microsoft YaHei", 9))
            using (var brush = new SolidBrush(Color.DimGray))
            {
                g.DrawString("Version 2026.1", font, brush, 30, this.Height - 40);
                g.DrawString("© 2026 Company Inc.", font, brush, this.Width - 150, this.Height - 40);
            }
        }

        // 防止 Alt+F4 关闭
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (mainForm == null) e.Cancel = true; // 加载完成前禁止关闭
            base.OnFormClosing(e);
        }
    }

    // 进度报告结构
    public struct LoadingProgress
    {
        public int Percentage;
        public string Message;
        public LoadingProgress(int p, string m) { Percentage = p; Message = m; }
    }
}
