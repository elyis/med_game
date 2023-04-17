using System.Net.WebSockets;

namespace med_game.src.Entities.Game
{
    public class PlayerPayload
    {
        public long UserId { get; set; }
        public WebSocket Socket { get; set; }
        public RoomSettings RoomSettings { get; set; }
    }
}
