using Helper;
using MvCameraControl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace HY_Rearview_Mirror.Functions
{
    public class CameraController : IDisposable
    {
        readonly DeviceTLayerType enumTLayerType = DeviceTLayerType.MvGigEDevice | DeviceTLayerType.MvUsbDevice
            | DeviceTLayerType.MvGenTLGigEDevice | DeviceTLayerType.MvGenTLCXPDevice
            | DeviceTLayerType.MvGenTLCameraLinkDevice | DeviceTLayerType.MvGenTLXoFDevice;

        List<IDeviceInfo> deviceInfoList = new List<IDeviceInfo>();
        public Dictionary<string, MyCamera> MyCameraArr = new Dictionary<string, MyCamera>();
        private bool disposed = false;

        public CameraController()
        {
            SDKSystem.Initialize();
        }

        // 新增Dispose方法
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // 释放托管资源
                    foreach (MyCamera myCamera in MyCameraArr.Values)
                    {
                        myCamera.CloseCam();
                    }
                    MyCameraArr.Clear();
                    SDKSystem.Finalize();
                }
                disposed = true;
            }
        }
        /// <summary>
        /// 扫描所有相机
        /// </summary>
        public void RefreshDeviceList()
        {
            deviceInfoList.Clear();
            MyCameraArr.Clear();

            int nRet = DeviceEnumerator.EnumDevices(enumTLayerType, out deviceInfoList);
            if (nRet != MvError.MV_OK)
            {
                global.writeLogSystem.Error("EnumDevices failed！");
                return;
            }

            for (int i = 0; i < deviceInfoList.Count; i++)
            {
                IDeviceInfo deviceInfo = deviceInfoList[i];
                string displayName = deviceInfo.UserDefinedName != ""
                    ? deviceInfo.UserDefinedName
                    : $"{deviceInfo.ManufacturerName}_{deviceInfo.ModelName}";

                MyCameraArr.Add(displayName, new MyCamera(deviceInfo, displayName));
            }
        }
        /// <summary>
        /// 打开相机
        /// </summary>
        /// <param name="name"></param>
        public void OpenCamDevice(string name)
        {
            if (MyCameraArr.TryGetValue(name, out MyCamera myCamera))
            {
                myCamera.Open();
            }
            else
            {
                global.writeLogSystem.Error($"Camera '{name}' not found!");
            }
        }
        /// <summary>
        /// 开始取流
        /// </summary>
        /// <param name="name"></param>
        public void CamDeviceStartGrab(string name)
        {
            if (MyCameraArr.TryGetValue(name, out MyCamera myCamera))
            {
                myCamera.StartGrab();
            }
        }
        /// <summary>
        /// 停止取流
        /// </summary>
        /// <param name="name"></param>
        public void CamDeviceStopGrab(string name)
        {
            if (MyCameraArr.TryGetValue(name, out MyCamera myCamera))
            {
                myCamera.StopGrab();
            }
        }
        /// <summary>
        /// 设置连续采集
        /// </summary>
        /// <param name="name"></param>
        public void CamDeviceContinuesMode(string name)
        {
            if (MyCameraArr.TryGetValue(name, out MyCamera myCamera))
            {
                myCamera.ContinuesMode();
            }
        }
        /// <summary>
        /// 设置软触发
        /// </summary>
        /// <param name="name"></param>
        public void CamDeviceSoftTrigger(string name)
        {
            if (MyCameraArr.TryGetValue(name, out MyCamera myCamera))
            {
                myCamera.SoftTrigger();
            }
        }
        /// <summary>
        /// 软触发一次
        /// </summary>
        /// <param name="name"></param>
        public void CamDeviceTriggerOnce(string name)
        {
            if (MyCameraArr.TryGetValue(name, out MyCamera myCamera))
            {
                myCamera.TriggerOnce();
            }
        }

        public class MyCamera
        {
            IDevice device = null;
            IDeviceInfo deviceInfo = null;
            private string cameraName;

            // 使用 volatile 确保多线程可见性
            private volatile bool isGrabbing = false;
            private Thread receiveThread = null;
            private ManualResetEvent threadExitEvent = new ManualResetEvent(false);  // 新增退出同步事件
            private readonly object threadLock = new object();  // 新增线程操作锁

            //private IFrameOut frameForSave;
            private readonly object saveImageLock = new object();

            public event EventHandler<Bitmap> FrameReceived;

            public MyCamera(IDeviceInfo _deviceInfo, string name)
            {
                deviceInfo = _deviceInfo;
                cameraName = name;
            }

            public void Open()
            {
                try
                {
                    device = DeviceFactory.CreateDevice(deviceInfo);
                }
                catch (Exception ex)
                {
                    global.writeLogSystem.Error($"[{cameraName}] Create Device fail! {ex}");
                    return;
                }

                int result = device.Open();
                if (result != MvError.MV_OK)
                {
                    global.writeLogSystem.Error($"[{cameraName}] Open Device fail! Error=0x{result:X}");
                    return;
                }

                if (device is IGigEDevice gigEDevice)
                {
                    int optionPacketSize;
                    result = gigEDevice.GetOptimalPacketSize(out optionPacketSize);
                    if (result == MvError.MV_OK)
                    {
                        result = device.Parameters.SetIntValue("GevSCPSPacketSize", (long)optionPacketSize);
                        if (result != MvError.MV_OK)
                        {
                            global.writeLogSystem.Error($"[{cameraName}] Set Packet Size failed!");
                        }
                    }
                    else
                    {
                        global.writeLogSystem.Error($"[{cameraName}] Get Packet Size failed!");
                    }
                }

                device.Parameters.SetEnumValueByString("AcquisitionMode", "Continuous");
                device.Parameters.SetEnumValueByString("TriggerMode", "Off");
            }

            public void StartGrab()
            {
                lock (threadLock) // 防止重复启动
                {
                    if (isGrabbing || receiveThread?.IsAlive == true)
                    {
                        global.writeLogSystem.Error($"[{cameraName}] Already grabbing, skip!");
                        return;
                    }

                    isGrabbing = true;
                    threadExitEvent.Reset(); // 重置退出事件

                    try
                    {
                        receiveThread = new Thread(ReceiveThreadProcess)
                        {
                            IsBackground = false,
                            Name = $"CameraThread_{cameraName}" // 命名方便调试
                        };
                        receiveThread.Start();
                    }
                    catch (Exception ex)
                    {
                        global.writeLogSystem.Error($"[{cameraName}] Start thread failed! {ex}");
                        StopGrabInternal(); // 清理状态
                        throw;
                    }

                    int result = device.StreamGrabber.StartGrabbing();
                    if (result != MvError.MV_OK)
                    {
                        global.writeLogSystem.Error($"[{cameraName}] Start Grabbing fail! Error=0x{result:X}");
                        StopGrabInternal(); // 启动失败必须清理线程
                        return;
                    }
                }
            }

            private void ReceiveThreadProcess()
            {
                try
                {
                    while (isGrabbing)
                    {
                        try
                        {
                            IFrameOut frameOut;
                            int nRet = device.StreamGrabber.GetImageBuffer(1000, out frameOut); // 保持超时

                            if (nRet == MvError.MV_OK)
                            {
                                try
                                {
                                    // 创建完全独立的Bitmap副本
                                    using (Bitmap originalBitmap = frameOut.Image.ToBitmap())
                                    {
                                        // 深拷贝：创建新的Bitmap对象，复制所有像素数据
                                        Bitmap clonedBitmap = new Bitmap(originalBitmap);

                                        // 触发事件（传递副本，事件接收方负责释放）
                                        FrameReceived?.Invoke(this, clonedBitmap);
                                    }
                                }
                                catch (Exception e)
                                {
                                    global.writeLogSystem.Error($"[{cameraName}] Bitmap processing error: {e.Message}");
                                }
                                finally
                                {
                                    // 确保释放帧缓存
                                    device.StreamGrabber.FreeImageBuffer(frameOut);
                                }
                            }
                            else if (nRet == MvError.MV_E_NODATA)
                            {
                                Thread.Sleep(5);
                            }
                            else
                            {
                                Thread.Sleep(10);
                            }
                        }
                        catch (Exception ex)
                        {
                            global.writeLogSystem.Error($"[{cameraName}] Inner loop exception: {ex.Message}");
                            Thread.Sleep(100); // 异常后延迟，防止CPU飙高
                        }
                    }
                }
                catch (Exception fatalEx)
                {
                    global.writeLogSystem.Error($"[{cameraName}] FATAL thread exception: {fatalEx}");
                }
                finally
                {
                    threadExitEvent.Set(); // 确保通知主线程
                }
            }

            public void StopGrab()
            {
                lock (threadLock)
                {
                    if (!isGrabbing) return;

                    // 正确顺序：先停止硬件采集，再设置标志，最后等待线程
                    int result = device.StreamGrabber.StopGrabbing();
                    if (result != MvError.MV_OK)
                    {
                        global.writeLogSystem.Error($"[{cameraName}] Stop Grabbing fail! Error=0x{result:X}");
                    }

                    isGrabbing = false; // 设置标志

                    // 带超时的安全等待
                    if (receiveThread?.IsAlive == true)
                    {
                        if (!threadExitEvent.WaitOne(5000)) // 等待最多5秒
                        {
                            global.writeLogSystem.Error($"[{cameraName}] Warning: Thread did not exit gracefully!");
                            // 不要 Abort，记录状态即可
                        }
                        receiveThread = null;
                    }
                }
            }

            /// <summary>
            /// 内部清理方法，用于启动失败时恢复状态
            /// </summary>
            private void StopGrabInternal()
            {
                isGrabbing = false;
                if (receiveThread?.IsAlive == true)
                {
                    threadExitEvent.WaitOne(1000);
                }
                receiveThread = null;
            }

            public void CloseCam()
            {
                StopGrab(); // 确保停止取流

                if (device != null)
                {
                    device.Close();
                    device.Dispose();
                    device = null; // 防止重复释放
                }

                threadExitEvent?.Dispose(); // 释放同步对象
                threadExitEvent = null;
            }
            /// <summary>
            /// 连续采集
            /// </summary>
            public void ContinuesMode()
            {
                device?.Parameters.SetEnumValueByString("TriggerMode", "Off");
            }
            /// <summary>
            /// 设置软触发
            /// </summary>
            public void SoftTrigger()
            {
                device?.Parameters.SetEnumValueByString("TriggerMode", "On");
                device?.Parameters.SetEnumValueByString("TriggerSource", "Software");
            }

            public void TriggerOnce()
            {
                if (device == null) return;

                int result = device.Parameters.SetCommandValue("TriggerSoftware");
                if (result != MvError.MV_OK)
                {
                    global.writeLogSystem.Error($"[{cameraName}] Trigger Software fail! Error=0x{result:X}");
                }
            }
        }
    }
}