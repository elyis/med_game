using System.Net.WebSockets;

namespace med_game.src.Entities.Game
{
    public class ConnectionWithEnemy
    {
        public WebSocket WebSocket { get; set; }
        public RoomSettings RoomSettings { get; set; }
        public string EnemyEmail { get; set; }
        public int enemyAcceptedRequest = 0;
    }
}
