using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities;
using med_game.src.Models;
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

        public async Task<Module?> CreateAsync(ModuleBody moduleBody, Lectern lectern)
        {
            var isExist = await GetAsync(lectern.Name, moduleBody.ModuleName);

            if (isExist != null)
                return null;

            Module module = new()
            { 
                Name = moduleBody.ModuleName, 
                Description = moduleBody.Description 
            };

            await _dbContext.Modules.AddAsync(module);

            module.Lectern = lectern;
            _dbContext.SaveChanges();
            return module;
        }

        public async Task<IEnumerable<Module>> CreateRangeAsync(List<ModuleBody> moduleBodies, Lectern lectern)
        {
            var isNotAdded =  lectern.Modules.Where(s => !moduleBodies.Contains(s.ToModuleBody())).ToList();
            await _dbContext.Modules.AddRangeAsync(isNotAdded);
            lectern.Modules.AddRange(isNotAdded);

            _dbContext.SaveChanges();
            return isNotAdded;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Module> GetAll()
            => _dbContext.Modules;

        public async Task<Module?> GetAsync(int id)
            => await _dbContext.Modules
                .FindAsync(id);

        public Task<Module?> GetAsync(string lecternName, string moduleName)
            => _dbContext.Modules
            .FirstOrDefaultAsync(m => 
                m.Name.ToLower() == moduleName.ToLower() && 
                m.Lectern.Name.ToLower() == lecternName.ToLower()
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
