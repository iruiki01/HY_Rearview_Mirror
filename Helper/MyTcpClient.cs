using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    public class MyTcpClient
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private readonly string _host;
        private readonly int _port;
        private bool _isRunning;
        private const int POLL_INTERVAL = 1000;
        private const int BUFFER_SIZE = 4096;

        public event Action<string> OnDataReceived;
        public event Action<string> OnStatusChanged;

        public MyTcpClient(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public void Start()
        {
            _isRunning = true;
            Connect();
            new Thread(ReceiveLoop) { IsBackground = true }.Start();
            new Thread(MonitorConnection) { IsBackground = true }.Start();
        }

        private void Connect()
        {
            try
            {
                _client = new TcpClient();
                _client.Connect(_host, _port);
                _stream = _client.GetStream();
                OnStatusChanged?.Invoke($"Connected to {_host}:{_port}");
            }
            catch (Exception ex)
            {
                OnStatusChanged?.Invoke($"Connection failed: {ex.Message}");
                Thread.Sleep(3000);
                if (_isRunning) Connect();
            }
        }

        private void MonitorConnection()
        {
            while (_isRunning)
            {
                try
                {
                    if (_client?.Client == null ||
                        (_client.Client.Poll(1000, SelectMode.SelectRead) &&
                         _client.Available == 0))
                    {
                        OnStatusChanged?.Invoke("Connection lost, reconnecting...");
                        Reconnect();
                    }
                }
                catch { /* Ignore */ }
                Thread.Sleep(POLL_INTERVAL);
            }
        }

        private void ReceiveLoop()
        {
            byte[] buffer = new byte[BUFFER_SIZE];
            while (_isRunning)
            {
                try
                {
                    if (_stream?.DataAvailable == true)
                    {
                        int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            OnDataReceived?.Invoke(data);
                        }
                    }
                    Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    OnStatusChanged?.Invoke($"Receive error: {ex.Message}");
                    Reconnect();
                }
            }
        }

        public void Send(string message)
        {
            try
            {
                if (_client?.Connected == true)
                {
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    _stream.Write(data, 0, data.Length);
                }
            }
            catch
            {
                Reconnect();
            }
        }

        private void Reconnect()
        {
            try { _stream?.Close(); } catch { }
            try { _client?.Close(); } catch { }
            Connect();
        }

        public void Stop()
        {
            _isRunning = false;
            try { _stream?.Close(); } catch { }
            try { _client?.Close(); } catch { }
        }
    }
}
/**
            var client = new RobustTcpClient("192.168.3.28", 8080);

            client.OnDataReceived += data =>
            Console.WriteLine($"[Received] {data}");

            client.OnStatusChanged += status =>
                Console.WriteLine($"[Status] {status}");

            client.Start();
 **/
