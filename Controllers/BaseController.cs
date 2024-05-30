using Microsoft.AspNetCore.Mvc;

namespace SentryTelemetryError.Controllers;

[ApiController]
[Route("[controller]")]
public abstract class BaseController : ControllerBase
{

}
