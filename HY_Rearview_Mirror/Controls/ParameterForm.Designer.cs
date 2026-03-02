namespace HY_Rearview_Mirror.Controls
{
    partial class ParameterForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.uiTitlePanel6 = new Sunny.UI.UITitlePanel();
            this.uiButton4 = new Sunny.UI.UIButton();
            this.uiButton3 = new Sunny.UI.UIButton();
            this.uiComboBox1 = new Sunny.UI.UIComboBox();
            this.uiLabel12 = new Sunny.UI.UILabel();
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.uiComboBox2 = new Sunny.UI.UIComboBox();
            this.uiCheckBox1 = new Sunny.UI.UICheckBox();
            this.uiTitlePanel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiTitlePanel6
            // 
            this.uiTitlePanel6.Controls.Add(this.uiCheckBox1);
            this.uiTitlePanel6.Controls.Add(this.uiComboBox2);
            this.uiTitlePanel6.Controls.Add(this.uiLabel1);
            this.uiTitlePanel6.Controls.Add(this.uiButton4);
            this.uiTitlePanel6.Controls.Add(this.uiButton3);
            this.uiTitlePanel6.Controls.Add(this.uiComboBox1);
            this.uiTitlePanel6.Controls.Add(this.uiLabel12);
            this.uiTitlePanel6.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTitlePanel6.Location = new System.Drawing.Point(4, 40);
            this.uiTitlePanel6.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTitlePanel6.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiTitlePanel6.Name = "uiTitlePanel6";
            this.uiTitlePanel6.ShowText = false;
            this.uiTitlePanel6.Size = new System.Drawing.Size(417, 837);
            this.uiTitlePanel6.TabIndex = 38;
            this.uiTitlePanel6.Text = "功能区";
            this.uiTitlePanel6.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiButton4
            // 
            this.uiButton4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton4.Location = new System.Drawing.Point(92, 102);
            this.uiButton4.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton4.Name = "uiButton4";
            this.uiButton4.Size = new System.Drawing.Size(80, 35);
            this.uiButton4.TabIndex = 3;
            this.uiButton4.Text = "放行";
            this.uiButton4.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton4.Click += new System.EventHandler(this.uiButton4_Click);
            // 
            // uiButton3
            // 
            this.uiButton3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton3.Location = new System.Drawing.Point(6, 102);
            this.uiButton3.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton3.Name = "uiButton3";
            this.uiButton3.Size = new System.Drawing.Size(80, 35);
            this.uiButton3.TabIndex = 2;
            this.uiButton3.Text = "重测";
            this.uiButton3.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton3.Click += new System.EventHandler(this.uiButton3_Click);
            // 
            // uiComboBox1
            // 
            this.uiComboBox1.DataSource = null;
            this.uiComboBox1.FillColor = System.Drawing.Color.White;
            this.uiComboBox1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiComboBox1.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.uiComboBox1.Items.AddRange(new object[] {
            "不重测",
            "自动重测",
            "手动重测"});
            this.uiComboBox1.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiComboBox1.Location = new System.Drawing.Point(93, 50);
            this.uiComboBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiComboBox1.MinimumSize = new System.Drawing.Size(63, 0);
            this.uiComboBox1.Name = "uiComboBox1";
            this.uiComboBox1.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.uiComboBox1.Size = new System.Drawing.Size(318, 29);
            this.uiComboBox1.SymbolSize = 24;
            this.uiComboBox1.TabIndex = 1;
            this.uiComboBox1.Text = "不重测";
            this.uiComboBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiComboBox1.Watermark = "";
            this.uiComboBox1.SelectedIndexChanged += new System.EventHandler(this.uiComboBox1_SelectedIndexChanged);
            // 
            // uiLabel12
            // 
            this.uiLabel12.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel12.Location = new System.Drawing.Point(3, 54);
            this.uiLabel12.Name = "uiLabel12";
            this.uiLabel12.Size = new System.Drawing.Size(98, 23);
            this.uiLabel12.TabIndex = 0;
            this.uiLabel12.Text = "是否重测：";
            this.uiLabel12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiLabel1
            // 
            this.uiLabel1.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel1.Location = new System.Drawing.Point(3, 191);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(113, 23);
            this.uiLabel1.TabIndex = 4;
            this.uiLabel1.Text = "单项重测次数：";
            this.uiLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiComboBox2
            // 
            this.uiComboBox2.DataSource = null;
            this.uiComboBox2.FillColor = System.Drawing.Color.White;
            this.uiComboBox2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiComboBox2.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.uiComboBox2.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.uiComboBox2.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiComboBox2.Location = new System.Drawing.Point(110, 183);
            this.uiComboBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiComboBox2.MinimumSize = new System.Drawing.Size(63, 0);
            this.uiComboBox2.Name = "uiComboBox2";
            this.uiComboBox2.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.uiComboBox2.Size = new System.Drawing.Size(301, 40);
            this.uiComboBox2.SymbolSize = 24;
            this.uiComboBox2.TabIndex = 5;
            this.uiComboBox2.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiComboBox2.Watermark = "";
            // 
            // uiCheckBox1
            // 
            this.uiCheckBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiCheckBox1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiCheckBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiCheckBox1.Location = new System.Drawing.Point(6, 263);
            this.uiCheckBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiCheckBox1.Name = "uiCheckBox1";
            this.uiCheckBox1.Size = new System.Drawing.Size(150, 29);
            this.uiCheckBox1.TabIndex = 6;
            this.uiCheckBox1.Text = "RFID功能屏蔽";
            // 
            // ParameterForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1445, 882);
            this.Controls.Add(this.uiTitlePanel6);
            this.MaximizeBox = false;
            this.Name = "ParameterForm";
            this.Text = "参数设置";
            this.TitleFont = new System.Drawing.Font("Noto Sans SC", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ZoomScaleRect = new System.Drawing.Rectangle(19, 19, 1413, 838);
            this.uiTitlePanel6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITitlePanel uiTitlePanel6;
        private Sunny.UI.UIButton uiButton4;
        private Sunny.UI.UIButton uiButton3;
        private Sunny.UI.UIComboBox uiComboBox1;
        private Sunny.UI.UILabel uiLabel12;
        private Sunny.UI.UILabel uiLabel1;
        private Sunny.UI.UIComboBox uiComboBox2;
        private Sunny.UI.UICheckBox uiCheckBox1;
    }
}