using med_game.src.Entities;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;


namespace med_game.src.Managers
{
    public static class GameLobbyManager
    {
        private static readonly ConcurrentDictionary<long, Connection> _connections = new();
        private static int isLocked = 0;

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

                    GamingLobby lobby = new GamingLobby(roomSettings);
                    
                    long[] userIds = new long[]
                    {
                        userId,
                        enemy.Key
                    };

                    await SendAll(lobby.Id, userIds);
                    await CloseAndRemoveAll(userIds);

                    Interlocked.Exchange(ref isLocked, 0);
                    return lobby.Id;
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
                if (_connections[userId].WebSocket.State == WebSocketState.Open)
                    await _connections[userId].WebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            }
        }
    }
}
