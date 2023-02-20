using med_game.src.Entities.Request;
using med_game.src.Models;

namespace med_game.src.Core.IService
{
    public interface IModuleService : IDisposable
    {
        Task<Module?> CreateModule(RequestedModuleBody moduleBody);
        Task<IEnumerable<Module>?> GetModules(string lecternName);
    }
}
