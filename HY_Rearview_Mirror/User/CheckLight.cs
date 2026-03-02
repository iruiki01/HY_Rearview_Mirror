using HalconDotNet;
using HY_Rearview_Mirror.InterfaceTool;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HY_Rearview_Mirror.User
{
    public class CheckLightTestParam : ITestParam
    {
        /// <summary>
        /// ROI区域
        /// </summary>
        //public HRegion region { get; set; }

        /// <summary>
        /// 运行结果
        /// </summary>
        public bool RunResult { get; set; }

        /// <summary>
        /// 亮度阈值
        /// </summary>
        public double deviation { get; set; }

        public int minThreshold { get; set; }
        public int maxThreshold { get; set; }
        public double minArea { get; set; }
        /// <summary>
        /// 相机编号
        /// </summary>
        public string CamId { get; set; }
        /// <summary>
        /// 判断是否是否要输出亮度值
        /// </summary>
        public string IsLightOutPut { get; set; }

        public CheckLightTestParam()
        {
            RunResult = false;
            minThreshold = 134;
            maxThreshold = 255;
            minArea = 100;
        }
    }
    /// <summary>
    /// 检测屏幕亮度
    /// </summary>
    public class CheckLight : IPlugin
    {
        public string PluginId => "检测屏幕亮度";

        public string moduleName => "视觉";

        public string DescribeMessage => "用于检测屏幕亮度变化";

        public string VersionStr1 = "";
        public string VersionStr => VersionStr1;

        [JsonIgnore]
        public CheckLightSetForm checkLightSetForm = null;
        public CheckLightTestParam checkLightTestParam = new CheckLightTestParam();
        [JsonIgnore]
        private HImage image = null;
        private double g_deviation = 0;

        private bool WaitNewCamMatFlag = false;

        [JsonIgnore]
        /// <summary>
        /// 当前操作图像
        /// </summary>
        private HImage g_hImage;
        private ManualResetEventSlim _flagEvent;

        public CheckLight()
        {
            MainForm.SendHImagetHandler += GetImage0;
            _flagEvent = new ManualResetEventSlim(false);
        }
        public Form CreateUI(ITestParam testParam, string VersionStr = "")
        {
            checkLightSetForm = new CheckLightSetForm();

            checkLightSetForm.eventManager.AddListener("Blob分析", new Func<HImage, int, int, double, List<BlobResult>>(ProcessRedScreenImage));
            checkLightSetForm.eventManager.AddListener("发送界面参数", new Action<List<object>>(SetDataForm_OrderCompleted));
            checkLightSetForm.eventManager.AddListener("关闭窗体", new Action(FormClosing));
            checkLightSetForm.eventManager.AddListener("运行", new Func<Dictionary<string, object>>(Run));
            checkLightSetForm.eventManager.AddListener("拉条", new Action<HImage, int, int>(Threshold));

            checkLightSetForm.SetDataToForm(testParam, VersionStr);
            if (testParam != null)
                checkLightTestParam = testParam as CheckLightTestParam;

            return checkLightSetForm;
        }
        public void FormClosing()
        {
            checkLightSetForm.eventManager.Clear();
        }
        public void Dispose()
        {
            MainForm.SendHImagetHandler -= GetImage0;
            GC.SuppressFinalize(this);
        }

        public ITestParam GetData()
        {
            return checkLightTestParam;
        }

        public string GetVersion()
        {
            return VersionStr;
        }

        public void Initialize()
        {

        }
        private void GetImage0(object sender, HImage img)
        {
            if (WaitNewCamMatFlag)
            {
                WaitNewCamMatFlag = false;
                g_hImage = img.Clone();
                img?.Dispose();

                _flagEvent.Set();
            }
        }

        /// <summary>
        /// 斑点分析
        /// </summary>
        /// <param name="image"></param>
        /// <param name="minThreshold"></param>
        /// <param name="maxThreshold"></param>
        /// <param name="minArea"></param>
        /// <returns></returns>
        public List<BlobResult> ProcessRedScreenImage(HImage image, int minThreshold = 60, int maxThreshold = 255, double minArea = 100)
        {
            List<BlobResult> results = new List<BlobResult>();

            if(checkLightSetForm != null)
            {
                checkLightSetForm.ContourDatas.Clear();
            }
            
            HTuple Information = new HTuple();
            HObject Region = null;
            HObject ConnectedRegions = null;
            HTuple NumConnected = new HTuple();
            HObject ConnectedRegionsMax = null;
            HTuple NumConnectedMax = new HTuple();
            HObject selectedRegions = null;
            try
            {
                // 获取并保存当前系统设置
                HOperatorSet.GetSystem("max_connection", out Information);

                // 设置 max_connection 为 0
                HOperatorSet.SetSystem("max_connection", 0);

                // 阈值处理
                HOperatorSet.Threshold(image, out Region, minThreshold, maxThreshold);

                // 连通域分析（使用 max_connection = 0）
                HOperatorSet.Connection(Region, out ConnectedRegions);
                HOperatorSet.CountObj(ConnectedRegions, out NumConnected);
                Console.WriteLine($"连通域数量（max_connection=0）: {NumConnected}");

                // 设置 max_connection 为 1000
                HOperatorSet.SetSystem("max_connection", 1000);

                // 重新进行连通域分析（使用 max_connection = 1000）
                HOperatorSet.Connection(Region, out ConnectedRegionsMax);
                HOperatorSet.CountObj(ConnectedRegionsMax, out NumConnectedMax);
                Console.WriteLine($"连通域数量（max_connection=1000）: {NumConnectedMax}");
                // 4. 面积筛选              
                HOperatorSet.SelectShape(ConnectedRegions, out selectedRegions, "area", "and", minArea, 9999999);

                if (checkLightSetForm != null)
                {
                    checkLightSetForm.ContourDatas.Add("Blob", selectedRegions.Clone());
                    checkLightSetForm.g_hWindow1.BindingContourDatas(checkLightSetForm.ContourDatas);
                }

                // 5. 获取区域属性
                HTuple areas, rows, cols, circularities;
                HOperatorSet.AreaCenter(selectedRegions, out areas, out rows, out cols);
                HOperatorSet.Circularity(selectedRegions, out circularities);

                // 6. 构建结果
                HTuple count;
                HOperatorSet.CountObj(selectedRegions, out count);

                if(count == 0)
                {
                    g_deviation = -1;
                    results.Add(new BlobResult
                    {
                        Index = -1,
                        Area = -1,
                        CenterY = -1,
                        CenterX = -1,
                        Circularity = -1
                    });
                    return results;
                }    

                for (int i = 0; i < count; i++)
                {
                    results.Add(new BlobResult
                    {
                        Index = i,
                        Area = areas[i].D,
                        CenterY = rows[i].D,
                        CenterX = cols[i].D,
                        Circularity = circularities[i].D
                    });
                }

                // 恢复原始系统设置
                HOperatorSet.SetSystem("max_connection", Information);

                HObject ho_Mask = null, ho_ImagePart = null;
                // 初始化Halcon对象
                HOperatorSet.GenEmptyObj(out ho_Mask);
                HOperatorSet.GenEmptyObj(out ho_ImagePart);
                // 从原图裁剪指定区域
                HOperatorSet.ReduceDomain(image, selectedRegions, out ho_Mask);
                HOperatorSet.CropDomain(ho_Mask, out ho_ImagePart);

                //checkLightSetForm.g_hWindow1.HobjectToHimage(ho_ImagePart);

                DetectBrightnessInRegion(ho_ImagePart, out g_deviation);


                return results;
            }
            catch (HOperatorException ex)
            {
                Console.WriteLine($"Halcon操作错误: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"一般错误: {ex.Message}");
                throw;
            }
            finally
            {
                // 清理资源
                ConnectedRegionsMax?.Dispose();
                ConnectedRegions?.Dispose();
                Region?.Dispose();
                image?.Dispose();
                selectedRegions?.Dispose();
            }
        }

        private static void Cleanup(params HObject[] objects)
        {
            foreach (var obj in objects)
            {
                if (obj != null && obj.IsInitialized())
                {
                    obj.Dispose();
                }
            }
        }

        /// <summary>
        /// 检测指定区域的亮度
        /// </summary>
        public double DetectBrightnessInRegion(HObject image, out double deviation)
        {
            HTuple mean = new HTuple();
            HTuple dev = new HTuple();

            try
            {
                HObject region1 = null;
                HOperatorSet.GetDomain(image, out region1);
                HOperatorSet.Intensity(region1, image, out mean, out dev);

                deviation = dev.D;
                return mean.D;
            }
            catch (HalconException hex)
            {
                throw new Exception($"Halcon检测亮度失败: {hex.Message}", hex);
            }
        }

        public Dictionary<string, object> Run(ITestParam testParam)
        {
            var p = (CheckLightTestParam)testParam;

            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();

            // 重置事件
            _flagEvent.Reset();

            WaitNewCamMatFlag = true;
            global.cameraController.CamDeviceTriggerOnce(checkLightTestParam.CamId);

            // 等待信号（带超时）
            bool signaled = _flagEvent.Wait(5000); // 等待5秒
            if (signaled)
            {
                image = g_hImage;

                ProcessRedScreenImage(image, p.minThreshold, p.maxThreshold, p.minArea);

                if (p.IsLightOutPut == "亮度值1")
                {
                    Context.GetInstance().LightValue1 = Convert.ToString(g_deviation);

                    keyValuePairs.Add("亮度", g_deviation);
                    keyValuePairs.Add("结果", true);
                    keyValuePairs.Add("描述","亮度值1设置成功");
                }
                else if (p.IsLightOutPut == "亮度值2")
                {
                    Context.GetInstance().LightValue2 = Convert.ToString(g_deviation);
                    keyValuePairs.Add("亮度", g_deviation);
                    keyValuePairs.Add("结果", true);
                    keyValuePairs.Add("描述", "亮度值2设置成功");
                }
                else
                {
                    if (g_deviation > checkLightTestParam.deviation)
                    {
                        keyValuePairs.Add("亮度", g_deviation);
                        keyValuePairs.Add("亮度阈值", checkLightTestParam.deviation);
                        keyValuePairs.Add("结果", true);
                    }
                    else
                    {
                        keyValuePairs.Add("亮度", g_deviation);
                        keyValuePairs.Add("亮度阈值", checkLightTestParam.deviation);
                        keyValuePairs.Add("结果", false);
                    }
                }        
            }
            else
            {
                keyValuePairs.Add("描述", "采图超时！");
                keyValuePairs.Add("结果", false);
            }    
            return keyValuePairs;
        }
        public Dictionary<string, object> Run()
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();

            // 重置事件
            _flagEvent.Reset();

            WaitNewCamMatFlag = true;
            global.cameraController.CamDeviceTriggerOnce(checkLightTestParam.CamId);

            // 等待信号（带超时）
            bool signaled = _flagEvent.Wait(5000); // 等待5秒
            if (signaled)
            {
                image = g_hImage;

                ProcessRedScreenImage(image, checkLightTestParam.minThreshold, checkLightTestParam.maxThreshold, checkLightTestParam.minArea);

                if (checkLightTestParam.IsLightOutPut == "亮度值1")
                {
                    Context.GetInstance().LightValue1 = Convert.ToString(g_deviation);

                    keyValuePairs.Add("亮度", g_deviation);
                    keyValuePairs.Add("结果", true);
                    keyValuePairs.Add("描述", "亮度值1设置成功");
                }
                else if (checkLightTestParam.IsLightOutPut == "亮度值2")
                {
                    Context.GetInstance().LightValue2 = Convert.ToString(g_deviation);

                    keyValuePairs.Add("亮度", g_deviation);
                    keyValuePairs.Add("结果", true);
                    keyValuePairs.Add("描述", "亮度值2设置成功");
                }
                else
                {
                    if (g_deviation > checkLightTestParam.deviation)
                    {
                        keyValuePairs.Add("亮度", g_deviation);
                        keyValuePairs.Add("亮度阈值", checkLightTestParam.deviation);
                        keyValuePairs.Add("结果", true);
                    }
                    else
                    {
                        keyValuePairs.Add("亮度", g_deviation);
                        keyValuePairs.Add("亮度阈值", checkLightTestParam.deviation);
                        keyValuePairs.Add("结果", false);
                    }
                }  
            }
            else
            {
                keyValuePairs.Add("描述", "采图超时！");
                keyValuePairs.Add("结果", false);
            }
            return keyValuePairs;
        }
        /// <summary>
        /// 二值化
        /// </summary>
        /// <param name="image"></param>
        /// <param name="minGray"></param>
        /// <param name="maxGray"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="Exception"></exception>
        public void Threshold(HImage image, int minGray = 0, int maxGray = 255)
        {
            checkLightTestParam.minThreshold = minGray;
            checkLightTestParam.maxThreshold = maxGray;
            // 参数验证
            if (image == null || !image.IsInitialized())
                return;

            if (minGray < 0 || maxGray > 255 || minGray > maxGray)
                return;

            try
            {
                // 使用明确的HalconDotNet命名空间
                HObject region;
                HOperatorSet.Threshold(image, out region, checkLightTestParam.minThreshold, checkLightTestParam.maxThreshold);

                // 方法1：使用out参数
                int width, height;
                image.GetImageSize(out width, out height);

                HImage binaryImage = ConvertRegionToBinImage(new HRegion(region), width, height);

                checkLightSetForm.g_hWindow1.HobjectToHimage(binaryImage);
            }
            catch (HalconException ex)
            {
                throw new Exception($"二值化失败: {ex.Message}", ex);
            }
            finally
            {
                image.Dispose();
            }
        }

        // 将Region转换为二值图像
        public HImage ConvertRegionToBinImage(HRegion region, int width, int height)
        {
            // 参数说明：前景灰度值，背景灰度值，图像宽度，图像高度
            return region.RegionToBin(255, 0, width, height);
        }

        private void SetDataForm_OrderCompleted(List<object> e)
        {
            VersionStr1 = Convert.ToString(e[0]);

            checkLightTestParam.minThreshold = Convert.ToInt32(e[1]);
            checkLightTestParam.maxThreshold = Convert.ToInt32(e[2]);
            checkLightTestParam.minArea = Convert.ToInt32(e[3]);
            checkLightTestParam.deviation = Convert.ToInt32(e[4]);
            checkLightTestParam.CamId = e[5].ToString();
            checkLightTestParam.IsLightOutPut = e[6].ToString();
        }
    }
    /// <summary>
    /// Blob分析结果类
    /// </summary>
    public class BlobResult
    {
        public int Index { get; set; }
        public double Area { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double Circularity { get; set; }
        public bool IsMuraFlag {  get; set; }
    }
}
