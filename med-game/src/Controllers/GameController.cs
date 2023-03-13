using med_game.src.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using System.Net.WebSockets;
using med_game.src.Utility;
using med_game.src.Entities;
using med_game.src.Entities.Response;

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
                    await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "session isn't exist", CancellationToken.None);
                    return;
                }

                if (!GlobalVariables.GamingLobbies[lobbyId].PlayerInfo.Keys.Contains(userId))
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "player don't belong to this room", CancellationToken.None);
                    return;
                }

                GamingLobby lobby = GlobalVariables.GamingLobbies[lobbyId];
                AnswerOption? playerAnswer = null;
                WebSocketReceiveResult? result = null;


                lobby.AddPlayer(userId, webSocket);
                if (lobby.Players.Count == lobby.PlayerInfo.Count)
                {
                    if (Interlocked.CompareExchange(ref lobby.isLobbyManaged, 1, 0) == 0)
                        await lobby.Start();
                    else
                        await Task.Delay(100);
                }
                
                
                try
                {
                    while (lobby.isLobbyRunning == 0)
                        await Task.Delay(500);

                    try
                    {
                        while (webSocket.State == WebSocketState.Open && lobby.isLobbyRunning == 1)
                        {
                            var answerOptions = await lobby.ReceiveJson<AnswerOption>(webSocket);

                        }

                    }catch(Exception ex)
                    {

                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            else
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }
}
