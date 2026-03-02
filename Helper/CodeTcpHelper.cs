using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class CodeTcpHelper
    {
        public static string SendCommandAndGetResponse(string serverIp, int serverPort, string command)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.ConnectAsync(serverIp, serverPort).Wait(1000);
                    using (NetworkStream stream = client.GetStream())
                    {
                        // 将命令转换为字节数组
                        byte[] commandBytes = Encoding.ASCII.GetBytes(command);

                        // 发送命令
                        stream.Write(commandBytes, 0, commandBytes.Length);

                        // 接收服务器响应
                        byte[] buffer = new byte[1024];
                        int bytesRead = stream.ReadAsync(buffer, 0, buffer.Length).Result;
                        // 将接收的字节数组转换为字符串
                        string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                        return response;
                    }
                }
            }
            catch (Exception e)
            {
                return "Error: " + e.Message;
            }
        }


        /// <summary>
        /// 只发送不接收返回值
        /// </summary>
        /// <param name="serverIp"></param>
        /// <param name="serverPort"></param>
        /// <param name="command"></param>
        public static void SendCommand(string serverIp, int serverPort, string command)
        {
            try
            {
                using (TcpClient client = new TcpClient(serverIp, serverPort))
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        // 将命令转换为字节数组
                        byte[] commandBytes = Encoding.ASCII.GetBytes(command);

                        // 发送命令
                        stream.Write(commandBytes, 0, commandBytes.Length);

                        // 关闭连接
                        client.Close();
                    }
                }
            }
            catch (Exception e)
            {
                // 可以选择记录错误信息，或者根据需要处理异常
                Console.WriteLine("Error: " + e.Message);
            }
        }
    }
}
