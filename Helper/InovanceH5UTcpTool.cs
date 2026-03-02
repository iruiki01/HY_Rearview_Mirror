using HslCommunication;
using HslCommunication.Profinet.Inovance;
using System;

public class InovanceH5UTcpTool : IDisposable
{
    private InovanceH5UTcp client;

    public InovanceH5UTcpTool(string ip, int port = 502)
    {
        client = new InovanceH5UTcp(ip, port);
        // 建议：在第一次操作时连接，或使用长连接
        var connectResult = client.ConnectServer();
        //if (!connectResult.IsSuccess)
        //    throw new Exception($"连接 PLC 失败: {connectResult.Message}");
    }

    // 写入（基础版返回是否成功）
    public bool Write(string address, byte value)
    {
        byte[] data = new byte[] { 0x00, value };  // 小端：低字节在前
        var result = client.Write(address, value);
        return result.IsSuccess;
    }

    // 读取（基础版返回字节数组）
    public int Read(string address, ushort length)
    {
        int value = -1;
        OperateResult<byte[]> result = client.Read(address, length);
        if (result.IsSuccess)
        {
            byte[] data = result.Content;

            // 解析数据
            value = Convert.ToInt32(data[1]);
        }
        else
        {
            Console.WriteLine($"读取失败: {result.Message}");
        }
        return value;
    }

    // 读取（便捷版：直接返回 bool 表示是否成功，通过 out 参数输出数据）
    public bool Read(string address, ushort length, out byte[] data)
    {
        var result = client.Read(address, length);
        data = result.IsSuccess ? result.Content : null;
        return result.IsSuccess;
    }

    // 读取 Int32（假设 PLC 中存储的是 32 位整数）
    public bool ReadInt32(string address, out int value)
    {
        var result = client.ReadInt32(address);
        value = result.IsSuccess ? result.Content : 0;
        return result.IsSuccess;
    }

    // 读取 Float（假设 PLC 中存储的是浮点数）
    public bool ReadFloat(string address, out float value)
    {
        var result = client.ReadFloat(address);
        value = result.IsSuccess ? result.Content : 0f;
        return result.IsSuccess;
    }

    // 读取 Bool（位地址，如 "M0", "Y0"）
    public bool ReadBool(string address, out bool value)
    {
        var result = client.ReadBool(address);
        value = result.IsSuccess ? result.Content : false;
        return result.IsSuccess;
    }

    public void Dispose()
    {
        client?.ConnectClose();
    }
}