using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hWindowTool
{
    /// <summary>
    /// 工具
    /// </summary>
    public class HWTools
    {
        /// <summary>
        /// Halcon颜色转换
        /// </summary>
        public static Dictionary<HColorEnum, string> ColorConversion { get; } = new Dictionary<HColorEnum, string>()
        {
            {HColorEnum.auto,"red" },
            {HColorEnum.auto_75,"#ff0000c0" },
            {HColorEnum.auto_50,"#ff000080" },
            {HColorEnum.auto_25,"#ff000040" },
            {HColorEnum.black,"black" },
            {HColorEnum.black_75,"#000000c0" },
            {HColorEnum.black_50,"#00000080" },
            {HColorEnum.black_25,"#00000040" },
            {HColorEnum.white,"white" },
            {HColorEnum.white_75,"#ffffffc0" },
            {HColorEnum.white_50,"#ffffff80" },
            {HColorEnum.white_25,"#ffffff40" },
            {HColorEnum.red,"red" },
            {HColorEnum.red_75,"#ff0000c0" },
            {HColorEnum.red_50,"#ff000080" },
            {HColorEnum.red_25,"#ff000040" },
            {HColorEnum.green,"green" },
            {HColorEnum.green_75,"#00ff00c0" },
            {HColorEnum.green_50,"#00ff0080" },
            {HColorEnum.green_25,"#00ff0040" },
            {HColorEnum.blue,"blue" },
            {HColorEnum.blue_75,"#0000ffc0" },
            {HColorEnum.blue_50,"#0000ff80" },
            {HColorEnum.blue_25,"#0000ff40" },
            {HColorEnum.dim_gray,"dim gray" },
            {HColorEnum.dim_gray_75,"#696969c0" },
            {HColorEnum.dim_gray_50,"#69696980" },
            {HColorEnum.dim_gray_25,"#69696940" },
            {HColorEnum.gray,"gray"},
            {HColorEnum.gray_75,"#bebebec0"},
            {HColorEnum.gray_50,"#bebebe80"},
            {HColorEnum.gray_25,"#bebebe40"},
            {HColorEnum.light_gray,"light gray"},
            {HColorEnum.light_gray_75,"#d3d3d3c0"},
            {HColorEnum.light_gray_50,"#d3d3d380"},
            {HColorEnum.light_gray_25,"#d3d3d340"},
            {HColorEnum.cyan,"cyan"},
            {HColorEnum.cyan_75,"#00ffffc0"},
            {HColorEnum.cyan_50,"#00ffff80"},
            {HColorEnum.cyan_25,"#00ffff40"},
            {HColorEnum.magenta,"magenta"},
            {HColorEnum.magenta_75,"#ff00ffc0"},
            {HColorEnum.magenta_50,"#ff00ff80"},
            {HColorEnum.magenta_25,"#ff00ff40"},
            {HColorEnum.yellow,"yellow"},
            {HColorEnum.yellow_75,"#ffff00c0"},
            {HColorEnum.yellow_50,"#ffff0080"},
            {HColorEnum.yellow_25,"#ffff0040"},
            {HColorEnum.medium_slate_blue,"medium slate blue"},
            {HColorEnum.medium_slate_blue_75,"#7b68eec0"},
            {HColorEnum.medium_slate_blue_50,"#7b68ee80"},
            {HColorEnum.medium_slate_blue_25,"#7b68ee40"},
            {HColorEnum.coral,"coral"},
            {HColorEnum.coral_75,"#ff7f50c0"},
            {HColorEnum.coral_50,"#ff7f5080"},
            {HColorEnum.coral_25,"#ff7f5040"},
            {HColorEnum.slate_blue,"slate blue"},
            {HColorEnum.slate_blue_75,"#6a5acdc0"},
            {HColorEnum.slate_blue_50,"#6a5acd80"},
            {HColorEnum.slate_blue_25,"#6a5acd40"},
            {HColorEnum.spring_green,"spring green"},
            {HColorEnum.spring_green_75,"#00ff7fc0"},
            {HColorEnum.spring_green_50,"#00ff7f80"},
            {HColorEnum.spring_green_25,"#00ff7f40"},
            {HColorEnum.orange_red,"orange red"},
            {HColorEnum.orange_red_75,"#ff4500c0"},
            {HColorEnum.orange_red_50,"#ff450080"},
            {HColorEnum.orange_red_25,"#ff450040"},
            {HColorEnum.dark_olive_green,"dark olive green"},
            {HColorEnum.dark_olive_green_75,"#556b2fc0"},
            {HColorEnum.dark_olive_green_50,"#556b2f80"},
            {HColorEnum.dark_olive_green_25,"#556b2f40"},
            {HColorEnum.pink,"pink"},
            {HColorEnum.pink_75,"#ffc0cbc0"},
            {HColorEnum.pink_50,"#ffc0cb80"},
            {HColorEnum.pink_25,"#ffc0cb40"},
            {HColorEnum.cadet_blue,"cadet blue"},
            {HColorEnum.cadet_blue_75,"#5f9ea0c0"},
            {HColorEnum.cadet_blue_50,"#5f9ea080"},
            {HColorEnum.cadet_blue_25,"#5f9ea040"},
            {HColorEnum.goldenrod,"goldenrod"},
            {HColorEnum.goldenrod_75,"#daa520c0"},
            {HColorEnum.goldenrod_50,"#daa52080"},
            {HColorEnum.goldenrod_25,"#daa52040"},
            {HColorEnum.orange,"orange"},
            {HColorEnum.orange_75,"#ffa500c0"},
            {HColorEnum.orange_50,"#ffa50080"},
            {HColorEnum.orange_25,"#ffa50040"},
            {HColorEnum.gold,"gold"},
            {HColorEnum.gold_75,"#ffd700c0"},
            {HColorEnum.gold_50,"#ffd70080"},
            {HColorEnum.gold_25,"#ffd70040"},
            {HColorEnum.forest_green,"forest green"},
            {HColorEnum.forest_green_75,"#228b22c0"},
            {HColorEnum.forest_green_50,"#228b2280"},
            {HColorEnum.forest_green_25,"#228b2240"},
            {HColorEnum.cornflower_blue,"cornflower blue"},
            {HColorEnum.cornflower_blue_75,"#6495edc0"},
            {HColorEnum.cornflower_blue_50,"#6495ed80"},
            {HColorEnum.cornflower_blue_25,"#6495ed40"},
            {HColorEnum.navy,"navy"},
            {HColorEnum.navy_75,"#000080c0"},
            {HColorEnum.navy_50,"#00008080"},
            {HColorEnum.navy_25,"#00008040"},
            {HColorEnum.turquoise,"turquoise"},
            {HColorEnum.turquoise_75,"#40e0d0c0c0"},
            {HColorEnum.turquoise_50,"#40e0d0c080"},
            {HColorEnum.turquoise_25,"#40e0d0c040"},
            {HColorEnum.dark_slate_blue,"dark slate blue"},
            {HColorEnum.dark_slate_blue_75,"#483d8bc0"},
            {HColorEnum.dark_slate_blue_50,"#483d8b80"},
            {HColorEnum.dark_slate_blue_25,"#483d8b40"},
            {HColorEnum.light_blue,"light blue"},
            {HColorEnum.light_blue_75,"#add8e6c0"},
            {HColorEnum.light_blue_50,"#add8e680"},
            {HColorEnum.light_blue_25,"#add8e640"},
            {HColorEnum.indian_red,"indian red"},
            {HColorEnum.indian_red_75,"#cd5c5cc0"},
            {HColorEnum.indian_red_50,"#cd5c5c80"},
            {HColorEnum.indian_red_25,"#cd5c5c40"},
            {HColorEnum.violet_red,"violet red"},
            {HColorEnum.violet_red_75,"#d02090c0"},
            {HColorEnum.violet_red_50,"#d0209080"},
            {HColorEnum.violet_red_25,"#d0209040"},
            {HColorEnum.light_steel_blue,"light steel blue"},
            {HColorEnum.light_steel_blue_75,"#b0c4dec0"},
            {HColorEnum.light_steel_blue_50,"#b0c4de80"},
            {HColorEnum.light_steel_blue_25,"#b0c4de40"},
            {HColorEnum.medium_blue,"medium blue"},
            {HColorEnum.medium_blue_75,"#0000cdc0"},
            {HColorEnum.medium_blue_50,"#0000cd80"},
            {HColorEnum.medium_blue_25,"#0000cd40"},
            {HColorEnum.khaki,"khaki"},
            {HColorEnum.khaki_75,"#f0e68cc0"},
            {HColorEnum.khaki_50,"#f0e68c80"},
            {HColorEnum.khaki_25,"#f0e68c40"},
            {HColorEnum.violet,"violet"},
            {HColorEnum.violet_75,"#ee82eec0"},
            {HColorEnum.violet_50,"#ee82ee80"},
            {HColorEnum.violet_25,"#ee82ee40"},
            {HColorEnum.firebrick,"firebrick"},
            {HColorEnum.firebrick_75,"#b22222c0"},
            {HColorEnum.firebrick_50,"#b2222280"},
            {HColorEnum.firebrick_25,"#b2222240"},
            {HColorEnum.midnight_blue,"midnight blue"},
            {HColorEnum.midnight_blue_75,"#191970c0"},
            {HColorEnum.midnight_blue_50,"#19197080"},
            {HColorEnum.midnight_blue_25,"#19197040"},
        };

        /// <summary>
        /// 鲜艳颜色不透明
        /// </summary>
        public static List<HColorEnum> ColorfulColor = new List<HColorEnum>()
        {
            HColorEnum.red,
            HColorEnum.green,
            HColorEnum.blue,
            HColorEnum.cyan,
            HColorEnum.magenta,
            HColorEnum.yellow,
            HColorEnum.coral,
            HColorEnum.spring_green,
            HColorEnum.medium_slate_blue,
            HColorEnum.orange_red,
            HColorEnum.goldenrod,
            HColorEnum.slate_blue,
        };

        /// <summary>
        /// 鲜艳颜色_显示度75%
        /// </summary>
        public static List<HColorEnum> ColorfulColor_75 = new List<HColorEnum>()
        {
            HColorEnum.red_75,
            HColorEnum.green_75,
            HColorEnum.blue_75,
            HColorEnum.cyan_75,
            HColorEnum.magenta_75,
            HColorEnum.yellow_75,
            HColorEnum.coral_75,
            HColorEnum.spring_green_75,
            HColorEnum.medium_slate_blue_75,
            HColorEnum.orange_red_75,
            HColorEnum.goldenrod_75,
            HColorEnum.slate_blue_75,
        };

        /// <summary>
        /// 鲜艳颜色_显示度50%
        /// </summary>
        public static List<HColorEnum> ColorfulColor_50 = new List<HColorEnum>()
        {
            HColorEnum.red_50,
            HColorEnum.green_50,
            HColorEnum.blue_50,
            HColorEnum.cyan_50,
            HColorEnum.magenta_50,
            HColorEnum.yellow_50,
            HColorEnum.coral_50,
            HColorEnum.spring_green_50,
            HColorEnum.medium_slate_blue_50,
            HColorEnum.orange_red_50,
            HColorEnum.goldenrod_50,
            HColorEnum.slate_blue_50,
        };

        /// <summary>
        /// 鲜艳颜色_显示度25%
        /// </summary>
        public static List<HColorEnum> ColorfulColor_25 = new List<HColorEnum>()
        {
            HColorEnum.red_25,
            HColorEnum.green_25,
            HColorEnum.blue_25,
            HColorEnum.cyan_25,
            HColorEnum.magenta_25,
            HColorEnum.yellow_25,
            HColorEnum.coral_25,
            HColorEnum.spring_green_25,
            HColorEnum.medium_slate_blue_25,
            HColorEnum.orange_red_25,
            HColorEnum.goldenrod_25,
            HColorEnum.slate_blue_25,
        };

        /// <summary>
        /// 小矩形大小
        /// </summary>
        internal const double MIN_RECTANGLE_SIZE = 0.0001;

        /// <summary>
        /// 小矩形边框大小
        /// </summary>
        internal const double MIN_RECTANGLE_BORDER_SIZE = 6;

        /// <summary>
        /// 小矩形填充大小
        /// </summary>
        internal const double MIN_RECTANGLE_FILL_SIZE = 5;

        /// <summary>
        /// 测试区域点
        /// 默认无方向矩形区域
        /// </summary>
        /// <param name="testRow">测试点row</param>
        /// <param name="testColumn">测试点columu</param>
        /// <param name="row">矩形中心row</param>
        /// <param name="column">矩形中心column</param>
        /// <param name="radius">矩形半径</param>
        /// <returns>true:在区域内 false:不在区域内</returns>
        public bool TestRegionPoint(double testRow, double testColumn, double row, double column, double radius = 5)
        {
            //计算小矩形左上角row column
            double lTRow = row - radius;
            double lTColumn = column - radius;
            //计算小矩形右下角row column
            double rBRow = row + radius;
            double rBColumn = column + radius;
            //在区域内
            if (testRow >= lTRow && testRow <= rBRow && testColumn >= lTColumn && testColumn <= rBColumn)
            {
                //返回true
                return true;
            }
            //不在区域内,返回false
            return false;
        }

        /// <summary>
        /// 角度转弧度
        /// </summary>
        /// <param name="angle">要转换的角度</param>
        /// <returns>弧度</returns>
        public double ATR(double angle)
        {
            return angle * Math.PI / 180.0;
        }

        /// <summary>
        /// 弧度转角度
        /// </summary>
        /// <param name="radian">要转换的弧度</param>
        /// <returns>角度</returns>
        public double RTA(double radian)
        {
            return radian * 180.0 / Math.PI;
        }

        /// <summary>
        /// 创建基础位图
        /// </summary>
        /// <param name="width">图像宽</param>
        /// <param name="height">图像高</param>
        /// <returns>创建成功返回位图，创建失败返回null</returns>
        public Bitmap CreateBaseBitmap(int width, int height)
        {
            //定义小矩形大小
            int size = 10;
            //图像宽高范围异常
            if (width <= 0 || height <= 0)
            {
                //结束方法
                return null;
            }
            //创建一个新的Bitmap对象
            Bitmap bitmap = new Bitmap(width, height);
            //创建Graphics对象  
            Graphics graphics = Graphics.FromImage(bitmap);
            //创建一个SolidBrush对象来定义填充颜色  
            SolidBrush brush1 = new SolidBrush(System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60))))));
            SolidBrush brush2 = new SolidBrush(System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45))))));
            //当前行
            int row = 0;
            //当前列
            int column = 0;
            //循环创建小矩形
            while (true)
            {
                //当前是偶数行
                if (IsEven(row))
                {
                    //绘制带有填充颜色的矩形  
                    graphics.FillRectangle(brush1, new Rectangle(column * size * 2, row * size, size, size));
                    graphics.FillRectangle(brush2, new Rectangle(column * size * 2 + size, row * size, size, size));
                    //列小矩形绘制完成
                    if (column * size >= width)
                    {
                        //行小矩形绘制完成
                        if (row * size >= height)
                        {
                            //结束循环
                            break;
                        }
                        //清除当前列
                        column = 0;
                        //当前行+1
                        row++;
                    }
                    else
                    {
                        //列数+1
                        column++;
                    }
                }
                //当前是奇数行
                else
                {
                    //绘制带有填充颜色的矩形  
                    graphics.FillRectangle(brush1, new Rectangle(column * size * 2 + size, row * size, size, size));
                    graphics.FillRectangle(brush2, new Rectangle(column * size * 2, row * size, size, size));
                    //列小矩形绘制完成
                    if (column * size >= width)
                    {
                        //行小矩形绘制完成
                        if (row * size >= height)
                        {
                            //结束循环
                            break;
                        }
                        //清除当前列
                        column = 0;
                        //当前行+1
                        row++;
                    }
                    else
                    {
                        //列数+1
                        column++;
                    }
                }
            }
            //清理资源  
            graphics.Dispose();
            brush1.Dispose();
            brush2.Dispose();
            //返回位图
            return bitmap;
        }

        /// <summary>
        /// 判断是否偶数
        /// </summary>
        /// <param name="num">要判断的值</param>
        /// <returns>true:偶数</returns>
        public bool IsEven(int num)
        {
            if (num % 2 == 0)
            {
                return true;
            }
            return false;
        }

        #region Bitmap类型转换成HObject类型
        /// <summary>
        /// Bitmap类型转换成HObject类型
        /// </summary>
        /// <param name="bmp">Bitmap对象</param>
        /// <param name="hObject">HObject对象</param>
        public void Bitmap2HObjectBpp24(Bitmap bmp, out HObject hObject)
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

                BitmapData bmp_data = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                byte[] arrayR = new byte[bmp_data.Width * bmp_data.Height];//红色数组 
                byte[] arrayG = new byte[bmp_data.Width * bmp_data.Height];//绿色数组 
                byte[] arrayB = new byte[bmp_data.Width * bmp_data.Height];//蓝色数组 
                unsafe
                {
                    //BitMap的头指针 
                    byte* pBmp = (byte*)bmp_data.Scan0;
                    //下面的循环分别提取出红绿蓝三色放入三个数组 
                    for (int R = 0; R < bmp_data.Height; R++)
                    {
                        for (int C = 0; C < bmp_data.Width; C++)
                        {
                            //因为内存BitMap的储存方式，行宽用Stride算，C*3是因为这是三通道，另外BitMap是按BGR储存的 
                            byte* pBase = pBmp + bmp_data.Stride * R + C * 3;
                            arrayR[R * bmp_data.Width + C] = *(pBase + 2);
                            arrayG[R * bmp_data.Width + C] = *(pBase + 1);
                            arrayB[R * bmp_data.Width + C] = *(pBase);
                        }
                    }
                    fixed (byte* pR = arrayR, pG = arrayG, pB = arrayB)
                    {
                        HOperatorSet.GenImage3(out hObject, "byte", bmp_data.Width, bmp_data.Height,
                                                                   new IntPtr(pR), new IntPtr(pG), new IntPtr(pB));
                        //如果这里报错，仔细看看前面有没有写错 
                    }
                }


            }
            catch (Exception)
            {
                hObject = null;
            }
        }
        #endregion
    }

    /// <summary>
    /// Halcon颜色定义枚举
    /// </summary>
    public enum HColorEnum
    {
        /// <summary>
        /// 自动选择颜色
        /// </summary>
        auto,
        /// <summary>
        /// 自动选择颜色显示度75%
        /// </summary>
        auto_75,
        /// <summary>
        /// 自动选择颜色显示度50%
        /// </summary>
        auto_50,
        /// <summary>
        /// 自动选择颜色显示度25%
        /// </summary>
        auto_25,
        /// <summary>
        /// 黑色
        /// </summary>
        black,
        /// <summary>
        /// 黑色显示度75%
        /// </summary>
        black_75,
        /// <summary>
        /// 黑色显示度50%
        /// </summary>
        black_50,
        /// <summary>
        /// 黑色显示度25%
        /// </summary>
        black_25,
        /// <summary>
        /// 白色
        /// </summary>
        white,
        /// <summary>
        /// 白色显示度75%
        /// </summary>
        white_75,
        /// <summary>
        /// 白色显示度50%
        /// </summary>
        white_50,
        /// <summary>
        /// 白色显示度25%
        /// </summary>
        white_25,
        /// <summary>
        /// 红色
        /// </summary>
        red,
        /// <summary>
        /// 红色显示度75%
        /// </summary>
        red_75,
        /// <summary>
        /// 红色显示度50%
        /// </summary>
        red_50,
        /// <summary>
        /// 红色显示度25%
        /// </summary>
        red_25,
        /// <summary>
        /// 绿色
        /// </summary>
        green,
        /// <summary>
        /// 绿色显示度75%
        /// </summary>
        green_75,
        /// <summary>
        /// 绿色显示度50%
        /// </summary>
        green_50,
        /// <summary>
        /// 绿色显示度25%
        /// </summary>
        green_25,
        /// <summary>
        /// 蓝色
        /// </summary>
        blue,
        /// <summary>
        /// 蓝色显示度75%
        /// </summary>
        blue_75,
        /// <summary>
        /// 蓝色显示度50%
        /// </summary>
        blue_50,
        /// <summary>
        /// 蓝色显示度25%
        /// </summary>
        blue_25,
        /// <summary>
        /// 暗灰色
        /// </summary>
        dim_gray,
        /// <summary>
        /// 暗灰色显示度75%
        /// </summary>
        dim_gray_75,
        /// <summary>
        /// 暗灰色显示度50%
        /// </summary>
        dim_gray_50,
        /// <summary>
        /// 暗灰色显示度25%
        /// </summary>
        dim_gray_25,
        /// <summary>
        /// 灰色
        /// </summary>
        gray,
        /// <summary>
        /// 灰色显示度75%
        /// </summary>
        gray_75,
        /// <summary>
        /// 灰色显示度50%
        /// </summary>
        gray_50,
        /// <summary>
        /// 灰色显示度25%
        /// </summary>
        gray_25,
        /// <summary>
        /// 浅灰色
        /// </summary>
        light_gray,
        /// <summary>
        /// 浅灰色显示度75%
        /// </summary>
        light_gray_75,
        /// <summary>
        /// 浅灰色显示度50%
        /// </summary>
        light_gray_50,
        /// <summary>
        /// 浅灰色显示度25%
        /// </summary>
        light_gray_25,
        /// <summary>
        /// 青色
        /// </summary>
        cyan,
        /// <summary>
        /// 青色显示度75%
        /// </summary>
        cyan_75,
        /// <summary>
        /// 青色显示度50%
        /// </summary>
        cyan_50,
        /// <summary>
        /// 青色显示度25%
        /// </summary>
        cyan_25,
        /// <summary>
        /// 洋红色
        /// </summary>
        magenta,
        /// <summary>
        /// 洋红色显示度75%
        /// </summary>
        magenta_75,
        /// <summary>
        /// 洋红色显示度50%
        /// </summary>
        magenta_50,
        /// <summary>
        /// 洋红色显示度25%
        /// </summary>
        magenta_25,
        /// <summary>
        /// 黄色
        /// </summary>
        yellow,
        /// <summary>
        /// 黄色显示度75%
        /// </summary>
        yellow_75,
        /// <summary>
        /// 黄色显示度50%
        /// </summary>
        yellow_50,
        /// <summary>
        /// 黄色显示度25%
        /// </summary>
        yellow_25,
        /// <summary>
        /// 中板岩蓝色
        /// </summary>
        medium_slate_blue,
        /// <summary>
        /// 中板岩蓝色显示度75%
        /// </summary>
        medium_slate_blue_75,
        /// <summary>
        /// 中板岩蓝色显示度50%
        /// </summary>
        medium_slate_blue_50,
        /// <summary>
        /// 中板岩蓝色显示度25%
        /// </summary>
        medium_slate_blue_25,
        /// <summary>
        /// 珊瑚色
        /// </summary>
        coral,
        /// <summary>
        /// 珊瑚色显示度75%
        /// </summary>
        coral_75,
        /// <summary>
        /// 珊瑚色显示度50%
        /// </summary>
        coral_50,
        /// <summary>
        /// 珊瑚色显示度25%
        /// </summary>
        coral_25,
        /// <summary>
        /// 石板蓝
        /// </summary>
        slate_blue,
        /// <summary>
        /// 石板蓝显示度75%
        /// </summary>
        slate_blue_75,
        /// <summary>
        /// 石板蓝显示度50%
        /// </summary>
        slate_blue_50,
        /// <summary>
        /// 石板蓝显示度25%
        /// </summary>
        slate_blue_25,
        /// <summary>
        /// 春绿色
        /// </summary>
        spring_green,
        /// <summary>
        /// 春绿色显示度75%
        /// </summary>
        spring_green_75,
        /// <summary>
        /// 春绿色显示度50%
        /// </summary>
        spring_green_50,
        /// <summary>
        /// 春绿色显示度25%
        /// </summary>
        spring_green_25,
        /// <summary>
        /// 橙红色
        /// </summary>
        orange_red,
        /// <summary>
        /// 橙红色显示度75%
        /// </summary>
        orange_red_75,
        /// <summary>
        /// 橙红色显示度50%
        /// </summary>
        orange_red_50,
        /// <summary>
        /// 橙红色显示度25%
        /// </summary>
        orange_red_25,
        /// <summary>
        /// 深橄榄绿色
        /// </summary>
        dark_olive_green,
        /// <summary>
        /// 深橄榄绿色显示度75%
        /// </summary>
        dark_olive_green_75,
        /// <summary>
        /// 深橄榄绿色显示度50%
        /// </summary>
        dark_olive_green_50,
        /// <summary>
        /// 深橄榄绿色显示度25%
        /// </summary>
        dark_olive_green_25,
        /// <summary>
        /// 粉红色
        /// </summary>
        pink,
        /// <summary>
        /// 粉红色显示度75%
        /// </summary>
        pink_75,
        /// <summary>
        /// 粉红色显示度50%
        /// </summary>
        pink_50,
        /// <summary>
        /// 粉红色显示度25%
        /// </summary>
        pink_25,
        /// <summary>
        /// 军校蓝
        /// </summary>
        cadet_blue,
        /// <summary>
        /// 军校蓝显示度75%
        /// </summary>
        cadet_blue_75,
        /// <summary>
        /// 军校蓝显示度50%
        /// </summary>
        cadet_blue_50,
        /// <summary>
        /// 军校蓝显示度25%
        /// </summary>
        cadet_blue_25,
        /// <summary>
        /// 金菊黄色
        /// </summary>
        goldenrod,
        /// <summary>
        /// 金菊黄色显示度75%
        /// </summary>
        goldenrod_75,
        /// <summary>
        /// 金菊黄色显示度50%
        /// </summary>
        goldenrod_50,
        /// <summary>
        /// 金菊黄色显示度25%
        /// </summary>
        goldenrod_25,
        /// <summary>
        /// 橙色
        /// </summary>
        orange,
        /// <summary>
        /// 橙色显示度75%
        /// </summary>
        orange_75,
        /// <summary>
        /// 橙色显示度50%
        /// </summary>
        orange_50,
        /// <summary>
        /// 橙色显示度25%
        /// </summary>
        orange_25,
        /// <summary>
        /// 金色
        /// </summary>
        gold,
        /// <summary>
        /// 金色显示度75%
        /// </summary>
        gold_75,
        /// <summary>
        /// 金色显示度50%
        /// </summary>
        gold_50,
        /// <summary>
        /// 金色显示度25%
        /// </summary>
        gold_25,
        /// <summary>
        /// 森林绿色
        /// </summary>
        forest_green,
        /// <summary>
        /// 森林绿色显示度75%
        /// </summary>
        forest_green_75,
        /// <summary>
        /// 森林绿色显示度50%
        /// </summary>
        forest_green_50,
        /// <summary>
        /// 森林绿色显示度25%
        /// </summary>
        forest_green_25,
        /// <summary>
        /// 矢车菊蓝
        /// </summary>
        cornflower_blue,
        /// <summary>
        /// 矢车菊蓝显示度75%
        /// </summary>
        cornflower_blue_75,
        /// <summary>
        /// 矢车菊蓝显示度50%
        /// </summary>
        cornflower_blue_50,
        /// <summary>
        /// 矢车菊蓝显示度25%
        /// </summary>
        cornflower_blue_25,
        /// <summary>
        /// 藏青色
        /// </summary>
        navy,
        /// <summary>
        /// 藏青色显示度75%
        /// </summary>
        navy_75,
        /// <summary>
        /// 藏青色显示度50%
        /// </summary>
        navy_50,
        /// <summary>
        /// 藏青色显示度25%
        /// </summary>
        navy_25,
        /// <summary>
        /// 绿宝石色
        /// </summary>
        turquoise,
        /// <summary>
        /// 绿宝石色显示度75%
        /// </summary>
        turquoise_75,
        /// <summary>
        /// 绿宝石色显示度50%
        /// </summary>
        turquoise_50,
        /// <summary>
        /// 绿宝石色显示度25%
        /// </summary>
        turquoise_25,
        /// <summary>
        /// 深石板蓝色
        /// </summary>
        dark_slate_blue,
        /// <summary>
        /// 深石板蓝色显示度75%
        /// </summary>
        dark_slate_blue_75,
        /// <summary>
        /// 深石板蓝色显示度50%
        /// </summary>
        dark_slate_blue_50,
        /// <summary>
        /// 深石板蓝色显示度25%
        /// </summary>
        dark_slate_blue_25,
        /// <summary>
        /// 浅蓝色
        /// </summary>
        light_blue,
        /// <summary>
        /// 浅蓝色显示度75%
        /// </summary>
        light_blue_75,
        /// <summary>
        /// 浅蓝色显示度50%
        /// </summary>
        light_blue_50,
        /// <summary>
        /// 浅蓝色显示度25%
        /// </summary>
        light_blue_25,
        /// <summary>
        /// 印度红色
        /// </summary>
        indian_red,
        /// <summary>
        /// 印度红色显示度75%
        /// </summary>
        indian_red_75,
        /// <summary>
        /// 印度红色显示度50%
        /// </summary>
        indian_red_50,
        /// <summary>
        /// 印度红色显示度25%
        /// </summary>
        indian_red_25,
        /// <summary>
        /// 紫罗兰红色
        /// </summary>
        violet_red,
        /// <summary>
        /// 紫罗兰红色显示度75%
        /// </summary>
        violet_red_75,
        /// <summary>
        /// 紫罗兰红色显示度50%
        /// </summary>
        violet_red_50,
        /// <summary>
        /// 紫罗兰红色显示度25%
        /// </summary>
        violet_red_25,
        /// <summary>
        /// 浅钢蓝色
        /// </summary>
        light_steel_blue,
        /// <summary>
        /// 浅钢蓝色显示度75%
        /// </summary>
        light_steel_blue_75,
        /// <summary>
        /// 浅钢蓝色显示度50%
        /// </summary>
        light_steel_blue_50,
        /// <summary>
        /// 浅钢蓝色显示度25%
        /// </summary>
        light_steel_blue_25,
        /// <summary>
        /// 中蓝色
        /// </summary>
        medium_blue,
        /// <summary>
        /// 中蓝色显示度75%
        /// </summary>
        medium_blue_75,
        /// <summary>
        /// 中蓝色显示度50%
        /// </summary>
        medium_blue_50,
        /// <summary>
        /// 中蓝色显示度25%
        /// </summary>
        medium_blue_25,
        /// <summary>
        /// 卡其色
        /// </summary>
        khaki,
        /// <summary>
        /// 卡其色显示度75%
        /// </summary>
        khaki_75,
        /// <summary>
        /// 卡其色显示度50%
        /// </summary>
        khaki_50,
        /// <summary>
        /// 卡其色显示度25%
        /// </summary>
        khaki_25,
        /// <summary>
        /// 紫罗兰色
        /// </summary>
        violet,
        /// <summary>
        /// 紫罗兰色显示度75%
        /// </summary>
        violet_75,
        /// <summary>
        /// 紫罗兰色显示度50%
        /// </summary>
        violet_50,
        /// <summary>
        /// 紫罗兰色显示度25%
        /// </summary>
        violet_25,
        /// <summary>
        /// 砖红色
        /// </summary>
        firebrick,
        /// <summary>
        /// 砖红色显示度75%
        /// </summary>
        firebrick_75,
        /// <summary>
        /// 砖红色显示度50%
        /// </summary>
        firebrick_50,
        /// <summary>
        /// 砖红色显示度25%
        /// </summary>
        firebrick_25,
        /// <summary>
        /// 午夜蓝
        /// </summary>
        midnight_blue,
        /// <summary>
        /// 午夜蓝显示度75%
        /// </summary>
        midnight_blue_75,
        /// <summary>
        /// 午夜蓝显示度50%
        /// </summary>
        midnight_blue_50,
        /// <summary>
        /// 午夜蓝显示度25%
        /// </summary>
        midnight_blue_25,
    }
}
