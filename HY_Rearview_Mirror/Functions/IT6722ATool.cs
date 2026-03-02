using Helper;
using Ivi.Visa.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY_Rearview_Mirror.Functions
{
    /// <summary>
    /// 直流电源IT6722A通信类
    /// </summary>
    public class IT6722ATool
    {
        // 1. 创建资源管理器
        ResourceManager rm = new ResourceManager();
        FormattedIO488 io = new FormattedIO488();

        public IT6722ATool()
        {
            open();
        }

        /// <summary>
        /// 打开设备（资源名需与NI MAX中一致）
        /// 格式：USB0::0xFFFF::0x6722::SERIAL::INSTR
        /// </summary>
        /// <param name="resourceName"></param>
        public string open(string resourceName = "USB0::0x2EC7::0x6700::802259073807670101::INSTR")
        {
            try
            {
                // 2. 打开设备（资源名需与NI MAX中一致）
                // 格式：USB0::0xFFFF::0x6722::SERIAL::INSTR
                io.IO = (IMessage)rm.Open(resourceName, AccessMode.NO_LOCK, 2000, "");

                // 3. 查询设备信息
                io.WriteString("*IDN?");
                return io.ReadString();
            }
            catch (Exception ex)
            {
                global.writeLogSystem.Error(ex.ToString());
                return null;
            }
         
        }
        public void set()
        {
            try
            {
                // 4. 设置电压和电流限值
                io.WriteString("VOLT 12.0");    // 设置12V
                io.WriteString("CURR 2.0");     // 设置2A限流

                // 5. 开启输出
                io.WriteString("OUTP ON");
            }
            catch (Exception ex)
            {
                global.writeLogSystem.Error(ex.ToString());
                return;
            }
          
        }
        /// <summary>
        /// 读取电压和电流
        /// </summary>
        /// <returns></returns>
        public Tuple<double, double> read()
        {
            try
            {
                io.WriteString("MEAS:VOLT?");
                double voltage = io.ReadNumber();
                io.WriteString("MEAS:CURR?");
                double current = io.ReadNumber();

                return Tuple.Create(voltage, current);
            }
            catch (Exception ex)
            {
                global.writeLogSystem.Error(ex.ToString());
                return Tuple.Create(0.0, 0.0);
            }
          
        }
        /// <summary>
        /// 关闭设备
        /// </summary>
        public void close()
        {
            try
            {
                // 7. 关闭输出并断开连接
                io.WriteString("OUTP OFF");
                io.IO.Close();
            }
            catch (Exception ex)
            {
                global.writeLogSystem.Error(ex.ToString());
                return;
            }
           
        }
    }
}
