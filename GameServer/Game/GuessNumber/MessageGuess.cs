using System.Text;
using GameServer.Shared;

namespace GameServer.Game.GuessNumber;

public class MessageGuess : IMessage
{
	public MessageType Type { get; }
	public string PayLoad { get; } = string.Empty;
	
	public string PlayerName = string.Empty;
	
	public new MessageType GetType() => Type;
	public string GetPayLoad() => PayLoad;
	public MessageGuess() { }

	public MessageGuess(MessageType type, string payLoad)
	{
		Type = type;
		PayLoad = payLoad;
	}
}