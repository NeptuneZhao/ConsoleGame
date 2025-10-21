namespace GameServer.Shared;

public interface IGame
{
    void OnMessageReceived(string playerId, Message message);
    void Update();
    bool GameStarted { get; }
    event EventHandler GameEnded;
}