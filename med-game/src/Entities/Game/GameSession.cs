﻿using med_game.src.Entities.Response;
using System.Net.WebSockets;

namespace med_game.src.Entities.Game
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