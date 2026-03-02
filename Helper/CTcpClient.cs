using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Helper
{
    public delegate void ReviceDelegate(string data);

    //预定义结构体，用于异步委托之间的传递。用户根据自己需要定制即可
    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;

        // Size of receive buffer.
        public const int BufferSize = 1024;

        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];

        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class CTcpClient
    {
        public ReviceDelegate reviceDelegate = null;

        bool m_bShowError = false;

        public const int port = 11000;

        //public  ManualResetEvent connectDone = new ManualResetEvent(false);
        //客户端连接成功
        public bool m_bConnect = false;
        //客户端接收数据OK
        public bool m_bTcpRecvOK = false;
        //收到的数据
        public string m_sRecvData = string.Empty;
        public int m_iPort = 0;
        public string m_shost = "";

        public Socket m_soTcpClient = null;

        public CTcpClient(string _shost, int _iPort)
        {
            m_shost = _shost;
            m_iPort = _iPort;
        }

        public void ShutDownConnect()
        {
            try
            {
                Thread.Sleep(200);
                if (m_soTcpClient != null && m_bConnect)
                {
                    m_bConnect = false;

                    m_soTcpClient.Shutdown(SocketShutdown.Both);
                    m_soTcpClient.Close();
                    m_soTcpClient = null;
                }

            }
            catch (System.Exception ex)
            {
                if (m_bShowError) { MessageBox.Show(ex.ToString()); }
            }
        }

        public bool StartClient()
        {
            try
            {
                m_bConnect = false;

                IPAddress ip = IPAddress.Parse(m_shost);
                IPEndPoint ipe = new IPEndPoint(ip, m_iPort);//把ip和端口转化为IPEndPoint实例
                m_soTcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建一个Socket
                m_soTcpClient.BeginConnect(ipe, new AsyncCallback(ConnectCallback), m_soTcpClient);

                return true;
            }
            catch (Exception ex)
            {
                if (m_bShowError) { MessageBox.Show(ex.ToString()); }
                return false;
            }

        }
        public void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndConnect(ar);

                m_bConnect = true;

                Receive(client);
            }
            catch (Exception e)
            {
                if (m_bShowError) { MessageBox.Show(e.ToString()); }
            }
        }

        public void Receive(Socket client)
        {
            try
            {
                if (!m_bConnect)
                {
                    return;
                }
                StateObject state = new StateObject();
                state.workSocket = client;
                // Begin receiving the data from the remote device.

                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                if (m_bShowError) { MessageBox.Show(e.ToString()); }
            }
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if (!m_bConnect)
                {
                    return;
                }
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;
                int bytesRead = client.EndReceive(ar);
                if (bytesRead > 0)
                {
                    state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));
                    if (state.sb.Length > 1)
                    {
                        m_sRecvData = state.sb.ToString();
                        m_sRecvData = m_sRecvData.Trim();
                        m_sRecvData = m_sRecvData.Replace("\"", "");
                        //为了避免粘包现象，以\r\n作为分割符
                        string[] arr = m_sRecvData.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        foreach (var item in arr)
                        {
                            if (item != string.Empty)
                            {
                                reviceDelegate?.Invoke(m_sRecvData);
                            }
                        }
                        state.sb.Clear();
                        m_bTcpRecvOK = true;
                    }
                }
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                ShutDownConnect();
                if (m_bShowError) { MessageBox.Show(e.ToString()); }
            }
        }

        public void Send(Socket client, String data)
        {
            if (!m_bConnect)
            {
                return;
            }
            data = data + "\r\n";
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
        }

        public void Send(Socket client, byte[] byteData)
        {
            if (!m_bConnect)
            {
                return;
            }
            //data = data + "\r\n";
            //byte[] byteData = Encoding.ASCII.GetBytes(data);
            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
        }

        public void SendCallback(IAsyncResult ar)
        {
            try
            {
                if (!m_bConnect)
                {
                    return;
                }
                Socket client = (Socket)ar.AsyncState;
                int bytesSent = client.EndSend(ar);
            }
            catch (Exception e)
            {
                m_bConnect = false;
                if (m_bShowError) { MessageBox.Show(e.ToString()); }
            }
        }
    }
}
