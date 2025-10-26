namespace GameServer.Game.ActionMenu;

public static class Display
{
	/// <summary>
	/// 玩家状态显示
	/// </summary>
	public static string DisplayPlayerStatus(this Player player) =>
		$"""
		 玩家状态:
		 行动点数:\t{player.ActionPoints.ToString()}\t距离修正:\t{player.LocationModification.ToString()}
		 生命值:\t{player.Health:.2f}\t回血量:{player.RegenerateHealth:.2f}\t
		 伤害:\t{player.Damage:.2f}\t攻击范围:{player.AttackRange.ToString()}
		 金钱:\t{player.Money.ToString()}
		 """;

	/// <summary>
	/// 首级菜单
	/// </summary>
	public static string DisplayMainMenu(this Player player) =>
		$"""
		 现在是你的回合，输入[#]数字行动并消耗行动点数，你拥有 {player.ActionPoints} 行动点数。
		 [0] 回血，当前可回复 {player.RegenerateHealth} 生命值。
		 [1] 攻击一个距离不大于 {player.AttackRange} 的玩家，造成 {player.Damage} 伤害。
		 [2] 不花钱增强数值，将在下一个菜单中展示。
		 [3] 上超市买东西。
		 [4] 结束回合，没用完的行动点数会保存。
		 """;

	public static string DisplayMainMenuChecked(this Player player, List<Player> others, int choice)
	{
		player.ActionPoints--;
		
		switch (choice)
		{
			case 0:
				player.Health += player.RegenerateHealth;
				return "你回复了 " + player.RegenerateHealth + " 点生命值，当前生命值为 " + player.Health + "。";
			case 1:
				return new AttackPlayerMenu(player, others).DisplayMenu();
			case 2:
				
				
		}
	}

}