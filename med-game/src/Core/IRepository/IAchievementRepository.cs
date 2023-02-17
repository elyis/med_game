using med_game.Models;
using med_game.src.Entities.Request;

namespace med_game.src.Core.IRepository
{
    public interface IAchievementRepository : IDisposable
    {
        Task<Achievement?> GetAsync(int id);
        Task<Achievement?> GetAsync(string name);
        Task<Achievement?> AddAsync(AchievementBody achievementBody);
        Task<bool> RemoveAsync(int id);
        Task<bool> RemoveAsync(string name);
        Task<ICollection<Achievement>> GetAllAsync();
    }
}
