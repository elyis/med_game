﻿using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities.Response;
using med_game.src.Repository;
using med_game.src.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace med_game.src.Controllers
{
    [Route("")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtUtilities _jwtUtilities;


         public PlayerController()
        {
            AppDbContext context = new AppDbContext(new DbContextOptions<AppDbContext>());
            _userRepository = new UserRepository(context);
            _jwtUtilities = new JwtUtilities();
        }



        [HttpPost("players")]
        [Authorize]
        [SwaggerOperation(Summary = "Get users by pattern of nickname")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<UserInfo>))]

        public async Task<IActionResult> GetPlayersByPattern()
        {
            string pattern;
            using (var reader = new StreamReader(Request.Body))
                pattern = await reader.ReadToEndAsync();

            string token = Request.Headers.Authorization!;
            string? userIdClaim = _jwtUtilities.GetClaimUserId(token);

            if (!long.TryParse(userIdClaim, out long userId))
                return Unauthorized();

            var result = await _userRepository.GetUsers(userId, pattern);
            return Ok(result);
        }


        [HttpGet("rating")]
        [Authorize]
        [SwaggerOperation(Summary = "Get user rating")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Rating))]
        [ProducesResponseType(typeof(Rating), (int)HttpStatusCode.OK)]

        public IActionResult GetPlayerRating()
        {
            var rating = _userRepository.GetRatingInfo();
            return Ok(rating);
        }

    }
}
