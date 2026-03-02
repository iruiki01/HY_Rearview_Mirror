using HslCommunication;
using HslCommunication.Core;
using HslCommunication.ModBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Helper
{
    public class ModbusTCP
    {      
        // 创建Modbus TCP客户端对象
        private ModbusTcpNet modbusClient;
        public bool Connect(string ip, int port)
        {
            try
            {
                // 在初始化函数或需要连接的地方创建和连接Modbus TCP客户端
                modbusClient = new ModbusTcpNet(ip, port); // 设置Modbus TCP服务器的IP地址和端口号
                modbusClient.ConnectServer(); // 连接到Modbus TCP服务器
                modbusClient.DataFormat = DataFormat.ABCD;  // 三菱通常用ABCD（大端）
                return true;
            }
            catch
            {
                return false;
            }

        }
        // 读取Modbus寄存器的示例
        public bool ReadModbusRegisters(string dataAddress, int dataAddressLength, ref List<short> returnArr)
        {          
            try
            {         
                OperateResult<short[]> result = modbusClient.ReadInt16(dataAddress, (ushort)dataAddressLength); // ("D100", 10) 读取D100~D109共10个寄存器的值
                returnArr.Clear();
                if (result.IsSuccess)
                {
                    short[] values = result.Content;
                    // 处理返回的寄存器值
                    foreach (short value in values)
                    {
                        returnArr.Add(value);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        // 写入Modbus寄存器的示例
        private static readonly object syncRoot = new object();
        public bool WriteModbusRegisters(string dataAddress, short[] valuesToWrite)
        {
            try
            {
                //short[] valuesToWrite = new short[] { 10, 20, 30 }; // 要写入的寄存器值
                lock (syncRoot)
                {
                    OperateResult result = modbusClient.Write(dataAddress, valuesToWrite); // 写入D100~D102共3个寄存器的值

                    if (result.IsSuccess)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        // 写入Modbus寄存器的示例
        public bool WriteModbusRegisters(string dataAddress, int valuesToWrite)
        {
            try
            {
                //short[] valuesToWrite = new short[] { 10, 20, 30 }; // 要写入的寄存器值

                OperateResult result = modbusClient.Write(dataAddress, valuesToWrite);

                if (result.IsSuccess)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        // 关闭Modbus TCP连接的示例
        public bool CloseModbusConnection()
        {
            if (modbusClient != null)
            {
                modbusClient.ConnectClose(); // 关闭Modbus TCP连接           
            }
            return true;
        }
    }
}
