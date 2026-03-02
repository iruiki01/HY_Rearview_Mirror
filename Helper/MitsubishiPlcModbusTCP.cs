using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    public class MitsubishiPlcModbusTCP
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private readonly object _lockObject = new object();
        private ushort _transactionId = 0;

        // TCP通信参数
        public string IpAddress { get; private set; }
        public int Port { get; private set; }

        public MitsubishiPlcModbusTCP(string ipAddress = "192.168.0.10", int port = 502)
        {
            IpAddress = ipAddress;
            Port = port;
        }

        /// <summary>
        /// 打开TCP连接
        /// </summary>
        public bool Open()
        {
            try
            {
                if (_tcpClient == null || !_tcpClient.Connected)
                {
                    _tcpClient = new TcpClient();
                    _tcpClient.Connect(IpAddress, Port);
                    _stream = _tcpClient.GetStream();
                    _stream.ReadTimeout = 1000;
                    _stream.WriteTimeout = 1000;
                    Thread.Sleep(100); // 等待稳定
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"建立TCP连接失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            _stream?.Close();
            _tcpClient?.Close();
            _stream = null;
            _tcpClient = null;
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

                    // Modbus TCP报文格式：[事务元标识符(2)][协议标识符(2)][长度(2)][单元标识符(1)][功能码(1)][起始地址(2)][寄存器数量(2)]
                    byte[] request = new byte[12];

                    // 事务元标识符（递增）
                    _transactionId++;
                    request[0] = (byte)(_transactionId >> 8);
                    request[1] = (byte)_transactionId;

                    // 协议标识符（Modbus=0）
                    request[2] = 0x00;
                    request[3] = 0x00;

                    // 长度（后面字节数）
                    request[4] = 0x00;
                    request[5] = 0x06; // 单元标识符1+功能码1+起始地址2+寄存器数量2=6

                    // 单元标识符（从站地址）
                    request[6] = slaveId;

                    // 功能码03：读保持寄存器
                    request[7] = 0x03;

                    // 起始地址
                    request[8] = (byte)(address >> 8);
                    request[9] = (byte)address;

                    // 寄存器数量
                    request[10] = (byte)(length >> 8);
                    request[11] = (byte)length;

                    _stream.Write(request, 0, 12);

                    // 响应报文格式：[事务元标识符(2)][协议标识符(2)][长度(2)][单元标识符(1)][功能码(1)][字节数(1)][数据区(N*2)][无CRC]
                    int expectedLength = 9 + 2 * length; // 头部9字节 + 数据区
                    byte[] response = new byte[expectedLength];
                    int bytesRead = 0;
                    int totalBytesRead = 0;

                    while (totalBytesRead < expectedLength)
                    {
                        bytesRead = _stream.Read(response, totalBytesRead, expectedLength - totalBytesRead);
                        totalBytesRead += bytesRead;
                        if (bytesRead == 0) 
                            Thread.Sleep(10);
                    }

                    // 验证响应
                    if (response[0] != request[0] || response[1] != request[1]) // 事务元标识符
                        throw new Exception("事务标识符不匹配");

                    if (response[6] != slaveId || response[7] != 0x03)
                        throw new Exception("响应格式错误");

                    // 解析数据
                    byte dataLength = response[8]; // 数据字节数
                    if (dataLength != length * 2)
                        throw new Exception("数据长度不匹配");

                    ushort[] result = new ushort[length];
                    for (int i = 0; i < length; i++)
                    {
                        int index = 9 + i * 2;
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

                    // Modbus TCP写单个寄存器报文
                    byte[] request = new byte[12];

                    // 事务元标识符
                    _transactionId++;
                    request[0] = (byte)(_transactionId >> 8);
                    request[1] = (byte)_transactionId;

                    // 协议标识符
                    request[2] = 0x00;
                    request[3] = 0x00;

                    // 长度
                    request[4] = 0x00;
                    request[5] = 0x06; // 单元标识符1+功能码1+起始地址2+寄存器值2=6

                    // 单元标识符
                    request[6] = slaveId;

                    // 功能码06：写单个寄存器
                    request[7] = 0x06;

                    // 起始地址
                    request[8] = (byte)(address >> 8);
                    request[9] = (byte)address;

                    // 寄存器值
                    request[10] = (byte)(value >> 8);
                    request[11] = (byte)value;

                    _stream.Write(request, 0, 12);

                    // 读取响应（应与请求相同）
                    byte[] response = new byte[12];
                    int bytesRead = _stream.Read(response, 0, 12);

                    if (bytesRead != 12)
                        throw new Exception("响应长度不足");

                    // 验证响应内容（比较关键字段）
                    for (int i = 0; i < 12; i++)
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
        /// 写入多个寄存器
        /// </summary>
        public void WriteMultipleRegisters(byte slaveId, ushort address, ushort[] values)
        {
            lock (_lockObject)
            {
                try
                {
                    Open();

                    int dataLength = values.Length * 2;
                    byte[] request = new byte[13 + dataLength];

                    // 事务元标识符
                    _transactionId++;
                    request[0] = (byte)(_transactionId >> 8);
                    request[1] = (byte)_transactionId;

                    // 协议标识符
                    request[2] = 0x00;
                    request[3] = 0x00;

                    // 长度
                    request[4] = (byte)((7 + dataLength) >> 8); // 单元标识符1+功能码1+起始地址2+寄存器数量2+字节数1+数据=dataLength
                    request[5] = (byte)(7 + dataLength);

                    // 单元标识符
                    request[6] = slaveId;

                    // 功能码16：写多个寄存器
                    request[7] = 0x10;

                    // 起始地址
                    request[8] = (byte)(address >> 8);
                    request[9] = (byte)address;

                    // 寄存器数量
                    request[10] = (byte)(values.Length >> 8);
                    request[11] = (byte)values.Length;

                    // 字节数
                    request[12] = (byte)dataLength;

                    // 数据
                    for (int i = 0; i < values.Length; i++)
                    {
                        request[13 + i * 2] = (byte)(values[i] >> 8);
                        request[14 + i * 2] = (byte)values[i];
                    }

                    _stream.Write(request, 0, request.Length);

                    // 读取响应
                    byte[] response = new byte[12];
                    int bytesRead = _stream.Read(response, 0, 12);

                    if (bytesRead != 12)
                        throw new Exception("响应长度不足");

                    // 验证响应
                    if (response[7] != 0x10)
                        throw new Exception("功能码不匹配");
                }
                catch (Exception ex)
                {
                    throw new Exception($"写入多个寄存器失败: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 检查连接状态
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return _tcpClient?.Connected == true;
            }
        }

        // 析构函数
        ~MitsubishiPlcModbusTCP()
        {
            Close();
        }
    }
}

/***
// 创建TCP连接
var plc = new MitsubishiPLCModbusTCP("192.168.1.100", 502);

try
{
    // 读取D100开始的2个寄存器
    ushort[] values = plc.ReadHoldingRegisters(1, 100, 2);
    
    // 写入单个寄存器D200
    plc.WriteSingleRegister(1, 200, 1234);
    
    // 批量写入D300开始的3个寄存器
    ushort[] writeValues = new ushort[] { 100, 200, 300 };
    plc.WriteMultipleRegisters(1, 300, writeValues);
}
catch (Exception ex)
{
    Console.WriteLine($"操作失败: {ex.Message}");
}
finally
{
    plc.Close();
}
 ***/