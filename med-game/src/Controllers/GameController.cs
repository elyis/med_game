using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities.Game;
using med_game.src.Managers;
using med_game.src.Repository;
using med_game.src.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace med_game.src.Controllers
{
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly JwtUtilities _jwtUtilities;
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
        private readonly IModuleRepository _moduleRepository;


        public GameController(ILoggerFactory loggerFactory, AppDbContext context)
        {
            _userRepository = new UserRepository(context);
            _moduleRepository = new ModuleRepository(context);

            _jwtUtilities = new JwtUtilities();
            _logger = loggerFactory.CreateLogger("");
        }

        [Authorize]
        [HttpGet("room/requests")]
        [SwaggerOperation(Summary = "Получить последний запрос на игру")]
        [SwaggerResponse((int) HttpStatusCode.OK, Description = "Последний запрос успешно выдан, остальные удалены", Type = typeof(RequestForFight))]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]

        public async Task<IActionResult> GetLastGameRequest()
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            string? userIdClaim = _jwtUtilities.GetClaimUserId(Request.Headers.Authorization!);
            if (!long.TryParse(userIdClaim, out long userId))
                return Unauthorized();

            var user = await _userRepository.GetAsync(userId);
            if (user == null)
                return NotFound();

            var getInviterPayload = GameLobbyDistributorWithEnemyManager.GetLastGameRequest(user.Email);
            if (getInviterPayload == null)
                return NotFound();

            var inviterPayload = await getInviterPayload;
            var inviter = await _userRepository.GetAsync(inviterPayload.UserId);
            var friendInfo = await _userRepository.GetFriendInfo(userId, inviterPayload.UserId);

            var lobby = new GamingLobby(inviterPayload.RoomSettings, _logger);
            if (!lobby.GenerateQuestion())
            {
                if (inviterPayload.Socket.State == WebSocketState.Open)
                    await inviterPayload.Socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            }
            lobby.AddPlayerInfo(userId, user.ToGameStatisticInfo());
            lobby.AddPlayerInfo(inviter.Id, inviter.ToGameStatisticInfo());

            GlobalVariables.GamingLobbies.TryAdd(lobby.Id, lobby);
            GlobalVariables.EnemyWebSocketSessions.TryAdd(userId, inviterPayload.Socket);

            var module = await _moduleRepository.GetAsync((int)inviterPayload.RoomSettings.ModuleId!);
            var requestForFight = new RequestForFight
            {
                TokenRoom = lobby.Id,
                FriendInfo = friendInfo,
                Module = module.Name
            };
            return Ok(requestForFight);
        }

        [Authorize]
        [HttpDelete("room/{lobbyId}")]
        [SwaggerOperation(Summary = "Отказаться от игры")]
        [SwaggerResponse((int) HttpStatusCode.OK, Description = "Запрос успешно отклонен")]
        
        public async Task<IActionResult> RefuseRequest(string lobbyId)
        {
            string? userIdClaim = _jwtUtilities.GetClaimUserId(Request.Headers.Authorization!);
            if (!long.TryParse(userIdClaim, out long userId))
                return Unauthorized();

            if(GlobalVariables.EnemyWebSocketSessions.TryGetValue(userId, out var enemySocket))
            {
                await enemySocket.SendAsync(Encoding.UTF8.GetBytes("424"), WebSocketMessageType.Text, true, CancellationToken.None);
                await enemySocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Запрос на игру отклонен", CancellationToken.None);
            }
            GlobalVariables.GamingLobbies.TryRemove(lobbyId, out var gamingLobbies);
            return Ok();
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

                if (GlobalVariables.EnemyWebSocketSessions.TryGetValue(userId, out var enemySocket))
                {
                    await enemySocket.SendAsync(Encoding.UTF8.GetBytes(lobbyId), WebSocketMessageType.Text, true, CancellationToken.None);
                    await enemySocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                    GlobalVariables.EnemyWebSocketSessions.Remove(userId, out var _);
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
