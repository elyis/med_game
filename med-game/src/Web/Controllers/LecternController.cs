using med_game.src.Domain.Entities.Shared;
using med_game.src.Domain.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;


namespace med_game.src.Controllers
{
    [Route("lectern")]
    [ApiController]
    public class LecternController : ControllerBase
    {
        private readonly ILecternRepository _lecternRepository;
        public LecternController(ILecternRepository lecternRepository)
        {
            _lecternRepository = lecternRepository;
        }


        [HttpPost]
        [SwaggerOperation(Summary = "Create a new lectern")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreateLectern(List<LecternBody> lecternBodies)
        {
            await _lecternRepository.AddRangeAsync(lecternBodies);
            return Ok();
        }


        [HttpDelete("rm/{name}")]
        [SwaggerOperation(Summary = "Remove lectern")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "Deleted successfully")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Not found by name")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> RemoveLectern(string name)
        {
            var result = await _lecternRepository.RemoveAsync(name);
            return result == false ? NotFound() : NoContent();
        }


        [HttpGet("{name}")]
        [SwaggerOperation(Summary = "Get lectern")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(LecternBody))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Not found by name")]

        public async Task<ActionResult<LecternBody>> GetByName(string name)
        {
            var result = await _lecternRepository.GetAsync(name);
            return result == null ? NotFound() : Ok(result.ToLecternBody());
        }


        [HttpGet("all")]
        [SwaggerOperation(Summary = "Get all lecterns")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<LecternBody>))]
        public async Task<ActionResult<List<LecternBody>>> GetAll()
        {
            var result = await _lecternRepository.GetAllAsync();
            return Ok(result.Select(l => l.ToLecternBody()));
        }


        [HttpGet("all/{pattern}")]
        [SwaggerOperation(Summary = "Get all lecterns by pattern")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<LecternBody>))]
        public async Task<ActionResult<List<LecternBody>>> GetAll(string pattern)
        {
            var result = await _lecternRepository.GetAllAsync(pattern);
            return Ok(result.Select(l => l.ToLecternBody()));
        }
    }
}
