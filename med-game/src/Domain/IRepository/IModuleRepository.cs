using med_game.src.Domain.Entities.Shared;
using med_game.src.Domain.Models;

namespace med_game.src.Domain.IRepository
{
    public interface IModuleRepository
    {
        Task<ModuleModel?> CreateAsync(ModuleBody moduleBody, LecternModel lectern);
        Task<IEnumerable<ModuleModel>> CreateRangeAsync(List<ModuleBody> moduleBodies, LecternModel lectern);
        Task<ModuleModel?> GetAsync(int id);
        Task<ModuleModel?> GetAsync(string lecternName, string moduleName);
        Task<bool> RemoveAsync(int id);
        Task<bool> RemoveAsync(string lecternName, string moduleName);
        Task<IEnumerable<ModuleModel>> GetAll();
    }
}