namespace HKMvCameraControlAPP
{
    partial class Form3
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
            this.button1 = new System.Windows.Forms.Button();
            this.textBox_Gain = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.TextBox_Expo = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_ZExpo = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.textBox_YExpo = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.textBox_XExpo = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.gyCamControl1 = new HKMvCameraControlAPP.GYCamControl();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Noto Sans SC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(89, 730);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 41);
            this.button1.TabIndex = 1;
            this.button1.Text = "设置参数";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox_Gain
            // 
            this.textBox_Gain.Font = new System.Drawing.Font("Noto Sans SC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_Gain.Location = new System.Drawing.Point(92, 557);
            this.textBox_Gain.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_Gain.Name = "textBox_Gain";
            this.textBox_Gain.Size = new System.Drawing.Size(90, 29);
            this.textBox_Gain.TabIndex = 11;
            this.textBox_Gain.Text = "0";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Noto Sans SC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label21.Location = new System.Drawing.Point(37, 564);
            this.label21.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(55, 21);
            this.label21.TabIndex = 10;
            this.label21.Text = "增益：";
            // 
            // TextBox_Expo
            // 
            this.TextBox_Expo.Font = new System.Drawing.Font("Noto Sans SC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TextBox_Expo.Location = new System.Drawing.Point(92, 519);
            this.TextBox_Expo.Margin = new System.Windows.Forms.Padding(4);
            this.TextBox_Expo.Name = "TextBox_Expo";
            this.TextBox_Expo.Size = new System.Drawing.Size(90, 29);
            this.TextBox_Expo.TabIndex = 9;
            this.TextBox_Expo.Text = "50000";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Noto Sans SC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(9, 523);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 21);
            this.label7.TabIndex = 8;
            this.label7.Text = "曝光(us)：";
            // 
            // textBox_ZExpo
            // 
            this.textBox_ZExpo.Font = new System.Drawing.Font("Noto Sans SC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_ZExpo.Location = new System.Drawing.Point(92, 673);
            this.textBox_ZExpo.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_ZExpo.Name = "textBox_ZExpo";
            this.textBox_ZExpo.Size = new System.Drawing.Size(90, 29);
            this.textBox_ZExpo.TabIndex = 27;
            this.textBox_ZExpo.Text = "50000";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Noto Sans SC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label25.Location = new System.Drawing.Point(9, 680);
            this.label25.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(90, 21);
            this.label25.TabIndex = 26;
            this.label25.Text = "Z曝光(us)：";
            // 
            // textBox_YExpo
            // 
            this.textBox_YExpo.Font = new System.Drawing.Font("Noto Sans SC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_YExpo.Location = new System.Drawing.Point(92, 635);
            this.textBox_YExpo.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_YExpo.Name = "textBox_YExpo";
            this.textBox_YExpo.Size = new System.Drawing.Size(90, 29);
            this.textBox_YExpo.TabIndex = 25;
            this.textBox_YExpo.Text = "50000";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Noto Sans SC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label23.Location = new System.Drawing.Point(9, 642);
            this.label23.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(89, 21);
            this.label23.TabIndex = 24;
            this.label23.Text = "Y曝光(us)：";
            // 
            // textBox_XExpo
            // 
            this.textBox_XExpo.Font = new System.Drawing.Font("Noto Sans SC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_XExpo.Location = new System.Drawing.Point(92, 597);
            this.textBox_XExpo.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_XExpo.Name = "textBox_XExpo";
            this.textBox_XExpo.Size = new System.Drawing.Size(90, 29);
            this.textBox_XExpo.TabIndex = 23;
            this.textBox_XExpo.Text = "50000";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Noto Sans SC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label24.Location = new System.Drawing.Point(9, 601);
            this.label24.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(90, 21);
            this.label24.TabIndex = 22;
            this.label24.Text = "X曝光(us)：";
            // 
            // dataGridView1
            // 
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.GridColor = System.Drawing.Color.White;
            this.dataGridView1.Location = new System.Drawing.Point(190, 519);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(360, 252);
            this.dataGridView1.TabIndex = 28;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Noto Sans SC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(558, 519);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(93, 41);
            this.button2.TabIndex = 29;
            this.button2.Text = "获取亮色度";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Noto Sans SC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(558, 568);
            this.button3.Margin = new System.Windows.Forms.Padding(4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(93, 41);
            this.button3.TabIndex = 30;
            this.button3.Text = "实时图像";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // gyCamControl1
            // 
            this.gyCamControl1.Location = new System.Drawing.Point(10, 14);
            this.gyCamControl1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.gyCamControl1.Name = "gyCamControl1";
            this.gyCamControl1.Size = new System.Drawing.Size(639, 495);
            this.gyCamControl1.TabIndex = 0;
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(659, 776);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.textBox_ZExpo);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.textBox_YExpo);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.textBox_XExpo);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.textBox_Gain);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.TextBox_Expo);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.gyCamControl1);
            this.Font = new System.Drawing.Font("Noto Sans SC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.MaximizeBox = false;
            this.Name = "Form3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "成像色度仪";
            this.Load += new System.EventHandler(this.Form3_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GYCamControl gyCamControl1;
        private System.Windows.Forms.Button button1;
        internal System.Windows.Forms.TextBox textBox_Gain;
        internal System.Windows.Forms.Label label21;
        internal System.Windows.Forms.TextBox TextBox_Expo;
        internal System.Windows.Forms.Label label7;
        internal System.Windows.Forms.TextBox textBox_ZExpo;
        internal System.Windows.Forms.Label label25;
        internal System.Windows.Forms.TextBox textBox_YExpo;
        internal System.Windows.Forms.Label label23;
        internal System.Windows.Forms.TextBox textBox_XExpo;
        internal System.Windows.Forms.Label label24;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}