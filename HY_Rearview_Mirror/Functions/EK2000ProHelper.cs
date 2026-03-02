using ChOpticsDriver;
using Helper;
using Helper.Motion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sunny.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HY_Rearview_Mirror.Functions
{
    public class EK2000ProHelper
    {
        public string name = "";//光谱仪名称
        public string sn = "";//光谱仪探测器序列号
        public int pixelCount = 0;//数组像素点数
        public double[] WaveArrry = null;//波长数组
        public double[] DarkArray = null;//暗噪声光谱数组
        public double[] SampleArray = null;//样品光谱数组
        public double[] ReferenceArray = null;//参考光谱数组
        public Wrapper wrapper = null;
        public EK2000ProHelper()
        {
                wrapper = new Wrapper();
                wrapper.OpenAllSpectrometers();//打开所有光谱仪
                name = wrapper.GetSpectrometerName(0);//获取光谱仪名称
                sn = wrapper.GetSerialNumberFromSpectrometer(0);//获取探测器序列号
                string darkFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DarkArray.json");
                if (File.Exists(darkFilePath))
                {
                    string jsonString = File.ReadAllText(darkFilePath);
                    DarkArray = JsonConvert.DeserializeObject< double[]> (jsonString);
                // DarkArray = JsonConvert.SerializeObject<double[]>(jsonString);
            }
                string referenceFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReferenceArray.json");
                if (File.Exists(referenceFilePath))
                {
                    string jsonString = File.ReadAllText(referenceFilePath);
                    ReferenceArray = JsonConvert.DeserializeObject< double[]> (jsonString);
                    //ReferenceArray = System.Text.Json.JsonSerializer.Deserialize<double[]>(jsonString);
                }
                string waveFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WaveArray.json");
                if (File.Exists(waveFilePath))
                {
                    string jsonString = File.ReadAllText(waveFilePath);
                    WaveArrry = JsonConvert.DeserializeObject<double[]>(jsonString);
                //WaveArrry = System.Text.Json.JsonSerializer.Deserialize<double[]>(jsonString);
                }
        }

        /// <summary>
        /// 设置积分时间(建议软件初始化写入,实例化wrap类会默认写回100)
        /// </summary>
        /// <param name="Time"></param>
        public void SetIntegrationTimeToSpectrometer(int Time)
        {
            wrapper.SetIntegrationTimeToSpectrometer(0, Time);          
        }

        /// <summary>
        /// 设置暗噪声
        /// </summary>
        public void SetDarkSpectrumArray()
        {
            pixelCount = wrapper.GetNumberOfPixelsOfSpectrometer(0);
            DarkArray = new double[pixelCount];
            Array.Reverse(DarkArray);
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DarkArray.json");

            string jsonString = JsonConvert.SerializeObject(DarkArray, Formatting.Indented);
            //string jsonString = System.Text.Json.JsonSerializer.Serialize(DarkArray, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
        }

        /// <summary>
        /// 获取波长数组
        /// </summary>
        public void GetWaveArray()
        {
            pixelCount = wrapper.GetNumberOfPixelsOfSpectrometer(0);
            WaveArrry = new double[pixelCount];
            WaveArrry = wrapper.GetCorrectionWavelengthOfSpectrometer(0);
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WaveArray.json");

            string jsonStr = JsonConvert.SerializeObject(WaveArrry, Formatting.Indented);
            //string jsonStr = System.Text.Json.JsonSerializer.Serialize(WaveArrry, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonStr);
        }

        /// <summary>
        /// 设置参考光谱数组
        /// </summary>
        public void SetReferenceArray()
        {
            pixelCount = wrapper.GetNumberOfPixelsOfSpectrometer(0);
            ReferenceArray = new double[pixelCount];
            ReferenceArray = wrapper.GetSpectrumFromSpectrometer(0);
            Array.Reverse(ReferenceArray);
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReferenceArray.json");
            string jsonString = JsonConvert.SerializeObject(ReferenceArray, Formatting.Indented);
            //string jsonString = System.Text.Json.JsonSerializer.Serialize(ReferenceArray, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
        }

        /// <summary>
        /// 获取样品光谱数组
        /// </summary>
        public void SetSampleArray()
        {
            pixelCount = wrapper.GetNumberOfPixelsOfSpectrometer(0);
            SampleArray = new double[pixelCount];
            SampleArray = wrapper.GetSpectrumFromSpectrometer(0);
            Array.Reverse(SampleArray);
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SampleArray.json");
            string jsonString = JsonConvert.SerializeObject(SampleArray, Formatting.Indented);
            //string jsonString = System.Text.Json.JsonSerializer.Serialize(SampleArray, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
        }

        
        public double CalcResult(int val)
        {
            int index= Array.IndexOf(WaveArrry, WaveArrry.OrderBy(n => Math.Abs(n - val)).First()) ;
            double rst = SampleArray[index] - DarkArray[index] / ReferenceArray[index] - DarkArray[index];
            return rst;
        }
    }
}
