using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace med_game.src.Entities
{
    public class GamingLobby
    {
        public GamingLobby(RoomSettings roomSettings)
        {
            RoomSettings = roomSettings;
        }

        public string Id { get; } = Guid.NewGuid().ToString();
        public RoomSettings RoomSettings { get; }
        public int CountPlayers { get; }

        private ConcurrentDictionary<long, WebSocket> Players { get; set; } = new();

        public bool AddPlayer(long userId, WebSocket ws)
            => Players.TryAdd(userId, ws);

    }
}
