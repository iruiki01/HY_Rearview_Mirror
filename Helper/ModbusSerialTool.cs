using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modbus.Device;

namespace Helper
{
    /// <summary>
    /// 用于与中盛继电器通电
    /// </summary>
    public class ModbusSerialTool
    {
        /// <summary>
        /// 私有串口实例
        /// </summary>
        private SerialPort serialPort = new SerialPort();
        /// <summary>
        /// 私有ModbusRTU主站字段
        /// </summary>
        private IModbusMaster master;

        /// <summary>
        /// 初始化串口
        /// </summary>
        /// <param name="SPD"></param>
        /// <returns></returns>
        public bool Init(SerialPortData SPD)
        {
            try
            {
                //设定串口参数
                serialPort.PortName = SPD.PortName;
                serialPort.BaudRate = SPD.Baud;
                serialPort.Parity = GetSelectedParity(SPD.parity);
                serialPort.DataBits = SPD.DataBits;
                serialPort.StopBits = GetSelectedStopBits(SPD.StopBits);

                //创建ModbusRTU主站实例
                master = ModbusSerialMaster.CreateRtu(serialPort);

                //打开串口
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
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

        /// <summary>
        /// 获取窗体选中的奇偶校验
        /// </summary>
        /// <returns></returns>
        public Parity GetSelectedParity(string parity)
        {
            switch (parity)
            {
                case "Odd":
                    return Parity.Odd;
                case "Even":
                    return Parity.Even;
                case "Mark":
                    return Parity.Mark;
                case "Space":
                    return Parity.Space;
                case "None":
                default:
                    return Parity.None;
            }
        }

        /// <summary>
        /// 写入单个线圈
        /// </summary>
        public void WriteSingleCoil(int SlaceID, int StartAdr, bool result)
        {
            try
            {
                master.WriteSingleCoil((byte)SlaceID, (ushort)StartAdr, result);
            }
            catch { }
        }
        /// <summary>
        /// 批量写入线圈
        /// </summary>
        public void WriteArrayCoil(int SlaceID, int StartAdr, bool[] result)
        {
            try
            {
                master.WriteMultipleCoils((byte)SlaceID, (ushort)StartAdr, result.ToArray());
            }
            catch { }

        }
        /// <summary>
        /// 写入单个寄存器
        /// </summary>
        public void WriteSingleRegister(int SlaceID, int StartAdr, string result)
        {
            try
            {
                master.WriteSingleRegister((byte)SlaceID, (ushort)StartAdr, Convert.ToUInt16(result));
            }
            catch { }

        }
        /// <summary>
        /// 批量写入寄存器
        /// </summary>
        public void WriteArrayRegister(int SlaceID, int StartAdr, List<string> strList)
        {
            try
            {
                List<ushort> result = new List<ushort>();
                strList.ForEach(m => result.Add(Convert.ToUInt16(m)));
                master.WriteMultipleRegisters((byte)SlaceID, (ushort)StartAdr, result.ToArray());
            }
            catch { }

        }
        /// <summary>
        /// 读取输出线圈
        /// </summary>
        /// <returns></returns>
        public bool[] ReadCoils(int SlaceID, int StartAdr, int Length)
        {
            bool[] result = new bool[1] { false };
            try
            {
                return master.ReadCoils((byte)SlaceID, (ushort)StartAdr, (ushort)Length);
            }
            catch
            {
                return result;
            }

        }
        /// <summary>
        /// 读取输入线圈
        /// </summary>
        /// <returns></returns>
        public bool[] ReadInputs(int SlaceID, int StartAdr, int Length)
        {
            bool[] result = new bool[1] { false };
            try
            {
                return master.ReadInputs((byte)SlaceID, (ushort)StartAdr, (ushort)Length);
            }
            catch
            {
                return result;
            }

        }
        /// <summary>
        /// 读取保持型寄存器
        /// </summary>
        /// <returns></returns>
        public ushort[] ReadHoldingRegisters(int SlaceID, int StartAdr, int Length)
        {
            ushort[] result = new ushort[1] { 0 };
            try
            {
                return master.ReadHoldingRegisters((byte)SlaceID, (ushort)StartAdr, (ushort)Length);
            }
            catch
            {
                return result;
            }

        }
        /// <summary>
        /// 读取输入寄存器
        /// </summary>
        /// <returns></returns>
        public ushort[] ReadInputRegisters(int SlaceID, int StartAdr, int Length)
        {

            ushort[] result = new ushort[1] { 0 };
            try
            {
                return master.ReadInputRegisters((byte)SlaceID, (ushort)StartAdr, (ushort)Length);
            }
            catch
            {
                return result;
            }

        }

        /// <summary>
        /// 获取窗体选中的停止位
        /// </summary>
        /// <returns></returns>
        public StopBits GetSelectedStopBits(string _stopBits)
        {
            switch ((int)Convert.ToDouble(_stopBits))
            {
                case 1:
                    return StopBits.One;
                //case 1.5:
                //    return StopBits.OnePointFive;
                case 2:
                    return StopBits.Two;
                default:
                    return StopBits.One;
            }
        }

        /// <summary>
        /// 获取计算机中可识别的串口号
        /// </summary>
        /// <returns></returns>
        public string[] GetPortNames()
        {
            return SerialPort.GetPortNames();
        }
    }

    public struct SerialPortData
    {
        /// <summary>
        /// 串口
        /// </summary>
        public string PortName;
        /// <summary>
        /// 波特率
        /// </summary>
        public int Baud;
        /// <summary>
        /// 奇偶校验
        /// </summary>
        public string parity;
        /// <summary>
        /// 数据位
        /// </summary>
        public int DataBits;
        /// <summary>
        /// 停止位
        /// </summary>
        public string StopBits;
        /// <summary>
        /// 读写模式
        /// </summary>
        public string RedaWriteMould;
    }
}
