using med_game.src.Core.IRepository;
using med_game.src.Core.IService;
using med_game.src.Data;
using med_game.src.Entities;
using med_game.src.Entities.Request;
using med_game.src.Repository;
using med_game.src.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;


namespace med_game.src.Controllers
{
    [Route("module")]
    [ApiController]
    public class ModuleController : ControllerBase
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly ILecternRepository _lecternRepository;
        private readonly IModuleService _moduleService;

        public ModuleController()
        {
            AppDbContext dbContext = new AppDbContext();
            _moduleRepository = new ModuleRepository(dbContext);
            _lecternRepository = new LecternRepository(dbContext);

            _moduleService = new ModuleService(_moduleRepository, _lecternRepository);
        }


        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Conflict)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> CreateModule(RequestedModuleBody moduleBody)
        {
            var lectern = await _lecternRepository.GetAsync(moduleBody.LecternName);
            if(lectern == null) 
                return NotFound();

            var result = await _moduleService.CreateModule(moduleBody);
            return result == null ? Conflict() : Ok();
        }


        [HttpGet("{lecternName}")]
        [ProducesResponseType(typeof(List<ModuleBody>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]

        public async Task<ActionResult> GetModulesByLecternName(string lecternName)
        {
            var result = await _moduleService.GetModules(lecternName);
            return result == null ? 
                NotFound() : 
                Ok(result
                    .Select(m => 
                        m.ToModuleBody()).ToList());
        }


        [HttpDelete]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> RemoveModule(RemovableModuleBody moduleBody)
        {
            var result = await _moduleRepository.RemoveAsync(moduleBody.LecternName, moduleBody.ModuleName);
            return result == false ? NotFound() : NoContent();  
        }
    }
}
