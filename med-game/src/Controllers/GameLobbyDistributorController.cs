using med_game.src.Core.IService;
using med_game.src.Data;
using med_game.src.Repository;
using med_game.src.Service;
using med_game.src.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace med_game.src.Controllers
{
    public class GameLobbyDistributorController : ControllerBase
    {
        private readonly IGameLobbyService _gameLobbyService;

        public GameLobbyDistributorController(ILoggerFactory loggerFactory, AppDbContext context)
        {
            var moduleRepository = new ModuleRepository(context);
            var lecternRepository = new LecternRepository(context);
            var jwtUtilities = new JwtUtilities();

            _gameLobbyService = new GameLobbyService(lecternRepository,
                                                     moduleRepository,
                                                     jwtUtilities,
                                                     loggerFactory.CreateLogger("Game lobby")
                                                     );
        }



        [HttpGet("main")]
        [Authorize]
        [SwaggerOperation(Summary = "Game search")]

        public async Task Get()
            => await _gameLobbyService.InvokeAsync(HttpContext);
    }
}
