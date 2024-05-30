using Microsoft.AspNetCore.Authorization;
using SentryTelemetryError.Enums;
using SentryTelemetryError.Requirements;
using SentryTelemetryError.VOs;

namespace SentryTelemetryError.Handlers;

public class RequirePermissionsAuthorizationHandler(IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<RequirePermissionsAttribute>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RequirePermissionsAttribute requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
        {
            context.Fail(new AuthorizationFailureReason(this, $"Cannot get {nameof(HttpContext)} inside {nameof(RequirePermissionsAuthorizationHandler)}."));
            return;
        }

        if (httpContext.User.Identity?.IsAuthenticated != true)
        {
            context.Fail(new AuthorizationFailureReason(this, "User is not authenticated."));
            return;
        }

        var userAuthVO = await UserAuthVO.FromHttpContextAsync(httpContext);

        if (!HasPermissions(userAuthVO.Permissions, requirement.Permissions))
        {
            context.Fail(new AuthorizationFailureReason(this, $"User '{userAuthVO.Id}' doesn't has required permissions."));
            return;
        }

        context.Succeed(requirement);
    }

    private static bool HasPermissions(IEnumerable<Permission> userPermissions, params Permission[] permissions)
    {
        if (userPermissions.Contains(Permission.All))
        {
            return true;
        }

        return permissions.All(p => userPermissions.Contains(p));
    }
}
