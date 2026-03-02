using HalconDotNet;
using Sunny.UI.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY_Rearview_Mirror.Functions
{
    /// <summary>
    /// Mura缺陷检测类
    /// 用于检测模糊图像中的Mura（不均匀）缺陷
    /// </summary>
    public class MuraDefectDetector : IDisposable
    {
        // 检测参数
        public double ScaleFactor { get; set; } = 0.4;           // 降采样因子
        public double Sigma { get; set; } = 3.0;                 // 高斯平滑Sigma（手动设置）
        public double LowThreshold { get; set; } = 2.0;          // 低阈值
        public double HighThreshold { get; set; } = 5.0;         // 高阈值
        public int ErosionSize { get; set; } = 7;                // 边界腐蚀尺寸
        public double BackgroundSigma { get; set; } = 100;       // 背景估计平滑度
        public double SubGain { get; set; } = 2;                 // 减背景增益
        public double SubOffset { get; set; } = 100;             // 减背景偏移

        private bool isInitialized = false;


        // Sigma = 3.0,        // 大sigma检测粗线
        // LowThreshold = 2.0,
        // HighThreshold = 5.0,

        /// <summary>
        /// 初始化检测器
        /// </summary>
        public void Initialize()
        {
            // 参数校验
            if (Sigma <= 0) throw new ArgumentException("Sigma必须大于0");
            if (LowThreshold >= HighThreshold)
                throw new ArgumentException("LowThreshold必须小于HighThreshold");

            isInitialized = true;
        }
        /// <summary>
        /// 检测单张图像的Mura缺陷
        /// </summary>
        public MuraDetectResult Detect(string imagePath)
        {
            if (!isInitialized) Initialize();

            HObject image = null;
            try
            {
                HOperatorSet.ReadImage(out image, imagePath);
                return Detect(image);
            }
            finally
            {
                image?.Dispose();
            }
        }
        /// <summary>
        /// 检测单张图像的Mura缺陷（HObject版本）
        /// </summary>
        public MuraDetectResult Detect(HObject image)
        {
            if (!isInitialized) Initialize();

            // 声明所有Halcon对象（用于finally释放）
            HObject r = null, g = null, b = null;
            HObject imageFFT = null, imageGauss = null, imageConvol = null, imageFFT1 = null;
            HObject imageSub = null, imageZoomed = null, domain = null, regionErosion = null;
            HObject imageReduced = null, lines = null, defects = null;

            HTuple width = new HTuple(), height = new HTuple();
            HTuple homMat2DIdentity = new HTuple(), homMat2DScale = new HTuple();

            try
            {
                // 1. 获取图像尺寸
                HOperatorSet.GetImageSize(image, out width, out height);

                // 2. 分离RGB通道，使用蓝色通道
                HOperatorSet.Decompose3(image, out r, out g, out b);

                // 3. 频域背景光照校正
                // 3.1 RFT转频域（实数快速傅里叶变换）
                HOperatorSet.RftGeneric(b, out imageFFT, "to_freq", "none", "complex", width);

                // 3.2 生成低通高斯滤波器（提取低频背景）
                HOperatorSet.GenGaussFilter(out imageGauss, BackgroundSigma, BackgroundSigma,
                    0, "n", "rft", width, height);

                // 3.3 频域卷积得到背景
                HOperatorSet.ConvolFft(imageFFT, imageGauss, out imageConvol);
                HOperatorSet.RftGeneric(imageConvol, out imageFFT1, "from_freq", "none", "byte", width);

                // 3.4 背景减除：原始图像 - 估计的背景
                HOperatorSet.SubImage(b, imageFFT1, out imageSub, SubGain, SubOffset);

                // 4. 降采样加速处理（双线性插值）
                HOperatorSet.ZoomImageFactor(imageSub, out imageZoomed,
                    ScaleFactor, ScaleFactor, "constant");

                // 5. 边界处理（避免lines_gauss边界效应）
                HOperatorSet.GetDomain(imageZoomed, out domain);
                HOperatorSet.ErosionRectangle1(domain, out regionErosion, ErosionSize, ErosionSize);
                HOperatorSet.ReduceDomain(imageZoomed, regionErosion, out imageReduced);

                // 6. Steger算法线条检测（手动传入参数）
                // Sigma: 高斯平滑参数，越大线条越粗
                // Low/High: 滞后阈值，类似Canny边缘检测
                HOperatorSet.LinesGauss(imageReduced, out lines,
                    new HTuple(Sigma),
                    new HTuple(LowThreshold),
                    new HTuple(HighThreshold),
                    "dark",      // 检测暗线条
                    "true",      // 完整线条模式
                    "gaussian",  // 高斯线模型
                    "true");     // 提取线条宽度

                // 7. 坐标变换：缩放回原始尺寸
                HOperatorSet.HomMat2dIdentity(out homMat2DIdentity);
                HOperatorSet.HomMat2dScaleLocal(homMat2DIdentity,
                    1.0 / ScaleFactor, 1.0 / ScaleFactor, out homMat2DScale);
                HOperatorSet.AffineTransContourXld(lines, out defects, homMat2DScale);

                // 8. 提取结果信息
                var result = ExtractResultInfo(defects, width, height);

                // 复制轮廓返回（因为原defects会在finally中释放）
                if (result.IsDetected)
                {
                    result.DefectContours = defects.CopyObj(1, -1);
                }

                return result;
            }
            finally
            {
                // 清理所有中间对象（防止内存泄漏）
                r?.Dispose(); g?.Dispose(); b?.Dispose();
                imageFFT?.Dispose(); imageGauss?.Dispose();
                imageConvol?.Dispose(); imageFFT1?.Dispose();
                imageSub?.Dispose(); imageZoomed?.Dispose();
                domain?.Dispose(); regionErosion?.Dispose();
                imageReduced?.Dispose(); lines?.Dispose();
                defects?.Dispose();
            }
        }
        /// <summary>
        /// 提取缺陷几何信息
        /// </summary>
        private MuraDetectResult ExtractResultInfo(HObject defects, HTuple width, HTuple height)
        {
            var result = new MuraDetectResult
            {
                ImageWidth = width.I,
                ImageHeight = height.I,
                IsDetected = false,
                Defects = new List<DefectInfo>()
            };

            // 检查缺陷数量
            HTuple numLines = new HTuple();
            HOperatorSet.CountObj(defects, out numLines);

            if (numLines.I == 0) return result;

            result.IsDetected = true;
            result.DefectCount = numLines.I;

            // 遍历每条缺陷线
            for (int i = 1; i <= numLines.I; i++)
            {
                HObject line = null;
                try
                {
                    HOperatorSet.SelectObj(defects, out line, i);

                    // 获取轮廓点坐标
                    HTuple row = new HTuple(), col = new HTuple();
                    HOperatorSet.GetContourXld(line, out row, out col);

                    // 计算长度
                    HTuple length = new HTuple();
                    HOperatorSet.LengthXld(line, out length);

                    // 计算包围盒
                    HTuple row1 = new HTuple(), col1 = new HTuple(),
                           row2 = new HTuple(), col2 = new HTuple();
                    HOperatorSet.SmallestRectangle1Xld(line, out row1, out col1, out row2, out col2);

                    var defectInfo = new DefectInfo
                    {
                        Index = i,
                        Length = length.D,
                        StartPoint = new PointD(row[0].D, col[0].D),
                        EndPoint = new PointD(row[row.Length - 1].D, col[col.Length - 1].D),
                        PointCount = row.Length,
                        BoundingBox = new RectD(col1.D, row1.D, col2.D - col1.D, row2.D - row1.D)
                    };

                    result.Defects.Add(defectInfo);
                }
                finally
                {
                    line?.Dispose();
                }
            }

            return result;
        }

        /// <summary>
        /// 可视化检测结果
        /// </summary>
        public void DisplayResult(HObject originalImage, MuraDetectResult result, HTuple windowHandle)
        {
            // 设置显示参数
            HOperatorSet.SetColor(windowHandle, "red");
            HOperatorSet.SetDraw(windowHandle, "margin");
            HOperatorSet.SetLineWidth(windowHandle, 3);

            // 显示原图
            HOperatorSet.DispObj(originalImage, windowHandle);

            // 显示缺陷轮廓
            if (result.IsDetected && result.DefectContours != null)
            {
                HOperatorSet.DispObj(result.DefectContours, windowHandle);
            }
        }

        public void Dispose()
        {
            isInitialized = false;
        }
    }
}

/// <summary>
/// 检测结果数据类
/// </summary>
public class MuraDetectResult : IDisposable
{
    public bool IsDetected { get; set; }
    public int DefectCount { get; set; }
    public int ImageWidth { get; set; }
    public int ImageHeight { get; set; }
    public List<DefectInfo> Defects { get; set; }

    /// <summary>
    /// 缺陷轮廓（需要调用者Dispose）
    /// </summary>
    public HObject DefectContours { get; set; }

    public void Dispose()
    {
        DefectContours?.Dispose();
        DefectContours = null;
    }

    public override string ToString()
    {
        if (!IsDetected) return "未检测到Mura缺陷";
        return $"检测到 {DefectCount} 个缺陷区域";
    }
}

/// <summary>
/// 单个缺陷信息
/// </summary>
public class DefectInfo
{
    public int Index { get; set; }
    public double Length { get; set; }
    public PointD StartPoint { get; set; }
    public PointD EndPoint { get; set; }
    public int PointCount { get; set; }
    public RectD BoundingBox { get; set; }

    public override string ToString()
    {
        return $"缺陷#{Index}: 长度={Length:F2}px, 点数量={PointCount}";
    }
}

/// <summary>
/// 二维点
/// </summary>
public struct PointD
{
    public double X, Y;
    public PointD(double x, double y) { X = x; Y = y; }
    public override string ToString() => $"({X:F2}, {Y:F2})";
}

/// <summary>
/// 矩形
/// </summary>
public struct RectD
{
    public double X, Y, Width, Height;
    public RectD(double x, double y, double w, double h)
    {
        X = x; Y = y; Width = w; Height = h;
    }
    public override string ToString() => $"[{X:F2},{Y:F2},{Width:F2}x{Height:F2}]";
}

