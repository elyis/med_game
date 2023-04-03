﻿using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities;
using med_game.src.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;


namespace med_game.src.Controllers
{
    [Route("lectern")]
    [ApiController]
    public class LecternController : ControllerBase
    {
        private readonly ILecternRepository _lecternRepository;
        public LecternController(AppDbContext context)
        {
            _lecternRepository = new LecternRepository(context);
        }


        [HttpPost]
        [SwaggerOperation(Summary = "Create a new lectern")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.Conflict)]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreateLectern(List<LecternBody> lecternBodies)
        {
            var result = await _lecternRepository.CreateRangeAsync(lecternBodies);
            return result == null ? Conflict() : Ok();
        }


        [HttpDelete("rm/{name}")]
        [SwaggerOperation(Summary = "Remove lectern")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "Deleted successfully")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Not found by name")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> RemoveLectern(string name)
        {
            if (name.IsNullOrEmpty())
                return BadRequest();

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
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<LecternBody>))]
        public ActionResult<List<LecternBody>> GetAll()
        {
            var result = _lecternRepository.GetAll();
            return Ok(result.Select(l => l.ToLecternBody())
                .ToList());
        }


        [HttpGet("all/{pattern}")]
        [SwaggerOperation(Summary = "Get all lecterns by pattern")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<LecternBody>))]
        public ActionResult<List<LecternBody>> GetAll(string pattern)
        {
            var result = _lecternRepository.GetAll(pattern);
            return Ok(result.Select(l => l.ToLecternBody())
                .ToList());
        }
    }
}
