using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    public class MyTcpServer
    {
        private readonly TcpListener _listener;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public MyTcpServer(IPAddress ip, int port)
        {
            _listener = new TcpListener(ip, port);
        }


        public async Task StartAsync()
        {
            _listener.Start();
            Console.WriteLine($"[Server] started {_listener.LocalEndpoint}");
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
                    _ = Task.Run(() => HandleClientAsync(client, _cts.Token));
                }
                catch (SocketException se)
                {
                    Console.WriteLine($"[Accept] {se.SocketErrorCode}");
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            using (client)
            {
                var ep = client.Client.RemoteEndPoint;
                Console.WriteLine($"[Client] connected {ep}");
                var stream = client.GetStream();
                var buffer = new byte[4096];
                var sb = new StringBuilder();

                while (client.Connected && !token.IsCancellationRequested)
                {
                    try
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token)
                                                    .ConfigureAwait(false);
                        if (bytesRead == 0) break; // 对端断开
                        sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

                        // 粘包处理：按 '\n' 切帧
                        int idx;
                        while ((idx = sb.ToString().IndexOf('\n')) >= 0)
                        {
                            var line = sb.ToString(0, idx).Trim();
                            sb.Remove(0, idx + 1);
                            if (!string.IsNullOrEmpty(line))
                            {
                                Console.WriteLine($"[Recv] {line}");
                                var reply = line == "ping" ? "pong\n" : $"echo:{line}\n";
                                var data = Encoding.UTF8.GetBytes(reply);
                                await stream.WriteAsync(data, 0, data.Length, token)
                                            .ConfigureAwait(false);
                            }
                        }
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"[IO] {ex.Message}");
                        break;
                    }
                }
                Console.WriteLine($"[Client] disconnected {ep}");
            }
        }

        public void Stop()
        {
            _cts.Cancel();
            _listener.Stop();
            Console.WriteLine("[Server] stopped.");
        }

        public void Dispose()
        {
            Stop();
            _cts.Dispose();
        }
    }
}
/**
        var server = new TcpServer(IPAddress.Any, 5000);
        await server.StartAsync();
 * */
