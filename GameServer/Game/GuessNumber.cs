using GameServer.Shared;
using GameServer.Sockets;

namespace GameServer.Game;

public class GuessNumber(Server server) : IGame
{
	private readonly Dictionary<string, string> _players = new(4); // id, name
	private bool _gameStarted;
	private int _numberToGuess;
	private int _turnIndex;
	
	private readonly Lock _turnLock = new();

	public void OnPlayerConnected(string playerId)
	{
		Console.WriteLine($"Player {playerId} joined ({_players.Count}/4)");
	}

	private void StartGame()
	{
		Thread.Sleep(1000);
		_gameStarted = true;
		_numberToGuess = new Random().Next(1, 101);
		Console.WriteLine("Game started.");
		Broadcast("System", "游戏开始了! 请猜一个 1 到 100 之间的数字.");
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

		// TODO: 解决粘包
		_ = server.BroadcastAsync(new Message { Type = "Turn", PayLoad = $"轮到玩家 {currentPlayer} 猜." });
		_ = server.SendAsync(currentId, new Message { Type = "Turn", PayLoad = "现在是你的回合, 请输入一个 1 到 100 的整数." });
	}

	public void OnMessageReceived(string playerId, Message message)
	{
		if (message.Type == "Login")
		{
			lock (_turnLock)
			{
				_players[playerId] = message.PayLoad;
			}

			Console.WriteLine($"Player {playerId} logged in as {message.PayLoad}!");
			Broadcast("System", $"玩家 {message.PayLoad} 加入了游戏! ({_players.Count}/4)");

			lock (_turnLock)
			{
				if (_players.Count == 4 && !_gameStarted)
					StartGame();
			}
			
			return;
		}
		
		if (!_gameStarted || message.Type != "Guess") return;

		string currentId, currentName;

		lock (_turnLock)
		{
			currentId = _players.Keys.ElementAt(_turnIndex);
			currentName = _players[currentId];
		}
		
		if (playerId != currentId)
		{
			_ = server.SendAsync(playerId, new Message
			{
				Type = "System",
				PayLoad = "现在不是你的回合, 等一会儿."
			});
			return;
		}

		if (!int.TryParse(message.PayLoad, out var number))
		{
			_ = server.SendAsync(playerId, new Message
			{
				Type = "System",
				PayLoad = "你说的不是整数."
			});
			return;
		}
			
		Console.WriteLine($"[{currentName}] guessed {number}");
			
		if (number == _numberToGuess)
		{
			_ = server.BroadcastAsync(new Message
			{
				Type = "System",
				PayLoad = $"玩家 {currentName} 猜对了! 正确数字是 {_numberToGuess}."
			});
			_gameStarted = false;
		}
		else
		{
			var hint = number > _numberToGuess ? "太大了" : "太小了";
			_ = server.BroadcastAsync(new Message
			{
				Type = "System",
				PayLoad = $"玩家 {currentName} 猜的数字 {number} {hint}."
			});
			
			PromptNextPlayer();
		}
	}

	public void Update()
	{
		
	}

	private void Broadcast(string type, string payLoad)
	{
		_ = server.BroadcastAsync(new Message { Type = type, PayLoad = payLoad });
	}
}