using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY_Rearview_Mirror.Functions
{
    /// 串口通信类
    /// </summary>
    public class RobustSerialPort : IDisposable
    {
        private SerialPort _serialPort;
        private bool _isDisposed = false;

        // 事件定义
        public event EventHandler<DataReceivedEventArgs> DataReceived;
        public event EventHandler<ErrorReceivedEventArgs> ErrorReceived;
        public event EventHandler<string> StatusChanged;

        public RobustSerialPort(string portName, int baudRate = 9600, int dataBits = 8,
                        Parity parity = Parity.None, StopBits stopBits = StopBits.One,
                        Handshake handshake = Handshake.None)
        {
            _serialPort = new SerialPort();
            _serialPort.DataReceived += OnDataReceived;
            _serialPort.ErrorReceived += OnErrorReceived;
            
            Open(portName,baudRate,dataBits,
                        parity, stopBits,
                        handshake);
        }

        public RobustSerialPort()
        {
            _serialPort = new SerialPort();
            _serialPort.DataReceived += OnDataReceived;
            _serialPort.ErrorReceived += OnErrorReceived;
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        public bool Open(string portName, int baudRate = 9600, int dataBits = 8,
                        Parity parity = Parity.None, StopBits stopBits = StopBits.One,
                        Handshake handshake = Handshake.None)
        {
            try
            {
                if (_serialPort.IsOpen)
                    Close();

                _serialPort.PortName = portName;
                _serialPort.BaudRate = baudRate;
                _serialPort.DataBits = dataBits;
                _serialPort.Parity = parity;
                _serialPort.StopBits = stopBits;
                _serialPort.Handshake = handshake;
                _serialPort.ReadTimeout = 1000;
                _serialPort.WriteTimeout = 1000;
                _serialPort.Encoding = Encoding.UTF8;

                _serialPort.Open();
                OnStatusChanged($"串口 {portName} 已打开");
                return true;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"打开串口失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        public void Close()
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                    OnStatusChanged("串口已关闭");
                }
            }
            catch (Exception ex)
            {
                OnStatusChanged($"关闭串口失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 检查串口是否打开
        /// </summary>
        public bool IsOpen => _serialPort.IsOpen;

        /// <summary>
        /// 发送字符串数据
        /// </summary>
        public bool SendString(string data)
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    OnStatusChanged("串口未打开，无法发送数据");
                    return false;
                }

                _serialPort.Write(data);
                OnStatusChanged($"发送字符串: {data}");
                return true;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"发送数据失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 发送字节数组数据
        /// </summary>
        public bool SendBytes(byte[] data)
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    OnStatusChanged("串口未打开，无法发送数据");
                    return false;
                }

                _serialPort.Write(data, 0, data.Length);
                OnStatusChanged($"发送字节数据，长度: {data.Length}");
                return true;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"发送数据失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 发送十六进制字符串
        /// </summary>
        public bool SendHex(string hexString)
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    OnStatusChanged("串口未打开，无法发送数据");
                    return false;
                }

                hexString = hexString.Replace(" ", "").Replace("-", "");
                if (hexString.Length % 2 != 0)
                    throw new ArgumentException("十六进制字符串长度必须为偶数");

                byte[] data = new byte[hexString.Length / 2];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                }

                _serialPort.Write(data, 0, data.Length);
                OnStatusChanged($"发送十六进制数据: {hexString}");
                return true;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"发送十六进制数据失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 读取字符串数据
        /// </summary>
        public string ReadString()
        {
            try
            {
                if (!_serialPort.IsOpen)
                    return string.Empty;

                return _serialPort.ReadExisting();
            }
            catch (TimeoutException)
            {
                return string.Empty;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"读取数据失败: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 读取字节数据
        /// </summary>
        public byte[] ReadBytes()
        {
            try
            {
                if (!_serialPort.IsOpen || _serialPort.BytesToRead == 0)
                    return Array.Empty<byte>();

                byte[] buffer = new byte[_serialPort.BytesToRead];
                int bytesRead = _serialPort.Read(buffer, 0, buffer.Length);

                if (bytesRead < buffer.Length)
                {
                    Array.Resize(ref buffer, bytesRead);
                }

                return buffer;
            }
            catch (TimeoutException)
            {
                return Array.Empty<byte>();
            }
            catch (Exception ex)
            {
                OnStatusChanged($"读取字节数据失败: {ex.Message}");
                return Array.Empty<byte>();
            }
        }

        /// <summary>
        /// 获取可用串口列表
        /// </summary>
        public static string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }

        // 事件处理
        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (_serialPort.BytesToRead > 0)
                {
                    byte[] data = ReadBytes();
                    DataReceived?.Invoke(this, new DataReceivedEventArgs(data));
                }
            }
            catch (Exception ex)
            {
                OnStatusChanged($"处理接收数据时出错: {ex.Message}");
            }
        }

        private void OnErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            ErrorReceived?.Invoke(this, new ErrorReceivedEventArgs(e.EventType));
        }

        private void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(this, status);
        }

        /// <summary>
        /// 设置接收事件触发模式
        /// </summary>
        public void SetReceiveTriggerMode(SerialData receiveTrigger)
        {
            _serialPort.ReceivedBytesThreshold = 1;
        }

        /// <summary>
        /// 设置超时时间
        /// </summary>
        public void SetTimeouts(int readTimeout = 1000, int writeTimeout = 1000)
        {
            _serialPort.ReadTimeout = readTimeout;
            _serialPort.WriteTimeout = writeTimeout;
        }

        /// <summary>
        /// 清空缓冲区
        /// </summary>
        public void ClearBuffers()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();
                OnStatusChanged("缓冲区已清空");
            }
        }

        // 资源释放
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Close();
                    _serialPort?.Dispose();
                }
                _isDisposed = true;
            }
        }

        ~RobustSerialPort()
        {
            Dispose(false);
        }
    }
    // 事件参数类
    public class DataReceivedEventArgs : EventArgs
    {
        public byte[] Data { get; }
        public string DataString => Encoding.UTF8.GetString(Data);
        public string HexString => BitConverter.ToString(Data).Replace("-", " ");

        public DataReceivedEventArgs(byte[] data)
        {
            Data = data ?? Array.Empty<byte>();
        }
    }

    public class ErrorReceivedEventArgs : EventArgs
    {
        public SerialError ErrorType { get; }

        public ErrorReceivedEventArgs(SerialError errorType)
        {
            ErrorType = errorType;
        }
    }
}

/**
class Program
{
    static void Main(string[] args)
    {
        using (var serialHelper = new SerialPortHelper())
        {
            // 订阅事件
            serialHelper.DataReceived += OnDataReceived;
            serialHelper.StatusChanged += OnStatusChanged;
            serialHelper.ErrorReceived += OnErrorReceived;

            // 打开串口
            if (serialHelper.Open("COM1", 9600, 8, Parity.None, StopBits.One))
            {
                Console.WriteLine("串口打开成功");

                // 发送数据
                serialHelper.SendString("Hello Serial Port!");
                serialHelper.SendHex("A0 B1 C2 D3");

                Thread.Sleep(1000);

                // 手动读取数据
                string received = serialHelper.ReadString();
                if (!string.IsNullOrEmpty(received))
                {
                    Console.WriteLine($"收到数据: {received}");
                }

                // 保持程序运行
                Console.WriteLine("按任意键退出...");
                Console.ReadKey();

                serialHelper.Close();
            }
        }
    }

    private static void OnDataReceived(object sender, DataReceivedEventArgs e)
    {
        Console.WriteLine($"接收到数据 - 字符串: {e.DataString}");
        Console.WriteLine($"接收到数据 - 十六进制: {e.HexString}");
    }

    private static void OnStatusChanged(object sender, string status)
    {
        Console.WriteLine($"[状态] {status}");
    }

    private static void OnErrorReceived(object sender, ErrorReceivedEventArgs e)
    {
        Console.WriteLine($"[错误] {e.ErrorType}");
    }
}
**/
