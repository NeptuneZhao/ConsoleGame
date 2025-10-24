using GameServer.Shared;

namespace GameServer.Game.Project28Kill;

public class Message28Kill: IMessage
{
	public MessageType Type { get; }
	public string PayLoad { get; } = string.Empty;
	public KillAction Action { get; }
	
	public new MessageType GetType() => Type;
	public string GetPayLoad() => PayLoad;
	
	public Message28Kill() { }

	public Message28Kill(MessageType type, KillAction action, string payLoad = "")
	{
		Type = type;
		PayLoad = payLoad;
		Action = action;
	}
}