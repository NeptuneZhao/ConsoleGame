using GameServer.Shared;

namespace GameServer;

public static class Program
{
	public static async Task Main()
	{
		var server = new Server(5000);
		server.SetGame(GameType.GuessNumber);
		await server.StartAsync();
	}
}