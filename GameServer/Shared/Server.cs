using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace GameServer.Shared;

public class Server(int port)
{
    private readonly TcpListener _listener = new(IPAddress.Any, port);
    private readonly Dictionary<string, TcpClient> _clients = new();

    private IGame? _game;

    public void SetGame(IGame game)
    {
        _game = game;
    }
    
    public async Task StartAsync()
    {
        _listener.Start();
        
        Console.WriteLine($"Listening on {_listener.LocalEndpoint}");

        while (_clients.Count < 4)
        {
            var client = await _listener.AcceptTcpClientAsync();
            var clientId = Guid.NewGuid().ToString()[..8];
            _clients[clientId] = client;
            _ = HandleClientAsync(clientId, client);
        }
        
        var tcs = new TaskCompletionSource();
        _game!.GameEnded += (_, _) => tcs.SetResult();
        await tcs.Task;
    }
    
    public async Task BroadcastAsync(Message message)
    {
        var messageJson = JsonSerializer.Serialize(message);
        var messageBytes = Message.ToFramedMessage(messageJson);
        
        foreach (var stream in _clients.Values.Select(client => client.GetStream()))
            await stream.WriteAsync(messageBytes);
    }
    
    public async Task SendAsync(string clientId, Message message)
    {
        if (_clients.TryGetValue(clientId, out var client))
        {
            var messageJson = JsonSerializer.Serialize(message);
            var messageBytes = Message.ToFramedMessage(messageJson);
            await client.GetStream().WriteAsync(messageBytes);
        }
    }
    
    /// <summary>
    /// 阅读一个包大小的字节流, 别忘了在实现部分添加 null 检查!
    /// </summary>
    /// <param name="client">字节流上的客户端</param>
    /// <returns>反序列化后 Message 类</returns>
    /// <exception cref="Exception"></exception>
    /// <seealso cref="Message"/>
    private static async Task<Message?> ReadAsync(TcpClient client)
    {
        var buffer = new byte[4];
        var stream = client.GetStream();
        
        var byteCount = await stream.ReadAsync(buffer);
        
        switch (byteCount)
        { 
            case 4: break;
            default: throw new SocketException(0, $"应该读取 4 个字节, 但实际读取了 {byteCount.ToString()} 个字节.");
        }

        var msgLength = BitConverter.ToInt32(buffer);
                
        buffer = new byte[msgLength];
        byteCount = await stream.ReadAsync(buffer);
        
        if (byteCount == 0) return null;
        if (byteCount != msgLength) throw new SocketException(0, "读取的消息字节数与消息长度头不符.");
        
        var messageJson = Encoding.UTF8.GetString(buffer, 0, byteCount);
        return JsonSerializer.Deserialize<Message>(messageJson);
            
    }
    
    private async Task HandleClientAsync(string clientId, TcpClient client)
    {
        try
        {
            while (true)
            {
                var message = await ReadAsync(client);
                if (message is null) break;

                if (message.Type == MessageType.Chat)
                {
                    await BroadcastAsync(message);
                    continue;
                }

                _game?.OnMessageReceived(clientId, message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error with client {clientId}: {ex.Message}");
        }
        finally
        {
            client.Close();
            _clients.Remove(clientId);
            Console.WriteLine($"Client {clientId} disconnected.");
        }
    }
}