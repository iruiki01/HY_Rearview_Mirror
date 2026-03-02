using Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HY_Rearview_Mirror.Functions
{
    public class SaveImage
    {
        string _name = "";
        static bool StartDelImageFlag = true;
        private static readonly object _Draw_Write_Image_lockObj = new object();
        static Thread DelImageThread = null;
        public SaveImage(string name)
        {
            _name = name;
        }
        public void Save_Image(string CodeName, Bitmap image1 = null, Bitmap image2 = null, bool isSaveOrigImag = false)
        {
            try
            {
                // 保存结果图       
                string fileName = "D:\\运行图片\\" + _name + "\\" + DateTime.Now.ToString("yyyy-MM-dd");
                FileCopyHelper.DirectoryCreate(fileName);

                DateTime now = DateTime.Now;
                int hour = now.Hour; // 获取小时
                int minute = now.Minute; // 获取分钟
                int second = now.Second; // 获取秒
                string str = hour + "_" + minute + "_" + second;
                string str1 = fileName + @"\" + CodeName + "_" + str + @".jpeg";
                image1.Save(str1, ImageFormat.Png);
                //image1.SaveImage(str1);
                image1.Dispose();

                if (isSaveOrigImag)
                {
                    // 保存原图         
                    fileName = "D:\\运行图片\\" + _name + "原图\\" + DateTime.Now.ToString("yyyy-MM-dd");
                    FileCopyHelper.DirectoryCreate(fileName);

                    str1 = fileName + @"\" + CodeName + "_" + str + @".bmp";
                    image2.Save(str1, ImageFormat.Png);
                    //image2.SaveImage(str1);
                    image2.Dispose();
                }
            }
            catch
            {
                global.writeLogSystem.Error("保存" + _name + "图片出错了！");
            }
        }

        /// <summary>
        /// 删除图片
        /// </summary>
        public static void DelImageFile(List<string> pathList)
        {
            while (StartDelImageFlag)
            {
                double size = Get_D_Size();
                if (size < 50)
                {
                    try
                    {
                        foreach (string filePath in pathList)
                        {
                            string[] directoriesUp = TraverseFolder(filePath);

                            if (directoriesUp != null)
                            {
                                List<DateTime> times = new List<DateTime>();
                                times.Clear();

                                // 遍历并输出每个文件夹名称
                                foreach (string directory in directoriesUp)
                                {
                                    string[] arr = directory.Split('\\');
                                    times.Add(DateTime.Parse(arr[3]));
                                }

                                // 使用Sort方法排序
                                times.Sort();

                                string[] arr1 = times[0].ToString().Split('/');
                                string _SaveFilePath = filePath + @"\" + @times[0].ToString("yyyy-MM-dd");
                                // 检查文件是否存在
                                if (Directory.Exists(_SaveFilePath))
                                {
                                    FileCopyHelper.DeleteDirectory(_SaveFilePath);
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    Thread.Sleep(30000);
                }
            }
        }

        public static double Get_D_Size()
        {
            // 获取D盘的DriveInfo对象
            DriveInfo dDrive = new DriveInfo("D");

            // 检查D盘是否可用
            if (dDrive.IsReady)
            {
                // 获取D盘的总容量（以字节为单位）
                long totalSizeBytes = dDrive.TotalSize;

                // 获取D盘的可用空间（以字节为单位）
                long availableFreeSpaceBytes = dDrive.AvailableFreeSpace;

                // 将字节转换为更易读的格式，例如GB
                double totalSizeGB = (double)totalSizeBytes / 1024 / 1024 / 1024;// D盘总容量
                double availableFreeSpaceGB = (double)availableFreeSpaceBytes / 1024 / 1024 / 1024;// D盘可用空间

                return availableFreeSpaceGB;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 读取指定路径下的文件夹
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static string[] TraverseFolder(string folderPath)
        {
            string[] directories = null;
            try
            {
                // 检查文件夹是否存在
                if (Directory.Exists(folderPath))
                {
                    // 获取指定路径下的所有文件夹名称
                    directories = Directory.GetDirectories(folderPath);
                    return directories;
                }
                else
                {
                    Console.WriteLine("指定的文件夹不存在。");
                    return directories;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
                return directories;
            }
        }
        public static void Run(List<string> pathList)
        {
            DelImageThread = new Thread(() =>
            {
                DelImageFile(pathList);
            });
            DelImageThread.Start();
        }

        public static void Stop()
        {
            StartDelImageFlag = false;

            if (DelImageThread.IsAlive)
            {
                DelImageThread.Abort();
            }
        }

        /// <summary>
        /// 在图片上写字函数
        /// </summary>
        public static void Draw_Write_Image(System.Drawing.Point P, ref Bitmap image, string Message, string Colour)
        {
            lock (_Draw_Write_Image_lockObj)
            {
                try
                {
                    // 转换到Bitmap以便使用GDI+
                    //Bitmap bitmap = image.ToBitmap();
                    // 设置字体
                    FontFamily fontFamily = new FontFamily("Microsoft YaHei");
                    Font font = new Font(fontFamily, 100);
                    SolidBrush brush = null;
                    // 设置字体颜色
                    if (Colour == "绿色")
                    {
                        brush = new SolidBrush(Color.Green);
                    }
                    else if (Colour == "红色")
                    {
                        brush = new SolidBrush(Color.Red);
                    }
                    else
                    {
                        brush = new SolidBrush(Color.Green);
                    }

                    using (Graphics graphics = Graphics.FromImage(image))
                    {
                        using (Pen rectanglePen = new Pen(Color.Green, 5))
                        {
                            // 使用GDI+绘制中文
                            {
                                graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                                graphics.DrawString(Message, font, brush, new PointF(Convert.ToSingle(P.X), Convert.ToSingle(P.Y)));
                            }
                        }
                    }
                    //image = BitmapConverter.ToMat(bitmap);
                    //image.SaveImage(@"F:\llll.bmp");
                    //this.skImageBox1.Disp(tempMat);
                    //image.Dispose();
                    //bitmap.Dispose();
                    brush.Dispose();
                }
                catch (Exception) { }
            }
        }
        /// <summary>
        /// 计算生产良率百分比
        /// </summary>
        /// <param name="total">总生产数量</param>
        /// <param name="okCount">合格品数量</param>
        /// <returns>格式化后的良率百分比字符串</returns>
        /// <exception cref="ArgumentException">输入参数无效时抛出</exception>
        public static string CalculateYield(int total, int okCount)
        {
            // 参数验证
            if (total <= 0)
                throw new ArgumentException("总数必须大于0");

            if (okCount < 0 || okCount > total)
                throw new ArgumentException("合格数必须在0到总数之间");

            // 计算良率（保留两位小数）
            double yield = (double)okCount / total * 100;
            return $"{yield:F2}%";
        }
    }
}
