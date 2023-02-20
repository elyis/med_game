using med_game.src.Core.IRepository;
using med_game.src.Core.IService;
using med_game.src.Entities.Request;
using med_game.src.Models;
using med_game.src.Entities;

namespace med_game.src.Service
{
    public class ModuleService : IModuleService
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly ILecternRepository _lecternRepository;

        public ModuleService(IModuleRepository moduleRepository, ILecternRepository lecternRepository)
        {
            _moduleRepository = moduleRepository;
            _lecternRepository = lecternRepository;
        }

        public async Task<Module?> CreateModule(RequestedModuleBody moduleBody)
        {
            Lectern? lectern = await _lecternRepository.GetAsync(moduleBody.LecternName);
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

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Module>?> GetModules(string lecternName)
        {
            var result = await _lecternRepository.GetWithModulesAsync(lecternName);
            return result?.Modules;
        }
    }
}
