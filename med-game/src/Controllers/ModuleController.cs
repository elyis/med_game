using med_game.src.Core.IRepository;
using med_game.src.Core.IService;
using med_game.src.Data;
using med_game.src.Entities;
using med_game.src.Entities.Request;
using med_game.src.Repository;
using med_game.src.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;


namespace med_game.src.Controllers
{
    [Route("api/module")]
    [ApiController]
    public class ModuleController : ControllerBase
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly ILecternRepository _lecternRepository;
        private readonly IModuleService _moduleService;

        public ModuleController(AppDbContext context)
        {
            _moduleRepository = new ModuleRepository(context);
            _lecternRepository = new LecternRepository(context);

            _moduleService = new ModuleService(_moduleRepository, _lecternRepository);
        }


        [HttpPost]
        [SwaggerOperation(Summary = "Create lectern module")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Succesfully created")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Lectern is not exist")]
        [SwaggerResponse((int)HttpStatusCode.Conflict)]

        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> CreateModule(RequestedModuleBody moduleBody)
        {
            var lectern = await _lecternRepository.GetAsync(moduleBody.LecternName);
            if (lectern == null)
                return NotFound();

            var result = await _moduleService.CreateModule(moduleBody);
            return result == null ? Conflict() : Ok();
        }


        [HttpGet("{lecternName}")]
        [SwaggerOperation(Summary = "Get modules by lectern name")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ModuleBody>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Lectern is not exist")]

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
