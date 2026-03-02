using HalconDotNet;
using HY_Rearview_Mirror.InterfaceTool;
using Newtonsoft.Json;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HY_Rearview_Mirror.User
{
    /// <summary>
    /// 缺陷检测
    /// </summary>
    public class CheckDefectFunTestParam : ITestParam
    {
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
        public List<string> Data_List { get; set; }

        public CheckDefectFunTestParam()
        {
            RunResult = false;
            minThreshold = 134;
            maxThreshold = 255;
            minArea = 100;
            CamId = "相机1";
            Data_List = new List<string>();
        }
    }
    public class CheckDefectFun : IPlugin
    {
        public string PluginId => "缺陷检测";

        public string moduleName => "视觉";

        public string DescribeMessage => "用于缺陷检测";

        public string VersionStr1 = "";
        public string VersionStr => VersionStr1;

        /// <summary>
        /// 等待来图标志
        /// </summary>
        private bool WaitNewCamMatFlag = false;
        [JsonIgnore]
        /// <summary>
        /// 当前操作图像
        /// </summary>
        private HImage g_hImage;
        private ManualResetEventSlim _flagEvent;
        [JsonIgnore]
        private HImage image = null;

        [JsonIgnore]
        public CheckDefectSetForm checkDefectSetForm = null;
        public CheckDefectFunTestParam checkDefectFunTestParam = new CheckDefectFunTestParam();

        public CheckDefectFun()
        {
            MainForm.SendHImagetHandler += GetImage0;
            _flagEvent = new ManualResetEventSlim(false);
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
        public Form CreateUI(ITestParam testParam, string VersionStr = "")
        {
            checkDefectSetForm = new CheckDefectSetForm();

            checkDefectSetForm.eventManager.AddListener("发送界面参数", new Action<List<object>>(SetDataForm_OrderCompleted));
            checkDefectSetForm.eventManager.AddListener("Blob分析", new Func<HImage, int, int, double, List<BlobResult>>(ProcessRedScreenImage));
            checkDefectSetForm.eventManager.AddListener("关闭窗体", new Action(FormClosing));
            checkDefectSetForm.eventManager.AddListener("运行", new Func<Dictionary<string, object>>(Run));
            checkDefectSetForm.eventManager.AddListener("拉条", new Action<HImage, int, int>(Threshold));

            checkDefectSetForm.SetDataToForm(testParam, VersionStr);
            if (testParam != null)
                checkDefectFunTestParam = testParam as CheckDefectFunTestParam;
            return checkDefectSetForm;
        }

        private void FormClosing()
        {
            checkDefectSetForm.eventManager.Clear();
        }

        private void SetDataForm_OrderCompleted(List<object> e)
        {
            VersionStr1 = Convert.ToString(e[0]);                                                                // 描述
            checkDefectFunTestParam.minThreshold = Convert.ToInt32(e[1]);                  // 拉条1
            checkDefectFunTestParam.maxThreshold = Convert.ToInt32(e[2]);                  // 拉条2
            checkDefectFunTestParam.minArea = Convert.ToInt32(e[3]);                           // 最小面积
            checkDefectFunTestParam.CamId = e[4].ToString();                                          // 相机编号         
            checkDefectFunTestParam.Data_List = (List<string>)e[5];
        }

        public void Dispose()
        {
            MainForm.SendHImagetHandler -= GetImage0;
            GC.SuppressFinalize(this);
        }

        public ITestParam GetData()
        {
            return checkDefectFunTestParam;
        }

        public string GetVersion()
        {
            return VersionStr;
        }

        public void Initialize()
        {
            
        }

        public Dictionary<string, object> Run(ITestParam testParam)
        {
            var p = (CheckDefectFunTestParam)testParam;

            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();

            // 重置事件
            _flagEvent.Reset();

            WaitNewCamMatFlag = true;
            global.cameraController.CamDeviceTriggerOnce(p.CamId);

            // 等待信号（带超时）
            bool signaled = _flagEvent.Wait(5000); // 等待5秒
            if (signaled)
            {
                image = g_hImage.Clone();

                List<BlobResult> results = ProcessRedScreenImage(image, p.minThreshold, p.maxThreshold, p.minArea);

                keyValuePairs.Add("描述", "不合格！");
                keyValuePairs.Add("结果", false);

                if (p.Data_List.Count > 0)
                {
                    foreach (BlobResult result in results)
                    {
                        if (result.Area >= (Convert.ToDouble(p.Data_List[0])) - Convert.ToDouble(p.Data_List[1]) &&
                            result.Area <= (Convert.ToDouble(p.Data_List[0])) + Convert.ToDouble(p.Data_List[2]))
                        {
                            keyValuePairs.Clear();
                            keyValuePairs.Add("描述", "合格！");
                            keyValuePairs.Add("结果", true);
                            break;
                        }
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
            global.cameraController.CamDeviceTriggerOnce(checkDefectFunTestParam.CamId);

            // 等待信号（带超时）
            bool signaled = _flagEvent.Wait(5000); // 等待5秒
            if (signaled)
            {
                image = g_hImage.Clone();

                List<BlobResult> results = ProcessRedScreenImage(image, checkDefectFunTestParam.minThreshold, checkDefectFunTestParam.maxThreshold, checkDefectFunTestParam.minArea);

                if (checkDefectFunTestParam.Data_List.Count > 0) 
                {
                    foreach (BlobResult result in results)
                    {
                        if (result.Area >= (Convert.ToDouble(checkDefectFunTestParam.Data_List[0])) - Convert.ToDouble(checkDefectFunTestParam.Data_List[1]) &&
                            result.Area <= (Convert.ToDouble(checkDefectFunTestParam.Data_List[0])) + Convert.ToDouble(checkDefectFunTestParam.Data_List[2]))
                        {
                            keyValuePairs.Clear();
                            keyValuePairs.Add("描述", "合格！");
                            keyValuePairs.Add("结果", true);
                            break;
                        }
                        else
                        {
                            keyValuePairs.Clear();
                            keyValuePairs.Add("描述", "不合格！");
                            keyValuePairs.Add("结果", false);
                            break;
                        }
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

            if (checkDefectSetForm != null)
            {
                checkDefectSetForm.ContourDatas.Clear();
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

                if (checkDefectSetForm != null)
                {
                    checkDefectSetForm.ContourDatas.Add("Blob", selectedRegions.Clone());
                    checkDefectSetForm.g_hWindow1.BindingContourDatas(checkDefectSetForm.ContourDatas);
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
            checkDefectFunTestParam.minThreshold = minGray;
            checkDefectFunTestParam.maxThreshold = maxGray;
            // 参数验证
            if (image == null || !image.IsInitialized())
                return;

            if (minGray < 0 || maxGray > 255 || minGray > maxGray)
                return;

            try
            {
                // 使用明确的HalconDotNet命名空间
                HObject region;
                HOperatorSet.Threshold(image, out region, checkDefectFunTestParam.minThreshold, checkDefectFunTestParam.maxThreshold);

                // 方法1：使用out参数
                int width, height;
                image.GetImageSize(out width, out height);

                HImage binaryImage = ConvertRegionToBinImage(new HRegion(region), width, height);

                checkDefectSetForm.g_hWindow1.HobjectToHimage(binaryImage);
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
