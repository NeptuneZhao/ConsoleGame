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

	/// <summary>
	/// 本方法已经包含消息封包与定长逻辑!
	/// </summary>
	/// <param name="msg">要发送的内容</param>
	public async Task SendAsync(Message msg)
	{
		var data = Message.ToFramedMessage(JsonSerializer.Serialize(msg));
		await _client.GetStream().WriteAsync(data);
	}
	
	private async Task ReceiveAsync()
	{
		while (true)
		{
			var buffer = new byte[4];
			var stream = _client.GetStream();
			
			if (await stream.ReadAsync(buffer) == 0) break;

			var msgLength = BitConverter.ToInt32(buffer);

			buffer = new byte[msgLength];
			var byteCount = await stream.ReadAsync(buffer);

			var messageJson = Encoding.UTF8.GetString(buffer, 0, byteCount);
			var msg = JsonSerializer.Deserialize<Message>(messageJson);

			if (msg is not null) Console.WriteLine($"[{msg.Type}] {msg.PayLoad}");
		}
	}
}