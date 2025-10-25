using System.Text;
using GameServer.Shared;

namespace GameServer.Game;

public class Message(MessageType type, KillAction action, string payLoad = "")
{
	public MessageType Type { get; } = type;
	public readonly string PayLoad = payLoad;
	public KillAction Action { get; } = action;
	public string PlayerName = string.Empty;
	
	public static byte[] ToFramedMessage(string msg)
	{
		var buf = Encoding.UTF8.GetBytes(msg);
		var lengthPrefix = BitConverter.GetBytes(buf.Length);
		return lengthPrefix.Concat(buf).ToArray();
	}

	public static void WriteColor(Dictionary<string, ConsoleColor> map)
	{
		foreach (var pair in map)
		{
			Console.ForegroundColor = pair.Value;
			Console.WriteLine(pair.Key);
			Console.ResetColor();
		}
	}
}