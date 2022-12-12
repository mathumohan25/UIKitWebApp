using Microsoft.AspNetCore.Mvc;

namespace UIKitWebApp.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class PackageDownloaderController : ControllerBase {
    //    private static readonly string[] Summaries = new[]
    //    {
    //    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    //};

        private readonly ILogger<PackageDownloaderController> _logger;

        public PackageDownloaderController(ILogger<PackageDownloaderController> logger) {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Download(string json) {

            var appcreator = new AppCreator();

            appcreator.Start(json);

            Stream stream = appcreator.Compress(appcreator.AppName);

            if (stream == null)
                return NotFound(); // returns a NotFoundResult with Status404NotFound response.

            return File(stream, "application/zip", appcreator.AppName); // returns a FileStreamResult
        }
    }
}