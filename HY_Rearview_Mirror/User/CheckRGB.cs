using HalconDotNet;
using HY_Rearview_Mirror.Functions;
using HY_Rearview_Mirror.Functions.Colors;
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
    public class CheckRGBTestParam : ITestParam
    {
        /// <summary>
        /// ROI区域
        /// </summary>
        //public HRegion region { get; set; }

        /// <summary>
        /// 运行结果
        /// </summary>
        public bool RunResult { get; set; }

        public int minThreshold { get; set; }
        public int maxThreshold { get; set; }
        public double minArea { get; set; }
        /// <summary>
        /// 相机编号
        /// </summary>
        public string CamId { get; set; }
        /// <summary>
        /// 要判断的颜色
        /// </summary>
        public string Color {  get; set; }

        public CheckRGBTestParam()
        {
            RunResult = false;
            minThreshold = 134;
            maxThreshold = 255;
            minArea = 100000;
            CamId = "相机1";
            Color = "红色";
        }
    }
    public class CheckRGB : IPlugin
    {
        public string PluginId => "颜色检测";

        public string moduleName => "视觉";

        public string DescribeMessage => "用于颜色检测";

        public string VersionStr1 = "";
        public string VersionStr => VersionStr1;

        [JsonIgnore]
        public CheckRGBForm checkRGBForm = null;
        public CheckRGBTestParam checkRGBTestParam = new CheckRGBTestParam();
        [JsonIgnore]
        private HImage image = null;
        private bool WaitNewCamMatFlag = false;
        [JsonIgnore]
        /// <summary>
        /// 当前操作图像
        /// </summary>
        private HImage g_hImage;
        private ManualResetEventSlim _flagEvent;
        [JsonIgnore]
        private ColorData ColorData = new ColorData();

        public CheckRGB()
        {
            MainForm.SendHImagetHandler += GetImage0;
            _flagEvent = new ManualResetEventSlim(false);
        }

        public Form CreateUI(ITestParam testParam, string VersionStr = "")
        {
            checkRGBForm = new CheckRGBForm();

            checkRGBForm.eventManager.AddListener("Blob分析", new Func<HImage, int, int, double, bool, List<BlobResult>>(ProcessRedScreenImage));
            checkRGBForm.eventManager.AddListener("发送界面参数", new Action<List<object>>(SetDataForm_OrderCompleted));
            checkRGBForm.eventManager.AddListener("关闭窗体", new Action(FormClosing));
            checkRGBForm.eventManager.AddListener("运行", new Func<Dictionary<string, object>>(Run));
            checkRGBForm.eventManager.AddListener("拉条", new Action<HImage, int, int>(Threshold));

            checkRGBForm.SetDataToForm(testParam, VersionStr);
            if (testParam != null)
                checkRGBTestParam = testParam as CheckRGBTestParam;
            return checkRGBForm;
        }

        public void Dispose()
        {
            MainForm.SendHImagetHandler -= GetImage0;
            GC.SuppressFinalize(this);
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

        public ITestParam GetData()
        {
            return checkRGBTestParam;
        }

        public string GetVersion()
        {
            return VersionStr;
        }

        public void FormClosing()
        {
            checkRGBForm.eventManager.Clear();
        }
        /// <summary>
        /// 获取界面发送过来的数据
        /// </summary>
        /// <param name="e"></param>
        private void SetDataForm_OrderCompleted(List<object> e)
        {
            VersionStr1 = Convert.ToString(e[0]);                                              // 描述
            checkRGBTestParam.minThreshold = Convert.ToInt32(e[1]);           // 拉条1
            checkRGBTestParam.maxThreshold = Convert.ToInt32(e[2]);          // 拉条2
            checkRGBTestParam.minArea = Convert.ToInt32(e[3]);                   // 最小面积
            checkRGBTestParam.CamId = e[4].ToString();                                  // 选择相机
            checkRGBTestParam.Color = e[5].ToString();                                    // 颜色
        }
        public void Initialize()
        {

        }

        public Dictionary<string, object> Run(ITestParam testParam)
        {
            CheckRGBTestParam _checkRGBTestParam = testParam as CheckRGBTestParam;

            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();


            // 重置事件
            _flagEvent.Reset();

            WaitNewCamMatFlag = true;
            global.cameraController.CamDeviceTriggerOnce(_checkRGBTestParam.CamId);

            // 等待信号（带超时）
            bool signaled = _flagEvent.Wait(5000); // 等待5秒
            if (signaled)
            {
                image = g_hImage.Clone();

                var result = ProcessRedScreenImage(image, _checkRGBTestParam.minThreshold, _checkRGBTestParam.maxThreshold, _checkRGBTestParam.minArea, true);

                if (ColorData.Result == _checkRGBTestParam.Color)
                {
                    keyValuePairs.Add("描述", "颜色检测合格,颜色为->" + ColorData.Result);
                    keyValuePairs.Add("结果", true);
                }
                else
                {
                    keyValuePairs.Add("描述", "颜色检测不合格,颜色为->" + ColorData.Result);
                    keyValuePairs.Add("结果", false);
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
            global.cameraController.CamDeviceTriggerOnce(checkRGBTestParam.CamId);

            // 等待信号（带超时）
            bool signaled = _flagEvent.Wait(5000); // 等待5秒
            if (signaled)
            {
                image = g_hImage.Clone();

                var result = ProcessRedScreenImage(image, checkRGBTestParam.minThreshold, checkRGBTestParam.maxThreshold, checkRGBTestParam.minArea, true);

                if (ColorData.Result == checkRGBTestParam.Color)
                {
                    keyValuePairs.Add("描述", "颜色检测合格,颜色为->" + ColorData.Result);
                    keyValuePairs.Add("结果", true);
                }
                else
                {
                    keyValuePairs.Add("描述", "颜色检测不合格,颜色为->" + ColorData.Result);
                    keyValuePairs.Add("结果", false);
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
        /// 斑点分析
        /// </summary>
        /// <param name="image"></param>
        /// <param name="minThreshold"></param>
        /// <param name="maxThreshold"></param>
        /// <param name="minArea"></param>
        /// <returns></returns>
        public List<BlobResult> ProcessRedScreenImage(HImage image, int minThreshold = 60, int maxThreshold = 255, double minArea = 100, bool userMura = false)
        {
            List<BlobResult> results = new List<BlobResult>();

            if (checkRGBForm != null)
            {
                checkRGBForm.ContourDatas.Clear();
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
                // 面积筛选              
                HOperatorSet.SelectShape(ConnectedRegions, out selectedRegions, "area", "and", minArea, 9999999);

                if (!userMura)
                {
                    if (checkRGBForm != null)
                    {
                        checkRGBForm.ContourDatas.Clear();
                        checkRGBForm.ContourDatas.Add("Blob", selectedRegions.Clone());
                        checkRGBForm.g_hWindow1.BindingContourDatas(checkRGBForm.ContourDatas);
                    }
                }

                // 5. 获取区域属性
                HTuple areas, rows, cols, circularities;
                HOperatorSet.AreaCenter(selectedRegions, out areas, out rows, out cols);
                HOperatorSet.Circularity(selectedRegions, out circularities);

                // 6. 构建结果
                HTuple count;
                HOperatorSet.CountObj(selectedRegions, out count);

                if (count == 0)
                {
                    results.Add(new BlobResult
                    {
                        Index = -1,
                        Area = -1,
                        CenterY = -1,
                        CenterX = -1,
                        Circularity = -1,
                        IsMuraFlag = false
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

                if (userMura)
                {
                    HImage image1 = new HImage(ho_ImagePart);
                    // 保存图像
                    image1.WriteImage("bmp", 0, "D://output.bmp");
                    // 颜色提取
                    try
                    {
                        // 2. 区域平均（最高效，推荐）
                        HObject region;
                        HOperatorSet.GenCircle(out region, 200, 300, 50);
                        var mean = global.rGB.GetRegionMean(image1, region);
                        Console.WriteLine($"区域平均: R={mean.R:F2}, G={mean.G:F2}, B={mean.B:F2}");

                        string closest = global.rgbColorMatcher.FindClosestColor(mean.R, mean.G, mean.B, out double dist);
                        Console.WriteLine($"最接近: {closest}, 距离: {dist:F2}");

                        ColorData.Result = closest;
                    }
                    finally
                    {
                        image1?.Dispose();
                    }

                }
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
            checkRGBTestParam.minThreshold = minGray;
            checkRGBTestParam.maxThreshold = maxGray;
            // 参数验证
            if (image == null || !image.IsInitialized())
                return;

            if (minGray < 0 || maxGray > 255 || minGray > maxGray)
                return;

            try
            {
                // 使用明确的HalconDotNet命名空间
                HObject region;
                HOperatorSet.Threshold(image, out region, checkRGBTestParam.minThreshold, checkRGBTestParam.maxThreshold);

                // 方法1：使用out参数
                int width, height;
                image.GetImageSize(out width, out height);

                HImage binaryImage = ConvertRegionToBinImage(new HRegion(region), width, height);

                checkRGBForm.g_hWindow1.HobjectToHimage(binaryImage);
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
    }
}

public class ColorData
{
    public string Result { get; set; }
}
