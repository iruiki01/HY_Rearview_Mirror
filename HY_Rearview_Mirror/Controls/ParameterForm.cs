using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HY_Rearview_Mirror.Controls
{
    public partial class ParameterForm : UIForm
    {
        public ParameterForm()
        {
            InitializeComponent();

            uiComboBox1.DataBindings.Add("Text", AppConfig.GetInstance()._config, "IsRunType", true, DataSourceUpdateMode.OnPropertyChanged);
        }
        /// <summary>
        /// 放行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton4_Click(object sender, EventArgs e)
        {
            WorkThread.GetInstance().workStation.Run_Flag[(int)WorkThreadStep.测试] = false;
            Thread.Sleep(2000);
            WorkThread.GetInstance().workStation.Run_Step[(int)WorkThreadStep.测试] = 16;
            WorkThread.GetInstance().workStation.Run_Flag[(int)WorkThreadStep.测试] = true;
        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            WorkThread.GetInstance().workStation.Run_Flag[(int)WorkThreadStep.测试] = false;
            Thread.Sleep(2000);
            global.inovanceH5UTcpTool.Write("D112", 1);
            global.writeLogProduce.Info("给PLC发送重测, D112写入1！");
            WorkThread.GetInstance().workStation.Run_Step[(int)WorkThreadStep.测试] = 10;
            WorkThread.GetInstance().workStation.Run_Flag[(int)WorkThreadStep.测试] = true;
        }

        private void uiComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppConfig.GetInstance().SaveConfig();
        }
    }
}
