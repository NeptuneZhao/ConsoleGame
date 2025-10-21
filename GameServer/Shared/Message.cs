using System.Text;

namespace GameServer.Shared;

[Serializable]
public class Message
{
    public string Type { get; set; } = string.Empty;
    public string PayLoad { get; set; } = string.Empty;
    
    public override string ToString() => $"{Type}:{PayLoad}";

    public static byte[] ToFramedMessage(string msg)
    {
	    var buf = Encoding.UTF8.GetBytes(msg);
	    var lengthPrefix = BitConverter.GetBytes(buf.Length);
	    return lengthPrefix.Concat(buf).ToArray();
    }
}