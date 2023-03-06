using med_game.src.Core.IRepository;
using med_game.src.Data;
using med_game.src.Entities.Response;
using med_game.src.Repository;
using med_game.src.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;


namespace med_game.src.Controllers
{
    [Route("")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly FileUploader _fileUploader;
        private readonly JwtUtilities _jwtUtilities;

        private readonly List<string> _validFileFormats = new()
        {
            "image/jpeg",
            "image/png",
        };


        public ProfileController()
        {
            AppDbContext context = new AppDbContext();
            _userRepository = new UserRepository(context);
            _fileUploader = new FileUploader();
            _jwtUtilities = new JwtUtilities();
        }



        [HttpPost("profileIcon")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.UnsupportedMediaType)]

        public async Task<IActionResult> UploadProfileIcon()
        {
            string? contentType = Request.Headers.ContentType;
            if (contentType != null && _validFileFormats.Contains(contentType!))
            {
                string token = Request.Headers.Authorization!;
                string? userIdClaim = _jwtUtilities.GetClaimUserId(token);

                if (!long.TryParse(userIdClaim, out long userId))
                    return Unauthorized();

                var filename = await _fileUploader.UploadImage(Constants.pathToProfileIcons, Request.Body, contentType!.Split('/').Last());
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
        [ProducesResponseType(typeof(ProfileBody), (int)HttpStatusCode.OK)]

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
        [ProducesResponseType(typeof(File), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]

        public async Task<IActionResult> GetProfileImage(string filename)
        {
            var bytes = await _fileUploader.GetStreamImage(Constants.pathToProfileIcons, filename);
            if (bytes == null)
                return NotFound();

            string extension = filename.Split('.').Last();
            return File(bytes, $"image/{extension}", filename);
        }
    }
}
