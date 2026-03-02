namespace HY_Rearview_Mirror.Controls
{
    partial class LogForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.uiTabControl1 = new Sunny.UI.UITabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.uiLabel3 = new Sunny.UI.UILabel();
            this.txtSearch = new Sunny.UI.UITextBox();
            this.uiButton1 = new Sunny.UI.UIButton();
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.uiDatetimePicker1 = new Sunny.UI.UIDatetimePicker();
            this.uiListBox1 = new Sunny.UI.UIListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.uiLabel4 = new Sunny.UI.UILabel();
            this.uiTextBox1 = new Sunny.UI.UITextBox();
            this.uiDataGridView1 = new Sunny.UI.UIDataGridView();
            this.uiButton2 = new Sunny.UI.UIButton();
            this.uiLabel2 = new Sunny.UI.UILabel();
            this.uiDatetimePicker2 = new Sunny.UI.UIDatetimePicker();
            this.uiTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uiDataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // uiTabControl1
            // 
            this.uiTabControl1.Controls.Add(this.tabPage1);
            this.uiTabControl1.Controls.Add(this.tabPage2);
            this.uiTabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.uiTabControl1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTabControl1.ItemSize = new System.Drawing.Size(150, 40);
            this.uiTabControl1.Location = new System.Drawing.Point(3, 38);
            this.uiTabControl1.MainPage = "";
            this.uiTabControl1.Name = "uiTabControl1";
            this.uiTabControl1.SelectedIndex = 0;
            this.uiTabControl1.Size = new System.Drawing.Size(1465, 869);
            this.uiTabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.uiTabControl1.TabIndex = 0;
            this.uiTabControl1.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.uiLabel3);
            this.tabPage1.Controls.Add(this.txtSearch);
            this.tabPage1.Controls.Add(this.uiButton1);
            this.tabPage1.Controls.Add(this.uiLabel1);
            this.tabPage1.Controls.Add(this.uiDatetimePicker1);
            this.tabPage1.Controls.Add(this.uiListBox1);
            this.tabPage1.Location = new System.Drawing.Point(0, 40);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(1465, 829);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "查看日志";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // uiLabel3
            // 
            this.uiLabel3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel3.Location = new System.Drawing.Point(552, 29);
            this.uiLabel3.Name = "uiLabel3";
            this.uiLabel3.Size = new System.Drawing.Size(180, 23);
            this.uiLabel3.TabIndex = 5;
            this.uiLabel3.Text = "输入要搜索的内容:";
            this.uiLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtSearch
            // 
            this.txtSearch.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSearch.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtSearch.Location = new System.Drawing.Point(732, 16);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSearch.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Padding = new System.Windows.Forms.Padding(5);
            this.txtSearch.ShowText = false;
            this.txtSearch.Size = new System.Drawing.Size(357, 50);
            this.txtSearch.TabIndex = 4;
            this.txtSearch.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtSearch.Watermark = "";
            // 
            // uiButton1
            // 
            this.uiButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton1.Font = new System.Drawing.Font("Noto Sans SC", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiButton1.Location = new System.Drawing.Point(406, 7);
            this.uiButton1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton1.Name = "uiButton1";
            this.uiButton1.Size = new System.Drawing.Size(100, 64);
            this.uiButton1.TabIndex = 3;
            this.uiButton1.Text = "查看";
            this.uiButton1.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton1.Click += new System.EventHandler(this.uiButton1_Click);
            // 
            // uiLabel1
            // 
            this.uiLabel1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel1.Location = new System.Drawing.Point(3, 29);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(180, 23);
            this.uiLabel1.TabIndex = 2;
            this.uiLabel1.Text = "选择要查看的日期:";
            this.uiLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiDatetimePicker1
            // 
            this.uiDatetimePicker1.DateFormat = "yyyy-MM-dd";
            this.uiDatetimePicker1.FillColor = System.Drawing.Color.White;
            this.uiDatetimePicker1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiDatetimePicker1.Location = new System.Drawing.Point(183, 16);
            this.uiDatetimePicker1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiDatetimePicker1.MaxLength = 10;
            this.uiDatetimePicker1.MinimumSize = new System.Drawing.Size(63, 0);
            this.uiDatetimePicker1.Name = "uiDatetimePicker1";
            this.uiDatetimePicker1.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.uiDatetimePicker1.Size = new System.Drawing.Size(200, 50);
            this.uiDatetimePicker1.SymbolDropDown = 61555;
            this.uiDatetimePicker1.SymbolNormal = 61555;
            this.uiDatetimePicker1.SymbolSize = 24;
            this.uiDatetimePicker1.TabIndex = 1;
            this.uiDatetimePicker1.Text = "2026-01-08";
            this.uiDatetimePicker1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiDatetimePicker1.Value = new System.DateTime(2026, 1, 8, 20, 12, 18, 368);
            this.uiDatetimePicker1.Watermark = "";
            this.uiDatetimePicker1.ValueChanged += new Sunny.UI.UIDatetimePicker.OnDateTimeChanged(this.uiDatetimePicker1_ValueChanged);
            // 
            // uiListBox1
            // 
            this.uiListBox1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiListBox1.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.uiListBox1.ItemSelectForeColor = System.Drawing.Color.White;
            this.uiListBox1.Location = new System.Drawing.Point(4, 76);
            this.uiListBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiListBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiListBox1.Name = "uiListBox1";
            this.uiListBox1.Padding = new System.Windows.Forms.Padding(2);
            this.uiListBox1.ShowText = false;
            this.uiListBox1.Size = new System.Drawing.Size(1460, 748);
            this.uiListBox1.TabIndex = 0;
            this.uiListBox1.Text = "uiListBox1";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.uiLabel4);
            this.tabPage2.Controls.Add(this.uiTextBox1);
            this.tabPage2.Controls.Add(this.uiDataGridView1);
            this.tabPage2.Controls.Add(this.uiButton2);
            this.tabPage2.Controls.Add(this.uiLabel2);
            this.tabPage2.Controls.Add(this.uiDatetimePicker2);
            this.tabPage2.Location = new System.Drawing.Point(0, 40);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(1465, 829);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "查看CSV文件";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // uiLabel4
            // 
            this.uiLabel4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel4.Location = new System.Drawing.Point(554, 28);
            this.uiLabel4.Name = "uiLabel4";
            this.uiLabel4.Size = new System.Drawing.Size(180, 23);
            this.uiLabel4.TabIndex = 10;
            this.uiLabel4.Text = "输入要搜索的内容:";
            this.uiLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiTextBox1
            // 
            this.uiTextBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTextBox1.Location = new System.Drawing.Point(734, 15);
            this.uiTextBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox1.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox1.Name = "uiTextBox1";
            this.uiTextBox1.Padding = new System.Windows.Forms.Padding(5);
            this.uiTextBox1.ShowText = false;
            this.uiTextBox1.Size = new System.Drawing.Size(357, 50);
            this.uiTextBox1.TabIndex = 9;
            this.uiTextBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox1.Watermark = "";
            // 
            // uiDataGridView1
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiDataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
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
            this.uiDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
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
            this.uiDataGridView1.Location = new System.Drawing.Point(6, 73);
            this.uiDataGridView1.Name = "uiDataGridView1";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.uiDataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.uiDataGridView1.RowHeadersWidth = 51;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiDataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.uiDataGridView1.RowTemplate.Height = 27;
            this.uiDataGridView1.SelectedIndex = -1;
            this.uiDataGridView1.Size = new System.Drawing.Size(1456, 753);
            this.uiDataGridView1.StripeOddColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiDataGridView1.TabIndex = 8;
            // 
            // uiButton2
            // 
            this.uiButton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton2.Font = new System.Drawing.Font("Noto Sans SC", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiButton2.Location = new System.Drawing.Point(405, 6);
            this.uiButton2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton2.Name = "uiButton2";
            this.uiButton2.Size = new System.Drawing.Size(100, 64);
            this.uiButton2.TabIndex = 7;
            this.uiButton2.Text = "查看";
            this.uiButton2.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // uiLabel2
            // 
            this.uiLabel2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel2.Location = new System.Drawing.Point(2, 28);
            this.uiLabel2.Name = "uiLabel2";
            this.uiLabel2.Size = new System.Drawing.Size(180, 23);
            this.uiLabel2.TabIndex = 6;
            this.uiLabel2.Text = "选择要查看的日期:";
            this.uiLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiDatetimePicker2
            // 
            this.uiDatetimePicker2.FillColor = System.Drawing.Color.White;
            this.uiDatetimePicker2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiDatetimePicker2.Location = new System.Drawing.Point(182, 15);
            this.uiDatetimePicker2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiDatetimePicker2.MaxLength = 19;
            this.uiDatetimePicker2.MinimumSize = new System.Drawing.Size(63, 0);
            this.uiDatetimePicker2.Name = "uiDatetimePicker2";
            this.uiDatetimePicker2.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.uiDatetimePicker2.Size = new System.Drawing.Size(200, 50);
            this.uiDatetimePicker2.SymbolDropDown = 61555;
            this.uiDatetimePicker2.SymbolNormal = 61555;
            this.uiDatetimePicker2.SymbolSize = 24;
            this.uiDatetimePicker2.TabIndex = 5;
            this.uiDatetimePicker2.Text = "2026-01-08 20:12:18";
            this.uiDatetimePicker2.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiDatetimePicker2.Value = new System.DateTime(2026, 1, 8, 20, 12, 18, 0);
            this.uiDatetimePicker2.Watermark = "";
            // 
            // LogForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1471, 910);
            this.Controls.Add(this.uiTabControl1);
            this.MaximizeBox = false;
            this.Name = "LogForm";
            this.Text = "日志";
            this.ZoomScaleRect = new System.Drawing.Rectangle(19, 19, 1288, 785);
            this.Load += new System.EventHandler(this.LogForm_Load);
            this.uiTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uiDataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITabControl uiTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private Sunny.UI.UIListBox uiListBox1;
        private Sunny.UI.UIDatetimePicker uiDatetimePicker1;
        private Sunny.UI.UILabel uiLabel1;
        private Sunny.UI.UIButton uiButton1;
        private Sunny.UI.UIDataGridView uiDataGridView1;
        private Sunny.UI.UIButton uiButton2;
        private Sunny.UI.UILabel uiLabel2;
        private Sunny.UI.UIDatetimePicker uiDatetimePicker2;
        private Sunny.UI.UITextBox txtSearch;
        private Sunny.UI.UILabel uiLabel3;
        private Sunny.UI.UILabel uiLabel4;
        private Sunny.UI.UITextBox uiTextBox1;
    }
}