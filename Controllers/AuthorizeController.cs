using Microsoft.AspNetCore.Authorization;

namespace SentryTelemetryError.Controllers;

[Authorize]
public abstract class AuthorizeController : BaseController
{

}
