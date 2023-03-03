using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.IO.Pipelines;

namespace med_game.src.Controllers
{
    [Route("")]
    [ApiController]
    public class ImageLoaderController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> UploadProfileIcon(IFormFile file)
        {
            return Ok(file);
        }
    }
}
