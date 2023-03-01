using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Repository;
using med_game.src.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace med_game.src.Controllers
{
    [Route("friend")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtUtilities _jwtUtilities;

        public FriendController()
        {
            AppDbContext context = new AppDbContext();
            _userRepository = new UserRepository(context);

            _jwtUtilities = new JwtUtilities();
        }


        //[HttpPost("{email")]
        [HttpPatch("{email}")]
        [Authorize]
        public async Task<IActionResult> ChangeSubscriberToFriend(string email)
        {
            string token = Request.Headers.Authorization!;
            string? userIdClaim = _jwtUtilities.GetClaimUserId(token);

            if (!long.TryParse(userIdClaim, out long userId))
                return Unauthorized();

            return NotFound();
        }


        [HttpGet("{email}")]
        public async Task Get(string email)
        {
            
        }


    }
}
