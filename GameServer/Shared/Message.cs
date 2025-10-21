namespace GameServer.Shared;

[Serializable]
public class Message
{
    public string Type { get; set; } = string.Empty;
    public string PayLoad { get; set; } = string.Empty;
    
    public override string ToString() => $"{Type}:{PayLoad}";
}