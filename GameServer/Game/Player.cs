namespace GameServer.Game;

public class Player
{
	// 构造完毕后 +1 就是全部玩家数量
	private static int _setLocation;
	
	public string PlayerName { get; set; }
	public int Location { get; set; }
	public double Health { get; set; } = 10.0;
	public double Damage { get; set; } = 1.0;
	public int Money { get; set; } = 10;

	public Player(string playerName)
	{
		Location = _setLocation++;
		PlayerName = playerName;
	}

	public int GetDistance(Player player2)
	{
		var diff = Math.Abs(Location - player2.Location);
		return Math.Min(diff, _setLocation + 1 - diff);
	}
}