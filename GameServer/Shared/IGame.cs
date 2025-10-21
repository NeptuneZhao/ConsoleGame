namespace GameServer.Shared;

public interface IGame
{
    void OnPlayerConnected(string playerId);
    void OnMessageReceived(string playerId, Message message);
    void Update();
}