using HalconDotNet;
using hWindowTool;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY_Rearview_Mirror.Functions
{
    public class HalconShapeMatcher
    {
        [JsonIgnore]
        /// <summary>
        /// 输入图像
        /// </summary>
        public HImage inputImage;

        [JsonIgnore]
        /// <summary>
        /// 匹配结果
        /// </summary>
        public MatchResult1 matchResult1 = new MatchResult1();

        /// <summary>
        /// 模板轮廓显示
        /// </summary>
        public Dictionary<string, HObject> RoiContourDatas = new Dictionary<string, HObject>();
        /// <summary>
        /// 执行匹配时保存轮廓
        /// </summary>
        public Dictionary<string, HObject> ContourDatas = new Dictionary<string, HObject>();

        public HTuple _modelID = new HTuple();
        public HObject _modelContours = new HObject();

        public double minScore = 0.5;

        /// <summary>
        /// 创建形状模板（需手动提供ROI区域）
        /// </summary>
        /// <param name="templateRegion">手动绘制的ROI区域</param>
        /// <param name="scaleMin">最小缩放比例</param>
        /// <param name="scaleMax">最大缩放比例</param>
        public bool CreateModel(HRegion hRegion, double scaleMin = 0.8, double scaleMax = 1.2)
        {
            HObject reducedImage;
            //HOperatorSet.ReadImage(out image, imagePath);
            HOperatorSet.ReduceDomain(inputImage, hRegion, out reducedImage);
            try
            {
                // 使用手动提供的ROI区域创建模板:ml-citation{ref="3,5" data="citationList"}
                HOperatorSet.CreateScaledShapeModel(
                    reducedImage,                  // 手动ROI区域
                    "auto",                          // 自动金字塔层级
                    -3.14, 6.28,                    // -180°~360°旋转范围:ml-citation{ref="1,4" data="citationList"}
                    "auto",                          // 自动角度步进
                    scaleMin, scaleMax,             // 缩放范围
                    "auto", "auto",                 // 优化参数
                    "use_polarity",                 // 对比度处理:ml-citation{ref="6" data="citationList"}
                    "auto", "auto",                 // 参数自动化
                     out _modelID
                );
                // 2. 设置参考点为ROI中心（解决轮廓偏移问题）
                HOperatorSet.AreaCenter(reducedImage, out _, out HTuple refRow, out HTuple refCol);
                HOperatorSet.SetShapeModelOrigin(_modelID, -refRow, -refCol);
                // 获取模板轮廓用于可视化:ml-citation{ref="1,4" data="citationList"}
                HOperatorSet.GetShapeModelContours(out _modelContours, _modelID, 1);
                RoiContourDatas.Clear();
                RoiContourDatas.Add("ROI0", _modelContours);
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// 执行形状匹配
        /// </summary>
        /// <param name="searchImage">待检测图像</param>
        /// <param name="minScore">最低匹配分数阈值</param>
        /// <returns>匹配结果集合</returns>
        public bool FindShape()
        {
            //HObject searchImage;
            //HOperatorSet.ReadImage(out searchImage, imagePath);

            HObject grayImage = null;
            HTuple hv_hvParams = new HTuple();
            try
            {
                // 灰度化处理提升稳定性:ml-citation{ref="1,6" data="citationList"}
                HOperatorSet.Rgb1ToGray(inputImage, out grayImage);

                hv_hvParams = new HTuple();
                hv_hvParams[0] = "least_squares";
                hv_hvParams[1] = "max_deformation 2";

                HTuple row, column, angle, scale, score;
                HOperatorSet.FindScaledShapeModel(
                    grayImage,                      // 搜索图像
                    _modelID,                       // 模板ID
                    -3.14, 6.28,                   // 旋转范围
                    scaleMin: 0.6, scaleMax: 1.5,  // 缩放范围
                    minScore,                       // 最低匹配分数:ml-citation{ref="4,7" data="citationList"}
                    1,                             // 最大匹配数量
                    0.5,                           // 重叠阈值
                    hv_hvParams,               // 亚像素优化方法:ml-citation{ref="4" data="citationList"}
                    3,                             // 金字塔层级
                    0.9,                           // 贪婪度
                    out row, out column,
                    out angle, out scale, out score
                );

                if (matchResult1 == null)
                {
                    matchResult1 = new MatchResult1();
                }
                // 转换匹配结果:ml-citation{ref="1,4" data="citationList"}
                for (int i = 0; i < score.Length; i++)
                {
                    matchResult1.Row = row[i].D;
                    matchResult1.Column = column[i].D;
                    matchResult1.Angle = angle[i].D;
                    matchResult1.Scale = scale[i].D;
                    matchResult1.Score = score[i].D;
                }

                if (score.Length == 0)
                {
                    matchResult1.Result = false;
                    return false;
                }

                // 修正变换矩阵构建顺序（先旋转后平移）
                HOperatorSet.HomMat2dIdentity(out HTuple homMat);
                HOperatorSet.HomMat2dTranslate(homMat, matchResult1.Row, matchResult1.Column, out homMat);  // 先平移
                HOperatorSet.HomMat2dRotate(homMat, matchResult1.Angle, matchResult1.Row, matchResult1.Column, out homMat);  // 绕匹配点旋转
                HOperatorSet.HomMat2dScale(homMat, matchResult1.Scale, matchResult1.Scale, matchResult1.Row, matchResult1.Column, out homMat);  // 绕匹配点缩放
                // 变换并显示轮廓
                HOperatorSet.AffineTransContourXld(
                    _modelContours,
                    out HObject transContours,
                    homMat
                );

                ContourDatas.Clear();
                ContourDatas.Add("ROI0", transContours);
                matchResult1.Result = true;
                return true;
            }
            catch
            {
                matchResult1.Result = false;
                return false;
            }
            finally
            {
                hv_hvParams?.Dispose();
                grayImage?.Dispose();
            }
        }

        /// <summary>
        /// 释放Halcon资源
        /// </summary>
        public void Dispose()
        {
            if (_modelID != null)
                HOperatorSet.ClearShapeModel(_modelID);
            _modelContours.Dispose();
        }
    }
    public class MatchResult1
    {
        public double Row { get; set; }    // Y坐标
        public double Column { get; set; } // X坐标
        public double Angle { get; set; }  // 旋转角度(弧度)
        public double Scale { get; set; }  // 缩放比例
        public double Score { get; set; }  // 匹配分数[0-1]
        public bool Result { get; set; }  // 匹配结果
    }
}
