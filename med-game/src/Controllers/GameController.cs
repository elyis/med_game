using med_game.src.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using System.Net.WebSockets;
using med_game.src.Utility;
using med_game.src.Entities;

namespace med_game.src.Controllers
{
    [Route("main/{lobbyId}")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly JwtUtilities _jwtUtilities;


        public GameController()
        {
            _jwtUtilities = new JwtUtilities();
        }



        [HttpGet]
        [Authorize]
        public async Task ConnectToGame(string lobbyId)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                string? userIdClaim = _jwtUtilities.GetClaimUserId(Request.Headers.Authorization!);
                if (!long.TryParse(userIdClaim, out long userId))
                {
                    Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }

                if (!GlobalVariables.GamingLobbies.ContainsKey(lobbyId))
                {
                    await webSocket.SendAsync(Encoding.UTF8.GetBytes("session isn't exist"), WebSocketMessageType.Text, true, CancellationToken.None);
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                    return;
                }

                if (!GlobalVariables.GamingLobbies[lobbyId].PlayerIds.Contains(userId))
                {
                    await webSocket.SendAsync(Encoding.UTF8.GetBytes("player don't belong to this room"), WebSocketMessageType.Text, true, CancellationToken.None);
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                    return;
                }

                GamingLobby lobby = GlobalVariables.GamingLobbies[lobbyId];
               
                AnswerOption? rightAnswer = null;
                WebSocketReceiveResult result = null;
                GlobalVariables.GamingLobbies[lobbyId].AddPlayer(userId, webSocket);
                if (GlobalVariables.GamingLobbies[lobbyId].Players.Count == GlobalVariables.GamingLobbies[lobbyId].PlayerIds.Count)

                while (webSocket.State == WebSocketState.Open)
                {
                    await webSocket.SendAsync(Encoding.UTF8.GetBytes("session is exist"), WebSocketMessageType.Text, true, CancellationToken.None);
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                }
            }
            else
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }
}
