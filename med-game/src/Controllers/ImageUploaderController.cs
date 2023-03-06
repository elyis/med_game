using med_game.src.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace med_game.src.Controllers
{
    [Route("")]
    [ApiController]
    public class ImageUploaderController : ControllerBase
    {
        private readonly FileUploader _fileUploader;

        private readonly List<string> _validFileFormats = new()
        {
            "image/jpeg",
            "image/png",
        };

        public ImageUploaderController()
        {
            _fileUploader = new FileUploader();
        }



        [HttpPost("answerIcon")]
        [Authorize]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.UnsupportedMediaType)]

        public async Task<IActionResult> UploadAnswerIcon()
        {
            string? contentType = Request.Headers.ContentType;
            if (contentType != null && _validFileFormats.Contains(contentType!))
            {
                string? filename = await _fileUploader.UploadImage(Constants.pathToAnswerIcons, Request.Body, contentType!.Split('/').Last());
                return filename == null ? BadRequest() : Ok(filename);
            }

            return new UnsupportedMediaTypeResult();
        }



        [HttpPost("questionIcon")]
        [Authorize]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.UnsupportedMediaType)]

        public async Task<IActionResult> UploadQuestionIcon()
        {
            string? contentType = Request.Headers.ContentType;
            if (contentType != null && _validFileFormats.Contains(contentType!))
            {
                string? filename = await _fileUploader.UploadImage(Constants.pathToQuestionIcons, Request.Body, contentType!.Split('/').Last());
                return filename == null ? BadRequest() : Ok(filename);
            }

            return new UnsupportedMediaTypeResult();
        }



        [HttpPost("achievementIcon")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.UnsupportedMediaType)]

        public async Task<IActionResult> UploadAchievementIcon()
        {
            string? contentType = Request.Headers.ContentType;
            if (contentType != null && _validFileFormats.Contains(contentType!))
            {
                string? filename = await _fileUploader.UploadImage(Constants.pathToAchievementIcons, Request.Body, contentType!.Split('/').Last());
                return filename == null ? BadRequest() : Ok(filename);
            }

            return new UnsupportedMediaTypeResult();
        }



        [HttpGet("answerIcon/{filename}")]
        [Authorize]
        [ProducesResponseType(typeof(File), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]

        public async Task<IActionResult> GetAnswerIcon(string filename)
        {
            var bytes = await _fileUploader.GetStreamImage(Constants.pathToAnswerIcons, filename);
            if (bytes == null)
                return NotFound();

            string extension = filename.Split('.').Last();
            return File(bytes, $"image/{extension}", filename);
        }



        [HttpGet("questionIcon/{filename}")]
        [Authorize]
        [ProducesResponseType(typeof(File), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]

        public async Task<IActionResult> GetQuestionIcon(string filename)
        {
            var bytes = await _fileUploader.GetStreamImage(Constants.pathToQuestionIcons, filename);
            if (bytes == null)
                return NotFound();

            string extension = filename.Split('.').Last();
            return File(bytes, $"image/{extension}", filename);
        }



        [HttpGet("achievementIcon/{filename}")]
        [Authorize]
        [ProducesResponseType(typeof(File), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]

        public async Task<IActionResult> GetAchievementIcon(string filename)
        {
            var bytes = await _fileUploader.GetStreamImage(Constants.pathToAchievementIcons, filename);
            if (bytes == null)
                return NotFound();

            string extension = filename.Split('.').Last();
            return File(bytes, $"image/{extension}", filename);
        }
    }
}
