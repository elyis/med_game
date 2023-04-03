using med_game.src.Core.IRepository;
using med_game.src.Core.IService;
using med_game.src.Data;
using med_game.src.Entities;
using med_game.src.Repository;
using med_game.src.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace med_game.src.Controllers
{
    [Route("api/achievement")]
    [ApiController]
    public class AchievementController : ControllerBase
    {
        private readonly IAchievementRepository _achievementRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAchievementService _achievementService;



        public AchievementController(AppDbContext context)
        {
            _achievementRepository = new AchievementRepository(context);
            _userRepository = new UserRepository(context);

            _achievementService = new AchievementService(
                _achievementRepository,
                _userRepository
                );
        }


        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, "Achievement created for all users")]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "There is an achievement with this name")]
        [SwaggerOperation(Summary = "Create a new achievement")]

        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreateAchievement(AchievementBody achievementBody)
        {
            var result = await _achievementService.AddAsync(achievementBody);
            return result == null ? Conflict() : Ok();
        }



        [HttpGet("all/{pattern}")]
        [SwaggerOperation(Summary = "Get all achievements by pattern")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<AchievementBody>))]

        public async Task<ActionResult<List<AchievementBody>>> GetByName(string pattern)
        {
            var result = await _achievementRepository.GetAllAsync(pattern);
            return result == null ?
                NotFound() :
                Ok(result.Select(a => a.ToAchievementBody())
                    .ToList());
        }


        [HttpGet("all")]
        [SwaggerOperation(Summary = "Get all achievements")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<AchievementBody>))]

        public async Task<ActionResult<List<AchievementBody>>> GetAllAchievements()
        {
            var result = await _achievementRepository.GetAllAsync();
            return result.Select(a => a.ToAchievementBody()).ToList();
        }


        [HttpDelete("rm/{name}")]
        [SwaggerOperation(Summary = "Remove achievement by name")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "Deleted successfully")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Achievement with this name does not exist")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> RemoveAchievementByName(string name)
        {
            if (name.IsNullOrEmpty())
                return BadRequest();

            var result = await _achievementRepository.RemoveAsync(name);
            return result == false ? NotFound() : NoContent();
        }
    }
}
