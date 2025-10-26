namespace GameServer.Shared;

public enum MessageType
{
	Login, // 仅一次, 用户登录 (CtS)
	LoginBack, // 仅一次, 用户登录后返回 ID (StC)
	PlayerUpdate, // 玩家状态更新, 生效自服务器接收到消息 (StC)
	System, // 系统消息 (StC & C)
	Select, // 玩家行动轮 (CtS)
	SelectBack, // 玩家行动反馈 (StC)
	Turn, // 轮到谁了 (StC)
}