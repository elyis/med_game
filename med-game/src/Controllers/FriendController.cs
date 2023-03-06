using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities.Response;
using med_game.src.Repository;
using med_game.src.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace med_game.src.Controllers
{
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


        //[HttpPost("friend/{email")]
        [HttpPatch("friend/{email}")]
        [Authorize]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]

        public async Task<IActionResult> ChangeSubscriberToFriend(string email)
        {
            string token = Request.Headers.Authorization!;
            string? userIdClaim = _jwtUtilities.GetClaimUserId(token);

            if (!long.TryParse(userIdClaim, out long userId))
                return Unauthorized();

            var result = await _userRepository.AddFriend(userId, email);
            return result == false ? NotFound() : Ok();
        }



        [HttpDelete("friend/{email}")]
        [Authorize]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]

        public async Task<IActionResult> RemoveFriend(string email)
        {
            string token = Request.Headers.Authorization!;
            string? userIdClaim = _jwtUtilities.GetClaimUserId(token);

            if (!long.TryParse(userIdClaim, out long userId))
                return Unauthorized();

            var result = await _userRepository.RemoveFriend(userId, email);
            return result == false ? NotFound() : NoContent();
        }

            

        [HttpGet("friends")]
        [Authorize]
        [ProducesResponseType(typeof(List<FriendInfo>), (int) HttpStatusCode.OK)]

        public async Task GetFriendsAndSubscribers()
        {
            string token = Request.Headers.Authorization!;
            string? userIdClaim = _jwtUtilities.GetClaimUserId(token);

            if (!long.TryParse(userIdClaim, out long userId))
            {
                Response.StatusCode = 401;
                return;
            }

            var result = await _userRepository.GetFriendsAndSubscibersInfo(userId);
            await Response.WriteAsJsonAsync(result);
        }

    }
}
