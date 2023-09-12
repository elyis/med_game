using med_game.src.Domain.Entities.Shared;
using med_game.src.Domain.IRepository;
using med_game.src.Domain.Models;
using med_game.src.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace med_game.src.Repository
{
    public class LecternRepository : ILecternRepository
    {
        private readonly AppDbContext _dbContext;
        public LecternRepository(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }

        public async Task<LecternModel?> CreateAsync(LecternBody lecternBody)
        {
            var lectern = await GetAsync(lecternBody.Name);
            if (lectern != null)
                return null;

            var model = new LecternModel 
            { 
                Name = lecternBody.Name, 
                Description = lecternBody.Description 
            };

            var result = await _dbContext.Lecterns.AddAsync(model);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task AddRangeAsync(IEnumerable<LecternBody> lecternBodies)
        {
            foreach(LecternBody lectern in lecternBodies)
                await CreateAsync(lectern);
        }

        public async Task<LecternModel?> GetAsync(int id)
            => await _dbContext.Lecterns.FindAsync(id);

        public async Task<LecternModel?> GetAsync(string name)
            => await _dbContext.Lecterns
            .FirstOrDefaultAsync(a => 
                a.Name.ToLower() == name.ToLower());

        public async Task<LecternModel?> GetWithModulesAsync(int id)
            => await _dbContext.Lecterns
            .Include(l => l.Modules)
            .FirstOrDefaultAsync(l => l.Id == id);

        public async Task<LecternModel?> GetWithModulesAsync(string name)
            => await _dbContext.Lecterns
            .Include(l => l.Modules)
            .FirstOrDefaultAsync(l => l.Name == name);

        public async Task<IEnumerable<LecternModel>> GetAllAsync()
            => await _dbContext.Lecterns.ToListAsync();

        public async Task<IEnumerable<LecternModel>> GetAllAsync(string pattern)
            => await _dbContext.Lecterns
            .Where(l => 
                EF.Functions.Like(l.Name.ToLower(), $"%{pattern.ToLower()}%"))
                .ToListAsync();

        public async Task<bool> RemoveAsync(int id)
        {
            var lectern = await GetAsync(id);
            if(lectern == null)
                return false;

            var result = _dbContext.Lecterns.Remove(lectern);
            await _dbContext.SaveChangesAsync();
            return result == null ? false : true;
        }

        public async Task<bool> RemoveAsync(string name)
        {
            var lectern = await GetAsync(name);
            if (lectern == null)
                return false;

            var result = _dbContext.Lecterns.Remove(lectern);
            _dbContext.SaveChanges();
            return result == null ? false : true;
        }
    }
}
