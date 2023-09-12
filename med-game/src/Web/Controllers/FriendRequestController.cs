using med_game.src.Domain.IRepository;
using med_game.src.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace med_game.src.Controllers
{
    [Route("friendReq")]
    [ApiController]
    public class FriendRequestController : ControllerBase
    {
        private readonly JwtUtilities _jwtUtilities;
        private readonly IUserRepository _userRepository;

        public FriendRequestController(JwtUtilities jwtUtilities, IUserRepository userRepository)
        {
            _jwtUtilities = jwtUtilities;
            _userRepository = userRepository;
        }


        [HttpPost("{email}")]
        [Authorize]
        [SwaggerOperation(Summary = "send friend request")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Application successfully sent")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "User is not exist")]

        public async Task<IActionResult> SendFriendRequest(string email, [FromHeader(Name = "Authorization")] string token)
        {
            string? userIdClaim = _jwtUtilities.GetClaimUserId(token);

            if (!long.TryParse(userIdClaim, out long userId))
                return Unauthorized();


            var result = await _userRepository.ApplyForFriendshipAsync(userId, email);
            return result == false ? NotFound() : Ok();
        }


        [HttpDelete("{email}")]
        [Authorize]
        [SwaggerOperation(Summary = "Сancel a friend request")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "Successfully unsubscribed")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Friend request does not exist")]

        public async Task<IActionResult> Delete(string email, [FromHeader(Name = "Authorization")] string token)
        {
            string? userIdClaim = _jwtUtilities.GetClaimUserId(token);

            if (!long.TryParse(userIdClaim, out long userId))
                return Unauthorized();

            var result = await _userRepository.RemoveSubscriptionAsync(userId, email);
            return result == null ? NotFound() : NoContent();
        }
    }
}
