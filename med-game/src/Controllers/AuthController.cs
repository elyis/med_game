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

namespace med_game.src.Controllers
{
    //[Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        private readonly IJwtManager _jwtManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public AuthController(ILoggerFactory loggerFactory)
        {
            AppDbContext context = new AppDbContext();
            _jwtManager = new JwtManager();
            _userRepository = new UserRepository(context);

            _authService = new AuthService(
                    _jwtManager,
                    _userRepository
                );
            _logger = loggerFactory.CreateLogger<AuthController>();
        }


        //[HttpPost("register")]
        [HttpPost("sign_up")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]

        public async Task<IActionResult> SignUp(RegistrationBody registrationBody)
        {
            var result = await _authService.RegisterAsync(registrationBody);

            int resultStatusCode = result == null ? 409 : 200;
            _logger.LogInformation($"SignUp route: {result} with status code {resultStatusCode}");
            
            return result == null ? Conflict("Email is exist") : Ok(result);
        }


        //[HttpPost("login")]
        [HttpPost("sign_in")]
        [ProducesResponseType(typeof(TokenPair), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]

        public async Task<IActionResult> SignIn(Login login)
        {
            var result = await _authService.LoginAsync(login);

            int resultStatusCode = result == null ? 404 : 200;
            _logger.LogInformation($"SignIn route: {result} with status code {resultStatusCode}");

            return result == null ? NotFound() : Ok(result);
        }


        [HttpPost("token")]
        [ProducesResponseType(typeof(TokenPair), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]

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

            int resultStatusCode = result == null ? 404 : 200;
            _logger.LogInformation($"Token route: {result} with status code {resultStatusCode}");

            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost("reg_admin")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreateAdmin(RegistrationBody registrationBody)
        {
            var result = await _authService.RegisterAsync(registrationBody, Roles.Admin);

            int resultStatusCode = result == null ? 409 : 200;
            _logger.LogInformation($"Registration admin route: {result} with status code {resultStatusCode}");

            return result == null ? Conflict("Email is exist") : Ok(result);
        }


    }
}
