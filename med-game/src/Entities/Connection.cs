using System.Net.WebSockets;

namespace med_game.src.Entities
{
    public class Connection
    {
        public WebSocket WebSocket { get; set; }
        public RoomSettings RoomSettings { get; set; }
        public int IsEnemyFound = 0;
    }
}
