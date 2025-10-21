using System.Net;
using System.Text;
using System.Text.Json;
using System.Net.Sockets;

using GameServer.Shared;

namespace GameServer.Sockets;

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

        while (true)
        {
            var client = await _listener.AcceptTcpClientAsync();
            var clientId = Guid.NewGuid().ToString()[..8];
            _clients[clientId] = client;
            _game?.OnPlayerConnected(clientId);
            _ = HandleClientAsync(clientId, client);
        }
    }
    
    public async Task BroadcastAsync(Message message)
    {
        var messageJson = JsonSerializer.Serialize(message);
        var messageBytes = Encoding.UTF8.GetBytes(messageJson);
        
        foreach (var stream in _clients.Values.Select(client => client.GetStream()))
            await stream.WriteAsync(messageBytes);
    }
    
    public async Task SendAsync(string clientId, Message message)
    {
        if (_clients.TryGetValue(clientId, out var client))
        {
            var messageJson = JsonSerializer.Serialize(message);
            var messageBytes = Encoding.UTF8.GetBytes(messageJson);
            await client.GetStream().WriteAsync(messageBytes);
        }
    }
    
    private async Task HandleClientAsync(string clientId, TcpClient client)
    {
        var stream = client.GetStream();
        var buffer = new byte[1024];
        
        try
        {
            while (true)
            {
                var byteCount = await stream.ReadAsync(buffer);
                if (byteCount == 0) break;
                
                var messageJson = Encoding.UTF8.GetString(buffer, 0, byteCount);
                var message = JsonSerializer.Deserialize<Message>(messageJson);

                if (message == null) continue;
                
                Console.WriteLine($"{clientId} [{message.Type}]: {message.PayLoad}");
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