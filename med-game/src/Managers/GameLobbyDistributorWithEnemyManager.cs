using med_game.src.Entities.Game;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace med_game.src.Managers
{
    public static class GameLobbyDistributorWithEnemyManager
    {
        private static readonly ConcurrentDictionary<EnemySearchBody, ConnectionWithEnemy> _connections = new();

        public static bool AddConnection(long userId, string enemyEmail, ConnectionWithEnemy connection)
        {
            var enemySearchBody = new EnemySearchBody
            {
                UserId = userId,
                EnemyEmail = enemyEmail,
            };
            return _connections.TryAdd(enemySearchBody, connection);
        }

        public static bool RemoveConnection(long userId, string enemyEmail)
        {
            var enemySearchBody = new EnemySearchBody
            {
                UserId = userId,
                EnemyEmail = enemyEmail
            };
            return _connections.TryRemove(enemySearchBody, out var _);
        }

        public async static Task<PlayerPayload?>? GetLastGameRequest(string email)
        {
            var opponents = _connections.Where(c => c.Key.EnemyEmail == email).ToList();
            if (opponents.Count == 0)
                return null;

            for (int i = 0; i < opponents.Count - 1; i++)
            {
                var socket = opponents[i].Value.WebSocket;
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(Encoding.UTF8.GetBytes("424"), WebSocketMessageType.Text, true, CancellationToken.None);
                    await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Игрок отклонил запрос", CancellationToken.None);
                }
            }

            var lastRequest = opponents.LastOrDefault();
            var playerPayload = new PlayerPayload
            {
                UserId = lastRequest.Key.UserId,
                RoomSettings = lastRequest.Value.RoomSettings,
                Socket = lastRequest.Value.WebSocket,
            };
            return playerPayload;
        }

    }
}
