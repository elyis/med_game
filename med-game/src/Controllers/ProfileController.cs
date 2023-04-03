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
    public class ProfileController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly FileUploader _fileUploader;
        private readonly JwtUtilities _jwtUtilities;

        public ProfileController(ILoggerFactory loggerFactory, AppDbContext context)
        {
            _userRepository = new UserRepository(context);
            _fileUploader = new FileUploader(loggerFactory);
            _jwtUtilities = new JwtUtilities();
        }



        [HttpPost("profileIcon")]
        [Authorize]
        [SwaggerOperation(Summary = "Upload profile icon to server")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Successfully created")]
        [SwaggerResponse((int)HttpStatusCode.UnsupportedMediaType, "all formats are supported 'image/'")]

        public async Task<IActionResult> UploadProfileIcon()
        {
            string? contentType = Request.Headers.ContentType;
            if (contentType?.StartsWith("image/") == true)
            {
                string token = Request.Headers.Authorization!;
                string? userIdClaim = _jwtUtilities.GetClaimUserId(token);

                if (!long.TryParse(userIdClaim, out long userId))
                    return Unauthorized();

                var filename = await _fileUploader.UploadImage(Constants.pathToProfileIcons, Request.Body);
                if (filename == null)
                    return BadRequest();

                var isUpdate = await _userRepository.UpdateImageAsync(userId, filename);
                if (!isUpdate)
                    return Unauthorized();
                return Ok(filename);
            }

            return new UnsupportedMediaTypeResult();
        }



        [HttpGet("profile")]
        [Authorize]
        [SwaggerOperation(Summary = "Get profile")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ProfileBody))]

        public async Task<IActionResult> GetProfile()
        {
            string token = Request.Headers.Authorization!;
            string? userIdClaim = _jwtUtilities.GetClaimUserId(token);

            if (!long.TryParse(userIdClaim, out long userId))
                return Unauthorized();

            var user = await _userRepository.GetProfileAsync(userId);
            return user == null ? NotFound() : Ok(user);
        }



        [HttpGet("profileIcon/{filename}")]
        [SwaggerOperation(Summary = "Get profile image")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(File))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]

        public async Task<IActionResult> GetProfileImage(string filename)
        {
            var bytes = await _fileUploader.GetStreamImage(Constants.pathToProfileIcons, filename);
            if (bytes == null)
                return NotFound();

            return File(bytes, $"image/jpeg", filename);
        }
    }
}
