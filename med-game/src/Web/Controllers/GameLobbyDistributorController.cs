using med_game.src.Application.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace med_game.src.Controllers
{
    public class GameLobbyDistributorController : ControllerBase
    {
        private readonly IGameLobbyService _gameLobbyService;

        public GameLobbyDistributorController(IGameLobbyService gameLobbyService)
        {
            _gameLobbyService = gameLobbyService;
        }



        [HttpGet("main")]
        [Authorize]
        [SwaggerOperation(Summary = "Game search")]

        public async Task GameSearch()
            => await _gameLobbyService.InvokeAsync(HttpContext);

        [HttpGet("main/{email}")]
        [Authorize]
        [SwaggerOperation(Summary = "Game search vs friend")]

        public async Task FindGameAgainstFriend(string email)
            => await _gameLobbyService.InvokeAsync(HttpContext, email);
    }
}
