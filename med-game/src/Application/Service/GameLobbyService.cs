using System.Net;
using System.Net.WebSockets;
using System.Text;
using med_game.src.Application.IService;
using med_game.src.Application.Managers;
using med_game.src.Domain.Entities.Game;
using med_game.src.Domain.Entities.Request;
using med_game.src.Domain.IRepository;
using med_game.src.Domain.Models;
using med_game.src.Utility;
using Newtonsoft.Json;

namespace med_game.src.Application.Service
{
    public class GameLobbyService : IGameLobbyService
    {
        private readonly ILecternRepository _lecternRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly JwtUtilities _jwtUtilities;
        private readonly ILogger<GameLobbyService> _logger;

        public GameLobbyService(ILecternRepository lecternRepository, 
                                IModuleRepository moduleRepository, 
                                JwtUtilities jwtUtilities,
                                ILogger<GameLobbyService> logger)
        {
            _lecternRepository = lecternRepository;
            _moduleRepository = moduleRepository;
            _jwtUtilities = jwtUtilities;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, string? enemyEmail = null)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            string? userIdClaim = _jwtUtilities.GetClaimUserId(context.Request.Headers.Authorization!);
            if (!long.TryParse(userIdClaim, out long userId))
                return;

            try
            {
                RoomSettingBody? roomSettingBody = await ReceiveRoomSettingJsonAsync(webSocket) ?? throw new JsonSerializationException();
                LecternModel? lectern = await _lecternRepository.GetAsync(roomSettingBody.LecternName);
                ModuleModel? module = roomSettingBody.ModuleName != null ? await _moduleRepository.GetAsync(roomSettingBody.LecternName, roomSettingBody.ModuleName) : null;

                if (module == null || lectern == null)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Module or lecture does not exist", CancellationToken.None);
                    return;
                }

                var roomSettings = new RoomSettings
                {
                    LecternId = lectern.Id,
                    ModuleId = module?.Id,
                    Type = roomSettingBody.Type,
                    CountPlayers = roomSettingBody.CountPlayers,
                };

                if (enemyEmail == null)
                {
                    var connection = new Connection
                    {
                        WebSocket = webSocket,
                        RoomSettings = roomSettings
                    };

                    if (!GameLobbyDistributorManager.AddConnection(userId, connection))
                        return;

                    while (webSocket.State == WebSocketState.Open && await GameLobbyDistributorManager.GetLobbyId(userId, roomSettings) == null)
                        await Task.Delay(2 * 1000);
                }
                else
                {
                    var connection = new ConnectionWithEnemy
                    {
                        RoomSettings = roomSettings,
                        WebSocket = webSocket,
                        EnemyEmail = enemyEmail,
                    };

                    if (!GameLobbyDistributorWithEnemyManager.AddConnection(userId, enemyEmail, connection))
                        return;

                    while (webSocket.State == WebSocketState.Open)
                        await Task.Delay(5 * 1000);
                }
            }
            catch (JsonSerializationException e)
            {
                _logger.LogCritical(e.Message);
                await webSocket.CloseOutputAsync(WebSocketCloseStatus.InvalidPayloadData, $"Serialization error: {e.Message}", CancellationToken.None);
            }
            catch (WebSocketException ex)
            {
                await webSocket.CloseOutputAsync(WebSocketCloseStatus.EndpointUnavailable, ex.Message, CancellationToken.None);
            }
            catch (Exception ex)
            {
                await webSocket.CloseOutputAsync(WebSocketCloseStatus.InternalServerError, ex.Message, CancellationToken.None);
            }
            finally
            {
                GameLobbyDistributorManager.RemoveConnection(userId);
                GameLobbyDistributorWithEnemyManager.RemoveConnection(userId, enemyEmail);
                if (webSocket.State == WebSocketState.Open)
                    await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            }
        }

        private static async Task<RoomSettingBody?> ReceiveRoomSettingJsonAsync(WebSocket webSocket)
        {
            byte[] buffer = new byte[2048];
            MemoryStream memoryStream = new();

            WebSocketReceiveResult result;

            do
            {
                result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                if (result.Count > 0)
                    memoryStream.Write(buffer, 0, result.Count);

            } while (!result.EndOfMessage);

            return JsonConvert.DeserializeObject<RoomSettingBody?>(
                Encoding.UTF8.GetString(memoryStream.ToArray()),
                new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                }
            );
        }
    }
}