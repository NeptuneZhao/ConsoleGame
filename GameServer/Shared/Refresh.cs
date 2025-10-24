using System.Net.NetworkInformation;
using System.Runtime.Versioning;

namespace GameServer.Shared;

[SupportedOSPlatform("Windows")]
public class Refresh(string address, string gameName, string playerName)
{
	public Queue<string> ChatLines { get; } = new(MaxChatLines);
	public Queue<Message> SystemMessages { get; } = new(MaxSystemMessages);
	
	public string PlayerId {private get; set; } = string.Empty;

	private const int MaxChatLines = 8, MaxSystemMessages = 9;
	
	public void Render()
	{
        Console.Clear();
		var latency = GetLatency(address);
		var latencyColor = latency switch
		{
			< 0 => ConsoleColor.Red,
			<= 100 => ConsoleColor.Green,
			<= 200 => ConsoleColor.Yellow,
			_ => ConsoleColor.Red
		};
		
		// 首行
		WriteColor(new Dictionary<string, ConsoleColor>
		{
			{ "[游戏: ", ConsoleColor.White }, { gameName, ConsoleColor.Yellow }, { "] ", ConsoleColor.White },
			{ "[玩家: ", ConsoleColor.White }, { playerName, ConsoleColor.Cyan }, 
			{ " (ID: ", ConsoleColor.White }, { PlayerId, ConsoleColor.Green }, { ")] ", ConsoleColor.White },
			{ "Ping: ", ConsoleColor.White }, { $"{latency}ms\n", latencyColor }
		}, false); 
		
		// 分割线
		SeparateLine();
		
		// 第 3 ~ 10 行为聊天区
		var chatLinesToShow = ChatLines.ToList();
		foreach (var line in chatLinesToShow)
		{
			WriteColor(new Dictionary<string, ConsoleColor>
			{
				{ "[Chat] ", ConsoleColor.Gray }, { line + '\n', ConsoleColor.White }
			}, false);
		}
		
		SeparateLine();
		
		// 系统提示
		var systemMessagesToShow = SystemMessages.ToList();
		foreach (var line in systemMessagesToShow)
		{
			WriteColor(new Dictionary<string, ConsoleColor>
			{
				{"[", ConsoleColor.White},
				{ line.Type.ToString(), ConsoleColor.Blue },
				{"]\t", ConsoleColor.White },
				{ line.PayLoad + '\n', ConsoleColor.White }
			}, false);
		}
		
		SeparateLine();
	}

	private static long GetLatency(string address)
	{
		var reply = new Ping().Send(address, 1000, new byte[32], new PingOptions { DontFragment = true });
		if (reply.Status == IPStatus.Success)
			return reply.RoundtripTime;
		return -1;
	}

	private static void SeparateLine(char sepChar = '-', int length = -1)
	{
		if (length == -1)
			length = Console.WindowWidth - 1;
		Console.WriteLine(new string(sepChar, length));
	}
	
	private static void WriteColor(Dictionary<string, ConsoleColor> map, bool lineBreak = true)
	{
		foreach (var pair in map)
		{
			Console.ForegroundColor = pair.Value;
			Console.Write(pair.Key + (lineBreak ? '\n' : ""));
			Console.ResetColor();
		}
	}
}