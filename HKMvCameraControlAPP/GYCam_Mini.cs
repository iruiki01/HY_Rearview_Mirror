using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;

namespace HKMvCameraControlAPP
{
    public static class GYCam_Mini
    {
        //双目初始化，形参为:是否为双目
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern void GY_HudInit(bool rectify);

        //初始化SDK，验证加密狗，若找不到加密狗则使用绑定仪器序列号的dll进行验证，开软件只需要运行一次
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int GY_SDKInit(byte[] serialNum);

        //滤光轮色阶初始化，形参为:白红绿蓝图数据、宽、高
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
        public static extern int GY_ColorRenditionInit(byte[] pWRGB, byte[] pRRGB, byte[] pGRGB, byte[] pBRGB, uint rgbW, uint rgbH);

        //设置出厂校准文件路径并读取校准数据
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GY_SetFactorCali(string calPath);

        ////设置出厂校准文件路径并读取校准数据
        //[DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //public static extern int GY_SetFactorCali2(string calPath);

        //设置用户校准文件路径
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GY_SetUserCali(string calPath);

        //生成用户四色校准数据
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GY_GenFourCalK(double[,] camLxy, double[,] refLxy, string screenType, string genFilePath);

        //连接左仪器，获取到长宽像素及型号
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int GY_CCDConnect(ref UInt32 w,ref UInt32 h,byte[] model);

        //连接右仪器，获取到长宽像素及型号
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int GY_CCDConnect2(ref UInt32 w, ref UInt32 h, byte[] model);

        //断开左仪器
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int GY_CCDDisconnect();
        //断开右仪器
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int GY_CCDDisconnect2();

        //设置us为单位的左曝光
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
        public static extern int GY_SetDeviceExposure(uint value);

        //获取us为单位的左曝光
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
        public static extern int GY_GetDeviceExposure(ref uint value);

        //设置us为单位的右曝光
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GY_SetDeviceExposure2(uint value);

        //获取us为单位的右曝光
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GY_GetDeviceExposure2(ref uint value);

        //设置三滤光片us为单位的曝光
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GY_SetDeviceExposure_Filter(uint[] value);

        //获取三滤光片us为单位的曝光
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GY_GetDeviceExposure_Filter(uint[] value);

        //设置增益，制冷型：0~90，非制冷：0~20
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GY_SetDeviceGain(uint value);
        //设置增益，制冷型：0~90，非制冷：0~20
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GY_SetDeviceGain2(uint value);
        //获取us为单位的左增益
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GY_GetDeviceGain(ref uint value);
        //获取us为单位的右增益
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GY_GetDeviceGain2(ref uint value);

        //设置滤光轮通道
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GY_SetFilterPos(int pos);

        //获取一帧视频数据
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)] 
        public static extern int GY_GetDeviceLiveFrame(byte[] pLiveRGB);
        //获取一帧右相机视频数据
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GY_GetDeviceLiveFrameR(byte[] pLiveRGB);

        //获取一帧单通道单帧数据
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GY_GetDeviceLiveOneFrame(byte[] pLiveRGB);

        //获取一帧右相机单通道单帧数据
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GY_GetDeviceLiveOneFrameR(byte[] pLiveRGB);

        //获取一帧Y通道数据（手动/自动曝光），形参为:平均次数、是否使用用户校准数据、屏幕类型、ND、是否自动曝光、自动曝光方式、曝光点、半径、暗电流、16位RGB数据、8位RGB数据、Y数据
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GY_GetDeviceSingleYFrameAutoExp(int avgNums, bool isUseUserK, string screenType, int ND, bool isAutoExp, bool maxOrAvg, Point[] expPoint, int radius, ushort[] pBlackAD, ushort[] pSingleRGB, byte[] pDisRGB, double[] YYYs);
        
        //获取一帧滤光轮数据（手动/自动曝光），形参为:平均次数、是否使用用户校准数据、屏幕类型、ND、是否自动曝光、自动曝光方式、曝光点、半径、暗电流、16位RGB数据、8位RGB数据、XYZ数据
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GY_GetDeviceSingleFrameAutoExp(int avgNums, bool isUseUserK, string screenType, int ND, bool isAutoExp, bool maxOrAvg, Point[] expPoint, int radius, ushort[] pBlackAD, ushort[] pSingleRGB, byte[] pDisRGB, double[] XYZs);
        //获取一帧滤光轮数据（手动/自动曝光），形参为:平均次数、是否使用用户校准数据、屏幕类型、ND、是否自动曝光、自动曝光方式、曝光点、半径、暗电流、16位RGB数据、8位RGB数据、XYZ数据
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GY_GetDeviceSingleFrameZC(int avgNums, bool isUseUserK, string screenType, int ND, bool isAutoExp, bool maxOrAvg, Point[] expPoint, int radius, ushort[] pBlackAD, ushort[] pSingleRGB, byte[] pDisRGB, double[] XYZs);
        //获取一帧滤光轮数据（自动曝光，形参为:平均次数、是否使用用户校准数据、屏幕类型、ND、暗电流、16位RGB数据、8位RGB数据、XYZ数据
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GY_GetDeviceSingleFrameAutoExp2(int avgNums, bool isUseUserK, string screenType, int ND, ushort[] pBlackAD, ushort[] pSingleRGB, byte[] pDisRGB, double[] XYZs);

        //获取双目各一帧单帧数据，形参为:双目矫正、左相机曝光、右相机曝光、左相机8位RGB数据、右相机8位RGB数据
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GY_GetDeviceSingleFrameHud(bool rectify, uint exp, uint exp2, byte[] pDisRGB, byte[] pDisRGB2);

        //获取所有矩形的平均亮色度数据，形参为:XYZ数据、宽、高、矩形中心的x坐标、矩形中心的y坐标、矩形宽、矩形高、矩形数量、机差、亮度、色度x、色度y、点均匀性、最大亮度、最小亮度、面积、色度u、色度v、色温、主波长、X、Y、Z、最大点X坐标、最大点Y坐标、最小点X坐标、最小点Y坐标
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern void CalRectXYZCIEFromXYZ(double[] XYZ, uint w, uint h, uint[] x, uint[] y, uint[] ww, uint[] hh, uint num, double[,] errlist, double[] L, double[] x_s, double[] y_s, double[] unifs, double[] maxL, double[] minL, double[] area, double[] us, double[] vs, double[] Tc, double[] Dom, double[] Xs, double[] Ys, double[] Zs, uint[] maxX, uint[] minX, uint[] maxY, uint[] minY);

        //获取所有圆形的平均亮色度数据，形参为:XYZ数据、宽、高、圆中心的x坐标、圆中心的y坐标、半径、圆数量、机差、亮度、色度x、色度y、点均匀性、最大亮度、最小亮度、面积、色度u、色度v、色温、主波长、X、Y、Z、最大点X坐标、最大点Y坐标、最小点X坐标、最小点Y坐标
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern void CalCircleXYZCIEFromXYZ(double[] XYZ, uint w, uint h, uint[] x, uint[] y, int[] radius, uint num, double[,] errlist, double[] L, double[] x_s, double[] y_s, double[] unifs, double[] maxL, double[] minL, double[] area, double[] us, double[] vs, double[] Tc, double[] Dom, double[] Xs, double[] Ys, double[] Zs);


        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int CalCircleAvgMaxRGB8(byte[] pLiveRGB, uint w, uint h, Point[] expPoint, int nums, int radius, byte[] avgR, byte[] avgG, byte[] avgB, byte[] maxR, byte[] maxG, byte[] maxB, ref byte maxV);
        //[DllImport("Dll1.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //public static extern int Hud_Identify(byte[] pSingleRGB, int w, int h, byte[] pSingleRGB2);

        //[DllImport("Dll1.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //public static extern int Hud_Handle(byte[] pSingleRGB2, int w, int h, double Value);

        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int VIDTest(int rows, int cols, byte[] pDisRGB, byte[] pDisRGB2, double[] Vids);

        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int BDTest(int rows, int cols, double vid, byte[] pDisRGB, byte[] pDisRGB2, double[] Bxs, double[] Bys);

        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int MTFTest(bool isUseUserK, string screenType, int ND, bool isAutoExp, Point[] expPoint, int radius, int rows, int cols, bool LineStyle, double Linearea, byte[] pDisRGB, double[] MTFsh, double[] MTFsv);

        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GamutTest(double[] Ru, double[] Rv, double[] Gu, double[] Gv, double[] Bu, double[] Bv, int nums, double[] S);

        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int LvxyTest(bool isUseUserK, string screenType, int ND, bool isAutoExp, Point[] expPoint, int radius, bool ThreOrRoi, double Thre, int RoiX, int RoiY, int RoiW, int RoiH, int rows, int cols, double radius2, byte[] pDisRGB, int[] centerXp, int[] centerYp, int[] Tradius, double[] Ls, double[] xs, double[] ys, double[] us, double[] vs, double[] unif);
        
        [DllImport("GYCam_MiniSDK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int findDotOffset(byte[] rgbs, int w, int h, double thre, int offsetX, int offsetY, int[] PointX, int[] PointY, int nums);
    }
}
