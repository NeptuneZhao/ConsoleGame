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

		await client.SendAsync(new Message
		{
			Type = MessageType.Login,
			PayLoad = name
		});

		while (true)
		{
			var input = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(input)) continue;

			await client.SendAsync(new Message
			{
				Type = MessageType.Guess,
				PayLoad = input.Trim()
			});
		}
	}
}