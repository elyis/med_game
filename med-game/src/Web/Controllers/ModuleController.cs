using med_game.src.Application.IService;
using med_game.src.Domain.Entities.Request;
using med_game.src.Domain.Entities.Shared;
using med_game.src.Domain.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace med_game.src.Controllers
{
    [Route("module")]
    [ApiController]
    public class ModuleController : ControllerBase
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly ILecternRepository _lecternRepository;
        private readonly ICreateModuleService _moduleService;

        public ModuleController(
            IModuleRepository moduleRepository, 
            ILecternRepository lecternRepository, 
            ICreateModuleService moduleService)
        {
            _moduleRepository = moduleRepository;
            _lecternRepository = lecternRepository;

            _moduleService = moduleService;
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Create lectern module")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succesfully created")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Lectern is not exist")]

        public async Task<ActionResult> CreateModule(RequestedModuleBody moduleBody)
        {
            var result = await _moduleService.Invoke(moduleBody);
            return result == null ? NotFound() : Ok();
        }


        [HttpGet("{lecternName}")]
        [SwaggerOperation(Summary = "Get modules by lectern name")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ModuleBody>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Lectern is not exist")]

        public async Task<ActionResult> GetModulesByLecternName(string lecternName)
        {
            var result = await _lecternRepository.GetWithModulesAsync(lecternName);
            return result == null ?
                NotFound() :
                Ok(result.Modules
                    .Select(m => m.Name));
        }


        [HttpDelete]
        [SwaggerOperation(Summary = "Remove module")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "Successfully removed")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Module is not exist")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> RemoveModule(RemovableModuleBody moduleBody)
        {
            var result = await _moduleRepository.RemoveAsync(moduleBody.LecternName, moduleBody.ModuleName);
            return result == false ? NotFound() : NoContent();
        }
    }
}
