namespace HY_Rearview_Mirror
{
    partial class MainForm
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("新建");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("打开");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("保存");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("文件", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("参数设置");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("MES");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("设置", new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode6});
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("登录");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("日志");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("读取RFID");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("工具", new System.Windows.Forms.TreeNode[] {
            treeNode8,
            treeNode9,
            treeNode10});
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.uiNavBar1 = new Sunny.UI.UINavBar();
            this.uiButton1 = new Sunny.UI.UIButton();
            this.uiButton2 = new Sunny.UI.UIButton();
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.uiTitlePanel1 = new Sunny.UI.UITitlePanel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.uiTitlePanel2 = new Sunny.UI.UITitlePanel();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.uiContextMenuStrip1 = new Sunny.UI.UIContextMenuStrip();
            this.添加ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.执行一次ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.uiProcessBar1 = new Sunny.UI.UIProcessBar();
            this.uiLabel2 = new Sunny.UI.UILabel();
            this.uiTextBox1 = new Sunny.UI.UITextBox();
            this.uiTextBox2 = new Sunny.UI.UITextBox();
            this.uiLabel3 = new Sunny.UI.UILabel();
            this.uiLabel4 = new Sunny.UI.UILabel();
            this.uiTextBox3 = new Sunny.UI.UITextBox();
            this.uiTextBox4 = new Sunny.UI.UITextBox();
            this.uiLabel5 = new Sunny.UI.UILabel();
            this.uiTextBox5 = new Sunny.UI.UITextBox();
            this.uiLabel6 = new Sunny.UI.UILabel();
            this.uiTextBox6 = new Sunny.UI.UITextBox();
            this.uiLabel7 = new Sunny.UI.UILabel();
            this.uiTextBox7 = new Sunny.UI.UITextBox();
            this.uiLabel8 = new Sunny.UI.UILabel();
            this.uiTextBox8 = new Sunny.UI.UITextBox();
            this.uiLabel9 = new Sunny.UI.UILabel();
            this.uiDataGridView1 = new Sunny.UI.UIDataGridView();
            this.名称 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.测试项目 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.前提条件 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.操作步骤 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.期望结果 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.实际结果 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.结果描述 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.测试时间 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.uiLabel10 = new Sunny.UI.UILabel();
            this.uiLabel11 = new Sunny.UI.UILabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.uiTitlePanel5 = new Sunny.UI.UITitlePanel();
            this.hWindow3 = new hWindowTool.HWindow();
            this.uiTitlePanel4 = new Sunny.UI.UITitlePanel();
            this.hWindow2 = new hWindowTool.HWindow();
            this.uiTitlePanel3 = new Sunny.UI.UITitlePanel();
            this.hWindow1 = new hWindowTool.HWindow();
            this.uiTextBox9 = new Sunny.UI.UITextBox();
            this.uiLabel13 = new Sunny.UI.UILabel();
            this.uiTextBox10 = new Sunny.UI.UITextBox();
            this.uiLabel14 = new Sunny.UI.UILabel();
            this.uiTitlePanel1.SuspendLayout();
            this.uiTitlePanel2.SuspendLayout();
            this.uiContextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uiDataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.uiTitlePanel5.SuspendLayout();
            this.uiTitlePanel4.SuspendLayout();
            this.uiTitlePanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiNavBar1
            // 
            this.uiNavBar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.uiNavBar1.DropMenuFont = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiNavBar1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiNavBar1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.uiNavBar1.Location = new System.Drawing.Point(0, 35);
            this.uiNavBar1.MenuStyle = Sunny.UI.UIMenuStyle.Custom;
            this.uiNavBar1.Name = "uiNavBar1";
            this.uiNavBar1.NodeAlignment = System.Drawing.StringAlignment.Near;
            this.uiNavBar1.NodeInterval = 1;
            treeNode1.Name = "节点0";
            treeNode1.Text = "新建";
            treeNode2.Name = "节点1";
            treeNode2.Text = "打开";
            treeNode3.Name = "节点2";
            treeNode3.Text = "保存";
            treeNode4.Name = "节点0";
            treeNode4.Text = "文件";
            treeNode5.Name = "节点0";
            treeNode5.Text = "参数设置";
            treeNode6.Name = "节点0";
            treeNode6.Text = "MES";
            treeNode7.Name = "节点0";
            treeNode7.Text = "设置";
            treeNode8.Name = "节点1";
            treeNode8.Text = "登录";
            treeNode9.Name = "节点1";
            treeNode9.Text = "日志";
            treeNode10.Name = "节点0";
            treeNode10.Text = "读取RFID";
            treeNode11.Name = "节点1";
            treeNode11.Text = "工具";
            this.uiNavBar1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode7,
            treeNode11});
            this.uiNavBar1.NodeSize = new System.Drawing.Size(100, 45);
            this.uiNavBar1.Size = new System.Drawing.Size(1534, 48);
            this.uiNavBar1.TabIndex = 0;
            this.uiNavBar1.Text = "uiNavBar1";
            this.uiNavBar1.NodeMouseClick += new Sunny.UI.UINavBar.OnNodeMouseClick(this.uiNavBar1_NodeMouseClick);
            // 
            // uiButton1
            // 
            this.uiButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton1.Location = new System.Drawing.Point(419, 89);
            this.uiButton1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton1.Name = "uiButton1";
            this.uiButton1.Size = new System.Drawing.Size(72, 51);
            this.uiButton1.TabIndex = 3;
            this.uiButton1.Text = "启动";
            this.uiButton1.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton1.Click += new System.EventHandler(this.uiButton1_Click);
            // 
            // uiButton2
            // 
            this.uiButton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton2.Location = new System.Drawing.Point(493, 89);
            this.uiButton2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton2.Name = "uiButton2";
            this.uiButton2.Size = new System.Drawing.Size(72, 51);
            this.uiButton2.TabIndex = 7;
            this.uiButton2.Text = "停止";
            this.uiButton2.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton2.Click += new System.EventHandler(this.uiButton2_Click);
            // 
            // uiLabel1
            // 
            this.uiLabel1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel1.Location = new System.Drawing.Point(423, 160);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(100, 23);
            this.uiLabel1.TabIndex = 9;
            this.uiLabel1.Text = "测试进度:";
            this.uiLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiTitlePanel1
            // 
            this.uiTitlePanel1.Controls.Add(this.richTextBox1);
            this.uiTitlePanel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTitlePanel1.Location = new System.Drawing.Point(2, 89);
            this.uiTitlePanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel1.Name = "uiTitlePanel1";
            this.uiTitlePanel1.ShowText = false;
            this.uiTitlePanel1.Size = new System.Drawing.Size(415, 404);
            this.uiTitlePanel1.TabIndex = 10;
            this.uiTitlePanel1.Text = "生产信息";
            this.uiTitlePanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Font = new System.Drawing.Font("Consolas", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(0, 36);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(409, 365);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // uiTitlePanel2
            // 
            this.uiTitlePanel2.Controls.Add(this.richTextBox2);
            this.uiTitlePanel2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTitlePanel2.Location = new System.Drawing.Point(2, 495);
            this.uiTitlePanel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel2.Name = "uiTitlePanel2";
            this.uiTitlePanel2.ShowText = false;
            this.uiTitlePanel2.Size = new System.Drawing.Size(415, 415);
            this.uiTitlePanel2.TabIndex = 11;
            this.uiTitlePanel2.Text = "设备日志";
            this.uiTitlePanel2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // richTextBox2
            // 
            this.richTextBox2.Font = new System.Drawing.Font("Consolas", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox2.Location = new System.Drawing.Point(3, 37);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(409, 374);
            this.richTextBox2.TabIndex = 0;
            this.richTextBox2.Text = "";
            // 
            // uiContextMenuStrip1
            // 
            this.uiContextMenuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            this.uiContextMenuStrip1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiContextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.uiContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.添加ToolStripMenuItem,
            this.删除ToolStripMenuItem,
            this.执行一次ToolStripMenuItem});
            this.uiContextMenuStrip1.Name = "uiContextMenuStrip1";
            this.uiContextMenuStrip1.Size = new System.Drawing.Size(139, 70);
            // 
            // 添加ToolStripMenuItem
            // 
            this.添加ToolStripMenuItem.Name = "添加ToolStripMenuItem";
            this.添加ToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.添加ToolStripMenuItem.Text = "添加";
            this.添加ToolStripMenuItem.Click += new System.EventHandler(this.添加ToolStripMenuItem_Click);
            // 
            // 删除ToolStripMenuItem
            // 
            this.删除ToolStripMenuItem.Name = "删除ToolStripMenuItem";
            this.删除ToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.删除ToolStripMenuItem.Text = "删除";
            this.删除ToolStripMenuItem.Click += new System.EventHandler(this.删除ToolStripMenuItem_Click);
            // 
            // 执行一次ToolStripMenuItem
            // 
            this.执行一次ToolStripMenuItem.Name = "执行一次ToolStripMenuItem";
            this.执行一次ToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.执行一次ToolStripMenuItem.Text = "执行一次";
            this.执行一次ToolStripMenuItem.Click += new System.EventHandler(this.执行一次ToolStripMenuItem_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // uiProcessBar1
            // 
            this.uiProcessBar1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiProcessBar1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiProcessBar1.Location = new System.Drawing.Point(529, 157);
            this.uiProcessBar1.MinimumSize = new System.Drawing.Size(70, 3);
            this.uiProcessBar1.Name = "uiProcessBar1";
            this.uiProcessBar1.Size = new System.Drawing.Size(861, 29);
            this.uiProcessBar1.TabIndex = 16;
            this.uiProcessBar1.Text = "uiProcessBar1";
            // 
            // uiLabel2
            // 
            this.uiLabel2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel2.Location = new System.Drawing.Point(770, 89);
            this.uiLabel2.Name = "uiLabel2";
            this.uiLabel2.Size = new System.Drawing.Size(73, 23);
            this.uiLabel2.TabIndex = 17;
            this.uiLabel2.Text = "产品型号:";
            this.uiLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiTextBox1
            // 
            this.uiTextBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTextBox1.Location = new System.Drawing.Point(829, 87);
            this.uiTextBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox1.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox1.Name = "uiTextBox1";
            this.uiTextBox1.Padding = new System.Windows.Forms.Padding(5);
            this.uiTextBox1.ShowText = false;
            this.uiTextBox1.Size = new System.Drawing.Size(189, 29);
            this.uiTextBox1.TabIndex = 18;
            this.uiTextBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox1.Watermark = "";
            // 
            // uiTextBox2
            // 
            this.uiTextBox2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTextBox2.Location = new System.Drawing.Point(829, 122);
            this.uiTextBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox2.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox2.Name = "uiTextBox2";
            this.uiTextBox2.Padding = new System.Windows.Forms.Padding(5);
            this.uiTextBox2.ShowText = false;
            this.uiTextBox2.Size = new System.Drawing.Size(189, 29);
            this.uiTextBox2.TabIndex = 20;
            this.uiTextBox2.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox2.Watermark = "";
            // 
            // uiLabel3
            // 
            this.uiLabel3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel3.Location = new System.Drawing.Point(770, 124);
            this.uiLabel3.Name = "uiLabel3";
            this.uiLabel3.Size = new System.Drawing.Size(73, 23);
            this.uiLabel3.TabIndex = 19;
            this.uiLabel3.Text = "工单号:";
            this.uiLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiLabel4
            // 
            this.uiLabel4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel4.Location = new System.Drawing.Point(1198, 84);
            this.uiLabel4.Name = "uiLabel4";
            this.uiLabel4.Size = new System.Drawing.Size(58, 23);
            this.uiLabel4.TabIndex = 21;
            this.uiLabel4.Text = "总数:";
            this.uiLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiTextBox3
            // 
            this.uiTextBox3.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTextBox3.Location = new System.Drawing.Point(1242, 86);
            this.uiTextBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox3.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox3.Name = "uiTextBox3";
            this.uiTextBox3.Padding = new System.Windows.Forms.Padding(5);
            this.uiTextBox3.ReadOnly = true;
            this.uiTextBox3.ShowText = false;
            this.uiTextBox3.Size = new System.Drawing.Size(118, 29);
            this.uiTextBox3.TabIndex = 22;
            this.uiTextBox3.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox3.Watermark = "";
            // 
            // uiTextBox4
            // 
            this.uiTextBox4.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTextBox4.Location = new System.Drawing.Point(1242, 120);
            this.uiTextBox4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox4.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox4.Name = "uiTextBox4";
            this.uiTextBox4.Padding = new System.Windows.Forms.Padding(5);
            this.uiTextBox4.ReadOnly = true;
            this.uiTextBox4.ShowText = false;
            this.uiTextBox4.Size = new System.Drawing.Size(118, 29);
            this.uiTextBox4.TabIndex = 24;
            this.uiTextBox4.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox4.Watermark = "";
            // 
            // uiLabel5
            // 
            this.uiLabel5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel5.Location = new System.Drawing.Point(1198, 118);
            this.uiLabel5.Name = "uiLabel5";
            this.uiLabel5.Size = new System.Drawing.Size(58, 23);
            this.uiLabel5.TabIndex = 23;
            this.uiLabel5.Text = "PASS:";
            this.uiLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiTextBox5
            // 
            this.uiTextBox5.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTextBox5.Location = new System.Drawing.Point(1411, 86);
            this.uiTextBox5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox5.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox5.Name = "uiTextBox5";
            this.uiTextBox5.Padding = new System.Windows.Forms.Padding(5);
            this.uiTextBox5.ReadOnly = true;
            this.uiTextBox5.ShowText = false;
            this.uiTextBox5.Size = new System.Drawing.Size(118, 29);
            this.uiTextBox5.TabIndex = 26;
            this.uiTextBox5.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox5.Watermark = "";
            // 
            // uiLabel6
            // 
            this.uiLabel6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel6.Location = new System.Drawing.Point(1361, 86);
            this.uiLabel6.Name = "uiLabel6";
            this.uiLabel6.Size = new System.Drawing.Size(58, 23);
            this.uiLabel6.TabIndex = 25;
            this.uiLabel6.Text = "合格率:";
            this.uiLabel6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiTextBox6
            // 
            this.uiTextBox6.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTextBox6.Location = new System.Drawing.Point(1411, 119);
            this.uiTextBox6.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox6.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox6.Name = "uiTextBox6";
            this.uiTextBox6.Padding = new System.Windows.Forms.Padding(5);
            this.uiTextBox6.ReadOnly = true;
            this.uiTextBox6.ShowText = false;
            this.uiTextBox6.Size = new System.Drawing.Size(118, 29);
            this.uiTextBox6.TabIndex = 28;
            this.uiTextBox6.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox6.Watermark = "";
            // 
            // uiLabel7
            // 
            this.uiLabel7.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel7.Location = new System.Drawing.Point(1361, 119);
            this.uiLabel7.Name = "uiLabel7";
            this.uiLabel7.Size = new System.Drawing.Size(58, 23);
            this.uiLabel7.TabIndex = 27;
            this.uiLabel7.Text = "FAIL:";
            this.uiLabel7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiTextBox7
            // 
            this.uiTextBox7.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox7.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTextBox7.Location = new System.Drawing.Point(1080, 120);
            this.uiTextBox7.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox7.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox7.Name = "uiTextBox7";
            this.uiTextBox7.Padding = new System.Windows.Forms.Padding(5);
            this.uiTextBox7.ReadOnly = true;
            this.uiTextBox7.ShowText = false;
            this.uiTextBox7.Size = new System.Drawing.Size(118, 29);
            this.uiTextBox7.TabIndex = 32;
            this.uiTextBox7.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox7.Watermark = "";
            // 
            // uiLabel8
            // 
            this.uiLabel8.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel8.Location = new System.Drawing.Point(1020, 119);
            this.uiLabel8.Name = "uiLabel8";
            this.uiLabel8.Size = new System.Drawing.Size(58, 23);
            this.uiLabel8.TabIndex = 31;
            this.uiLabel8.Text = "工位号:";
            this.uiLabel8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiTextBox8
            // 
            this.uiTextBox8.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox8.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTextBox8.Location = new System.Drawing.Point(1080, 86);
            this.uiTextBox8.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox8.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox8.Name = "uiTextBox8";
            this.uiTextBox8.Padding = new System.Windows.Forms.Padding(5);
            this.uiTextBox8.ReadOnly = true;
            this.uiTextBox8.ShowText = false;
            this.uiTextBox8.Size = new System.Drawing.Size(118, 29);
            this.uiTextBox8.TabIndex = 30;
            this.uiTextBox8.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox8.Watermark = "";
            // 
            // uiLabel9
            // 
            this.uiLabel9.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel9.Location = new System.Drawing.Point(1020, 86);
            this.uiLabel9.Name = "uiLabel9";
            this.uiLabel9.Size = new System.Drawing.Size(86, 23);
            this.uiLabel9.TabIndex = 29;
            this.uiLabel9.Text = "登录权限:";
            this.uiLabel9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiDataGridView1
            // 
            this.uiDataGridView1.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiDataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.uiDataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.uiDataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.uiDataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.uiDataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.uiDataGridView1.ColumnHeadersHeight = 32;
            this.uiDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.uiDataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.名称,
            this.测试项目,
            this.前提条件,
            this.操作步骤,
            this.期望结果,
            this.实际结果,
            this.结果描述,
            this.测试时间});
            this.uiDataGridView1.ContextMenuStrip = this.uiContextMenuStrip1;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.uiDataGridView1.DefaultCellStyle = dataGridViewCellStyle3;
            this.uiDataGridView1.EnableHeadersVisualStyles = false;
            this.uiDataGridView1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiDataGridView1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiDataGridView1.Location = new System.Drawing.Point(423, 192);
            this.uiDataGridView1.Name = "uiDataGridView1";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.uiDataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.uiDataGridView1.RowHeadersVisible = false;
            this.uiDataGridView1.RowHeadersWidth = 51;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiDataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.uiDataGridView1.RowTemplate.Height = 27;
            this.uiDataGridView1.SelectedIndex = -1;
            this.uiDataGridView1.Size = new System.Drawing.Size(1111, 439);
            this.uiDataGridView1.StripeOddColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiDataGridView1.TabIndex = 0;
            this.uiDataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.uiDataGridView1_CellClick);
            this.uiDataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.uiDataGridView1_CellDoubleClick);
            // 
            // 名称
            // 
            this.名称.HeaderText = "名称";
            this.名称.MinimumWidth = 6;
            this.名称.Name = "名称";
            // 
            // 测试项目
            // 
            this.测试项目.HeaderText = "测试项目";
            this.测试项目.MinimumWidth = 6;
            this.测试项目.Name = "测试项目";
            // 
            // 前提条件
            // 
            this.前提条件.HeaderText = "前提条件";
            this.前提条件.MinimumWidth = 6;
            this.前提条件.Name = "前提条件";
            // 
            // 操作步骤
            // 
            this.操作步骤.HeaderText = "操作步骤";
            this.操作步骤.MinimumWidth = 6;
            this.操作步骤.Name = "操作步骤";
            // 
            // 期望结果
            // 
            this.期望结果.HeaderText = "期望结果";
            this.期望结果.MinimumWidth = 6;
            this.期望结果.Name = "期望结果";
            // 
            // 实际结果
            // 
            this.实际结果.FillWeight = 200F;
            this.实际结果.HeaderText = "实际结果";
            this.实际结果.MinimumWidth = 6;
            this.实际结果.Name = "实际结果";
            // 
            // 结果描述
            // 
            this.结果描述.FillWeight = 80F;
            this.结果描述.HeaderText = "结果描述";
            this.结果描述.MinimumWidth = 6;
            this.结果描述.Name = "结果描述";
            // 
            // 测试时间
            // 
            this.测试时间.HeaderText = "测试时间";
            this.测试时间.MinimumWidth = 6;
            this.测试时间.Name = "测试时间";
            // 
            // uiLabel10
            // 
            this.uiLabel10.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel10.Location = new System.Drawing.Point(1390, 160);
            this.uiLabel10.Name = "uiLabel10";
            this.uiLabel10.Size = new System.Drawing.Size(73, 23);
            this.uiLabel10.TabIndex = 35;
            this.uiLabel10.Text = "测试时间:";
            this.uiLabel10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiLabel11
            // 
            this.uiLabel11.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel11.Location = new System.Drawing.Point(1444, 161);
            this.uiLabel11.Name = "uiLabel11";
            this.uiLabel11.Size = new System.Drawing.Size(78, 23);
            this.uiLabel11.TabIndex = 36;
            this.uiLabel11.Text = "0ms";
            this.uiLabel11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.uiTitlePanel5);
            this.panel1.Controls.Add(this.uiTitlePanel4);
            this.panel1.Controls.Add(this.uiTitlePanel3);
            this.panel1.Location = new System.Drawing.Point(420, 633);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1111, 277);
            this.panel1.TabIndex = 38;
            // 
            // uiTitlePanel5
            // 
            this.uiTitlePanel5.Controls.Add(this.hWindow3);
            this.uiTitlePanel5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTitlePanel5.Location = new System.Drawing.Point(740, 0);
            this.uiTitlePanel5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel5.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel5.Name = "uiTitlePanel5";
            this.uiTitlePanel5.ShowText = false;
            this.uiTitlePanel5.Size = new System.Drawing.Size(370, 277);
            this.uiTitlePanel5.TabIndex = 37;
            this.uiTitlePanel5.Text = "色度仪";
            this.uiTitlePanel5.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // hWindow3
            // 
            this.hWindow3.AutomaticFocus = false;
            this.hWindow3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.hWindow3.CoordinateMode = hWindowTool.CoordinateMode.XY;
            this.hWindow3.CoordinateProportion = 0.05D;
            this.hWindow3.CoordinateTextSize = 20;
            this.hWindow3.DisplayCoordinate = false;
            this.hWindow3.DisplayCrosses = false;
            this.hWindow3.DisplayPixelGrid = true;
            this.hWindow3.DisplayStatus = true;
            this.hWindow3.DisplayWindowNumber = false;
            this.hWindow3.DrawModel = false;
            this.hWindow3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hWindow3.Location = new System.Drawing.Point(4, 39);
            this.hWindow3.Margin = new System.Windows.Forms.Padding(4);
            this.hWindow3.MinZoom = false;
            this.hWindow3.Name = "hWindow3";
            this.hWindow3.PixelGrid = 30;
            this.hWindow3.Size = new System.Drawing.Size(359, 233);
            this.hWindow3.StopRefresh = false;
            this.hWindow3.TabIndex = 0;
            this.hWindow3.WindowNumber = 1;
            // 
            // uiTitlePanel4
            // 
            this.uiTitlePanel4.Controls.Add(this.hWindow2);
            this.uiTitlePanel4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTitlePanel4.Location = new System.Drawing.Point(369, 0);
            this.uiTitlePanel4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel4.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel4.Name = "uiTitlePanel4";
            this.uiTitlePanel4.ShowText = false;
            this.uiTitlePanel4.Size = new System.Drawing.Size(368, 277);
            this.uiTitlePanel4.TabIndex = 36;
            this.uiTitlePanel4.Text = "相机2";
            this.uiTitlePanel4.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // hWindow2
            // 
            this.hWindow2.AutomaticFocus = false;
            this.hWindow2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.hWindow2.CoordinateMode = hWindowTool.CoordinateMode.XY;
            this.hWindow2.CoordinateProportion = 0.05D;
            this.hWindow2.CoordinateTextSize = 20;
            this.hWindow2.DisplayCoordinate = false;
            this.hWindow2.DisplayCrosses = false;
            this.hWindow2.DisplayPixelGrid = true;
            this.hWindow2.DisplayStatus = true;
            this.hWindow2.DisplayWindowNumber = false;
            this.hWindow2.DrawModel = false;
            this.hWindow2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hWindow2.Location = new System.Drawing.Point(4, 39);
            this.hWindow2.Margin = new System.Windows.Forms.Padding(4);
            this.hWindow2.MinZoom = false;
            this.hWindow2.Name = "hWindow2";
            this.hWindow2.PixelGrid = 30;
            this.hWindow2.Size = new System.Drawing.Size(359, 233);
            this.hWindow2.StopRefresh = false;
            this.hWindow2.TabIndex = 0;
            this.hWindow2.WindowNumber = 1;
            // 
            // uiTitlePanel3
            // 
            this.uiTitlePanel3.Controls.Add(this.hWindow1);
            this.uiTitlePanel3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTitlePanel3.Location = new System.Drawing.Point(0, 0);
            this.uiTitlePanel3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel3.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel3.Name = "uiTitlePanel3";
            this.uiTitlePanel3.ShowText = false;
            this.uiTitlePanel3.Size = new System.Drawing.Size(367, 277);
            this.uiTitlePanel3.TabIndex = 35;
            this.uiTitlePanel3.Text = "相机1";
            this.uiTitlePanel3.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // hWindow1
            // 
            this.hWindow1.AutomaticFocus = false;
            this.hWindow1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.hWindow1.CoordinateMode = hWindowTool.CoordinateMode.XY;
            this.hWindow1.CoordinateProportion = 0.05D;
            this.hWindow1.CoordinateTextSize = 20;
            this.hWindow1.DisplayCoordinate = false;
            this.hWindow1.DisplayCrosses = false;
            this.hWindow1.DisplayPixelGrid = true;
            this.hWindow1.DisplayStatus = true;
            this.hWindow1.DisplayWindowNumber = false;
            this.hWindow1.DrawModel = false;
            this.hWindow1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hWindow1.Location = new System.Drawing.Point(4, 39);
            this.hWindow1.Margin = new System.Windows.Forms.Padding(4);
            this.hWindow1.MinZoom = false;
            this.hWindow1.Name = "hWindow1";
            this.hWindow1.PixelGrid = 30;
            this.hWindow1.Size = new System.Drawing.Size(359, 233);
            this.hWindow1.StopRefresh = false;
            this.hWindow1.TabIndex = 0;
            this.hWindow1.WindowNumber = 1;
            // 
            // uiTextBox9
            // 
            this.uiTextBox9.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox9.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTextBox9.Location = new System.Drawing.Point(625, 122);
            this.uiTextBox9.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox9.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox9.Name = "uiTextBox9";
            this.uiTextBox9.Padding = new System.Windows.Forms.Padding(5);
            this.uiTextBox9.ShowText = false;
            this.uiTextBox9.Size = new System.Drawing.Size(146, 29);
            this.uiTextBox9.TabIndex = 42;
            this.uiTextBox9.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox9.Watermark = "";
            // 
            // uiLabel13
            // 
            this.uiLabel13.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel13.Location = new System.Drawing.Point(567, 124);
            this.uiLabel13.Name = "uiLabel13";
            this.uiLabel13.Size = new System.Drawing.Size(73, 23);
            this.uiLabel13.TabIndex = 41;
            this.uiLabel13.Text = "SN:";
            this.uiLabel13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiTextBox10
            // 
            this.uiTextBox10.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox10.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTextBox10.Location = new System.Drawing.Point(625, 87);
            this.uiTextBox10.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox10.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox10.Name = "uiTextBox10";
            this.uiTextBox10.Padding = new System.Windows.Forms.Padding(5);
            this.uiTextBox10.ShowText = false;
            this.uiTextBox10.Size = new System.Drawing.Size(146, 29);
            this.uiTextBox10.TabIndex = 40;
            this.uiTextBox10.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox10.Watermark = "";
            // 
            // uiLabel14
            // 
            this.uiLabel14.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel14.Location = new System.Drawing.Point(566, 89);
            this.uiLabel14.Name = "uiLabel14";
            this.uiLabel14.Size = new System.Drawing.Size(73, 23);
            this.uiLabel14.TabIndex = 39;
            this.uiLabel14.Text = "工程名:";
            this.uiLabel14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1534, 913);
            this.Controls.Add(this.uiTextBox9);
            this.Controls.Add(this.uiLabel13);
            this.Controls.Add(this.uiTextBox10);
            this.Controls.Add(this.uiLabel14);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.uiLabel11);
            this.Controls.Add(this.uiLabel10);
            this.Controls.Add(this.uiDataGridView1);
            this.Controls.Add(this.uiTextBox7);
            this.Controls.Add(this.uiLabel8);
            this.Controls.Add(this.uiTextBox8);
            this.Controls.Add(this.uiLabel9);
            this.Controls.Add(this.uiTextBox6);
            this.Controls.Add(this.uiLabel7);
            this.Controls.Add(this.uiTextBox5);
            this.Controls.Add(this.uiLabel6);
            this.Controls.Add(this.uiTextBox4);
            this.Controls.Add(this.uiLabel5);
            this.Controls.Add(this.uiTextBox3);
            this.Controls.Add(this.uiLabel4);
            this.Controls.Add(this.uiTextBox2);
            this.Controls.Add(this.uiLabel3);
            this.Controls.Add(this.uiTextBox1);
            this.Controls.Add(this.uiLabel2);
            this.Controls.Add(this.uiProcessBar1);
            this.Controls.Add(this.uiTitlePanel2);
            this.Controls.Add(this.uiTitlePanel1);
            this.Controls.Add(this.uiLabel1);
            this.Controls.Add(this.uiButton2);
            this.Controls.Add(this.uiButton1);
            this.Controls.Add(this.uiNavBar1);
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowTitleIcon = true;
            this.Text = "华阳流媒体测试程序";
            this.ZoomScaleRect = new System.Drawing.Rectangle(26, 26, 1684, 1086);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.uiTitlePanel1.ResumeLayout(false);
            this.uiTitlePanel2.ResumeLayout(false);
            this.uiContextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uiDataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.uiTitlePanel5.ResumeLayout(false);
            this.uiTitlePanel4.ResumeLayout(false);
            this.uiTitlePanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UINavBar uiNavBar1;
        private Sunny.UI.UIButton uiButton1;
        private Sunny.UI.UIButton uiButton2;
        private Sunny.UI.UILabel uiLabel1;
        private Sunny.UI.UITitlePanel uiTitlePanel1;
        private Sunny.UI.UITitlePanel uiTitlePanel2;
        private Sunny.UI.UIContextMenuStrip uiContextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 添加ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除ToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private Sunny.UI.UIProcessBar uiProcessBar1;
        private Sunny.UI.UILabel uiLabel2;
        private Sunny.UI.UITextBox uiTextBox1;
        private Sunny.UI.UITextBox uiTextBox2;
        private Sunny.UI.UILabel uiLabel3;
        private Sunny.UI.UILabel uiLabel4;
        private Sunny.UI.UITextBox uiTextBox3;
        private Sunny.UI.UITextBox uiTextBox4;
        private Sunny.UI.UILabel uiLabel5;
        private Sunny.UI.UITextBox uiTextBox5;
        private Sunny.UI.UILabel uiLabel6;
        private Sunny.UI.UITextBox uiTextBox6;
        private Sunny.UI.UILabel uiLabel7;
        private Sunny.UI.UITextBox uiTextBox7;
        private Sunny.UI.UILabel uiLabel8;
        private Sunny.UI.UITextBox uiTextBox8;
        private Sunny.UI.UILabel uiLabel9;
        private Sunny.UI.UIDataGridView uiDataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn 名称;
        private System.Windows.Forms.DataGridViewTextBoxColumn 测试项目;
        private System.Windows.Forms.DataGridViewTextBoxColumn 前提条件;
        private System.Windows.Forms.DataGridViewTextBoxColumn 操作步骤;
        private System.Windows.Forms.DataGridViewTextBoxColumn 期望结果;
        private System.Windows.Forms.DataGridViewTextBoxColumn 实际结果;
        private System.Windows.Forms.DataGridViewTextBoxColumn 结果描述;
        private System.Windows.Forms.DataGridViewTextBoxColumn 测试时间;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private Sunny.UI.UILabel uiLabel10;
        private Sunny.UI.UILabel uiLabel11;
        private System.Windows.Forms.ToolStripMenuItem 执行一次ToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private Sunny.UI.UITitlePanel uiTitlePanel5;
        private hWindowTool.HWindow hWindow3;
        private Sunny.UI.UITitlePanel uiTitlePanel4;
        private hWindowTool.HWindow hWindow2;
        private Sunny.UI.UITitlePanel uiTitlePanel3;
        private hWindowTool.HWindow hWindow1;
        private Sunny.UI.UITextBox uiTextBox9;
        private Sunny.UI.UILabel uiLabel13;
        private Sunny.UI.UITextBox uiTextBox10;
        private Sunny.UI.UILabel uiLabel14;
    }
}

