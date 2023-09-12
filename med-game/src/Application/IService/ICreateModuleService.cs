using med_game.src.Domain.Entities.Request;
using med_game.src.Domain.Models;

namespace med_game.src.Application.IService
{
    public interface ICreateModuleService
    {
        Task<ModuleModel?> Invoke(RequestedModuleBody moduleBody);
    }
}