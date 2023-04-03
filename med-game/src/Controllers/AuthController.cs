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
using System.Text;
using Microsoft.AspNetCore.Http.Features;
using Swashbuckle.AspNetCore.Annotations;


namespace med_game.src.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        private readonly IJwtManager _jwtManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public AuthController(ILoggerFactory loggerFactory, AppDbContext context)
        {
            _jwtManager = new JwtManager();
            _userRepository = new UserRepository(context);

            _authService = new AuthService(
                    _jwtManager,
                    _userRepository
                );
            _logger = loggerFactory.CreateLogger<AuthController>();
        }


        [HttpPost("signup")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Successful registration", Type = typeof(TokenPair))]
        [SwaggerResponse((int)HttpStatusCode.Conflict)]


        public async Task<IActionResult> SignUp(RegistrationBody registrationBody)
        {
            var result = await _authService.RegisterAsync(registrationBody);
            return result == null ? Conflict("Email is exist") : Ok(result);
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
            var syncIOFeature = HttpContext.Features.Get<IHttpBodyControlFeature>();
            if (syncIOFeature != null)
            {
                syncIOFeature.AllowSynchronousIO = true;
            }

            using MemoryStream memoryStream = new MemoryStream();
            Request.Body.CopyTo(memoryStream);

            string refreshToken = Encoding.UTF8.GetString(memoryStream.ToArray());
            var result = await _authService.UpdateTokenAsync(refreshToken);

            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost("reg_admin")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(TokenPair))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreateAdmin(RegistrationBody registrationBody)
        {
            var result = await _authService.RegisterAsync(registrationBody, Roles.Admin);
            return result == null ? Conflict("Email is exist") : Ok(result);
        }


    }
}
