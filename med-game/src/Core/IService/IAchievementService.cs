using med_game.Models;
using med_game.src.Entities;

namespace med_game.src.Core.IService
{
    public interface IAchievementService
    {
        Task<Achievement?> AddAsync(AchievementBody achievementBody);
    }
}
