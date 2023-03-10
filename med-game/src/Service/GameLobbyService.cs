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

        public GameLobbyService(ILecternRepository lecternRepository, 
                                IModuleRepository moduleRepository, 
                                JwtUtilities jwtUtilities)
        {
            _lecternRepository = lecternRepository;
            _moduleRepository = moduleRepository;
            _jwtUtilities = jwtUtilities;
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
                    RoomSettingBody roomSettingBody = await ReceiveRoomSettingJsonAsync(webSocket);
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
                        ModuleId = module == null ? null : module.Id,
                        Type = roomSettingBody.Type, 
                        CountPlayers = roomSettingBody.CountPlayers,
                    };

                    var connection = new Connection
                    {
                        WebSocket = webSocket,
                        RoomSettings = roomSettings
                    };

                    if (!GameLobbyManager.AddConnection(userId, connection))
                        return;

                    string? roomId = null;

                    while (webSocket.State == WebSocketState.Open && roomId == null)
                    {
                        roomId = await GameLobbyManager.GetLobbyId(userId, roomSettings);
                        await Task.Delay(1000);
                    }
                }

                catch (JsonReaderException)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Serialization error", CancellationToken.None);
                }

                catch (Exception ex)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, ex.Message, CancellationToken.None);
                }
                finally
                {
                    GameLobbyManager.RemoveConnection(userId);
                    if (webSocket.State == WebSocketState.Open)
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                }
            }
            else
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }

        private async Task<RoomSettingBody> ReceiveRoomSettingJsonAsync(WebSocket webSocket)
        {
            byte[] buffer = new byte[2048];

            WebSocketReceiveResult? result = null;
            MemoryStream memoryStream = new MemoryStream();

            do
            {
                result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                if (result.Count > 0)
                    memoryStream.Write(buffer.ToArray(), 0, result.Count);

            } while (!result.EndOfMessage);

            return JsonConvert.DeserializeObject<RoomSettingBody>(Encoding.UTF8.GetString(memoryStream.ToArray()))!;
        }
    }
}
