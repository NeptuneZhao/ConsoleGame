using System.Runtime.Versioning;
using GameServer.Game;
using GameServer.Shared;

namespace GameClient;

// 注意本 csproj 添加了 GameServer 的引用
[SupportedOSPlatform("Windows")]
public static class Program
{
	public static async Task Main()
	{
		Console.Title = "二八杀客户端";
		
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

		await client.SendAsync(new Message(MessageType.Login, KillAction.System, name));

		while (true)
		{
			var input = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(input)) continue;
			if (input.StartsWith('/'))
			{
				await client.SendAsync(ParseCommand(input, name));
				continue;
			}
			Console.WriteLine("说啥呢？不懂。");
		}
	}
	
	// 客户端消息命令处理方法
	private static Message ParseCommand(string input, string playerName)
	{
		var splitInput = input.Split(' ');
		var command = splitInput[0].ToLower();
		
		switch (splitInput.Length)
		{
			case 0:
				return new Message(MessageType.System, KillAction.System, "无效命令。");
			case 1:
				return new Message(MessageType.System, KillAction.System, "命令缺少参数。");
			default:
				switch (command)
				{
					// 处理聊天命令
					case "/chat":
					case "/c" when splitInput.Length > 1:
					{
						var chatMessage = string.Join(' ', splitInput.Skip(1));
						return new MessageGuess(MessageType.Chat, chatMessage) { PlayerName = playerName };
					}
					// 处理猜测命令
					case "/guess":
					case "/g" when splitInput.Length > 1:
					{
						var guessMessage = string.Join(' ', splitInput.Skip(1));
						return new MessageGuess(MessageType.Guess, guessMessage);
					}
					default:
						return new MessageGuess(MessageType.System, "未知命令。");
				}
		}
	}
}