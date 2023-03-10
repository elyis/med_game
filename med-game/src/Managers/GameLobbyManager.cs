using med_game.src.Entities;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace med_game.src.Managers
{
    public class GameLobbyManager
    {
        private static readonly ConcurrentDictionary<long, Connection> _connections = new();
        private static GameLobbyManager? _instance;
        private static int isLocked = 0;

        private GameLobbyManager()
        {
        }

        public static GameLobbyManager GetInstance()
        {
            _instance ??= new GameLobbyManager();
            return _instance;
        }

        public static bool AddConnection(long userId, Connection connection)
            => _connections.TryAdd(userId, connection);

        public static bool RemoveConnection(long userId)
            => _connections.TryRemove(userId, out var connection);


        public static async Task<string?> GetLobbyId(long userId, RoomSettings roomSettings)
        {
            if(Interlocked.CompareExchange(ref isLocked,1,0) == 0)
            {
                if (Interlocked.CompareExchange(ref _connections[userId].IsEnemyFound, 1, 0) == 0)
                {
                    var enemy = _connections.FirstOrDefault(connection => connection.Key != userId &&
                                                            connection.Value.IsEnemyFound == 0 &&
                                                            connection.Value.RoomSettings.Equals(roomSettings));
                    if(enemy.Equals(default(KeyValuePair<long, Connection>))) {
                        Interlocked.Exchange(ref isLocked, 0);
                        Interlocked.Exchange(ref _connections[userId].IsEnemyFound, 0);
                        return null;
                    }

                    Interlocked.Exchange(ref _connections[enemy.Key].IsEnemyFound, 1);

                    string roomId = Guid.NewGuid().ToString();
                    long[] userIds = new long[]
                    {
                        userId,
                        enemy.Key
                    };

                    await SendAll(roomId, userIds);
                    await CloseAndRemoveAll(userIds);

                    Interlocked.Exchange(ref isLocked, 0);
                    return roomId;
                }

                Interlocked.Exchange(ref isLocked,0);
            }
            return null;
        }


        private static async Task SendAll(string message, long[] userIds)
        {
            foreach(var userId in userIds)
            {
                if (_connections[userId].WebSocket.CloseStatus == null)
                    await _connections[userId].WebSocket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }


        private static async Task CloseAndRemoveAll(long[] userIds)
        {
            foreach (var userId in userIds)
            {
                if (_connections[userId].WebSocket.CloseStatus == null)
                    _connections[userId].WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            }
        }
    }
}
