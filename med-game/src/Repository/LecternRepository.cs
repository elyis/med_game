using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities;
using med_game.src.Models;
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

        public async Task<Lectern?> CreateAsync(LecternBody lecternBody)
        {
            var lectern = await GetAsync(lecternBody.Name);
            if (lectern != null)
                return null;

            Lectern model = new Lectern 
            { 
                Name = lecternBody.Name, 
                Description = lecternBody.Description 
            };

            var result = await _dbContext.Lecterns.AddAsync(model);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<IEnumerable<Lectern>> CreateRangeAsync(IEnumerable<LecternBody> lecternBodies)
        {
            List<Lectern> lecterns= new List<Lectern>();

            foreach(LecternBody lectern in lecternBodies)
            {
                var isAdded = await CreateAsync(lectern);
                if (isAdded != null)
                    lecterns.Add(isAdded);
            }
            return lecterns;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<Lectern?> GetAsync(int id)
            => await _dbContext.Lecterns.FindAsync(id);

        public async Task<Lectern?> GetAsync(string name)
            => await _dbContext.Lecterns
            .FirstOrDefaultAsync(a => 
                a.Name.ToLower() == name.ToLower());

        public async Task<Lectern?> GetWithModulesAsync(int id)
            => await _dbContext.Lecterns
            .Include(l => l.Modules)
            .FirstOrDefaultAsync(l => l.Id == id);

        public async Task<Lectern?> GetWithModulesAsync(string name)
            => await _dbContext.Lecterns
            .Include(l => l.Modules)
            .FirstOrDefaultAsync(l => l.Name == name);

        public IEnumerable<Lectern> GetAll()
            => _dbContext.Lecterns;

        public IEnumerable<Lectern> GetAll(string pattern)
            => _dbContext.Lecterns
            .Where(l => 
                EF.Functions.Like(l.Name.ToLower(), $"%{pattern.ToLower()}%"));

        public async Task<bool> RemoveAsync(int id)
        {
            var lectern = await GetAsync(id);
            if(lectern == null)
                return false;

            var result = _dbContext.Lecterns.Remove(lectern);
            _dbContext.SaveChanges();
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
