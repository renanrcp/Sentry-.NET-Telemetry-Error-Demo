using Microsoft.AspNetCore.Authorization;
using SentryTelemetryError.Enums;

namespace SentryTelemetryError.Requirements;

public class RequirePermissionsAttribute(params Permission[] permissions) : AuthorizeAttribute, IAuthorizationRequirement, IAuthorizationRequirementData
{
    public Permission[] Permissions { get; } = permissions;

    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return this;
    }
}
