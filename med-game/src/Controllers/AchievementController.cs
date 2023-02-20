using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities;
using med_game.src.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace med_game.src.Controllers
{
    [Route("achievement")]
    [ApiController]
    public class AchievementController : ControllerBase
    {
        private readonly IAchievementRepository _achievementRepository;

        public AchievementController()
        {
            AppDbContext dbContext = new AppDbContext();
            _achievementRepository = new AchievementRepository(dbContext);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAchievement(AchievementBody achievementBody)
        {
            if (achievementBody == null) 
                return BadRequest();

            var result = await _achievementRepository.AddAsync(achievementBody);
            return result == null ? Conflict() : Ok();
        }


        [HttpGet("all/{pattern}")]
        [ProducesResponseType(typeof(List<AchievementBody>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<AchievementBody>>> GetByName(string pattern)
        {
            if(pattern.IsNullOrEmpty())
                return BadRequest();

            var result = await _achievementRepository.GetAllAsync(pattern);
            return result == null ? 
                NotFound() : 
                Ok(result.Select(a => a.ToAchievementBody())
                    .ToList());
        }


        [HttpGet("all")]
        [ProducesResponseType(typeof(List<AchievementBody>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<AchievementBody>>> GetAllAchievements()
        {
            var result = await _achievementRepository.GetAllAsync();
            return result.Select(a => a.ToAchievementBody()).ToList();
        }


        [HttpDelete("rm/{name}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveAchievementByName(string name)
        {
            if(name.IsNullOrEmpty())
                return BadRequest();

            var result = await _achievementRepository.RemoveAsync(name);
            return result == false ? NotFound() : NoContent();
        }
    }
}
