using med_game.src.Entities;
using med_game.src.Models;

namespace med_game.src.Core.IRepository
{
    public interface IModuleRepository : IDisposable
    {
        Task<Module?> CreateAsync(ModuleBody moduleBody, Lectern lectern);
        Task<IEnumerable<Module>> CreateRangeAsync(List<ModuleBody> moduleBodies, Lectern lectern);
        Task<Module?> GetAsync(int id);
        Task<Module?> GetAsync(string lecternName, string moduleName);
        Task<bool> RemoveAsync(int id);
        Task<bool> RemoveAsync(string lecternName, string moduleName);
        IEnumerable<Module> GetAll();
    }
}
