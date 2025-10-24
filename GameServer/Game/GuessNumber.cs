using GameServer.Shared;

namespace GameServer.Game;

public class GuessNumber(Server server) : IGame
{
	private readonly Dictionary<string, string> _players = new(4); // id, name
	private readonly Lock _turnLock = new();
	
	private int _numberToGuess;
	private int _turnIndex = -1;
	
	public bool GameStarted { get; private set; }

	public event EventHandler? GameEnded;
	
	private void StartGame()
	{
		Thread.Sleep(1000);
		GameStarted = true;
		_numberToGuess = new Random().Next(1, 101);
		Console.WriteLine("游戏开始.");
		Broadcast(MessageType.System, "游戏开始了! 请猜一个 1 到 100 之间的数字.");
		PromptNextPlayer();
	}

	/// <summary>
	/// 方法加锁使用
	/// </summary>
	private void PromptNextPlayer()
	{
		string currentId, currentPlayer;
		
		lock (_turnLock)
		{
			_turnIndex = (_turnIndex + 1) % _players.Count;
			currentId = _players.Keys.ElementAt(_turnIndex);
			currentPlayer = _players[currentId];
		}
		
		_ = server.BroadcastAsync(new Message(MessageType.System, $"轮到玩家 {currentPlayer} 猜."));
		_ = server.SendAsync(currentId, new Message(MessageType.Turn, "现在是你的回合, 请输入一个 1 到 100 的整数."));
	}

	public void OnMessageReceived(string playerId, Message message)
	{
		if (message.Type == MessageType.Login)
		{
			lock (_turnLock)
			{
				_players[playerId] = message.PayLoad;
			}

			Console.WriteLine($"玩家 {message.PayLoad} 加入了游戏! ({_players.Count}/4)!");
			
			_ = server.SendAsync(playerId, new Message(MessageType.LoginBack, playerId));
			
			Broadcast(MessageType.System, $"玩家 {message.PayLoad} 加入了游戏! ({_players.Count}/4)");

			lock (_turnLock)
			{
				if (_players.Count == 4 && !GameStarted) StartGame();
			}
			
			return;
		}
		
		if (!GameStarted || message.Type != MessageType.Guess) return;

		string currentId, currentName;

		lock (_turnLock)
		{
			currentId = _players.Keys.ElementAt(_turnIndex);
			currentName = _players[currentId];
		}
		
		if (playerId != currentId)
		{
			_ = server.SendAsync(playerId, new Message(MessageType.System, "现在不是你的回合, 等一会儿."));
			return;
		}

		if (!int.TryParse(message.PayLoad, out var number))
		{
			_ = server.SendAsync(playerId, new Message(MessageType.System, "你说的不是整数."));
			return;
		}
			
		Console.WriteLine($"[{currentName}] guessed {number}");
			
		if (number == _numberToGuess)
		{
			_ = server.BroadcastAsync(new Message(MessageType.System, $"玩家 {currentName} 猜对了! 正确数字是 {_numberToGuess}."));
			GameStarted = false;
			GameEnded?.Invoke(this, EventArgs.Empty);
		}
		else
		{
			var hint = number > _numberToGuess ? "太大了" : "太小了";
			_ = server.BroadcastAsync(new Message(MessageType.System, $"玩家 {currentName} 猜的数字 {number} {hint}."));
			
			PromptNextPlayer();
		}
	}

	public void Update()
	{
		
	}

	private void Broadcast(MessageType type, string payLoad)
	{
		_ = server.BroadcastAsync(new Message(type, payLoad));
	}
}