
#define GET_FIRMWARE_INFO
#define CAN_MODE_LOOPBACK
#define CAN_SEND_MSG
#define CAN_GET_STATUS
#define CAN_GET_MSG
//using Ivi.Visa.Interop;
using Ivi.Visa.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using USB2XXX;


namespace HKMvCameraControlAPP
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 1. 创建资源管理器
            ResourceManager rm = new ResourceManager();

            // 2. 打开设备（资源名需与NI MAX中一致）
            // 格式：USB0::0xFFFF::0x6722::SERIAL::INSTR
            string resourceName = "USB0::0x2EC7::0x6700::802259073807670101::INSTR";
            FormattedIO488 io = new FormattedIO488();
            io.IO = (IMessage)rm.Open(resourceName, AccessMode.NO_LOCK, 2000, "");

            Console.WriteLine("设备已连接");

            // 3. 查询设备信息
            io.WriteString("*IDN?");
            string idn = io.ReadString();
            Console.WriteLine($"设备信息: {idn}");

            // 4. 设置电压和电流限值
            io.WriteString("VOLT 12.0");    // 设置12V
            io.WriteString("CURR 2.0");     // 设置2A限流
            Console.WriteLine("参数设置完成");

            // 5. 开启输出
            io.WriteString("OUTP ON");
            Console.WriteLine("输出已开启");

            // 6. 读取实际输出值
            io.WriteString("MEAS:VOLT?");
            double voltage = io.ReadNumber();
            io.WriteString("MEAS:CURR?");
            double current = io.ReadNumber();
            Console.WriteLine($"当前输出: {voltage:F3}V, {current:F3}A");

            // 7. 关闭输出并断开连接
            //io.WriteString("OUTP OFF");
            io.IO.Close();
            Console.WriteLine("设备已关闭并断开");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            usb_device.DEVICE_INFO DevInfo = new usb_device.DEVICE_INFO();
            Int32[] DevHandles = new Int32[20];
            Int32 DevHandle = 0;
            Byte WriteCANIndex = 0;
            Byte ReadCANIndex = 0;
            bool state;
            Int32 DevNum, ret;
            //扫描查找设备
            DevNum = usb_device.USB_ScanDevice(DevHandles);
            if (DevNum <= 0)
            {
                Console.WriteLine("No device connected!");
                return;
            }
            else
            {
                Console.WriteLine("Have {0} device connected!", DevNum);
            }
            DevHandle = DevHandles[0];
            //打开设备
            state = usb_device.USB_OpenDevice(DevHandle);
            if (!state)
            {
                Console.WriteLine("Open device error!");
                return;
            }
            else
            {
                Console.WriteLine("Open device success!");
            }
            //获取固件信息
            StringBuilder FuncStr = new StringBuilder(256);
            state = usb_device.DEV_GetDeviceInfo(DevHandle, ref DevInfo, FuncStr);
            if (!state)
            {
                Console.WriteLine("Get device infomation error!");
                return;
            }
            else
            {
                Console.WriteLine("Firmware Info:");
                Console.WriteLine("    Name:" + Encoding.Default.GetString(DevInfo.FirmwareName));
                Console.WriteLine("    Build Date:" + Encoding.Default.GetString(DevInfo.BuildDate));
                Console.WriteLine("    Firmware Version:v{0}.{1}.{2}", (DevInfo.FirmwareVersion >> 24) & 0xFF, (DevInfo.FirmwareVersion >> 16) & 0xFF, DevInfo.FirmwareVersion & 0xFFFF);
                Console.WriteLine("    Hardware Version:v{0}.{1}.{2}", (DevInfo.HardwareVersion >> 24) & 0xFF, (DevInfo.HardwareVersion >> 16) & 0xFF, DevInfo.HardwareVersion & 0xFFFF);
                Console.WriteLine("    Functions:" + DevInfo.Functions.ToString("X8"));
                Console.WriteLine("    Functions String:" + FuncStr);
            }
            //初始化配置CAN
            //USB2CAN.CAN_INIT_CONFIG CANConfig = new USB2CAN.CAN_INIT_CONFIG();
            // 2. 初始化CAN（参数必须与接收方一致）
            USB2CAN.CAN_INIT_CONFIG CANConfig = new USB2CAN.CAN_INIT_CONFIG
            {
                CAN_BRP = 12,
                CAN_SJW = 1,
                CAN_BS1 = 5,
                CAN_BS2 = 2,
                CAN_Mode = 0x80,
                CAN_ABOM = 1,
                CAN_NART = 0,
                CAN_RFLM = 0,
                CAN_TXFP = 1
            };

            ret = USB2CAN.CAN_GetCANSpeedArg(DevHandle, ref CANConfig, 500000);
            if (ret != USB2CAN.CAN_SUCCESS)
            {
                Console.WriteLine("Get CAN Speed failed!");
                return;
            }
            else
            {
                Console.WriteLine("Get CAN Speed Success!");
            }
#if CAN_MODE_LOOPBACK
            //CANConfig.CAN_Mode = 0;//环回模式
#else
            CANConfig.CAN_Mode = 0x80;//正常模式并接入终端电阻
#endif
            ret = USB2CAN.CAN_Init(DevHandle, WriteCANIndex, ref CANConfig);
            if (ret != USB2CAN.CAN_SUCCESS)
            {
                Console.WriteLine("Config CAN failed!");
                return;
            }
            else
            {
                Console.WriteLine("Config CAN Success!");
            }
            ret = USB2CAN.CAN_Init(DevHandle, ReadCANIndex, ref CANConfig);
            if (ret != USB2CAN.CAN_SUCCESS)
            {
                Console.WriteLine("Config CAN failed!");
                return;
            }
            else
            {
                Console.WriteLine("Config CAN Success!");
            }
            //配置过滤器，接收所有数据
            USB2CAN.CAN_FILTER_CONFIG CANFilter = new USB2CAN.CAN_FILTER_CONFIG();
            CANFilter.Enable = 1;
            CANFilter.ExtFrame = 0;
            CANFilter.FilterIndex = 0;
            CANFilter.FilterMode = 0;
            CANFilter.MASK_IDE = 0;
            CANFilter.MASK_RTR = 0;
            CANFilter.MASK_Std_Ext = 0;
            CANFilter.ID_Std_Ext = 0x475;
            ret = USB2CAN.CAN_Filter_Init(DevHandle, ReadCANIndex, ref CANFilter);
            if (ret != USB2CAN.CAN_SUCCESS)
            {
                Console.WriteLine("Config CAN Filter failed!");
                return;
            }
            else
            {
                Console.WriteLine("Config CAN Filter Success!");
            }
            //启动CAN接收数据
            ret = USB2CAN.CAN_StartGetMsg(DevHandle, ReadCANIndex);
            if (ret != USB2CAN.CAN_SUCCESS)
            {
                Console.WriteLine("Start CAN failed!");
                return;
            }
            else
            {
                Console.WriteLine("Start CAN Success!");
            }
#if CAN_SEND_MSG//发送CAN帧
            //USB2CAN.CAN_MSG[] CanMsg = new USB2CAN.CAN_MSG[1];
            ////for(int i=0;i<5;i++)
            //{
            //    CanMsg[0] = new USB2CAN.CAN_MSG();
            //    CanMsg[0].ExternFlag = 0;
            //    CanMsg[0].RemoteFlag = 0;
            //    CanMsg[0].ID = 0x7FF;
            //    CanMsg[0].DataLen = 8;
            //    CanMsg[0].Data = new Byte[CanMsg[0].DataLen];
            //    //for(int j=0;j<CanMsg[i].DataLen;j++)
            //    {
            //        CanMsg[0].Data[0] = 0x03;
            //        CanMsg[0].Data[1] = 0x03;
            //        CanMsg[0].Data[2] = 0x00;
            //        CanMsg[0].Data[3] = 0x00;
            //        CanMsg[0].Data[4] = 0x00;
            //        CanMsg[0].Data[5] = 0x00;
            //        CanMsg[0].Data[6] = 0x00;
            //        CanMsg[0].Data[7] = 0x00;
            //    }
            //}
            // 在CAN_Init之后、CAN_SendMsg之前必须加：
            USB2CAN.CAN_ClearMsg(DevHandle, 0); // 0=通道0
            Console.WriteLine("已清空发送FIFO");

            USB2CAN.CAN_MSG[] CanMsg = new USB2CAN.CAN_MSG[1];
            //CanMsg[0] = new USB2CAN.CAN_MSG
            //{
            //    ExternFlag = 0,          // 0=标准帧，1=扩展帧
            //    RemoteFlag = 0,          // 0=数据帧，1=远程帧
            //    ID = 0x7FF,              // 标准帧ID（11位）
            //    DataLen = 8,
            //    Data = new byte[] { 0x55 , 0xAA, 0x5A, 0x5A, 0xA5, 0xA5, 0xAA, 0x55 }
            //};

            CanMsg[0] = new USB2CAN.CAN_MSG
            {
                ExternFlag = 0,          // 0=标准帧，1=扩展帧
                RemoteFlag = 0,          // 0=数据帧，1=远程帧
                ID = 0x7FF,              // 标准帧ID（11位）
                DataLen = 8,
                Data = new byte[] { 0x02, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
            };

            int SendedNum = USB2CAN.CAN_SendMsg(DevHandle, WriteCANIndex, CanMsg, (UInt32)CanMsg.Length);
            if(SendedNum >= 0){
                Console.WriteLine("Success send frames:{0}",SendedNum);
            }else{
                Console.WriteLine("Send CAN data failed!");
            }
#endif
#if CAN_GET_STATUS
            USB2CAN.CAN_STATUS CANStatus = new USB2CAN.CAN_STATUS();
            ret = USB2CAN.CAN_GetStatus(DevHandle, WriteCANIndex, ref CANStatus);
            if(ret == USB2CAN.CAN_SUCCESS){
                Console.WriteLine("TSR = {0:X8}",CANStatus.TSR);
                Console.WriteLine("ESR = {0:X8}",CANStatus.ESR);
            }else{
                Console.WriteLine("Get CAN status error!\n");
            }
#endif
            //延时
            System.Threading.Thread.Sleep(500);

#if CAN_GET_MSG
            //for (int t = 0; t < 10; t++)
            //{
            //    USB2CAN.CAN_MSG[] CanMsgBuffer = new USB2CAN.CAN_MSG[1024];
            //    //申请存储数据缓冲区
            //    IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(USB2CAN.CAN_MSG)) * CanMsgBuffer.Length);
            //    int CanNum = USB2CAN.CAN_GetMsgWithSize(DevHandle, ReadCANIndex, pt, CanMsgBuffer.Length);
            //    if (CanNum > 0)
            //    {
            //        for (int i = 0; i < CanNum; i++)
            //        {
            //            //从缓冲区中获取数据
            //            CanMsgBuffer[i] = (USB2CAN.CAN_MSG)Marshal.PtrToStructure((IntPtr)((UInt32)pt + i * Marshal.SizeOf(typeof(USB2CAN.CAN_MSG))), typeof(USB2CAN.CAN_MSG));
            //            Console.WriteLine("CanMsg[{0}].ID = {1}", i, CanMsgBuffer[i].ID);
            //            Console.WriteLine("CanMsg[{0}].TimeStamp = {1}", i, CanMsgBuffer[i].TimeStamp);
            //            Console.Write("CanMsg[{0}].Data = ", i);
            //            for (int j = 0; j < CanMsgBuffer[i].DataLen; j++)
            //            {
            //                Console.Write("{0:X2} ", CanMsgBuffer[i].Data[j]);
            //            }
            //            Console.WriteLine("\n");
            //        }
            //    }
            //    else if (CanNum < 0)
            //    {
            //        Console.WriteLine("Get CAN data error!");
            //    }
            //    //延时
            //    System.Threading.Thread.Sleep(100);
            //    //释放申请的数据缓冲区
            //    Marshal.FreeHGlobal(pt);
            //}

            // 在循环外计算一次大小，避免重复计算
            int msgSize = Marshal.SizeOf(typeof(USB2CAN.CAN_MSG));

            for (int t = 0; t < 10; t++)
            {
                USB2CAN.CAN_MSG[] CanMsgBuffer = new USB2CAN.CAN_MSG[1024];
                IntPtr pt = Marshal.AllocHGlobal(msgSize * CanMsgBuffer.Length);

                try
                {
                    int CanNum = USB2CAN.CAN_GetMsgWithSize(DevHandle, ReadCANIndex, pt, CanMsgBuffer.Length);

                    if (CanNum > 0)
                    {
                        for (int i = 0; i < CanNum; i++)
                        {
                            // 关键修改：使用 ToInt64 计算偏移，兼容 32/64 位
                            long offset = (long)i * msgSize;
                            IntPtr pMsg = new IntPtr(pt.ToInt64() + offset);

                            CanMsgBuffer[i] = (USB2CAN.CAN_MSG)Marshal.PtrToStructure(pMsg, typeof(USB2CAN.CAN_MSG));

                            if (CanMsgBuffer[i].ID == 0x7FE)
                            {
                                Console.WriteLine("接收到 ID=0x7FE 的数据：");
                                for (int j = 0; j < CanMsgBuffer[i].DataLen; j++)  // 建议用 DataLen 而非 Data.Length
                                {
                                    Console.WriteLine("Data[{0}] = 0x{1:X2} ({2})", j, CanMsgBuffer[i].Data[j], CanMsgBuffer[i].Data[j]);
                                }
                            }
                        }
                    }
                    else if (CanNum < 0)
                    {
                        Console.WriteLine("Get CAN data error! 错误码: " + CanNum);
                    }

                    Thread.Sleep(100);
                }
                finally
                {
                    Marshal.FreeHGlobal(pt);  // 确保释放，防止内存泄漏
                }
            }
            //停止CAN
            USB2CAN.CAN_StopGetMsg(DevHandle, ReadCANIndex);

            Console.ReadLine(); 
#endif
        }
    }
}
