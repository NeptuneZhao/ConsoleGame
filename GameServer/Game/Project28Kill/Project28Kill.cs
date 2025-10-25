using GameServer.Game.GuessNumber;
using GameServer.Shared;

namespace GameServer.Game.Project28Kill;

public class Project28Kill(Server server)
{
	private readonly Dictionary<string, string> _playersReflection = new(4);
	private readonly List<Player> _players = new(4);
	private readonly Lock _turnLock = new();

	private int _turnIndex = -1;
	public bool GameStarted { get; private set; }
	
	public event EventHandler? GameEnded;
	
	private void StartGame()
	{
		Thread.Sleep(1000);
		GameStarted = true;
		Console.WriteLine("游戏开始.");
		_ = server.BroadcastAsync(new Message28Kill(MessageType.System, KillAction.System, "游戏开始了! 请猜一个 1 到 100 之间的数字."));
		PromptNextPlayer();
	}

	private void PromptNextPlayer()
	{
		string currentId, currentPlayer;
		
		lock (_turnLock)
		{
			_turnIndex = (_turnIndex + 1) % _playersReflection.Count;
			currentId = _playersReflection.Keys.ElementAt(_turnIndex);
			currentPlayer = _playersReflection[currentId];
		}
		
		// TODO: 修改提示信息
		_ = server.BroadcastAsync(new Message28Kill(MessageType.System, KillAction.System,  $"轮到玩家 {currentPlayer} 猜."));
		_ = server.SendAsync(currentId, new Message28Kill(MessageType.Turn, KillAction.System, "现在是你的回合, 请输入一个 1 到 100 的整数."));
	}

	public void OnMessageReceived(string playerId, Message28Kill message)
	{
		if (message.Type == MessageType.Login)
		{
			lock (_turnLock)
			{
				_playersReflection[playerId] = message.PayLoad;
				_players.Add(new Player(message.PayLoad));
			}

			Console.WriteLine($"玩家 {message.PayLoad} 加入了游戏! ({_playersReflection.Count}/4)!");
			
			_ = server.SendAsync(playerId, new Message28Kill(MessageType.LoginBack, KillAction.System, playerId));
			_ = server.BroadcastAsync(new Message28Kill(MessageType.System, KillAction.System, $"玩家 {message.PayLoad} 加入了游戏! ({_playersReflection.Count}/4)"));

			lock (_turnLock)
			{
				if (_playersReflection.Count == 4 && !GameStarted) StartGame();
			}
			
			return;
		}

		string currentId, currentName;
		
		lock (_turnLock)
		{
			currentId = _playersReflection.Keys.ElementAt(_turnIndex);
			currentName = _playersReflection[currentId];
		}

		if (playerId != currentId)
		{
			_ = server.SendAsync(playerId, new Message28Kill(MessageType.System, KillAction.System, "现在不是你的回合, 等一会儿。"));
			return;
		}
	}

	public void EndGame()
	{
		_ = server.BroadcastAsync(new Message28Kill(MessageType.System, KillAction.System, "游戏结束!"));
		GameEnded?.Invoke(this, EventArgs.Empty);
	}
}