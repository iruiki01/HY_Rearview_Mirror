using MvCameraControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HKMvCameraControlAPP
{
    public partial class Form1 : Form
    {
        readonly DeviceTLayerType enumTLayerType = DeviceTLayerType.MvGigEDevice | DeviceTLayerType.MvUsbDevice
            | DeviceTLayerType.MvGenTLGigEDevice | DeviceTLayerType.MvGenTLCXPDevice | DeviceTLayerType.MvGenTLCameraLinkDevice | DeviceTLayerType.MvGenTLXoFDevice;

        List<IDeviceInfo> deviceInfoList = new List<IDeviceInfo>();
        IDevice device = null;

        bool isGrabbing = false;        // ch:是否正在取图 | en: Grabbing flag
        Thread receiveThread = null;    // ch:接收图像线程 | en: Receive image thread

        private IFrameOut frameForSave;                         // ch:获取到的帧信息, 用于保存图像 | en:Frame for save image
        private readonly object saveImageLock = new object();
        public Form1()
        {
            InitializeComponent();
            SDKSystem.Initialize();

            RefreshDeviceList();
            Control.CheckForIllegalCrossThreadCalls = false;
        }
        ~Form1() 
        {
            CloseCam();
            SDKSystem.Finalize();
        }
        /// <summary>
        /// ch:程序关闭事件，释放SDK资源 | en:FormClosing, dispose SDK resources
        /// </summary>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseCam();
            SDKSystem.Finalize();
        }
        private void RefreshDeviceList()
        {
            // ch:创建设备列表 | en:Create Device List
            //cbDeviceList.Items.Clear();
            int nRet = DeviceEnumerator.EnumDevices(enumTLayerType, out deviceInfoList);
            if (nRet != MvError.MV_OK)
            {
                return;
            }

            // ch:在窗体列表中显示设备名 | en:Display device name in the form list
            for (int i = 0; i < deviceInfoList.Count; i++)
            {
                IDeviceInfo deviceInfo = deviceInfoList[i];
                if (deviceInfo.UserDefinedName != "")// 用户自定义相机名称
                {
                    //cbDeviceList.Items.Add(deviceInfo.TLayerType.ToString() + ": " + deviceInfo.UserDefinedName + " (" + deviceInfo.SerialNumber + ")");

                }
                else// 相机自带的名称
                {
                    //cbDeviceList.Items.Add(deviceInfo.TLayerType.ToString() + ": " + deviceInfo.ManufacturerName + " " + deviceInfo.ModelName + " (" + deviceInfo.SerialNumber + ")");
                }
            }

            // ch:选择第一项 | en:Select the first item
            if (deviceInfoList.Count != 0)
            {
                //cbDeviceList.SelectedIndex = 0;
            }
        }
        private void Open(int id)
        {

            // ch:获取选择的设备信息 | en:Get selected device information
            IDeviceInfo deviceInfo = deviceInfoList[id];

            try
            {
                // ch:打开设备 | en:Open device
                device = DeviceFactory.CreateDevice(deviceInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Create Device fail!" + ex.Message);
                return;
            }

            int result = device.Open();
            if (result != MvError.MV_OK)
            {
                //ShowErrorMsg("Open Device fail!", result);
                return;
            }

            //ch: 判断是否为gige设备 | en: Determine whether it is a GigE device
            if (device is IGigEDevice)
            {
                //ch: 转换为gigE设备 | en: Convert to Gige device
                IGigEDevice gigEDevice = device as IGigEDevice;

                // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
                int optionPacketSize;
                result = gigEDevice.GetOptimalPacketSize(out optionPacketSize);
                if (result != MvError.MV_OK)
                {
                    //ShowErrorMsg("Warning: Get Packet Size failed!", result);
                }
                else
                {
                    result = device.Parameters.SetIntValue("GevSCPSPacketSize", (long)optionPacketSize);
                    if (result != MvError.MV_OK)
                    {
                        //ShowErrorMsg("Warning: Set Packet Size failed!", result);
                    }
                }
            }

            // ch:设置采集连续模式 | en:Set Continues Aquisition Mode
            device.Parameters.SetEnumValueByString("AcquisitionMode", "Continuous");
            device.Parameters.SetEnumValueByString("TriggerMode", "Off");

            // ch:控件操作 | en:Control operation
            //SetCtrlWhenOpen();

            // ch:获取参数 | en:Get parameters
            //bnGetParam_Click(null, null);
        }
        /// <summary>
        /// 开始取流
        /// </summary>
        private void StartGrab()
        {
            try
            {
                // ch:标志位置位true | en:Set position bit true
                isGrabbing = true;

                receiveThread = new Thread(ReceiveThreadProcess);
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Start thread failed!, " + ex.Message);
                throw;
            }

            // ch:开始采集 | en:Start Grabbing
            int result = device.StreamGrabber.StartGrabbing();
            if (result != MvError.MV_OK)
            {
                isGrabbing = false;
                receiveThread.Join();
                //ShowErrorMsg("Start Grabbing Fail!", result);
                return;
            }

            // ch:控件操作 | en:Control Operation
            //SetCtrlWhenStartGrab();
        }

        /// <summary>
        /// 取流线程
        /// </summary>
        public void ReceiveThreadProcess()
        {
            int nRet;

            Graphics graphics = null;   // ch:使用GDI在pictureBox上绘制图像 | en:Display frame using a graphics

            while (isGrabbing)
            {
                IFrameOut frameOut;

                nRet = device.StreamGrabber.GetImageBuffer(1000, out frameOut);
                if (MvError.MV_OK == nRet)
                {

                    lock (saveImageLock)
                    {
                        try
                        {
                            frameForSave = frameOut.Clone() as IFrameOut;
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("IFrameOut.Clone failed, " + e.Message);
                            return;
                        }
                    }

#if GDI_RENDER
                    device.ImageRender.DisplayOneFrame(pictureBox1.Handle, frameOut.Image);
#else
                    // 使用GDI绘制图像
                    try
                    {
                        using (Bitmap bitmap = frameOut.Image.ToBitmap())
                        {
                            if (graphics == null)
                            {
                                graphics = pictureBox1.CreateGraphics();
                            }

                            Rectangle srcRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                            Rectangle dstRect = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
                            graphics.DrawImage(bitmap, dstRect, srcRect, GraphicsUnit.Pixel);
                        }
                    }
                    catch (Exception e)
                    {
                        device.StreamGrabber.FreeImageBuffer(frameOut);
                        MessageBox.Show(e.Message);
                        return;
                    }
#endif


                    device.StreamGrabber.FreeImageBuffer(frameOut);
                }
                else
                {
                    //if (bnTriggerMode.Checked)
                    //{
                    //    Thread.Sleep(5);
                    //}
                    //Thread.Sleep(5);
                }
            }
        }

        private void CloseCam()
        {
            // ch:取流标志位清零 | en:Reset flow flag bit
            if (isGrabbing == true)
            {
                StopGrab();
            }

            // ch:关闭设备 | en:Close Device
            if (device != null)
            {
                device.Close();
                device.Dispose();
            }

            // ch:控件操作 | en:Control Operation
            //SetCtrlWhenClose();
        }
        private void StopGrab()
        {

            // ch:标志位设为false | en:Set flag bit false
            isGrabbing = false;
            receiveThread.Join();

            // ch:停止采集 | en:Stop Grabbing
            int result = device.StreamGrabber.StopGrabbing();
            if (result != MvError.MV_OK)
            {
                //ShowErrorMsg("Stop Grabbing Fail!", result);
            }

            // ch:控件操作 | en:Control Operation
            //SetCtrlWhenStopGrab();
        }

        /// <summary>
        /// 连续取流标志
        /// </summary>
        private void ContinuesMode()
        {
            device.Parameters.SetEnumValueByString("TriggerMode", "Off");
        }
        /// <summary>
        /// 设置软触发
        /// </summary>
        private void SoftTrigger()
        {
            // ch:触发源设为软触发 | en:Set trigger source as Software
            device.Parameters.SetEnumValueByString("TriggerSource", "Software");
        }
        /// <summary>
        /// 软触发一次
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnTriggerExec_Click(object sender, EventArgs e)
        {
            // ch:触发命令 | en:Trigger command
            int result = device.Parameters.SetCommandValue("TriggerSoftware");
            if (result != MvError.MV_OK)
            {
                //ShowErrorMsg("Trigger Software Fail!", result);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Open(0);
            //ContinuesMode();
            //SoftTrigger();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RefreshDeviceList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StartGrab();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            CloseCam();
            SDKSystem.Finalize();
        }
    }
}
