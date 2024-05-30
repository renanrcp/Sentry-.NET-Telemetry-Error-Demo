using SentryTelemetryError.Enums;
using SentryTelemetryError.Requirements;

namespace SentryTelemetryError.Controllers;

[RequirePermissions(Permission.CanAccessManager)]
public abstract class BaseManagerController : AuthorizeController
{

}
