using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HKMvCameraControlAPP
{  
    public partial class Form3 : Form
    {
        
        public Form3()
        {
            InitializeComponent();

            this.FormClosed += Form3_FormClosed;
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            GYCamTool.GetInstance().CloseCam();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            GYCamTool.GetInstance().InitCam();
            GYCamTool.GetInstance().ConnectCam();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str = TextBox_Expo.Text;
            string str1 = textBox_Gain.Text;
            string str2 = textBox_XExpo.Text;
            string str3 = textBox_YExpo.Text;
            string str4 = textBox_ZExpo.Text;
            GYCamTool.GetInstance().SetExpoGain(str, str1, str2, str3, str4);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var list =  GYCamTool.GetInstance().GetDeviceSingleFrameAutoExp();
            dataGridView1.DataSource = list;
        }
    }
}
