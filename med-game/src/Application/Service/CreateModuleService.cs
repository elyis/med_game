using med_game.src.Application.IService;
using med_game.src.Domain.IRepository;
using med_game.src.Domain.Entities.Request;
using med_game.src.Domain.Models;
using med_game.src.Domain.Entities.Shared;

namespace med_game.src.Service
{
    public class ModuleService : ICreateModuleService
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly ILecternRepository _lecternRepository;

        public ModuleService(IModuleRepository moduleRepository, ILecternRepository lecternRepository)
        {
            _moduleRepository = moduleRepository;
            _lecternRepository = lecternRepository;
        }

        public async Task<ModuleModel?> Invoke(RequestedModuleBody moduleBody)
        {
            var lectern = await _lecternRepository.GetAsync(moduleBody.LecternName);
            if (lectern == null)
                return null;

            var result = await _moduleRepository.CreateAsync(
                    new ModuleBody 
                    { 
                        ModuleName = moduleBody.ModuleName, 
                        Description = moduleBody.Description 
                    },
                    lectern
                );
            return result;
        }
    }
}
