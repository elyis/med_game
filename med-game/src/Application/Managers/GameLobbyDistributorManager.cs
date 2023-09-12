
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using med_game.src.Domain.Entities.Game;
using med_game.src.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace med_game.src.Application.Managers
{
    public class GameLobbyDistributorManager
    {
        private static readonly ConcurrentDictionary<long, Connection> _connections = new();
        private static int isLocked = 0;
        private static readonly AppDbContext _context = new AppDbContext(new DbContextOptions<AppDbContext>());
        private static readonly ILogger _logger = LoggerFactory.Create(config => config.AddConsole()).CreateLogger("distributor");
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public static bool AddConnection(long userId, Connection connection)
            => _connections.TryAdd(userId, connection);

        public static bool RemoveConnection(long userId)
            => _connections.TryRemove(userId, out var _);


        public static async Task<string?> GetLobbyId(long userId, RoomSettings roomSettings)
        {
            await semaphore.WaitAsync();
            try
            {
                if (_connections[userId].IsEnemyFound == 0 && Interlocked.CompareExchange(ref _connections[userId].IsEnemyFound, 1, 0) == 0)
                {
                    var opponents = _connections.Where(connection =>
                            connection.Key != userId &&
                            connection.Value.IsEnemyFound == 0 &&
                            connection.Value.RoomSettings.Equals(roomSettings))
                        .Take(roomSettings.CountPlayers - 1)
                        .ToArray();

                    if (opponents.Length != roomSettings.CountPlayers - 1)
                    {
                        _connections[userId].IsEnemyFound = 0;
                        return null;
                    }

                    long[] playerIds = opponents.Select(p => p.Key).Append(userId).ToArray();

                    foreach (var opponent in opponents)
                        opponent.Value.IsEnemyFound = 1;

                    var lobby = new GamingLobby(roomSettings, _logger);
                    if (!lobby.GenerateQuestion())
                    {
                        await CloseAll(playerIds, "Module does not contain questions", WebSocketCloseStatus.InvalidPayloadData);
                        return null;
                    }

                    foreach (var playerId in playerIds)
                    {
                        var player = await _context.Users.FindAsync(playerId);
                        lobby.AddPlayerInfo(playerId, player?.ToGameStatisticInfo()!);
                    }

                    await SendAll(lobby.Id, playerIds);
                    await CloseAll(playerIds, "Lobby successfully created", WebSocketCloseStatus.NormalClosure);
                    GlobalVariables.GamingLobbies.TryAdd(lobby.Id, lobby);

                    return lobby.Id;
                }
            }
            finally
            {
                semaphore.Release();
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