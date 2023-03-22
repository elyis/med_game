using med_game.src.Core.IRepository;
using med_game.src.Core.IService;
using med_game.src.Entities.Request;
using med_game.src.Entities;
using med_game.src.Managers;
using med_game.src.Models;
using med_game.src.Utility;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using System.Net;

namespace med_game.src.Service
{
    public class GameLobbyService : IGameLobbyService
    {
        private readonly ILecternRepository _lecternRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly JwtUtilities _jwtUtilities;
        private readonly ILogger _logger;

        public GameLobbyService(ILecternRepository lecternRepository, 
                                IModuleRepository moduleRepository, 
                                JwtUtilities jwtUtilities,
                                ILogger logger)
        {
            _lecternRepository = lecternRepository;
            _moduleRepository = moduleRepository;
            _jwtUtilities = jwtUtilities;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                string? userIdClaim = _jwtUtilities.GetClaimUserId(context.Request.Headers.Authorization!);
                if (!long.TryParse(userIdClaim, out long userId))
                    return;



                try
                {
                    RoomSettingBody? roomSettingBody = await ReceiveRoomSettingJsonAsync(webSocket) ?? throw new JsonSerializationException();
                    Lectern? lectern = await _lecternRepository.GetAsync(roomSettingBody.LecternName);
                    Module? module = null;



                    if (roomSettingBody.ModuleName != null)
                        module = await _moduleRepository.GetAsync(roomSettingBody.LecternName, roomSettingBody.ModuleName);


                    if ((roomSettingBody.ModuleName != null && module == null) || lectern == null)
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

                    var connection = new Connection
                    {
                        WebSocket = webSocket,
                        RoomSettings = roomSettings
                    };

                    string? roomId = null;
                    if (!GameLobbyDistributorManager.AddConnection(userId, connection))
                        return;

                    byte[] buffer = new byte[512];
                    CancellationTokenSource source = new();
                    CancellationToken token = source.Token;

                    
                    Task<WebSocketReceiveResult> task = webSocket.ReceiveAsync(buffer, token);
                    while (webSocket.State == WebSocketState.Open && roomId == null)
                    {
                        if (task.IsCompletedSuccessfully && task.Result.MessageType == WebSocketMessageType.Close)
                            await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Distributor accepted close frame", CancellationToken.None);

                        roomId = await GameLobbyDistributorManager.GetLobbyId(userId, roomSettings);
                        await Task.Delay(1000);
                    }
                    if (task.Status == TaskStatus.Running)
                        source.Cancel();
                }

                catch (JsonSerializationException)
                {
                    await webSocket.CloseOutputAsync(WebSocketCloseStatus.InvalidPayloadData, "Serialization error", CancellationToken.None);
                }

                catch(WebSocketException ex)
                {

                }

                catch (Exception ex)
                {
                    await webSocket.CloseOutputAsync(WebSocketCloseStatus.InternalServerError, ex.Message, CancellationToken.None);
                }
                finally
                {
                    GameLobbyDistributorManager.RemoveConnection(userId);
                    if(webSocket.State == WebSocketState.Open)
                        await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                }
            }
            else
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }

        private async Task<RoomSettingBody?> ReceiveRoomSettingJsonAsync(WebSocket webSocket)
        {
            byte[] buffer = new byte[2048];

            WebSocketReceiveResult? result = null;
            MemoryStream memoryStream = new();

            do
            {
                result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                if (result.Count > 0)
                    memoryStream.Write(buffer.ToArray(), 0, result.Count);

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
