namespace GameServer.Shared;

public enum MessageType
{
	Login, // 用户登录
	LoginBack, // 用户登录后返回 ID
	PlayerUpdate, // 玩家状态更新
	System, // 系统消息
	Guess, // 玩游戏
	Turn, // 系统提示轮到谁了
}