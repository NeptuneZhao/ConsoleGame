using System.Net.Sockets;
using System.Text;
using System.Text.Json;

using GameClient.Shared;

namespace GameClient;

public class Client
{
	private readonly TcpClient _client = new();

	public async Task ConnectAsync(string host, int port)
	{
		await _client.ConnectAsync(host, port);
		Console.WriteLine("Connected to server.");
		_ = ReceiveAsync();
	}

	public async Task SendAsync(Message msg)
	{
		var data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(msg));
		await _client.GetStream().WriteAsync(data);
	}

	private async Task ReceiveAsync()
	{
		var stream = _client.GetStream();
		var buffer = new byte[1024];

		while (true)
		{
			var bytesRead = await stream.ReadAsync(buffer);
			if (bytesRead == 0) break;
			var json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
			Console.WriteLine(json);
			var msg = JsonSerializer.Deserialize<Message>(json);
			Console.WriteLine($"[{msg?.Type}] {msg?.PayLoad}");
		}
	}
}