using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Repository;
using med_game.src.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace med_game.src.Controllers
{
    [Route("api/friendReq")]
    [ApiController]
    public class FriendRequestController : ControllerBase
    {
        private readonly JwtUtilities _jwtUtilities;

        private readonly IUserRepository _userRepository;

        public FriendRequestController()
        {
            _jwtUtilities = new JwtUtilities();
            AppDbContext context = new AppDbContext();

            _userRepository = new UserRepository(context);
        }


        [HttpPost("{email}")]
        [Authorize]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]

        public async Task<IActionResult> Post(string email)
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
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]

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
