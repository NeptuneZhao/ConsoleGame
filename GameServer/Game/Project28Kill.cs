using System.Text.Json;
using GameServer.Shared;

namespace GameServer.Game;

public class Project28Kill(Server server)
{
	private readonly Dictionary<string, string> _playerIdToName = new(4);
	private readonly Dictionary<string, Player> _playerIdToClass = new(4);
	private readonly Lock _turnLock = new();

	private int _turnIndex = -1;
	private int _round;
	private bool _gameStarted;
	
	public event EventHandler? GameEnded;
	
	private void StartGame()
	{
		Thread.Sleep(1000);
		_gameStarted = true;
		Console.WriteLine("游戏开始.");
		_ = server.BroadcastAsync(new Message(MessageType.System, "游戏开始了。"));
		PromptNextPlayer();
	}

	private void PromptNextPlayer()
	{
		_round++;
		
		string currentId, currentPlayerName;
		Player currentPlayer;
		
		lock (_turnLock)
		{
			_turnIndex = (_turnIndex + 1) % _playerIdToName.Count;
			currentId = _playerIdToName.Keys.ElementAt(_turnIndex);
			currentPlayerName = _playerIdToName[currentId];
			currentPlayer = _playerIdToClass[currentId];
		}
		
		_ = server.BroadcastAsync(new Message(MessageType.System,  $"第 {_round} 回合，轮到玩家 {currentPlayerName} 了。"));
		_ = server.SendAsync(currentId, new Message(MessageType.Turn,
			$"现在是你的回合，输入[#]数字行动并消耗行动点数，你拥有 {currentPlayer.ActionPoints} 行动点数。" +
			$"\n[0] 回血，当前可回复 {currentPlayer.RegenerateHealth} 生命值。" +
			$"\n[1] 攻击一个距离不大于 {currentPlayer.AttackRange} 的玩家，造成 {currentPlayer.Damage} 伤害。" +
			 "\n[2] 不花钱增强数值，将在下一个菜单中展示。" +
			 "\n[3] 上超市买东西。" +
			 "\n[4] 结束回合。"));
	}

	public void OnMessageReceived(string playerId, Message message)
	{
		foreach (var pair in _playerIdToClass) _ = server.SendAsync(pair.Key, new Message(MessageType.PlayerUpdate, JsonSerializer.Serialize(pair.Value)));
		
		if (message.Type == MessageType.Login)
		{
			lock (_turnLock)
			{
				_playerIdToName[playerId] = message.PayLoad;
				_playerIdToClass[playerId] = new Player(message.PayLoad);
			}

			Console.WriteLine($"玩家 {message.PayLoad} 加入了游戏! ({_playerIdToName.Count}/4)!");
			
			_ = server.SendAsync(playerId, new Message(MessageType.LoginBack, playerId));
			_ = server.BroadcastAsync(new Message(MessageType.System, $"玩家 {message.PayLoad} 加入了游戏! ({_playerIdToName.Count}/4)"));

			lock (_turnLock)
			{
				if (_playerIdToName.Count == 4 && !_gameStarted) StartGame();
			}
			
			return;
		}

		string currentId, currentName;
		
		lock (_turnLock)
		{
			currentId = _playerIdToName.Keys.ElementAt(_turnIndex);
			currentName = _playerIdToName[currentId];
		}

		if (playerId != currentId)
		{
			_ = server.SendAsync(playerId, new Message(MessageType.System, "现在不是你的回合，等一会儿。"));
			return;
		}
	}

	public void EndGame()
	{
		_ = server.BroadcastAsync(new Message(MessageType.System, "游戏结束！"));
		GameEnded?.Invoke(this, EventArgs.Empty);
	}
}