using System.Text;

namespace GameServer.Shared;

[Serializable]
public class Message
{
    public MessageType Type { get; set; }
    public string PayLoad { get; set; } = string.Empty;

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

public enum MessageType
{
	Login, // 用户登录
	LoginBack, // 用户登录后返回 ID
	System, // 系统消息
	Guess, // 玩游戏
	Turn, // 系统提示轮到谁了
	Chat // 聊天消息
}