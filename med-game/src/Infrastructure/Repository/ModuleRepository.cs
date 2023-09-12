
using med_game.src.Domain.Entities.Shared;
using med_game.src.Domain.IRepository;
using med_game.src.Domain.Models;
using med_game.src.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace med_game.src.Repository
{
    public class ModuleRepository : IModuleRepository
    {
        private readonly AppDbContext _dbContext;

        public ModuleRepository(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }

        public async Task<ModuleModel?> CreateAsync(ModuleBody moduleBody, LecternModel lectern)
        {
            var isExist = await GetAsync(lectern.Name, moduleBody.ModuleName);

            if (isExist != null)
                return null;

            ModuleModel module = new()
            { 
                Name = moduleBody.ModuleName, 
                Description = moduleBody.Description 
            };

            await _dbContext.Modules.AddAsync(module);

            module.LecternModel = lectern;
            _dbContext.SaveChanges();
            return module;
        }

        public async Task<IEnumerable<ModuleModel>> CreateRangeAsync(List<ModuleBody> moduleBodies, LecternModel lectern)
        {
            var isNotAdded =  lectern.Modules.Where(s => !moduleBodies.Contains(s.ToModuleBody())).ToList();
            await _dbContext.Modules.AddRangeAsync(isNotAdded);
            lectern.Modules.AddRange(isNotAdded);

            _dbContext.SaveChanges();
            return isNotAdded;
        }

        public async Task<IEnumerable<ModuleModel>> GetAll()
            => await _dbContext.Modules.ToListAsync();

        public async Task<ModuleModel?> GetAsync(int id)
            => await _dbContext.Modules
                .FindAsync(id);

        public Task<ModuleModel?> GetAsync(string lecternName, string moduleName)
            => _dbContext.Modules
            .FirstOrDefaultAsync(m => 
                m.Name.ToLower() == moduleName.ToLower() && 
                m.LecternModel.Name.ToLower() == lecternName.ToLower()
            );

        public async Task<bool> RemoveAsync(int id)
        {
            var module = await GetAsync(id);

            if (module == null)
                return false;

            _dbContext.Modules.Remove(module);
            _dbContext.SaveChanges();
            return true;
        }

        public async Task<bool> RemoveAsync(string lecternName, string moduleName)
        {
            var module = await GetAsync(lecternName, moduleName);
            if(module == null) 
                return false;    

            var result = _dbContext.Modules.Remove(module);
            _dbContext.SaveChanges();
            return result == null ? false : true;
        }
    }
}
