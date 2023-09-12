using med_game.src.Domain.Entities.Response;
using med_game.src.Domain.IRepository;
using med_game.src.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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


         public PlayerController(IUserRepository userRepository, JwtUtilities jwtUtilities)
        {
            _userRepository = userRepository;
            _jwtUtilities = jwtUtilities;
        }



        [HttpPost("players")]
        [Authorize]
        [SwaggerOperation(Summary = "Get users by pattern of nickname")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<UserInfo>))]

        public async Task<IActionResult> GetPlayersByPattern([FromHeader(Name = "Authorization")] string token)
        {
            string pattern;
            using (var reader = new StreamReader(Request.Body))
                pattern = await reader.ReadToEndAsync();

            string? userIdClaim = _jwtUtilities.GetClaimUserId(token);

            if (!long.TryParse(userIdClaim, out long userId))
                return Unauthorized();

            var result = await _userRepository.GetUsersAsync(userId, pattern);
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
