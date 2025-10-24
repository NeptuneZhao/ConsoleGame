using System.Runtime.Versioning;
using GameServer.Shared;

namespace GameClient;

// 注意本 csproj 添加了 GameServer 的引用
[SupportedOSPlatform("Windows")]
public static class Program
{
	public static async Task Main()
	{
		var confirmation = "N";
		var name = "Anonymous";

		while (confirmation != "Y")
		{
			Console.Write("输入你的名字: ");
			name = Console.ReadLine()?.Trim() ?? "Anonymous";
			Console.Write($"确定使用 \"{name}\" 作为你的名字? (Y/N): ");
			confirmation = Console.ReadLine()?.Trim().ToUpper();
		}

		var client = new Client("127.0.0.1", 5000, name);
		await client.ConnectAsync();

		await client.SendAsync(new Message(MessageType.Login, name));

		while (true)
		{
			var input = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(input)) continue;
			if (input.StartsWith('/'))
			{
				await client.SendAsync(ParseCommand(input));
				continue;
			}
			Console.WriteLine("说啥呢? 输入 /chat <消息> 发送聊天消息 或 /guess <消息> 进行游戏。");
		}
	}
	
	// 客户端消息命令处理方法
	private static Message ParseCommand(string input)
	{
		if (input.StartsWith("/chat "))
		{
			var chatMessage = input[6..].Trim();
			return new Message(MessageType.Chat, chatMessage);
		}

		if (!input.StartsWith("/guess ")) return new Message(MessageType.System, "未知命令。");
		
		var guessMessage = input[7..].Trim();
		return new Message(MessageType.Guess, guessMessage);

	}
}