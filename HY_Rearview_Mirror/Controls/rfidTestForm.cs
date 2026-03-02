using Newtonsoft.Json;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HY_Rearview_Mirror.Controls
{
    public partial class rfidTestForm : UIForm
    {
        public rfidTestForm()
        {
            InitializeComponent();
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            // 判断上个工位的RFID的数据
            var value = global.rFIDHelper.Read();
            global.writeLogProduce.Info("读取到上工位的RFID数据" + value.ToString());

            RFIDData rFIDData = JsonConvert.DeserializeObject<RFIDData>(value, new JsonSerializerSettings() { Formatting = Formatting.None });
            //RFIDData rFIDData = JsonConvert.DeserializeObject<RFIDData>(value);
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            RFIDData m_rFIDData = new RFIDData();
            m_rFIDData.StationName = "12345";
            m_rFIDData.SN = "dsefdsgv";
            //m_rFIDData.datas.Add(new Data() { Name = "qweee", Message = "qwerq" });
            //m_rFIDData.datas.Add(new Data() { Name = "qweee", Message = "qwerq" });
            //m_rFIDData.datas.Add(new Data() { Name = "qweee", Message = "qwerq" });
            //m_rFIDData.datas.Add(new Data() { Name = "qweee", Message = "qwerq" });


            string json = JsonConvert.SerializeObject(m_rFIDData, Formatting.Indented);
            global.rFIDHelper.Write(json);
            global.writeLogProduce.Info("写RFID->" + json);
        }
    }
}
