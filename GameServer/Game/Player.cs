namespace GameServer.Game;

public class Player
{
	// 构造完毕后 +1 就是全部玩家数量
	private static int _setLocation;

	public string PlayerName;
	public int Location;
	public int LocationModification = 0;

	public int ActionPoints = 1;
	
	public double Health = 10.0;
	public double RegenerateHealth = 1.0;
	
	public double Damage = 1.0;
	public int AttackRange = 1;
	
	public int Money = 10;

	public Player(string playerName)
	{
		Location = _setLocation++;
		PlayerName = playerName;
	}

	public int GetDistance(Player player2)
	{
		var diff = Math.Abs(Location - player2.Location);
		return Math.Min(diff, _setLocation + 1 - diff) + LocationModification;
	}
}