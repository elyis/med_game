using med_game.src.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace med_game.src.Controllers
{
    [Route("")]
    [ApiController]
    public class ImageUploaderController : ControllerBase
    {
        private readonly FileUploader _fileUploader;
        public ImageUploaderController(ILoggerFactory loggerFactory)
        {
            _fileUploader = new FileUploader(loggerFactory);
        }



        [HttpPost("answerIcon")]
        [Authorize]
        [SwaggerOperation(Summary = "Upload answer icon to server")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.UnsupportedMediaType, "All formats are supported 'image/'")]

        public async Task<IActionResult> UploadAnswerIcon()
        {
            string? contentType = Request.Headers.ContentType;
            if (contentType?.StartsWith("image/") == true)
            {
                string? filename = await _fileUploader.UploadImage(Constants.pathToAnswerIcons, Request.Body);
                return filename == null ? BadRequest() : Ok(filename);
            }

            return new UnsupportedMediaTypeResult();
        }



        [HttpPost("questionIcon")]
        [Authorize]
        [SwaggerOperation(Summary = "Upload question icon to server")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.UnsupportedMediaType, "All formats are supported 'image/'")]

        public async Task<IActionResult> UploadQuestionIcon()
        {
            string? contentType = Request.Headers.ContentType;
            if (contentType?.StartsWith("image/") == true)
            {
                string? filename = await _fileUploader.UploadImage(Constants.pathToQuestionIcons, Request.Body);
                return filename == null ? BadRequest() : Ok(filename);
            }

            return new UnsupportedMediaTypeResult();
        }



        [HttpPost("achievementIcon")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Upload achievement icon to server")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.UnsupportedMediaType, "All formats are supported 'image/'")]

        public async Task<IActionResult> UploadAchievementIcon()
        {
            string? contentType = Request.Headers.ContentType;
            if (contentType?.StartsWith("image/") == true)
            {
                string? filename = await _fileUploader.UploadImage(Constants.pathToAchievementIcons, Request.Body);
                return filename == null ? BadRequest() : Ok(filename);
            }

            return new UnsupportedMediaTypeResult();
        }



        [HttpGet("answerIcon/{filename}")]
        [SwaggerOperation(Summary = "Get answer icon")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(File))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Image not exist")]

        public async Task<IActionResult> GetAnswerIcon(string filename)
        {
            var bytes = await _fileUploader.GetStreamImage(Constants.pathToAnswerIcons, filename);
            if (bytes == null)
                return NotFound();

            return File(bytes, "image/jpeg", filename);
        }



        [HttpGet("questionIcon/{filename}")]
        [SwaggerOperation(Summary = "Get question icon")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(File))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Image not exist")]

        public async Task<IActionResult> GetQuestionIcon(string filename)
        {
            var bytes = await _fileUploader.GetStreamImage(Constants.pathToQuestionIcons, filename);
            if (bytes == null)
                return NotFound();

            return File(bytes, "image/jpeg", filename);
        }



        [HttpGet("achievementIcon/{filename}")]
        [SwaggerOperation(Summary = "Get achievement icon")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(File))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Image not exist")]

        public async Task<IActionResult> GetAchievementIcon(string filename)
        {
            var bytes = await _fileUploader.GetStreamImage(Constants.pathToAchievementIcons, filename);
            if (bytes == null)
                return NotFound();

            return File(bytes, "image/jpeg", filename);
        }
    }
}
