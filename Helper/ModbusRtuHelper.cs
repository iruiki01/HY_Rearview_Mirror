using HslCommunication;
using HslCommunication.ModBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Helper
{
    public class ModbusRtuHelper
    {
        #region 字段

        private ModbusRtu _modbusRtu;
        private readonly object _lockObj = new object();
        private bool _isDisposed = false;

        #endregion

        #region 属性

        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool IsConnected => _modbusRtu?.IsOpen() ?? false;

        /// <summary>
        /// 串口名称
        /// </summary>
        public string PortName { get; private set; }

        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate { get; private set; }

        /// <summary>
        /// 数据位
        /// </summary>
        public int DataBits { get; private set; }

        /// <summary>
        /// 停止位
        /// </summary>
        public System.IO.Ports.StopBits StopBits { get; private set; }

        /// <summary>
        /// 校验位
        /// </summary>
        public System.IO.Ports.Parity Parity { get; private set; }

        /// <summary>
        /// 站号（从机地址）
        /// </summary>
        public byte Station { get; private set; }

        /// <summary>
        /// 连接超时时间（毫秒）
        /// </summary>
        public int ConnectTimeout { get; set; } = 3000;

        /// <summary>
        /// 读取超时时间（毫秒）
        /// </summary>
        public int ReadTimeout { get; set; } = 5000;

        #endregion

        #region 事件

        /// <summary>
        /// 连接状态改变事件
        /// </summary>
        public event EventHandler<bool> ConnectionStateChanged;

        /// <summary>
        /// 异常事件
        /// </summary>
        public event EventHandler<Exception> OnException;

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ModbusRtuHelper()
        {
        }

        /// <summary>
        /// 带参数构造函数
        /// </summary>
        public ModbusRtuHelper(string portName, int baudRate = 9600, byte station = 1)
        {
            PortName = portName;
            BaudRate = baudRate;
            Station = station;
            DataBits = 8;
            StopBits = System.IO.Ports.StopBits.One;
            Parity = System.IO.Ports.Parity.None;
        }

        #endregion

        #region 初始化与连接

        /// <summary>
        /// 配置串口参数
        /// </summary>
        public void Configure(string portName, int baudRate = 9600, int dataBits = 8,
            System.IO.Ports.StopBits stopBits = System.IO.Ports.StopBits.One,
            System.IO.Ports.Parity parity = System.IO.Ports.Parity.None,
            byte station = 1)
        {
            PortName = portName;
            BaudRate = baudRate;
            DataBits = dataBits;
            StopBits = stopBits;
            Parity = parity;
            Station = station;
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        public OperateResult Open()
        {
            try
            {
                Close();

                _modbusRtu = new ModbusRtu(Station);
                _modbusRtu.SerialPortInni(sp =>
                {
                    sp.PortName = PortName;
                    sp.BaudRate = BaudRate;
                    sp.DataBits = DataBits;
                    sp.StopBits = StopBits;
                    sp.Parity = Parity;
                    sp.ReadTimeout = ReadTimeout;
                    sp.WriteTimeout = ConnectTimeout;
                });

                // 设置日志（可选）
                _modbusRtu.LogNet = new HslCommunication.LogNet.LogNetSingle("ModbusRtuLog.txt");

                var result = _modbusRtu.Open();

                if (result.IsSuccess)
                {
                    ConnectionStateChanged?.Invoke(this, true);
                }

                return result;
            }
            catch (Exception ex)
            {
                OnException?.Invoke(this, ex);
                return new OperateResult(ex.Message);
            }
        }

        /// <summary>
        /// 异步打开连接
        /// </summary>
        public async Task<OperateResult> OpenAsync()
        {
            return await Task.Run(() => Open());
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            lock (_lockObj)
            {
                _modbusRtu?.Close();
                _modbusRtu?.Dispose();
                _modbusRtu = null;
                ConnectionStateChanged?.Invoke(this, false);
            }
        }
        #endregion

        #region 读取操作

        /// <summary>
        /// 读取线圈（0x01功能码）
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">读取数量</param>
        /// <returns>读取结果</returns>
        public OperateResult<bool[]> ReadCoil(string address, ushort length)
        {
            lock (_lockObj)
            {
                try
                {
                    EnsureConnected();
                    return _modbusRtu.ReadCoil(address, length);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    return new OperateResult<bool[]>(ex.Message);
                }
            }
        }

        /// <summary>
        /// 读取离散输入（0x02功能码）
        /// </summary>
        public OperateResult<bool[]> ReadDiscrete(string address, ushort length)
        {
            lock (_lockObj)
            {
                try
                {
                    EnsureConnected();
                    return _modbusRtu.ReadDiscrete(address, length);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    return new OperateResult<bool[]>(ex.Message);
                }
            }
        }

        /// <summary>
        /// 读取保持寄存器（0x03功能码）- 单个
        /// </summary>
        public OperateResult<short> ReadInt16(string address)
        {
            lock (_lockObj)
            {
                try
                {
                    EnsureConnected();
                    return _modbusRtu.ReadInt16(address);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    return new OperateResult<short>(ex.Message);
                }
            }
        }

        /// <summary>
        /// 读取保持寄存器（0x03功能码）- 多个
        /// </summary>
        public OperateResult<short[]> ReadInt16(string address, ushort length)
        {
            lock (_lockObj)
            {
                try
                {
                    EnsureConnected();
                    return _modbusRtu.ReadInt16(address, length);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    return new OperateResult<short[]>(ex.Message);
                }
            }
        }

        /// <summary>
        /// 读取保持寄存器 - ushort
        /// </summary>
        public OperateResult<ushort> ReadUInt16(string address)
        {
            lock (_lockObj)
            {
                try
                {
                    EnsureConnected();
                    return _modbusRtu.ReadUInt16(address);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    return new OperateResult<ushort>(ex.Message);
                }
            }
        }

        /// <summary>
        /// 读取保持寄存器 - int32（2个寄存器）
        /// </summary>
        public OperateResult<int> ReadInt32(string address)
        {
            lock (_lockObj)
            {
                try
                {
                    EnsureConnected();
                    return _modbusRtu.ReadInt32(address);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    return new OperateResult<int>(ex.Message);
                }
            }
        }

        /// <summary>
        /// 读取保持寄存器 - float（2个寄存器）
        /// </summary>
        public OperateResult<float> ReadFloat(string address)
        {
            lock (_lockObj)
            {
                try
                {
                    EnsureConnected();
                    return _modbusRtu.ReadFloat(address);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    return new OperateResult<float>(ex.Message);
                }
            }
        }

        /// <summary>
        /// 读取保持寄存器 - string
        /// </summary>
        public OperateResult<string> ReadString(string address, ushort length)
        {
            lock (_lockObj)
            {
                try
                {
                    EnsureConnected();
                    return _modbusRtu.ReadString(address, length);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    return new OperateResult<string>(ex.Message);
                }
            }
        }
        /// <summary>
        /// 批量读取寄存器（通用）
        /// </summary>
        public OperateResult<byte[]> Read(string address, ushort length)
        {
            lock (_lockObj)
            {
                try
                {
                    EnsureConnected();
                    return _modbusRtu.Read(address, length);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    return new OperateResult<byte[]>(ex.Message);
                }
            }
        }

        /// <summary>
        /// 读取字符串（指定编码）
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="registerCount">寄存器数量</param>
        /// <param name="encoding">编码方式</param>
        /// <param name="trimNull">是否去除末尾空字符</param>
        /// <returns>操作结果</returns>
        public OperateResult<string> ReadString(string address, ushort registerCount, System.Text.Encoding encoding, bool trimNull = true)
        {
            lock (_lockObj)
            {
                try
                {
                    EnsureConnected();

                    // 先读取原始字节
                    var result = _modbusRtu.Read(address, registerCount);
                    if (!result.IsSuccess)
                    {
                        return new OperateResult<string>(result.ErrorCode, result.Message);
                    }

                    // 使用指定编码解码
                    string str = encoding.GetString(result.Content);

                    if (trimNull)
                    {
                        str = str.TrimEnd('\0', ' ');
                    }

                    return OperateResult.CreateSuccessResult(str);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    return new OperateResult<string>(ex.Message);
                }
            }
        }
        /// <summary>
        /// 读取UTF-8编码的字符串
        /// </summary>
        public OperateResult<string> ReadStringUtf8(string address, ushort byteLength, bool trimNull = true)
        {
            return ReadString(address, (ushort)(byteLength / 2), System.Text.Encoding.UTF8, trimNull);
        }

        #endregion

        #region 写入操作
        /// <summary>
        /// 写入字符串（指定编码和固定长度）
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">字符串内容</param>
        /// <param name="length">固定占用的寄存器数量（不足补空格，超出截断）</param>
        /// <param name="encoding">编码方式，默认ASCII</param>
        /// <returns>操作结果</returns>
        public OperateResult WriteStringFixed(string address, string value, ushort length, System.Text.Encoding encoding = null)
        {
            lock (_lockObj)
            {
                try
                {
                    EnsureConnected();
                    if(encoding == null)
                    {
                        encoding = System.Text.Encoding.ASCII;
                    }
                  
                    // 计算实际可容纳的字符数：寄存器数 × 2
                    int maxChars = length * 2;

                    // 处理字符串长度
                    string processedValue = value?.PadRight(maxChars, ' ') ?? new string(' ', maxChars);
                    if (processedValue.Length > maxChars)
                    {
                        processedValue = processedValue.Substring(0, maxChars);
                    }

                    // 转换为字节数组
                    byte[] bytes = encoding.GetBytes(processedValue);

                    // 确保字节数组长度为 寄存器数 × 2
                    byte[] finalBytes = new byte[length * 2];
                    Array.Copy(bytes, finalBytes, Math.Min(bytes.Length, finalBytes.Length));

                    // 写入寄存器
                    return _modbusRtu.Write(address, finalBytes);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    return new OperateResult(ex.Message);
                }
            }
        }
        /// <summary>
        /// 写入字符串（UTF-8编码，支持中文）
        /// 注意：UTF-8变长编码，需确保PLC侧解析方式一致
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">字符串内容</param>
        /// <param name="byteLength">占用的字节长度（决定寄存器数量 = byteLength/2）</param>
        /// <returns>操作结果</returns>
        public OperateResult WriteStringUtf8(string address, string value, ushort byteLength)
        {
            lock (_lockObj)
            {
                try
                {
                    EnsureConnected();
                    var encoding = System.Text.Encoding.UTF8;

                    // 编码并截断/填充
                    byte[] bytes = encoding.GetBytes(value ?? "");
                    byte[] finalBytes = new byte[byteLength];

                    if (bytes.Length >= byteLength)
                    {
                        Array.Copy(bytes, finalBytes, byteLength);
                    }
                    else
                    {
                        Array.Copy(bytes, finalBytes, bytes.Length);
                        // 剩余补0
                        for (int i = bytes.Length; i < byteLength; i++)
                        {
                            finalBytes[i] = 0;
                        }
                    }

                    return _modbusRtu.Write(address, finalBytes);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    return new OperateResult(ex.Message);
                }
            }
        }


        /// <summary>
        /// 写入线圈（0x05功能码）- 单个
        /// </summary>
        public OperateResult WriteCoil(string address, bool value)
        {
            lock (_lockObj)
            {
                try
                {
                    EnsureConnected();
                    return _modbusRtu.Write(address, value);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    return new OperateResult(ex.Message);
                }
            }
        }

        /// <summary>
        /// 写入线圈（0x0F功能码）- 多个
        /// </summary>
        public OperateResult WriteCoil(string address, bool[] values)
        {
            lock (_lockObj)
            {
                try
                {
                    EnsureConnected();
                    return _modbusRtu.Write(address, values);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    return new OperateResult(ex.Message);
                }
            }
        }

        /// <summary>
        /// 写入寄存器（0x06功能码）- short
        /// </summary>
        public OperateResult Write(string address, short value)
        {
            lock (_lockObj)
            {
                try
                {
                    EnsureConnected();
                    return _modbusRtu.Write(address, value);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    return new OperateResult(ex.Message);
                }
            }
        }

        /// <summary>
        /// 写入寄存器（0x06功能码）- ushort
        /// </summary>
        public OperateResult Write(string address, ushort value)
        {
            lock (_lockObj)
            {
                try
                {
                    EnsureConnected();
                    return _modbusRtu.Write(address, value);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    return new OperateResult(ex.Message);
                }
            }
        }

        /// <summary>
        /// 写入寄存器（0x10功能码）- int32
        /// </summary>
        public OperateResult Write(string address, int value)
        {
            lock (_lockObj)
            {
                try
                {
                    EnsureConnected();
                    return _modbusRtu.Write(address, value);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    return new OperateResult(ex.Message);
                }
            }
        }

        /// <summary>
        /// 写入寄存器（0x10功能码）- float
        /// </summary>
        public OperateResult Write(string address, float value)
        {
            lock (_lockObj)
            {
                try
                {
                    EnsureConnected();
                    return _modbusRtu.Write(address, value);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    return new OperateResult(ex.Message);
                }
            }
        }

        /// <summary>
        /// 写入寄存器（0x10功能码）- string
        /// </summary>
        public OperateResult Write(string address, string value)
        {
            lock (_lockObj)
            {
                try
                {
                    EnsureConnected();
                    return _modbusRtu.Write(address, value);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    return new OperateResult(ex.Message);
                }
            }
        }

        /// <summary>
        /// 写入多个寄存器（0x10功能码）
        /// </summary>
        public OperateResult Write(string address, short[] values)
        {
            lock (_lockObj)
            {
                try
                {
                    EnsureConnected();
                    return _modbusRtu.Write(address, values);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    return new OperateResult(ex.Message);
                }
            }
        }

        /// <summary>
        /// 批量写入（通用）
        /// </summary>
        public OperateResult Write(string address, byte[] value)
        {
            lock (_lockObj)
            {
                try
                {
                    EnsureConnected();
                    return _modbusRtu.Write(address, value);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    return new OperateResult(ex.Message);
                }
            }
        }

        #endregion

        #region 高级功能

        /// <summary>
        /// 检查连接状态，断开则尝试重连
        /// </summary>
        public OperateResult CheckAndReconnect()
        {
            if (!IsConnected)
            {
                return Open();
            }
            return OperateResult.CreateSuccessResult();
        }

        #endregion

        #region 私有方法

        private void EnsureConnected()
        {
            if (_modbusRtu == null || !_modbusRtu.IsOpen())
            {
                throw new InvalidOperationException("Modbus RTU 未连接");
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (!_isDisposed)
            {
                Close();
                _isDisposed = true;
            }
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    #region 扩展方法

    /// <summary>
    /// 地址格式化扩展
    /// </summary>
    public static class ModbusAddressExtensions
    {
        /// <summary>
        /// 格式化线圈地址（0x）
        /// </summary>
        public static string ToCoilAddress(this int address) => $"0x{address}";

        /// <summary>
        /// 格式化离散输入地址（1x）
        /// </summary>
        public static string ToDiscreteAddress(this int address) => $"1x{address}";

        /// <summary>
        /// 格式化输入寄存器地址（3x）
        /// </summary>
        public static string ToInputRegisterAddress(this int address) => $"3x{address}";

        /// <summary>
        /// 格式化保持寄存器地址（4x）
        /// </summary>
        public static string ToHoldingRegisterAddress(this int address) => $"4x{address}";

        /// <summary>
        /// 格式化保持寄存器地址（简写）
        /// </summary>
        public static string To4XAddress(this int address) => $"4{address:D4}";
    }

    #endregion
}
