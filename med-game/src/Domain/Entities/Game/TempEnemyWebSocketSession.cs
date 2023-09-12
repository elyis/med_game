using System.Net.WebSockets;

namespace med_game.src.Domain.Entities.Game
{
    public class TempEnemyWebSocketSession
    {
        public WebSocket EnemySocket { get; set; }
    }
}