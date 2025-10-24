using System.Text;
using System.Text.Json.Serialization;
using GameServer.Game.GuessNumber;
using GameServer.Game.Project28Kill;

namespace GameServer.Shared;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(MessageGuess), "MessageGuess")]
[JsonDerivedType(typeof(Message28Kill), "Message28Kill")]
public interface IMessage
{
	MessageType GetType();
	string GetPayLoad();
	
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