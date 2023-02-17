using med_game.Models;
using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities.Request;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

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
                Image = achievementBody.urlIcon
            };

            var result = await _dbContext.AddAsync(achievement);
            _dbContext.SaveChanges();
            return result.Entity;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

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
            var isExist = await GetAsync(id);
            if (isExist == null) 
                return false;

            var res = _dbContext.Remove(id);
            return res == null ? true : false;
        }

        public Task<bool> RemoveAsync(string name)
        {
            throw new NotImplementedException();
        }
    }
}
