using med_game.src.Core.IService;
using med_game.src.Data;
using med_game.src.Entities.Request;
using med_game.src.Entities;
using med_game.src.Managers;
using med_game.src.Repository;
using med_game.src.Service;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using med_game.src.Core.IRepository;
using Microsoft.AspNetCore.Authorization;
using med_game.src.Models;
using med_game.src.Core.IManager;

namespace med_game.src.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtManager _jwtManager;
        private readonly IUserRepository _userRepository;

        public AuthController()
        {
            AppDbContext context = new AppDbContext();
            _jwtManager = new JwtManager();
            _userRepository = new UserRepository(context);

            _authService = new AuthService(
                    _jwtManager,
                    _userRepository
                );
        }


        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]

        public async Task<IActionResult> SignUp(RegistrationBody registrationBody)
        {
            var result = await _authService.RegisterAsync(registrationBody);
            return result == null ? Conflict("Email is exist") : Ok(result);
        }

        
        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenPair), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]

        public async Task<IActionResult> SignIn(Login login)
        {
            var result = await _authService.LoginAsync(login);
            return result == null ? NotFound() : Ok(result);
        }


        [HttpPost("restore-token")]
        [ProducesResponseType(typeof(TokenPair), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]

        public async Task<IActionResult> RestoreToken(TokenPair tokenPair)
        {
            var result = await _authService.UpdateTokenAsync(tokenPair);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost("reg_admin")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreateAdmin(RegistrationBody registrationBody)
        {
            var result = await _authService.RegisterAsync(registrationBody, Roles.Admin);
            return result == null ? Conflict("Email is exist") : Ok(result);
        }


    }
}
