using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities;
using med_game.src.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace med_game.src.Controllers
{
    [Route("lectern")]
    [ApiController]
    public class LecternController : ControllerBase
    {
        private readonly ILecternRepository _lecternRepository;
        public LecternController()
        {
            AppDbContext dbContext = new AppDbContext();
            _lecternRepository = new LecternRepository(dbContext);
        }


        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Conflict)]
        public async Task<IActionResult> CreateLectern(List<LecternBody> lecternBodies)
        {
            var result = await _lecternRepository.CreateRangeAsync(lecternBodies);
            return result == null ? Conflict() : Ok();
        }


        [HttpDelete("rm/{name}")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveLectern(string name)
        {
            if(name.IsNullOrEmpty()) 
                return BadRequest();

            var result = await _lecternRepository.RemoveAsync(name);
            return result == false ? NotFound() : NoContent();
        }


        [HttpGet("{name}")]
        [ProducesResponseType(typeof(LecternBody), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<LecternBody>> GetByName(string name)
        {
            if(name.IsNullOrEmpty())
                return BadRequest();

            var result = await _lecternRepository.GetAsync(name);
            return result == null ? NotFound() : Ok(result.ToLecternBody());
        }


        [HttpGet("all")]
        [ProducesResponseType(typeof(List<LecternBody>), (int) HttpStatusCode.OK)]
        public ActionResult<List<LecternBody>> GetAll()
        {
            var result = _lecternRepository.GetAll();
            return Ok(result.Select(l => l.ToLecternBody())
                .ToList());
        }


        [HttpGet("all/{pattern}")]
        [ProducesResponseType(typeof(List<LecternBody>), (int)HttpStatusCode.OK)]
        public ActionResult<List<LecternBody>> GetAll(string pattern)
        {
            if(pattern.IsNullOrEmpty())
                return BadRequest();

            var result = _lecternRepository.GetAll(pattern);
            return Ok(result.Select(l => l.ToLecternBody())
                .ToList());
        }
    }
}
