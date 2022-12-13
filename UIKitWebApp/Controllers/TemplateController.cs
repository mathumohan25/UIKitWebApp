using Microsoft.AspNetCore.Mvc;
using System.Xml;

namespace UIKitWebApp.Controllers; 

[ApiController]
[Route("api/[controller]")]
public class TemplateController : ControllerBase {

    [HttpGet("PlatformTemplates")]
    public List<Template> GetTemplates(Platform platform) {

        TemplateParser templateParser = new TemplateParser();

        List<Template> templates = templateParser.GetAllTemplates();

        if (templates == null) return null;

        return templates.Where(template => template.Platform == platform).ToList();
    }
}
