using GameClient.Shared;

namespace GameClient;

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

		var client = new Client();
		await client.ConnectAsync("127.0.0.1", 5000);

		await client.SendAsync(new Message
		{
			Type = "Login",
			PayLoad = name
		});

		while (true)
		{
			var input = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(input)) continue;

			await client.SendAsync(new Message
			{
				Type = "Guess",
				PayLoad = input.Trim()
			});
		}
	}
}