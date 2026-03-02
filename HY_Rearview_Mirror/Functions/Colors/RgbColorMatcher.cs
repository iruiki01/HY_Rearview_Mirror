using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY_Rearview_Mirror.Functions.Colors
{
    /// <summary>
    /// RGB颜色判断器
    /// </summary>
    public class RgbColorMatcher
    {
        /// <summary>
        /// 预定义颜色
        /// </summary>
        public static class Colors
        {
            public static readonly (double R, double G, double B) Red = (255, 0, 0);
            public static readonly (double R, double G, double B) Green = (0, 255, 0);
            public static readonly (double R, double G, double B) Blue = (0, 0, 255);
            public static readonly (double R, double G, double B) Yellow = (255, 255, 0);
            public static readonly (double R, double G, double B) Cyan = (0, 255, 255);
            public static readonly (double R, double G, double B) Magenta = (255, 0, 255);
            public static readonly (double R, double G, double B) White = (255, 255, 255);
            public static readonly (double R, double G, double B) Black = (0, 0, 0);
            public static readonly (double R, double G, double B) Orange = (255, 165, 0);
            public static readonly (double R, double G, double B) Purple = (128, 0, 128);
            public static readonly (double R, double G, double B) Pink = (255, 192, 203);
            public static readonly (double R, double G, double B) Gray = (128, 128, 128);
            public static readonly (double R, double G, double B) Brown = (165, 42, 42);
            public static readonly (double R, double G, double B) DarkRed = (84.82, 16.71, 5.63);
        }

        /// <summary>
        /// 判断颜色是否匹配（欧氏距离）
        /// </summary>
        /// <param name="inputR">输入R值</param>
        /// <param name="inputG">输入G值</param>
        /// <param name="inputB">输入B值</param>
        /// <param name="targetR">目标R值</param>
        /// <param name="targetG">目标G值</param>
        /// <param name="targetB">目标B值</param>
        /// <param name="threshold">距离阈值（默认50）</param>
        /// <returns>是否匹配</returns>
        public bool IsMatch(
            double inputR, double inputG, double inputB,
            double targetR, double targetG, double targetB,
            double threshold = 50)
        {
            double distance = CalculateEuclideanDistance(
                inputR, inputG, inputB,
                targetR, targetG, targetB);

            return distance <= threshold;
        }

        /// <summary>
        /// 判断颜色是否匹配（使用预定义颜色）
        /// </summary>
        public bool IsMatch(
            double inputR, double inputG, double inputB,
            (double R, double G, double B) targetColor,
            double threshold = 50)
        {
            return IsMatch(inputR, inputG, inputB,
                targetColor.R, targetColor.G, targetColor.B, threshold);
        }

        /// <summary>
        /// 计算欧氏距离
        /// </summary>
        private double CalculateEuclideanDistance(
            double r1, double g1, double b1,
            double r2, double g2, double b2)
        {
            return Math.Sqrt(
                Math.Pow(r1 - r2, 2) +
                Math.Pow(g1 - g2, 2) +
                Math.Pow(b1 - b2, 2));
        }

        /// <summary>
        /// 计算曼哈顿距离（更快，不用开方）
        /// </summary>
        private double CalculateManhattanDistance(
            double r1, double g1, double b1,
            double r2, double g2, double b2)
        {
            return Math.Abs(r1 - r2) + Math.Abs(g1 - g2) + Math.Abs(b1 - b2);
        }

        /// <summary>
        /// 使用曼哈顿距离判断（更快）
        /// </summary>
        public bool IsMatchManhattan(
            double inputR, double inputG, double inputB,
            double targetR, double targetG, double targetB,
            double threshold = 80)
        {
            double distance = CalculateManhattanDistance(
                inputR, inputG, inputB,
                targetR, targetG, targetB);

            return distance <= threshold;
        }

        /// <summary>
        /// 找出最接近的颜色名称
        /// </summary>
        public string FindClosestColor(double r, double g, double b, out double minDistance)
        {
            string[] colorNames = { "红色", "绿色", "蓝色", "黄色", "青色",
            "玫瑰色", "白色", "黑色", "橙色", "紫色", "粉红色", "灰色", "浅棕色","暗红色" };

            (double, double, double)[] colorValues = {
            Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Cyan,
            Colors.Magenta, Colors.White, Colors.Black, Colors.Orange,
            Colors.Purple, Colors.Pink, Colors.Gray, Colors.Brown,Colors.DarkRed};

            minDistance = double.MaxValue;
            string closestColor = "Unknown";

            for (int i = 0; i < colorValues.Length; i++)
            {
                double dist = CalculateEuclideanDistance(r, g, b,
                    colorValues[i].Item1, colorValues[i].Item2, colorValues[i].Item3);

                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestColor = colorNames[i];
                }
            }

            return closestColor;
        }
    }
}
