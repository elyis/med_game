using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities.Response;
using med_game.src.Repository;
using med_game.src.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace med_game.src.Controllers
{
    [ApiController]
    [Route("")]
    public class FriendController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtUtilities _jwtUtilities;

        public FriendController(AppDbContext context)
        {
            _userRepository = new UserRepository(context);
            _jwtUtilities = new JwtUtilities();
        }


        [HttpPatch("friend/{email}")]
        [Authorize]
        [SwaggerOperation(Summary = "Add friend from subscribers")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The subscriber became a friend")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "no subscriber")]

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
        [SwaggerOperation(Summary = "Remove friend")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "Deleted successfully")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Friend is not exist")]

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
        [SwaggerOperation(Summary = "Get a list of friends, followings and subscribers")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<FriendInfo>))]

        public async Task<IActionResult> GetFriendsAndSubscribers()
        {
            string token = Request.Headers.Authorization!;
            string? userIdClaim = _jwtUtilities.GetClaimUserId(token);

            if (!long.TryParse(userIdClaim, out long userId))
                return Unauthorized();

            var result = await _userRepository.GetFriendsAndSubscibersInfo(userId);
            return Ok(result.ToList());
        }
    }
}
