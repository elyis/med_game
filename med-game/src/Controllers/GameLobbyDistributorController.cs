using med_game.src.Core.IRepository;
using med_game.src.Core.IService;
using med_game.src.Data;
using med_game.src.Entities;
using med_game.src.Entities.Request;
using med_game.src.Managers;
using med_game.src.Models;
using med_game.src.Repository;
using med_game.src.Service;
using med_game.src.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace med_game.src.Controllers
{
    public class GameLobbyDistributorController : ControllerBase
    {
        private readonly IGameLobbyService _gameLobbyService;

        public GameLobbyDistributorController()
        {
            AppDbContext context = new AppDbContext();
            var moduleRepository = new ModuleRepository(context);
            var lecternRepository = new LecternRepository(context);
            var jwtUtilities = new JwtUtilities();

            _gameLobbyService = new GameLobbyService(lecternRepository,
                                                     moduleRepository,
                                                     jwtUtilities);
        }



        [Route("main")]
        [HttpGet]
        [Authorize]
        public async Task Get()
        {
            await _gameLobbyService.InvokeAsync(HttpContext);
        }
    }
}
