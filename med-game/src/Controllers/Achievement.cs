using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities.Request;
using med_game.src.Repository;
using Microsoft.AspNetCore.Mvc;

namespace med_game.src.Controllers
{
    [Route("achievement")]
    [ApiController]
    public class Achievement : ControllerBase
    {
        private readonly IAchievementRepository _achievementRepository;

        public Achievement()
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
            return result == null ? Conflict() : Created($"{result.Id}", result);
        }
    }
}
