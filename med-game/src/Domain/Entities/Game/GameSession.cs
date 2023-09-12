using System.Net.WebSockets;
using med_game.src.Domain.Entities.Response;

namespace med_game.src.Domain.Entities.Game
{
    public class GameSession
    {
        public GameSession(WebSocket webSocket, GameStatistics statistics)
        {
            WebSocket = webSocket;
            Statistics = statistics;
        }

        public WebSocket WebSocket { get; set; }
        public GameStatistics Statistics { get; set; }
        public int IsPlayerAnswer = 0;
    }
}