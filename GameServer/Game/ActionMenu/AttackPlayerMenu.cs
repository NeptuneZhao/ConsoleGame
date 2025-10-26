namespace GameServer.Game.ActionMenu;

public class AttackPlayerMenu(Player player, List<Player> others)
{
	public string DisplayMenu()
	{
		var sb = new System.Text.StringBuilder();
		sb.AppendLine("请选择你要攻击的玩家：");
		
		for (var i = 0; i < others.Count; i++)
		{
			var other = others[i];
			var distance = player.GetDistance(other);
			if (distance <= player.AttackRange)
			{
				sb.AppendLine($"[{i}] 玩家 {other.PlayerName}，距离 {distance}，当前生命值 {other.Health:.2f}");
			}
		}
		return sb.ToString();
	}
}