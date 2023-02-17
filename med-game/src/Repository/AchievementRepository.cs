using med_game.Models;
using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities;
using Microsoft.EntityFrameworkCore;

namespace med_game.src.Repository
{
    public class AchievementRepository : IAchievementRepository
    {
        private readonly AppDbContext _dbContext;

        public AchievementRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Achievement?> AddAsync(AchievementBody achievementBody)
        {
            var isExist = await GetAsync(achievementBody.Name);
            if(isExist != null)
                return null;


            Achievement achievement = new Achievement
            {
                Name = achievementBody.Name,
                Description = achievementBody.Description,
                CountPoints = achievementBody.CountPoints,
                MaxCountPoints = achievementBody.MaxCountPoints,
                Image = achievementBody.UrlIcon
            };

            var result = await _dbContext.AddAsync(achievement);
            _dbContext.SaveChanges();
            return result.Entity;
        }

        public async Task<ICollection<Achievement>> GetAllAsync(string pattern)
            => await _dbContext.Achievements
            .Where(a => EF.Functions
                .Like(a.Name.ToLower(), $"%{pattern.ToLower()}%"))
            .ToListAsync();

        public async Task<ICollection<Achievement>> GetAllAsync()
            => await _dbContext.Achievements
                .ToListAsync();

        public async Task<Achievement?> GetAsync(int id)
            => await _dbContext.Achievements
                .FindAsync(id);

        public async Task<Achievement?> GetAsync(string name)
            => await _dbContext.Achievements
                .FirstOrDefaultAsync(a => a.Name.ToLower() == name.ToLower());

        public async Task<bool> RemoveAsync(int id)
        {
            var achivement = await GetAsync(id);
            if (achivement == null) 
                return false;

            var result = _dbContext.Remove(achivement);
            _dbContext.SaveChanges();
            return result == null ? false : true;
        }

        public async Task<bool> RemoveAsync(string name)
        {
            var achivement = await GetAsync(name);
            if (achivement == null) 
                return false;

            var result = _dbContext.Achievements.Remove(achivement);
            _dbContext.SaveChanges();
            return result == null ? false : true;
        }

        public void Dispose()
        {

        }
    }
}
