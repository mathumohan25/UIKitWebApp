using Microsoft.AspNetCore.Mvc;

namespace UIKitWebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PackageController : ControllerBase {

    private readonly ILogger<PackageController> _logger;

    public PackageController(ILogger<PackageController> logger) {
        _logger = logger;
    }

    [HttpPost("Bundle")]
    public async Task<IActionResult> GetBundle([FromBody] SelectionModel model) {
        try {

            var package = new PackageFactory().GetPackage(model.Platform);

            if (string.IsNullOrWhiteSpace(model.AppName))
                model.AppName = "Package";

            Stream stream = null;

            if (model.PackageType == PackageType.Bundle)
                stream = await package.GetBundleAsync(model);
            else if (model.PackageType == PackageType.Standalone)
                stream = await package.GetStandalonePackageAsync(model);
            if (stream == null)
                return null;

            return File(stream, "application/zip", model.AppName + ".zip");
        } catch (Exception) {
            return null;

        }
    }
}