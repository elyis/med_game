using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Swashbuckle.AspNetCore.Annotations;
using med_game.src.Application.IService;
using med_game.src.Domain.IRepository;
using med_game.src.Domain.Entities.Request;
using med_game.src.Domain.Entities.Shared;
using med_game.src.Domain.Enums;

namespace med_game.src.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public AuthController(ILogger<AuthController> logger, IUserRepository userRepository, IAuthService authService)
        {
            _userRepository = userRepository;
            _logger = logger;
            _authService = authService;
        }


        [HttpPost("signup")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Successful registration", Type = typeof(TokenPair))]
        [SwaggerResponse((int)HttpStatusCode.Conflict)]


        public async Task<IActionResult> SignUp(SignUpBody body)
        {
            var result = await _authService.RegisterAsync(body);
            return result == null ? Conflict() : Ok(result);
        }


        [HttpPost("signin")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(TokenPair))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]

        public async Task<IActionResult> SignIn(Login login)
        {
            var user = await _userRepository.GetAsync(login.Mail);
            if (user == null)
                return NotFound();

            if (user.Password != login.Password)
                return BadRequest();

            var result = await _authService.LoginAsync(user);
            return result == null ? NotFound() : Ok(result);
        }


        [HttpPost("token")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(TokenPair))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]

        public async Task<IActionResult> RestoreToken()
        {
            using MemoryStream memoryStream = new MemoryStream();
            await Request.Body.CopyToAsync(memoryStream);

            string refreshToken = Encoding.UTF8.GetString(memoryStream.ToArray());
            var result = await _authService.UpdateTokenAsync(refreshToken);

            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost("reg_admin")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(TokenPair))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreateAdmin(SignUpBody registrationBody)
        {
            var result = await _authService.RegisterAsync(registrationBody, Roles.Admin);
            return result == null ? Conflict() : Ok(result);
        }
    }
}
