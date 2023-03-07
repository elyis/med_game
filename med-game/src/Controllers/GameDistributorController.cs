using med_game.src.Core.IRepository;
using med_game.src.Core.IService;
using med_game.src.Data;
using med_game.src.Entities.Request;
using med_game.src.Repository;
using med_game.src.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace med_game.src.Controllers
{
    public class GameDistributorController : ControllerBase
    {
        private readonly IModuleService _moduleService;

        public GameDistributorController()
        {
            AppDbContext context = new AppDbContext();
            ILecternRepository lecternRepository = new LecternRepository(context);
            IModuleRepository moduleRepository = new ModuleRepository(context);

            _moduleService = new ModuleService(moduleRepository, lecternRepository);
        }

        [Route("ws")]
        [HttpGet]
        public async Task Get(RoomSetting roomSetting)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                byte[] buffer = new byte[1024];

                WebSocketReceiveResult? result = null;
                MemoryStream memoryStream = new MemoryStream();

                try
                {
                    do
                    {
                        result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                        if (result.Count > 0)
                            memoryStream.Write(buffer.ToArray(), 0, result.Count);
                        else
                            throw new Exception();

                    } while (!result.EndOfMessage);

                    var json = JsonConvert.DeserializeObject<RoomSetting>(Encoding.UTF8.GetString(memoryStream.ToArray()));
                }
                catch (Exception ex)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, ex.Message, CancellationToken.None);
                }
            }
            
        }
    }
}
