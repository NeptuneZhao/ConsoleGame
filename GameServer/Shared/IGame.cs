using GameServer.Game.GuessNumber;

namespace GameServer.Shared;

public interface IGame
{
    void OnMessageReceived(string playerId, IMessage messageGuess);
    void Update();
    bool GameStarted { get; }
    event EventHandler GameEnded;
}