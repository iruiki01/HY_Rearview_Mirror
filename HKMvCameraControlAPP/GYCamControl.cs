using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HKMvCameraControlAPP
{
    public partial class GYCamControl : UserControl
    {
        
        public GYCamControl()
        {
            InitializeComponent();         
        }

        public void GetImag(Bitmap bitmap)
        {
            // 检查是否需要Invoke
            if (pictureBox1.InvokeRequired)
            {
                pictureBox1?.Invoke(new Action(() => GetImag(bitmap)));
                return;
            }
            bitmap.SetResolution(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bitmap;
            pictureBox1.Update();
        }

        private void GYCamControl_Load(object sender, EventArgs e)
        {
            //GYCamTool.GetInstance().InitCam();
            //GYCamTool.GetInstance().ConnectCam();

            GYCamTool.GetInstance().SendImagHandler += GetImag;
        }
    }
    public class GYCamData
    {
        public int Version_CCD = 1; //单目1   双目2
        public Thread liveThread;   //视频线程
        public bool bStop = false;  //停止视频线程标志
        public byte[] rgb8;         //存放8位rgb数据
        public ushort[] rgb16;      //存放16位rgb数据
        public double[] XYZs;       //存放XYZ三刺激值数据
        public double[] YYYs;       //存放Y数据

        public float[,] Xs;        //存放X数据的二维数组，第一维为列
        public float[,] Ys;        //存放X数据的二维数组，第一维为列
        public float[,] Zs;        //存放X数据的二维数组，第一维为列
        public Bitmap disBMP;       //用于显示画面
        public UInt32 ccdW;         //ccd宽
        public UInt32 ccdH;         //ccd高
        public byte[] model = new byte[64];
        
    }
    public class GYCamTool
    {
        #region 单例
        /// <summary>
        /// 单例
        /// </summary>
        private static GYCamTool instance = null;
        private static readonly object syncRoot = new object();
        public static GYCamTool GetInstance()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new GYCamTool();
                }
            }
            return instance;
        }
        #endregion
        public event Action<Bitmap> SendImagHandler;

        GYCamData camData = new GYCamData();

        /// <summary>
        /// 初始化相机
        /// </summary>
        /// <returns></returns>
        public string InitCam()
        {
            byte[] serialNum = new byte[100];
            int nRet = GYCam_Mini.GY_SDKInit(serialNum);//SDK_初始化SDK，验证加密狗，若无加密狗则验证绑定仪器序列号的dll文件，开软件只需要运行一次
            if (nRet != 0)
            {
                return "初始化失败";
            }
            string serialStr = System.Text.Encoding.ASCII.GetString(serialNum);
            return serialStr;
        }
        /// <summary>
        /// 关闭相机
        /// </summary>
        public void CloseCam()
        {
            camData.bStop = false;
            StopLive();
        }
        /// <summary>
        /// 连接相机
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public string ConnectCam(string FilePath = "D:\\华阳流媒体后视镜测试程序\\新建文件夹\\DA6277781")
        {
            int ret = GYCam_Mini.GY_CCDConnect(ref camData.ccdW, ref camData.ccdH, camData.model);
            if (ret != 0)
            {
                return "连接失败";
            }
            GYCam_Mini.GY_SetFilterPos(1);

            camData.rgb8 = new byte[camData.ccdW * camData.ccdH * 3];
            camData.rgb16 = new ushort[camData.ccdW * camData.ccdH * 3];
            camData.XYZs = new double[camData.ccdW * camData.ccdH * 3];
            camData.YYYs = new double[camData.ccdW * camData.ccdH];

            camData.Xs = new float[camData.ccdW, camData.ccdH];
            camData.Ys = new float[camData.ccdW, camData.ccdH];
            camData.Zs = new float[camData.ccdW, camData.ccdH];

            string strMode = System.Text.Encoding.ASCII.GetString(camData.model);

            GYCam_Mini.GY_SetFactorCali(FilePath);//SDK_设置出厂校准文件路径并读取校准数据
            camData.liveThread = new Thread(LiveFunc);
            camData.liveThread.Start();

            return strMode;
        }
        /// <summary>
        /// 相机1采图线程
        /// </summary>
        public void LiveFunc()
        {
            do
            {
                if (camData.bStop == true)
                {
                    break;
                }

                int ret = GYCam_Mini.GY_GetDeviceLiveFrame(camData.rgb8); //SDK_获取一帧视频数据
                Point[] pps = new Point[1];
                pps[0].X = (int)(camData.ccdW / 2);
                pps[0].Y = (int)(camData.ccdH / 2);
                int radius = 50;
                Byte maxV = 0; ;
                byte[] avgR = new byte[1];
                byte[] avgG = new byte[1];
                byte[] avgB = new byte[1];
                byte[] maxR = new byte[1];
                byte[] maxG = new byte[1];
                byte[] maxB = new byte[1];
                GYCam_Mini.CalCircleAvgMaxRGB8(camData.rgb8, camData.ccdW, camData.ccdH, pps, 1, radius, avgR, avgG, avgB, maxR, maxG, maxB, ref maxV);
                RGB2BGR8(camData.rgb8); //为了转成bgr给picturebox显示
                if (ret == 0 || ret == -314)
                {
                    camData.disBMP = ByteToBitmap(camData.rgb8, camData.ccdW, camData.ccdH); //byte数组转bitmap
                    SendImagHandler?.Invoke(camData.disBMP);
                }
                else
                {
                    Thread.Sleep(1);
                }
            } while (!(camData.bStop == true));
            camData.bStop = false;
        }
        /// <summary>
        /// 设置相机参数
        /// </summary>
        /// <param name="ExpoValue">曝光</param>
        /// <param name="Gain">增益</param>
        /// <param name="XExpo">X曝光</param>
        /// <param name="YExpo">Y曝光</param>
        /// <param name="ZExpo">Z曝光</param>
        public void SetExpoGain(string ExpoValue, string Gain, string XExpo, string YExpo, string ZExpo)
        {
            GYCam_Mini.GY_SetDeviceExposure(Convert.ToUInt32(ExpoValue));//设置曝光

            uint expo = 0;
            GYCam_Mini.GY_GetDeviceExposure(ref expo);//获取曝光值

            GYCam_Mini.GY_SetDeviceGain(Convert.ToUInt32(Gain));//设置增益

            uint[] filterExps = new uint[3];
            filterExps[0] = Convert.ToUInt32(XExpo);// 设置X曝光
            filterExps[1] = Convert.ToUInt32(YExpo);// 设置Y曝光
            filterExps[2] = Convert.ToUInt32(ZExpo);// 设置Z曝光
            GYCam_Mini.GY_SetDeviceExposure_Filter(filterExps);
        }

        /// <summary>
        /// 获取亮色度数据
        /// </summary>
        public List<TestData> GetDeviceSingleFrameAutoExp()
        {
            uint[] filterExps = new uint[3];
            uint[] filterExps2 = new uint[3];
            Point[] pps = new Point[1];
            pps[0].X = (int)camData.ccdW / 2;
            pps[0].Y = (int)camData.ccdH / 2;
            int radius2 = 50;
            ushort[] blackAD16 = new ushort[camData.ccdW * camData.ccdH];
            StopLive();  //停止视频模式

            GYCam_Mini.GY_GetDeviceSingleFrameAutoExp(1, false, "LCD", 0, false, true, pps, radius2, blackAD16, camData.rgb16, camData.rgb8, camData.XYZs);

            RGB2BGR8(camData.rgb8); //为了转成bgr给picturebox显示
            camData.disBMP = ByteToBitmap(camData.rgb8, camData.ccdW, camData.ccdH); //byte数组转bitmap
            // 发送图片
            SendImagHandler?.Invoke(camData.disBMP);

            int cenOrg = (int)(camData.ccdH / 2 * camData.ccdW * 3 + camData.ccdW / 2 * 3);
            //以下三行为XYZ取中心点数据转Lxy显示, GY_GetDeviceSingleFrame

            uint[] centerXp = new uint[1] { camData.ccdW / 2 };
            uint[] centerYp = new uint[1] { camData.ccdH / 2 };
            uint[] rectW = new uint[1] { 200 };
            uint[] rectH = new uint[1] { 200 };
            int[] radius = new int[1] { 200 };
            double[,] mcErr = new double[2, 3];

            mcErr[0, 0] = 0.0;
            mcErr[0, 1] = 0.0;
            mcErr[0, 2] = 0.0;
            mcErr[1, 0] = 1;
            mcErr[1, 1] = 0;
            mcErr[1, 2] = 0;

            // 3. 分配输出参数内存
            double[] Ls = new double[1]; // 亮度
            double[] x_s = new double[1];// x色坐标
            double[] y_s = new double[1];// y色坐标
            double[] us = new double[1]; // u'坐标
            double[] vs = new double[1]; // v'坐标
            double[] TCs = new double[1];// 色温
            double[] Doms = new double[1];// 主波长
            double[] Xs = new double[1];// 平均X
            double[] Ys = new double[1];// 平均Y
            double[] Zs = new double[1];// 平均Z
            double[] unifs = new double[1];// 均匀性
            double[] MaxL = new double[1];// 最大亮度
            double[] MinL = new double[1];// 最小亮度
            double[] area = new double[1];// 面积
            //uint[] maxX = new uint[1];
            //uint[] minX = new uint[1];
            //uint[] maxY = new uint[1];
            //uint[] minY = new uint[1];
            // 获取亮色度数据
            //GYCam_Mini.CalRectXYZCIEFromXYZ(camData.XYZs, camData.ccdW, camData.ccdH, centerXp, centerYp, rectW, rectH, 1, mcErr, Ls, x_s, y_s, unifs, MaxL, MinL, area, us, vs, TCs, Doms, Xs, Ys, Zs, maxX, minX, maxY, minY);

            GYCam_Mini.CalCircleXYZCIEFromXYZ(camData.XYZs, camData.ccdW, camData.ccdH, centerXp, centerYp, radius, 1, mcErr, Ls, x_s, y_s, unifs, MaxL, MinL, area, us, vs, TCs, Doms, Xs, Ys, Zs);

            double[] Ls2 = new double[1];

            List<TestData> list = new List<TestData>();
            list.Clear();
            list.Add(new TestData {亮度 = Math.Round(Ls[0], 2).ToString(),X色坐标= Math.Round(x_s[0], 4).ToString() ,Y色坐标 = Math.Round(y_s[0], 4).ToString() });

            //list.Add(Math.Round(Ls[0], 2).ToString());
            //list.Add(Math.Round(x_s[0], 4).ToString());
            //list.Add(Math.Round(y_s[0], 4).ToString());

            GC.Collect();
            return list;
        }
        /// <summary>
        /// 停止采集
        /// </summary>
        private void StopLive()
        {
            if (camData.ccdW == 0 || (ReferenceEquals(camData.liveThread, null))) //未连接仪器或未开启视频线程
            {
                return;
            }
            if (camData.liveThread.IsAlive == true)
            {
                camData.bStop = true;
                while (camData.bStop == true)
                {
                    Application.DoEvents();
                }
            }
        }
        /// <summary>
        /// 开启实时采集
        /// </summary>
        public void VideoStart()
        {
            camData.liveThread = new Thread(LiveFunc);
            camData.liveThread.Start();
        }
        //byte数组转bitmap
        private dynamic ByteToBitmap(byte[] rawValues, uint w, uint h)
        {
            Bitmap bmp = new Bitmap((int)w, (int)h, PixelFormat.Format24bppRgb);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, (int)w, (int)h), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            int stride = bmpData.Stride;
            int offset = (int)(stride - w * 3);
            IntPtr iptr = bmpData.Scan0;
            int scanBytes = (int)(stride * h);

            int posScan = 0;
            int posReal = 0;
            byte[] pixelValues = new byte[scanBytes];

            for (var y = 0; y <= h - 1; y++)
            {
                for (var x = 0; x <= w * 3 - 1; x++)
                {
                    pixelValues[posScan] = rawValues[posReal];
                    posScan++;
                    posReal++;
                }
                posScan += offset;
            }

            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, iptr, scanBytes);
            bmp.UnlockBits(bmpData);

            return bmp;
        }
        //RGB转BGR
        private void RGB2BGR8(byte[] dstRGB)
        {
            byte tempByte = 0;
            for (var i = 0; i <= dstRGB.Count() - 1; i += 3)
            {
                tempByte = dstRGB[(int)i];
                dstRGB[(int)i] = dstRGB[i + 2];
                dstRGB[i + 2] = tempByte;
            }
        }
        public class TestData
        {
            public string 亮度 { get; set; }
            public string X色坐标 { get; set; }
            public string Y色坐标 { get; set; }
        }

    }
}
