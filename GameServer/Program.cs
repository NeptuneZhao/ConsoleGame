using GameServer.Shared;

namespace GameServer;

public static class Program
{
	public static async Task Main()
	{
		Console.Title = "二八杀服务器";
		
		var server = new Server(5000);
		server.SetGame();
		await server.StartAsync();
	}
}