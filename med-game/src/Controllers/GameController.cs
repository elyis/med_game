using med_game.src.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.WebSockets;
using med_game.src.Utility;
using med_game.src.Entities.Game;
using Swashbuckle.AspNetCore.Annotations;

namespace med_game.src.Controllers
{


    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly JwtUtilities _jwtUtilities;
        private readonly ILogger _logger;


        public GameController(ILoggerFactory loggerFactory)
        {
            _jwtUtilities = new JwtUtilities();
            _logger = loggerFactory.CreateLogger("");
        }


        [Authorize]
        [HttpGet("room/{lobbyId}")]
        [SwaggerOperation(Summary = "Game lobby")]

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
                    await webSocket.CloseOutputAsync(WebSocketCloseStatus.InvalidPayloadData, "session isn't exist", CancellationToken.None);
                    return;
                }

                if (!GlobalVariables.GamingLobbies[lobbyId].PlayerInfo.ContainsKey(userId))
                {
                    await webSocket.CloseOutputAsync(WebSocketCloseStatus.InvalidPayloadData, "player don't belong to this room", CancellationToken.None);
                    return;
                }

                GamingLobby lobby = GlobalVariables.GamingLobbies[lobbyId];
                lobby.AddPlayer(userId, webSocket);

                await lobby.ProcessLoop(webSocket, userId);
            }
            else
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }
}
