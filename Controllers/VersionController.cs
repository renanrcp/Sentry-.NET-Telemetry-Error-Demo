using Microsoft.AspNetCore.Mvc;

namespace SentryTelemetryError.Controllers;

public class VersionController : BaseController
{
    [HttpGet]
    public IActionResult Get()
    {
        // Real version has different implementation.
        return Ok(new
        {
            Cache = 8,
        });
    }
}
