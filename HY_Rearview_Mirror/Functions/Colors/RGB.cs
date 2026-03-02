using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY_Rearview_Mirror.Functions.Colors
{
    public class RGB
    {
        /// <summary>
        /// 获取区域平均 RGB（使用 Halcon 内置算子，高效）
        /// </summary>
        public (double R, double G, double B) GetRegionMean(HObject image, HObject region)
        {
            HObject r = null, g = null, b = null;

            try
            {
                HOperatorSet.Decompose3(image, out r, out g, out b);

                HTuple meanR, meanG, meanB, deviation;

                HOperatorSet.Intensity(region, r, out meanR, out deviation);
                HOperatorSet.Intensity(region, g, out meanG, out deviation);
                HOperatorSet.Intensity(region, b, out meanB, out deviation);

                return (meanR.D, meanG.D, meanB.D);
            }
            finally
            {
                r?.Dispose(); g?.Dispose(); b?.Dispose();
            }
        }

    }
}
