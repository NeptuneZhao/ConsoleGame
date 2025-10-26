
using System.Net.NetworkInformation;
using System.Runtime.Versioning;
using GameServer.Game;

namespace GameServer.Shared;

[SupportedOSPlatform("Windows")]
public class Refresh(string address, string gameName, string playerName)
{
	public Queue<Message> SystemMessages { get; } = new(1);
	
	public Player? PlayerInstance { get; set; }
	public string PlayerId { private get; set; } = string.Empty;

	private const int MaxSystemMessages = 10;
	
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
		
		SeparateLine();
		
		// 玩家状态
		if (PlayerInstance is null)
		{
			WriteColor(new Dictionary<string, ConsoleColor>
			{
				{ "玩家状态: ", ConsoleColor.White },
				{ "<未登录>\n", ConsoleColor.Red }
			}, false);
		}
		else
		{
			WriteColor(new Dictionary<string, ConsoleColor>
			{
				{ "玩家状态\n行动点数:\t", ConsoleColor.White},
				{ PlayerInstance.ActionPoints.ToString(), ConsoleColor.Cyan},
				{ "\t距离修正:\t", ConsoleColor.White },
				{ PlayerInstance.LocationModification.ToString(), ConsoleColor.Blue },
				{ "\n生命值:\t", ConsoleColor.White },
				{ PlayerInstance.Health.ToString(".2f"), ConsoleColor.Green },
				{ "\t回血量:\t", ConsoleColor.White },
				{ PlayerInstance.RegenerateHealth.ToString(".2f"), ConsoleColor.DarkGreen },
				{ "\n伤害:\t", ConsoleColor.White },
				{ PlayerInstance.Damage.ToString(".2f"), ConsoleColor.DarkYellow },
				{ "\t攻击范围:\t", ConsoleColor.White },
				{ PlayerInstance.AttackRange.ToString(), ConsoleColor.Magenta },
				{ "\n金钱:\t", ConsoleColor.White },
				{ PlayerInstance.Money.ToString() + '\n', ConsoleColor.Yellow}
			}, false);
		}
		
		SeparateLine();
		
		// 系统提示
		if (SystemMessages.Count > MaxSystemMessages) SystemMessages.Dequeue();
		var systemMessagesToShow = SystemMessages.ToList();
		foreach (var line in systemMessagesToShow)
		{
			WriteColor(new Dictionary<string, ConsoleColor>
			{
				{ "[", ConsoleColor.White }, 
				{ line.GetType().ToString(), ConsoleColor.Blue }, 
				{ "]\t", ConsoleColor.White }, 
				{ line.PayLoad + '\n', ConsoleColor.White }
			}, false);
		}
		
		for (var i = MaxSystemMessages; i > systemMessagesToShow.Count; i--)
			Console.WriteLine();
		
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