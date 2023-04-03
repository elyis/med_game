using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Repository;
using med_game.src.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace med_game.src.Controllers
{
    [Route("api/friendReq")]
    [ApiController]
    public class FriendRequestController : ControllerBase
    {
        private readonly JwtUtilities _jwtUtilities;
        private readonly IUserRepository _userRepository;

        public FriendRequestController(AppDbContext context)
        {
            _jwtUtilities = new JwtUtilities();
            _userRepository = new UserRepository(context);
        }


        [HttpPost("{email}")]
        [Authorize]
        [SwaggerOperation(Summary = "send friend request")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Application successfully sent")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "User is not exist")]

        public async Task<IActionResult> SendFriendRequest(string email)
        {
            string token = Request.Headers.Authorization!;
            string? userIdClaim = _jwtUtilities.GetClaimUserId(token);

            if (!long.TryParse(userIdClaim, out long userId))
                return Unauthorized();


            var result = await _userRepository.ApplyForFriendship(userId, email);
            return result == false ? NotFound() : Ok();
        }


        [HttpDelete("{email}")]
        [Authorize]
        [SwaggerOperation(Summary = "Сancel a friend request")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "Successfully unsubscribed")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Friend request does not exist")]

        public async Task<IActionResult> Delete(string email)
        {
            string token = Request.Headers.Authorization!;
            string? userIdClaim = _jwtUtilities.GetClaimUserId(token);

            if (!long.TryParse(userIdClaim, out long userId))
                return Unauthorized();

            var result = await _userRepository.RemoveFriendRequestAsync(userId, email);
            return result == null ? NotFound() : NoContent();
        }
    }
}
