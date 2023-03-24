using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using med_game.src.Data;
using med_game.src.Models;
using med_game.src.Entities.Game;

namespace med_game.src.Managers
{
    public static class GameLobbyDistributorManager
    {
        private static readonly ConcurrentDictionary<long, Connection> _connections = new();
        private static int isLocked = 0;
        private static readonly AppDbContext _context = new AppDbContext();
        private static readonly ILogger _logger = LoggerFactory.Create(config => config.AddConsole()).CreateLogger("distributor");

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
                        Interlocked.Exchange(ref _connections[userId].IsEnemyFound, 0);
                        Interlocked.Exchange(ref isLocked, 0);
                        return null;
                    }


                    Interlocked.Exchange(ref _connections[enemy.Key].IsEnemyFound, 1);
                    long[] userIds = new long[]
                    {
                        userId,
                        enemy.Key
                    };


                    GamingLobby lobby = new GamingLobby(roomSettings,_logger);
                    if (!lobby.GenerateQuestion())
                        await CloseAll(userIds,"Module does not contain questions", WebSocketCloseStatus.InvalidPayloadData);

                    User? user = await _context.Users.FindAsync(userId)!;
                    User? enemyUser = await _context.Users.FindAsync(enemy.Key);

                    lobby.AddPlayerInfo(userId, user?.ToGameStatisticInfo()!);
                    lobby.AddPlayerInfo(enemy.Key, enemyUser?.ToGameStatisticInfo()!);

                    await SendAll(lobby.Id, userIds);
                    await CloseAll(userIds,"Lobby successfully created", WebSocketCloseStatus.NormalClosure);
                    GlobalVariables.GamingLobbies.TryAdd(lobby.Id, lobby);


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
                if (_connections[userId].WebSocket.State == WebSocketState.Open)
                    await _connections[userId].WebSocket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }


        private static async Task CloseAll(long[] userIds, string? errorMessage, WebSocketCloseStatus status)
        {
            foreach (var userId in userIds)
            {
                if (_connections[userId].WebSocket.State == WebSocketState.Open)
                    await _connections[userId].WebSocket.CloseOutputAsync(status, errorMessage, CancellationToken.None);
            }
        }
    }
}
