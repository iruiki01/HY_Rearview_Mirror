using HslCommunication;
using HY_Rearview_Mirror.InterfaceTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using USB2XXX;

namespace HY_Rearview_Mirror.CAN
{
    /// <summary>
    /// CAN消息封装类
    /// </summary>
    public class CANMessage
    {
        public uint ID { get; set; }
        public byte[] Data { get; set; }
        public byte DataLen { get; set; }
        public uint TimeStamp { get; set; }
        public bool IsExtendedFrame { get; set; }
        public bool IsRemoteFrame { get; set; }
    }
    public class CanTool
    {
        public CanChannel Channel { get; }

        private Int32 devHandle;
        private Int32[] devHandles;
        private Byte writeCANIndex;
        private Byte readCANIndex;
        private static bool isInitialized;
        private bool isReceiving;
        private static bool isRunOne = true;

        public event EventHandler<CANMessage> MessageReceivedHandler;

        private bool isProcessing = true;

        private Thread messageProcessingThread;

        public CanTool(CanChannel channel)
        {
            Channel = channel;

            devHandles = new Int32[20];

            isReceiving = false;

            if (isRunOne)
            {
                isInitialized = false;
                isRunOne = false;
                try
                {
                    if (!Initialize())
                    {
                        isRunOne = true;
                        return;
                    }
                }
                catch (Exception)
                {
                    isRunOne = true;
                    return;
                }
            }

            if (channel == CanChannel.CAN1)
            {
                writeCANIndex = 0;
                readCANIndex = 0;
                StartReceiving(out string error);  // 启动硬件接收
            }
            else if (channel == CanChannel.CAN2)
            {
                writeCANIndex = 1;
                readCANIndex = 1;
                StartReceiving(out string error);  // 启动硬件接收
            }
            isProcessing = true;
            messageProcessingThread = new Thread(MessageProcessingLoop)
            {
                IsBackground = true,
                Name = "CAN_MessageProcessor"
            };
            messageProcessingThread.Start();
        }

        /// <summary>
        /// 初始化CAN设备
        /// </summary>
        /// <param name="baudRate">波特率，默认500Kbps</param>
        /// <param name="errorMessage">错误信息输出</param>
        /// <returns>是否初始化成功</returns>
        public bool Initialize(uint baudRate = 500000)
        {
            string errorMessage = string.Empty;

            // 1. 扫描设备
            Int32 devNum = usb_device.USB_ScanDevice(devHandles);
            if (devNum <= 0)
            {
                errorMessage = "No device connected!";
                return false;
            }
            Console.WriteLine($"Have {devNum} device(s) connected!");

            devHandle = devHandles[0];

            // 2. 打开设备
            if (!usb_device.USB_OpenDevice(devHandle))
            {
                errorMessage = "Open device error!";
                return false;
            }
            Console.WriteLine("Open device success!");

            // 3. 获取设备信息
            usb_device.DEVICE_INFO devInfo = new usb_device.DEVICE_INFO();
            StringBuilder funcStr = new StringBuilder(256);
            if (!usb_device.DEV_GetDeviceInfo(devHandle, ref devInfo, funcStr))
            {
                errorMessage = "Get device information error!";
                return false;
            }

            //PrintDeviceInfo(devInfo, funcStr);

            // 4. 配置CAN参数
            USB2CAN.CAN_INIT_CONFIG canConfig = new USB2CAN.CAN_INIT_CONFIG
            {
                CAN_BRP = 12,
                CAN_SJW = 1,
                CAN_BS1 = 5,
                CAN_BS2 = 2,
                CAN_Mode = 0x80, // 正常模式并接入终端电阻
                CAN_ABOM = 1,
                CAN_NART = 0,
                CAN_RFLM = 0,
                CAN_TXFP = 1
            };

            // 5. 获取并设置波特率
            int ret = USB2CAN.CAN_GetCANSpeedArg(devHandle, ref canConfig, baudRate);
            if (ret != USB2CAN.CAN_SUCCESS)
            {
                errorMessage = $"Get CAN Speed failed! Error code: {ret}";
                return false;
            }
            Console.WriteLine("Get CAN Speed Success!");

            // 6. 初始化CAN通道
            ret = USB2CAN.CAN_Init(devHandle, writeCANIndex, ref canConfig);
            if (ret != USB2CAN.CAN_SUCCESS)
            {
                errorMessage = $"Config CAN Write channel failed! Error code: {ret}";
                return false;
            }
            Console.WriteLine("Config CAN Write channel Success!");

            ret = USB2CAN.CAN_Init(devHandle, readCANIndex, ref canConfig);
            if (ret != USB2CAN.CAN_SUCCESS)
            {
                errorMessage = $"Config CAN Read channel failed! Error code: {ret}";
                return false;
            }
            Console.WriteLine("Config CAN Read channel Success!");

            isInitialized = true;
            return true;
        }

        /// <summary>
        /// 发送CAN消息
        /// </summary>
        /// <param name="id">CAN ID</param>
        /// <param name="data">数据字节数组(最大8字节)</param>
        /// <param name="isExtendedFrame">是否为扩展帧</param>
        /// <returns>成功发送的帧数，-1表示失败</returns>
        public int Send(uint id, byte[] data, bool isExtendedFrame = false)
        {
            if (!isInitialized)
            {
                Console.WriteLine("Error: Device not initialized!");
                return -1;
            }

            // 清空发送FIFO
            USB2CAN.CAN_ClearMsg(devHandle, writeCANIndex);

            // 准备消息
            USB2CAN.CAN_MSG[] canMsg = new USB2CAN.CAN_MSG[1];

            canMsg[0] = new USB2CAN.CAN_MSG
            {
                //ExternFlag = (byte)(isExtendedFrame ? 1 : 0),
                ExternFlag = 0,          // 0=标准帧，1=扩展帧
                RemoteFlag = 0, // 数据帧
                ID = id,
                DataLen = (byte)Math.Min(data.Length, 8),
                Data = new byte[8]
            };

            //canMsg[0] = new USB2CAN.CAN_MSG
            //{
            //    ExternFlag = 0,          // 0=标准帧，1=扩展帧
            //    RemoteFlag = 0,          // 0=数据帧，1=远程帧
            //    ID = 0x7FF,              // 标准帧ID（11位）
            //    DataLen = 8,
            //    Data = new byte[] { 0x03, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
            //};

            // 复制数据
            Array.Copy(data, canMsg[0].Data, canMsg[0].DataLen);

            // 发送
            int sendedNum = USB2CAN.CAN_SendMsg(devHandle, writeCANIndex, canMsg, (uint)canMsg.Length);
            if (sendedNum >= 0)
            {
                Console.WriteLine($"Success send frames: {sendedNum}");
                return sendedNum;
            }
            else
            {
                Console.WriteLine("Send CAN data failed!");
                return -1;
            }
        }

        /// <summary>
        /// 启动CAN消息接收
        /// </summary>
        /// <param name="errorMessage">错误信息输出</param>
        /// <returns>是否启动成功</returns>
        public bool StartReceiving(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!isInitialized)
            {
                errorMessage = "Device not initialized!";
                return false;
            }

            int ret = USB2CAN.CAN_StartGetMsg(devHandle, readCANIndex);
            if (ret != USB2CAN.CAN_SUCCESS)
            {
                errorMessage = $"Start CAN receiving failed! Error code: {ret}";
                return false;
            }

            Console.WriteLine("Start CAN receiving Success!");
            isReceiving = true;
            return true;
        }



        // 在 USB2CANController 类中添加以下私有成员
        private Thread receivingThread;
        private readonly object queueLock = new object();
        private Queue<CANMessage> messageQueue = new Queue<CANMessage>();
        private bool isBackgroundThreadRunning = false;
        private ManualResetEvent stopThreadEvent = new ManualResetEvent(false);
     

        /// <summary>
        /// 工作线程：自动调用ReceiveMessages并处理消息
        /// </summary>
        public void MessageProcessingLoop()
        {
            while (isProcessing)
            {
                var messages = ReceiveMessages();  // 自动使用后台线程              
                foreach (var msg in messages)
                {
                    MessageReceivedHandler?.Invoke(this, msg);              
                }
            }
            Console.WriteLine("Message processing thread stopped.");
        }

        /// <summary>
        /// 接收CAN消息
        /// </summary>
        public List<CANMessage> ReceiveMessages(int maxMessages = 1024, int timeoutMs = 100)
        {
            // 确保硬件接收已启动
            if (!isReceiving)
            {
                Console.WriteLine("Warning: Hardware receiving not started. Call StartReceiving() first.");
                return new List<CANMessage>();
            }

            // 延迟启动后台读取线程（首次调用时自动启动）
            if (!isBackgroundThreadRunning)
            {
                StartBackgroundReceivingThread();
            }

            var messages = new List<CANMessage>();

            // 从线程安全队列中批量取出消息
            lock (queueLock)
            {
                while (messageQueue.Count > 0 && messages.Count < maxMessages)
                {
                    messages.Add(messageQueue.Dequeue());
                }
            }

            // 如果队列为空，稍作等待避免CPU空转
            if (messages.Count == 0)
            {
                Thread.Sleep(timeoutMs);
            }

            return messages;
        }

        // 启动后台线程
        private void StartBackgroundReceivingThread()
        {
            lock (queueLock)
            {
                if (isBackgroundThreadRunning) return;

                isBackgroundThreadRunning = true;
                stopThreadEvent.Reset();
                messageQueue.Clear();

                receivingThread = new Thread(ReceivingThreadLoop)
                {
                    IsBackground = true,
                    Name = "CAN_BackgroundReceiver"
                };

                receivingThread.Start();
                Console.WriteLine("Background CAN receiving thread started automatically.");
            }
        }

        // 后台接收循环
        //private void ReceivingThreadLoop()
        //{
        //    const int BUFFER_SIZE = 1024;
        //    int msgSize = Marshal.SizeOf(typeof(USB2CAN.CAN_MSG));

        //    while (!stopThreadEvent.WaitOne(0))
        //    {
        //        IntPtr bufferPtr = IntPtr.Zero;

        //        try
        //        {
        //            bufferPtr = Marshal.AllocHGlobal(msgSize * BUFFER_SIZE);

        //            // 非阻塞读取CAN消息
        //            int canNum = USB2CAN.CAN_GetMsgWithSize(devHandle, readCANIndex, bufferPtr, BUFFER_SIZE);

        //            if (canNum > 0)
        //            {
        //                // 将消息加入队列
        //                for (int i = 0; i < canNum; i++)
        //                {
        //                    //IntPtr msgPtr = new IntPtr(bufferPtr.ToInt64() + i * msgSize);
        //                    // 关键修改：使用 ToInt64 计算偏移，兼容 32/64 位
        //                    long offset = (long)i * msgSize;
        //                    IntPtr pMsg = new IntPtr(bufferPtr.ToInt64() + offset);
        //                    USB2CAN.CAN_MSG msg = (USB2CAN.CAN_MSG)Marshal.PtrToStructure(pMsg, typeof(USB2CAN.CAN_MSG));


        //                    if (msg.ID == 0x7FE)
        //                    {
        //                        Console.WriteLine("接收到 ID=0x7FE 的数据：");
        //                        for (int j = 0; j < msg.DataLen; j++)  // 建议用 DataLen 而非 Data.Length
        //                        {
        //                            Console.WriteLine("Data[{0}] = 0x{1:X2} ({2})", j, msg.Data[j], msg.Data[j]);
        //                        }
        //                    }


        //                    var canMessage = new CANMessage
        //                    {
        //                        ID = msg.ID,
        //                        DataLen = msg.DataLen,
        //                        Data = new byte[msg.DataLen],
        //                        TimeStamp = msg.TimeStamp,
        //                        IsExtendedFrame = msg.ExternFlag == 1,
        //                        IsRemoteFrame = msg.RemoteFlag == 1
        //                    };

        //                    Array.Copy(msg.Data, canMessage.Data, msg.DataLen);

        //                    lock (queueLock)
        //                    {
        //                        messageQueue.Enqueue(canMessage);
        //                    }
        //                }
        //            }
        //            else if (canNum < 0)
        //            {
        //                Console.WriteLine($"Get CAN data error! Code: {canNum}");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Background receiving error: {ex.Message}");
        //        }
        //        finally
        //        {
        //            if (bufferPtr != IntPtr.Zero)
        //            {
        //                Marshal.FreeHGlobal(bufferPtr);
        //            }
        //        }

        //        // 短暂休眠降低CPU占用
        //        Thread.Sleep(20);
        //    }

        //    Console.WriteLine("Background CAN receiving thread stopped.");
        //}

        private void ReceivingThreadLoop()
        {
            const int BUFFER_SIZE = 1024;
            int msgSize = Marshal.SizeOf(typeof(USB2CAN.CAN_MSG));

            while (!stopThreadEvent.WaitOne(0))
            {
                // 1. 每次循环申请缓冲区（按照你指定的模式）
                USB2CAN.CAN_MSG[] CanMsgBuffer = new USB2CAN.CAN_MSG[BUFFER_SIZE];
                IntPtr pt = Marshal.AllocHGlobal(msgSize * CanMsgBuffer.Length);

                try
                {
                    int CanNum = USB2CAN.CAN_GetMsgWithSize(devHandle, readCANIndex, pt, CanMsgBuffer.Length);

                    if (CanNum > 0)
                    {
                        for (int i = 0; i < CanNum; i++)
                        {
                            // 2. 使用你验证正确的64位安全指针计算方式
                            long offset = (long)i * msgSize;
                            IntPtr pMsg = new IntPtr(pt.ToInt64() + offset);

                            CanMsgBuffer[i] = (USB2CAN.CAN_MSG)Marshal.PtrToStructure(pMsg, typeof(USB2CAN.CAN_MSG));

                            // 特定ID调试输出（保留你的0x7FE检测）
                            if (CanMsgBuffer[i].ID == 0x7FE)
                            {
                                Console.WriteLine("接收到 ID=0x7FE 的数据：");
                                for (int j = 0; j < CanMsgBuffer[i].DataLen; j++)
                                {
                                    Console.WriteLine("Data[{0}] = 0x{1:X2} ({2})", j, CanMsgBuffer[i].Data[j], CanMsgBuffer[i].Data[j]);
                                }
                            }

                            // 3. 构造消息并入队（保留队列逻辑）
                            var canMessage = new CANMessage
                            {
                                ID = CanMsgBuffer[i].ID,
                                DataLen = CanMsgBuffer[i].DataLen,
                                Data = new byte[CanMsgBuffer[i].DataLen],
                                TimeStamp = CanMsgBuffer[i].TimeStamp,
                                IsExtendedFrame = CanMsgBuffer[i].ExternFlag == 1,
                                IsRemoteFrame = CanMsgBuffer[i].RemoteFlag == 1
                            };

                            if (CanMsgBuffer[i].Data != null && CanMsgBuffer[i].DataLen > 0)
                            {
                                Array.Copy(CanMsgBuffer[i].Data, canMessage.Data, CanMsgBuffer[i].DataLen);
                            }

                            lock (queueLock)
                            {
                                // 防止队列无限增长导致内存溢出
                                if (messageQueue.Count < 10000)
                                {
                                    messageQueue.Enqueue(canMessage);
                                }
                            }
                        }
                    }
                    else if (CanNum < 0)
                    {
                        Console.WriteLine("Get CAN data error! 错误码: " + CanNum);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"接收线程异常: {ex.Message}");
                }
                finally
                {
                    // 4. 确保每次循环都释放内存（关键！防止内存泄漏）
                    Marshal.FreeHGlobal(pt);
                }

                Thread.Sleep(100); // 按照你指定的模式延时
            }

            Console.WriteLine("Background CAN receiving thread stopped.");
        }

        /// <summary>
        /// 关闭CAN设备
        /// </summary>
        public void Close()
        {
            isProcessing = false;
            // 停止后台线程
            if (isBackgroundThreadRunning)
            {
                Console.WriteLine("Stopping background receiving thread...");
                stopThreadEvent.Set();
                receivingThread?.Join(2000); // 等待最多2秒
                isBackgroundThreadRunning = false;
            }

            if (isReceiving)
            {
                USB2CAN.CAN_StopGetMsg(devHandle, readCANIndex);
                isReceiving = false;
                Console.WriteLine("Stopped CAN receiving.");
            }

            if (isInitialized)
            {
                // 注意：根据实际API，可能需要调用usb_device.USB_CloseDevice
                // 如果API支持的话，这里添加关闭设备代码
                isInitialized = false;
                Console.WriteLine("CAN device closed.");
            }
        }

        public void Dispose()
        {
            Close();
        }
        #region 辅助方法
        private void PrintDeviceInfo(usb_device.DEVICE_INFO devInfo, StringBuilder funcStr)
        {
            Console.WriteLine("Firmware Info:");
            Console.WriteLine($"    Name: {Encoding.Default.GetString(devInfo.FirmwareName).TrimEnd('\0')}");
            Console.WriteLine($"    Build Date: {Encoding.Default.GetString(devInfo.BuildDate).TrimEnd('\0')}");
            Console.WriteLine($"    Firmware Version: v{(devInfo.FirmwareVersion >> 24) & 0xFF}.{(devInfo.FirmwareVersion >> 16) & 0xFF}.{devInfo.FirmwareVersion & 0xFFFF}");
            Console.WriteLine($"    Hardware Version: v{(devInfo.HardwareVersion >> 24) & 0xFF}.{(devInfo.HardwareVersion >> 16) & 0xFF}.{devInfo.HardwareVersion & 0xFFFF}");
            Console.WriteLine($"    Functions: {devInfo.Functions:X8}");
            Console.WriteLine($"    Functions String: {funcStr}");
        }

        private void PrintCANMessage(CANMessage msg, int index)
        {
            Console.WriteLine($"CanMsg[{index}].ID = 0x{msg.ID:X3}");
            Console.WriteLine($"CanMsg[{index}].TimeStamp = {msg.TimeStamp}");
            Console.Write($"CanMsg[{index}].Data = ");
            for (int j = 0; j < msg.DataLen; j++)
            {
                Console.Write($"{msg.Data[j]:X2} ");
            }
            Console.WriteLine("\n");
        }

        #endregion
    }
}
