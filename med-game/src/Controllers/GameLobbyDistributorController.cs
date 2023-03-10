using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities;
using med_game.src.Entities.Request;
using med_game.src.Managers;
using med_game.src.Models;
using med_game.src.Repository;
using med_game.src.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace med_game.src.Controllers
{
    public class GameLobbyDistributorController : ControllerBase
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly ILecternRepository _lecternRepository;
        private readonly JwtUtilities _jwtUtilities;

        public GameLobbyDistributorController()
        {
            AppDbContext context = new AppDbContext();
            _moduleRepository = new ModuleRepository(context);
            _lecternRepository = new LecternRepository(context);
            _jwtUtilities = new JwtUtilities();
        }



        [Route("main")]
        [HttpGet]
        [Authorize]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                string token = Request.Headers.Authorization!;
                string? userIdClaim = _jwtUtilities.GetClaimUserId(token);

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

                    RoomSettings roomSettings = new RoomSettings
                    {
                        LecternId = lectern.Id,
                        ModuleId = module == null ? null : module.Id,
                        Type = roomSettingBody.Type
                    };

                    Connection connection = new Connection
                    {
                        WebSocket = webSocket,
                        RoomSettings = roomSettings
                    };

                    GameLobbyManager gameLobbyManager = GameLobbyManager.GetInstance();
                    if (!GameLobbyManager.AddConnection(userId, connection))
                        return;

                    string? roomId = null;
                    while(webSocket.CloseStatus == null && roomId == null)
                    {
                        roomId = await GameLobbyManager.GetLobbyId(userId, roomSettings);
                        await Task.Delay(1000);
                    }
                }
                catch(JsonReaderException ex)
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
                    if (webSocket.CloseStatus == null)
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                }
            }
            
        }

        [NonAction]
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
