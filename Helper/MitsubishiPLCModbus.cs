using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    /// <summary>
    /// 用C#实现的与三菱PLC进行Modbus通信的类,基于串口
    /// </summary>
    public class MitsubishiPLCModbus
    {
        private SerialPort _serialPort;
        private readonly object _lockObject = new object();

        // 默认通信参数（根据实际PLC配置修改）
        public MitsubishiPLCModbus(string portName = "COM1",
                                  int baudRate = 9600,
                                  Parity parity = Parity.Even,
                                  int dataBits = 7,
                                  StopBits stopBits = StopBits.One)
        {
            _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            _serialPort.ReadTimeout = 1000;
            _serialPort.WriteTimeout = 1000;
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        public bool Open()
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                    Thread.Sleep(100); // 等待稳定
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"打开串口失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            if (_serialPort.IsOpen)
                _serialPort.Close();
        }

        /// <summary>
        /// 读取保持寄存器（对应三菱D寄存器）
        /// </summary>
        /// <param name="slaveId">从站号</param>
        /// <param name="address">寄存器地址（十进制）</param>
        /// <param name="length">读取数量</param>
        public ushort[] ReadHoldingRegisters(byte slaveId, ushort address, ushort length)
        {
            lock (_lockObject)
            {
                try
                {
                    Open();

                    // Modbus RTU报文格式：[从站地址][功能码][起始地址高8位][低8位][寄存器数量高8位][低8位][CRC低8位][CRC高8位]
                    byte[] request = new byte[8];
                    request[0] = slaveId;
                    request[1] = 0x03; // 功能码03：读保持寄存器
                    request[2] = (byte)(address >> 8);
                    request[3] = (byte)address;
                    request[4] = (byte)(length >> 8);
                    request[5] = (byte)length;
                    byte[] crc = CalculateCRC(request, 6);
                    request[6] = crc[0];
                    request[7] = crc[1];

                    _serialPort.DiscardInBuffer();
                    _serialPort.Write(request, 0, 8);

                    // 响应报文格式：[从站地址][功能码][字节数][数据区][CRC]
                    int expectedLength = 5 + 2 * length;
                    byte[] response = new byte[expectedLength];
                    int bytesRead = 0;
                    int totalBytesRead = 0;

                    while (totalBytesRead < expectedLength)
                    {
                        bytesRead = _serialPort.Read(response, totalBytesRead, expectedLength - totalBytesRead);
                        totalBytesRead += bytesRead;
                        if (bytesRead == 0) Thread.Sleep(10);
                    }

                    // 验证响应
                    if (response[0] != slaveId || response[1] != 0x03)
                        throw new Exception("响应格式错误");

                    byte[] receivedCRC = { response[expectedLength - 2], response[expectedLength - 1] };
                    byte[] calculatedCRC = CalculateCRC(response, expectedLength - 2);
                    if (receivedCRC[0] != calculatedCRC[0] || receivedCRC[1] != calculatedCRC[1])
                        throw new Exception("CRC校验失败");

                    // 解析数据
                    ushort[] result = new ushort[length];
                    for (int i = 0; i < length; i++)
                    {
                        int index = 3 + i * 2;
                        result[i] = (ushort)((response[index] << 8) | response[index + 1]);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception($"读取寄存器失败: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 写入单个寄存器（对应三菱D寄存器）
        /// </summary>
        public void WriteSingleRegister(byte slaveId, ushort address, ushort value)
        {
            lock (_lockObject)
            {
                try
                {
                    Open();

                    byte[] request = new byte[8];
                    request[0] = slaveId;
                    request[1] = 0x06; // 功能码06：写单个寄存器
                    request[2] = (byte)(address >> 8);
                    request[3] = (byte)address;
                    request[4] = (byte)(value >> 8);
                    request[5] = (byte)value;
                    byte[] crc = CalculateCRC(request, 6);
                    request[6] = crc[0];
                    request[7] = crc[1];

                    _serialPort.DiscardInBuffer();
                    _serialPort.Write(request, 0, 8);

                    // 读取响应（应与请求相同）
                    byte[] response = new byte[8];
                    int bytesRead = _serialPort.Read(response, 0, 8);

                    if (bytesRead != 8)
                        throw new Exception("响应长度不足");

                    // 验证响应内容
                    for (int i = 0; i < 8; i++)
                    {
                        if (request[i] != response[i])
                            throw new Exception("响应数据不匹配");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"写入寄存器失败: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 计算CRC16校验码
        /// </summary>
        private byte[] CalculateCRC(byte[] data, int length)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < length; i++)
            {
                crc ^= data[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x0001) != 0)
                    {
                        crc >>= 1;
                        crc ^= 0xA001;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }
            return new byte[] { (byte)(crc & 0xFF), (byte)(crc >> 8) };
        }

        // 析构函数
        ~MitsubishiPLCModbus()
        {
            Close();
        }
    }
}


/***
 // 创建Modbus通信实例（根据实际参数配置）
var plc = new MitsubishiPLCModbus("COM3", 9600, Parity.Even, 7, StopBits.One);

try
{
    // 读取D100开始的2个寄存器
    ushort[] values = plc.ReadHoldingRegisters(1, 100, 2);
    Console.WriteLine($"D100: {values[0]}, D101: {values[1]}");

    // 写入D200寄存器
    plc.WriteSingleRegister(1, 200, 1234);
    Console.WriteLine("写入成功");
}
catch (Exception ex)
{
    Console.WriteLine($"通信错误: {ex.Message}");
}
finally
{
    plc.Close();
}

注意事项：

参数配置：需要根据实际PLC设置修改串口参数（端口号、波特率、校验位等）

地址映射：

三菱FX系列PLC的D寄存器对应Modbus的保持寄存器

地址使用十进制地址（如D100对应地址100）

从站地址：需要与PLC中设置的站号一致

异常处理：建议在实际使用中添加完整的异常处理

线程安全：使用lock确保多线程安全访问

请根据实际使用的PLC型号和配置调整通信参数。如果使用Q系列等高端PLC，可能需要调整地址映射规则。


 * **/
