namespace hWindowTool
{
    partial class HWindow
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.适应图像显示ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.显示隐藏状态栏ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.显示隐藏十字架ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.显示隐藏坐标指示ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.显示隐藏像素格ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.全屏显示ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.保存原始图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存窗口截图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label_Status = new System.Windows.Forms.Label();
            this.hWindowControl1 = new HalconDotNet.HWindowControl();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.适应图像显示ToolStripMenuItem,
            this.toolStripSeparator2,
            this.显示隐藏状态栏ToolStripMenuItem,
            this.显示隐藏十字架ToolStripMenuItem,
            this.显示隐藏坐标指示ToolStripMenuItem,
            this.显示隐藏像素格ToolStripMenuItem,
            this.toolStripSeparator1,
            this.全屏显示ToolStripMenuItem,
            this.toolStripSeparator3,
            this.保存原始图ToolStripMenuItem,
            this.保存窗口截图ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(198, 246);
            // 
            // 适应图像显示ToolStripMenuItem
            // 
            this.适应图像显示ToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.适应图像显示ToolStripMenuItem.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.适应图像显示ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.适应图像显示ToolStripMenuItem.Name = "适应图像显示ToolStripMenuItem";
            this.适应图像显示ToolStripMenuItem.Size = new System.Drawing.Size(197, 28);
            this.适应图像显示ToolStripMenuItem.Text = "适应图像显示";
            this.适应图像显示ToolStripMenuItem.Click += new System.EventHandler(this.适应图像显示ToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(194, 6);
            // 
            // 显示隐藏状态栏ToolStripMenuItem
            // 
            this.显示隐藏状态栏ToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.显示隐藏状态栏ToolStripMenuItem.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.显示隐藏状态栏ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.显示隐藏状态栏ToolStripMenuItem.Name = "显示隐藏状态栏ToolStripMenuItem";
            this.显示隐藏状态栏ToolStripMenuItem.Size = new System.Drawing.Size(197, 28);
            this.显示隐藏状态栏ToolStripMenuItem.Text = "显示/隐藏状态栏";
            this.显示隐藏状态栏ToolStripMenuItem.Click += new System.EventHandler(this.显示隐藏状态栏ToolStripMenuItem_Click);
            // 
            // 显示隐藏十字架ToolStripMenuItem
            // 
            this.显示隐藏十字架ToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.显示隐藏十字架ToolStripMenuItem.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.显示隐藏十字架ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.显示隐藏十字架ToolStripMenuItem.Name = "显示隐藏十字架ToolStripMenuItem";
            this.显示隐藏十字架ToolStripMenuItem.Size = new System.Drawing.Size(197, 28);
            this.显示隐藏十字架ToolStripMenuItem.Text = "显示/隐藏十字架";
            this.显示隐藏十字架ToolStripMenuItem.Click += new System.EventHandler(this.显示隐藏十字架ToolStripMenuItem_Click);
            // 
            // 显示隐藏坐标指示ToolStripMenuItem
            // 
            this.显示隐藏坐标指示ToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.显示隐藏坐标指示ToolStripMenuItem.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.显示隐藏坐标指示ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.显示隐藏坐标指示ToolStripMenuItem.Name = "显示隐藏坐标指示ToolStripMenuItem";
            this.显示隐藏坐标指示ToolStripMenuItem.Size = new System.Drawing.Size(197, 28);
            this.显示隐藏坐标指示ToolStripMenuItem.Text = "显示/隐藏坐标指示";
            this.显示隐藏坐标指示ToolStripMenuItem.Click += new System.EventHandler(this.显示隐藏坐标指示ToolStripMenuItem_Click);
            // 
            // 显示隐藏像素格ToolStripMenuItem
            // 
            this.显示隐藏像素格ToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.显示隐藏像素格ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.显示隐藏像素格ToolStripMenuItem.Name = "显示隐藏像素格ToolStripMenuItem";
            this.显示隐藏像素格ToolStripMenuItem.Size = new System.Drawing.Size(197, 28);
            this.显示隐藏像素格ToolStripMenuItem.Text = "显示/隐藏像素格";
            this.显示隐藏像素格ToolStripMenuItem.Click += new System.EventHandler(this.显示隐藏像素格ToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(194, 6);
            // 
            // 全屏显示ToolStripMenuItem
            // 
            this.全屏显示ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.全屏显示ToolStripMenuItem.Name = "全屏显示ToolStripMenuItem";
            this.全屏显示ToolStripMenuItem.Size = new System.Drawing.Size(197, 28);
            this.全屏显示ToolStripMenuItem.Text = "进入全屏(f)";
            this.全屏显示ToolStripMenuItem.Click += new System.EventHandler(this.全屏显示ToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(194, 6);
            // 
            // 保存原始图ToolStripMenuItem
            // 
            this.保存原始图ToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.保存原始图ToolStripMenuItem.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.保存原始图ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.保存原始图ToolStripMenuItem.Name = "保存原始图ToolStripMenuItem";
            this.保存原始图ToolStripMenuItem.Size = new System.Drawing.Size(197, 28);
            this.保存原始图ToolStripMenuItem.Text = "保存原始图像";
            this.保存原始图ToolStripMenuItem.Click += new System.EventHandler(this.保存原始图像ToolStripMenuItem_Click);
            // 
            // 保存窗口截图ToolStripMenuItem
            // 
            this.保存窗口截图ToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.保存窗口截图ToolStripMenuItem.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.保存窗口截图ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.保存窗口截图ToolStripMenuItem.Name = "保存窗口截图ToolStripMenuItem";
            this.保存窗口截图ToolStripMenuItem.Size = new System.Drawing.Size(197, 28);
            this.保存窗口截图ToolStripMenuItem.Text = "保存窗口截取图像";
            this.保存窗口截图ToolStripMenuItem.Click += new System.EventHandler(this.保存窗口截取图像ToolStripMenuItem_Click);
            // 
            // label_Status
            // 
            this.label_Status.AutoSize = true;
            this.label_Status.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.label_Status.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label_Status.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_Status.ForeColor = System.Drawing.Color.White;
            this.label_Status.Location = new System.Drawing.Point(0, 580);
            this.label_Status.Name = "label_Status";
            this.label_Status.Size = new System.Drawing.Size(0, 20);
            this.label_Status.TabIndex = 1;
            // 
            // hWindowControl1
            // 
            this.hWindowControl1.BackColor = System.Drawing.Color.Black;
            this.hWindowControl1.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl1.ContextMenuStrip = this.contextMenuStrip1;
            this.hWindowControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindowControl1.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl1.Location = new System.Drawing.Point(0, 0);
            this.hWindowControl1.Name = "hWindowControl1";
            this.hWindowControl1.Size = new System.Drawing.Size(978, 600);
            this.hWindowControl1.TabIndex = 2;
            this.hWindowControl1.WindowSize = new System.Drawing.Size(978, 600);
            // 
            // HWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.Controls.Add(this.label_Status);
            this.Controls.Add(this.hWindowControl1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "HWindow";
            this.Size = new System.Drawing.Size(978, 600);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label_Status;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 适应图像显示ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 显示隐藏十字架ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 显示隐藏状态栏ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 保存原始图ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 保存窗口截图ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 显示隐藏坐标指示ToolStripMenuItem;
        private HalconDotNet.HWindowControl hWindowControl1;
        private System.Windows.Forms.ToolStripMenuItem 显示隐藏像素格ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem 全屏显示ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}
