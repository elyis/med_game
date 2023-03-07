using med_game.src.Core.IManager;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace med_game.src.Managers
{
    public class GameLobbyManager : IGameLobbyManager
    {
        private static readonly ConcurrentDictionary<long, WebSocket> _sockets = new();
        private static GameLobbyManager? _instance;
        private bool isLocked = false;

        private GameLobbyManager()
        {
        }

        public static GameLobbyManager GetInstance()
        {
            if (_instance == null)
                _instance = new GameLobbyManager();
            return _instance;
        }

        public static bool AddWebSocket(long userId, WebSocket webSocket)
        {
            return _sockets.TryAdd(userId, webSocket);
        }


    }
}
