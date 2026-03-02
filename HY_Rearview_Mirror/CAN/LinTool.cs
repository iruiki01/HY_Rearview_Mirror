using HY_Rearview_Mirror.InterfaceTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USB2XXX;

namespace HY_Rearview_Mirror.CAN
{
    public class LinTool
    {
        usb_device.DEVICE_INFO DevInfo = new usb_device.DEVICE_INFO();
        Int32[] DevHandles = new Int32[20];
        Int32 DevHandle = 0;
        Byte LINIndex = 0;
        bool state;
        Int32 DevNum, ret;

        public  CanChannel Channel => throw new NotImplementedException();

        public event EventHandler<CANMessage> MessageReceivedHandler
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        public bool Initialize(uint baudRate = 19200)
        {
            //扫描查找设备
            DevNum = usb_device.USB_ScanDevice(DevHandles);
            if (DevNum <= 0)
            {
                Console.WriteLine("No device connected!");
                return false;
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
                return false;
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
                return false;
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
            //初始化配置LIN
            USB2LIN.LIN_CONFIG LINConfig = new USB2LIN.LIN_CONFIG();
            LINConfig.BaudRate = baudRate;
            LINConfig.BreakBits = USB2LIN.LIN_BREAK_BITS_11;
            LINConfig.CheckMode = USB2LIN.LIN_CHECK_MODE_EXT;
#if LIN_MASTER
            LINConfig.MasterMode = USB2LIN.LIN_MASTER;
#else
            LINConfig.MasterMode = USB2LIN.LIN_SLAVE;
#endif
            ret = USB2LIN.LIN_Init(DevHandle, LINIndex, ref LINConfig);
            if (ret != USB2LIN.LIN_SUCCESS)
            {
                Console.WriteLine("Config LIN failed!");
                return false;
            }
            else
            {
                Console.WriteLine("Config LIN Success!");
                return true;
            }
        }

        public int Send(uint id, byte[] data, bool isExtendedFrame)
        {
            USB2LIN.LIN_MSG[] msg = new USB2LIN.LIN_MSG[1];
            msg[0].Data = new Byte[9];
            // 复制数据
            Array.Copy(data, msg[0].Data, msg[0].DataLen);
            msg[0].DataLen = 8;
            msg[0].ID = (Byte)id;
            ret = USB2LIN.LIN_Write(DevHandle, LINIndex, msg, 1);
            if (ret != USB2LIN.LIN_SUCCESS)
            {
                Console.WriteLine("LIN write data failed!\n");
                return -1;
            }
            else
            {
                Console.WriteLine("LIN write data success!\n");
                return 0;
            }
        }
    }
}
