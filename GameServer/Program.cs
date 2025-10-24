using GameServer.Game;
using GameServer.Shared;

namespace GameServer;

public static class Program
{
	public static async Task Main()
	{
		var server = new Server(5000);
		server.SetGame(new GuessNumber(server));
		await server.StartAsync();

	}
}