using HalconDotNet;
using HY_Rearview_Mirror.Functions;
using HY_Rearview_Mirror.InterfaceTool;
using Newtonsoft.Json;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.LinkLabel;

namespace HY_Rearview_Mirror.User
{
    public class MuraFunTestParam : ITestParam
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

        public MuraFunTestParam()
        {
            RunResult = false;
            minThreshold = 134;
            maxThreshold = 255;
            minArea = 1000000;
            CamId = "相机1";
        }
    }
    public class MuraFun : IPlugin
    {
        public string PluginId => "Mura检测";

        public string moduleName => "视觉";

        public string DescribeMessage => "用于Mura检测";

        public string VersionStr1 = "";
        public string VersionStr => VersionStr1;

        [JsonIgnore]
        public MuraSetForm muraSetForm = null;
        public MuraFunTestParam muraFunTestParam = new MuraFunTestParam();
        [JsonIgnore]
        private HImage image = null;
        private bool WaitNewCamMatFlag = false;
        [JsonIgnore]
        /// <summary>
        /// 当前操作图像
        /// </summary>
        private HImage g_hImage;
        private ManualResetEventSlim _flagEvent;
        /// <summary>
        /// Mura检测
        /// </summary>
        private MuraDefectDetector muraDefectDetector = new MuraDefectDetector();

        public MuraFun()
        {
            MainForm.SendHImagetHandler += GetImage0;
            _flagEvent = new ManualResetEventSlim(false);
        }

        public Form CreateUI(ITestParam testParam, string VersionStr = "")
        {
            muraSetForm = new MuraSetForm();

            muraSetForm.eventManager.AddListener("Blob分析", new Func<HImage, int, int, double,bool, List<BlobResult>>(ProcessRedScreenImage));
            muraSetForm.eventManager.AddListener("发送界面参数", new Action<List<object>>(SetDataForm_OrderCompleted));
            muraSetForm.eventManager.AddListener("关闭窗体", new Action(FormClosing));
            muraSetForm.eventManager.AddListener("运行", new Func<Dictionary<string, object>>(Run));
            muraSetForm.eventManager.AddListener("拉条", new Action<HImage, int, int>(Threshold));

            muraSetForm.SetDataToForm(testParam, VersionStr);
            if (testParam != null)
                muraFunTestParam = testParam as MuraFunTestParam;
            return muraSetForm;
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
            return muraFunTestParam;
        }

        public string GetVersion()
        {
            return VersionStr;
        }

        public void Initialize()
        {
            
        }

        /// <summary>
        /// 斑点分析
        /// </summary>
        /// <param name="image"></param>
        /// <param name="minThreshold"></param>
        /// <param name="maxThreshold"></param>
        /// <param name="minArea"></param>
        /// <returns></returns>
        public List<BlobResult> ProcessRedScreenImage(HImage image, int minThreshold = 60, int maxThreshold = 255, double minArea = 100,bool userMura = false)
        {
            List<BlobResult> results = new List<BlobResult>();

            if (muraSetForm != null)
            {
                muraSetForm.ContourDatas.Clear();
                muraSetForm.g_hWindow1.BindingContourDatas(muraSetForm.ContourDatas);
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
                    if (muraSetForm != null)
                    {
                        muraSetForm.ContourDatas.Clear();
                        muraSetForm.ContourDatas.Add("Blob", selectedRegions.Clone());
                        muraSetForm.g_hWindow1.BindingContourDatas(muraSetForm.ContourDatas);
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


                // 步骤 1：获取 selectedRegions 的包围盒（原图坐标）
                HTuple row1, col1, row2, col2;
                HOperatorSet.SmallestRectangle1(selectedRegions, out row1, out col1, out row2, out col2);

                double offsetRow = row1.D;  // 裁剪区域左上角 Row（原图坐标）
                double offsetCol = col1.D;  // 裁剪区域左上角 Col（原图坐标）


                // 从原图裁剪指定区域
                HOperatorSet.ReduceDomain(image, selectedRegions, out ho_Mask);
                HOperatorSet.CropDomain(ho_Mask, out ho_ImagePart);

                if(userMura)
                {
                    HImage image1 = new HImage(ho_ImagePart);

                    // 获取裁剪后的图像大小
                    HTuple cropW, cropH;
                    HOperatorSet.GetImageSize(image1, out cropW, out cropH);
                    Console.WriteLine($"裁剪后图像大小: {cropW} x {cropH}");

                    // 保存图像
                    image1.WriteImage("bmp", 0, "D://output.bmp");
                    // Mura检测
                    var value = muraDefectDetector.Detect(image1);

                    if (muraSetForm != null)
                    {
                        if("未检测到Mura缺陷" != value.ToString())
                        {
                            muraSetForm.ContourDatas.Clear();

                            // 验证：检查缺陷坐标范围（应该在 0~cropH, 0~cropW 之间）
                            HObject firstDefect;
                            HOperatorSet.SelectObj(value.DefectContours, out firstDefect, 1);
                            HTuple r, c;
                            HOperatorSet.GetContourXld(firstDefect, out r, out c);
                            Console.WriteLine($"Mura缺陷[0]坐标: ({r[0]:F2}, {c[0]:F2})");
                            Console.WriteLine($"  裁剪图范围: 0-{cropH}, 0-{cropW}");
                            firstDefect.Dispose();

                            HTuple homMat2D;
                            HOperatorSet.HomMat2dIdentity(out homMat2D);

                            // 直接平移到原图位置
                            HOperatorSet.HomMat2dTranslate(homMat2D, offsetRow, offsetCol, out homMat2D);

                            HObject DefectContoursTransformed = null;
                            HOperatorSet.GenEmptyObj(out DefectContoursTransformed);

                            HOperatorSet.AffineTransContourXld(
                                value.DefectContours,           // 输入：裁剪图坐标（0~500）
                                out DefectContoursTransformed,  // 输出：原图坐标（offset~offset+500）
                                homMat2D                        // 只有平移，没有缩放
                            );

                            // 验证变换结果
                            //HObject firstTransformed;
                            //HOperatorSet.SelectObj(DefectContoursTransformed, out firstTransformed, 1);
                            //HTuple rt, ct;
                            //HOperatorSet.GetContourXld(firstTransformed, out rt, out ct);
                            //Console.WriteLine($"变换后坐标: ({rt[0]:F2}, {ct[0]:F2})");
                            //Console.WriteLine($"预期坐标: ({r[0] + offsetRow:F2}, {c[0] + offsetCol:F2})");
                            //firstTransformed.Dispose();

                            muraSetForm.ContourDatas.Add("Blob", DefectContoursTransformed);


                            //HImage hImage123 = new HImage(value.DefectContours.Clone());
                            //hImage123.WriteImage("bmp", 0, "D://output123.bmp");

                            muraSetForm.g_hWindow1.BindingContourDatas(muraSetForm.ContourDatas);
                        }                     
                    }

                    if (value.DefectCount > 0)
                    {
                        results.Clear();
                        for (int i = 0; i < count; i++)
                        {
                            results.Add(new BlobResult
                            {
                                Index = i,
                                Area = areas[i].D,
                                CenterY = rows[i].D,
                                CenterX = cols[i].D,
                                Circularity = circularities[i].D,
                                IsMuraFlag = true                                      //有缺陷
                            });
                        }
                    }
                    else
                    {
                        results.Clear();
                        for (int i = 0; i < count; i++)
                        {
                            results.Add(new BlobResult
                            {
                                Index = i,
                                Area = areas[i].D,
                                CenterY = rows[i].D,
                                CenterX = cols[i].D,
                                Circularity = circularities[i].D,
                                IsMuraFlag = false                                      //没有缺陷
                            });
                        }
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

        public void FormClosing()
        {
            muraSetForm.eventManager.Clear();
        }
        /// <summary>
        /// 获取界面发送过来的数据
        /// </summary>
        /// <param name="e"></param>
        private void SetDataForm_OrderCompleted(List<object> e)
        {
            VersionStr1 = Convert.ToString(e[0]);                                             // 描述
            muraFunTestParam.minThreshold = Convert.ToInt32(e[1]);           // 拉条1
            muraFunTestParam.maxThreshold = Convert.ToInt32(e[2]);          // 拉条2
            muraFunTestParam.minArea = Convert.ToInt32(e[3]);                   // 最小面积
            muraFunTestParam.CamId = e[4].ToString();                                  // 选择相机
        }

        public Dictionary<string, object> Run(ITestParam testParam)
        {
            MuraFunTestParam _muraFunTestParam = testParam as MuraFunTestParam;

            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Clear();

            // 重置事件
            _flagEvent.Reset();

            WaitNewCamMatFlag = true;
            global.cameraController.CamDeviceTriggerOnce(_muraFunTestParam.CamId);

            // 等待信号（带超时）
            bool signaled = _flagEvent.Wait(5000); // 等待5秒
            if (signaled)
            {
                image = g_hImage.Clone();

                var result = ProcessRedScreenImage(image, _muraFunTestParam.minThreshold, _muraFunTestParam.maxThreshold, _muraFunTestParam.minArea, true);
                if (result[0].IsMuraFlag)
                {
                    keyValuePairs.Add("描述", "检测到Mura缺陷！");
                    keyValuePairs.Add("结果", false);
                }
                else
                {
                    keyValuePairs.Add("描述", "未检测到Mura缺陷！");
                    keyValuePairs.Add("结果", true);
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
            global.cameraController.CamDeviceTriggerOnce(muraFunTestParam.CamId);

            // 等待信号（带超时）
            bool signaled = _flagEvent.Wait(5000); // 等待5秒
            if (signaled)
            {
                image = g_hImage.Clone();

                var result = ProcessRedScreenImage(image, muraFunTestParam.minThreshold, muraFunTestParam.maxThreshold, muraFunTestParam.minArea,true);
                if (result[0].IsMuraFlag)
                {
                    keyValuePairs.Add("描述", "检测到Mura缺陷！");
                    keyValuePairs.Add("结果", false);
                }
                else
                {
                    keyValuePairs.Add("描述", "未检测到Mura缺陷！");
                    keyValuePairs.Add("结果", true);
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
            muraFunTestParam.minThreshold = minGray;
            muraFunTestParam.maxThreshold = maxGray;
            // 参数验证
            if (image == null || !image.IsInitialized())
                return;

            if (minGray < 0 || maxGray > 255 || minGray > maxGray)
                return;

            try
            {
                // 使用明确的HalconDotNet命名空间
                HObject region;
                HOperatorSet.Threshold(image, out region, muraFunTestParam.minThreshold, muraFunTestParam.maxThreshold);

                // 方法1：使用out参数
                int width, height;
                image.GetImageSize(out width, out height);

                HImage binaryImage = ConvertRegionToBinImage(new HRegion(region), width, height);

                muraSetForm.g_hWindow1.HobjectToHimage(binaryImage);
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
