using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SentryTelemetryError.Enums;
using SentryTelemetryError.Options;

namespace SentryTelemetryError.VOs;

public class UserAuthVO
{
    public required int Id { get; init; }

    public required string Nickname { get; init; }

    public required string Username { get; init; }

    public required IEnumerable<Permission> Permissions { get; set; }

    public string? Token { get; set; }

    private static string ToPermissionName(Permission permission)
    {
        return Enum.GetName(permission)!;
    }

    private static IEnumerable<string> ToPermissionsNames(IEnumerable<Permission> permissions)
    {
        return permissions.Select(ToPermissionName);
    }

    public static Permission ToPermissionEnum(string permissionName)
    {
        return Enum.Parse<Permission>(permissionName, true);
    }

    public IEnumerable<Claim> ToClaims()
    {
        var permissionsStr = JsonSerializer.Serialize(ToPermissionsNames(Permissions));

        return
        [
            new Claim("id", Id.ToString(), ClaimValueTypes.Integer),
            new Claim("nickname", Nickname, ClaimValueTypes.String),
            new Claim("username", Username, ClaimValueTypes.String),
            new Claim("permissions", permissionsStr, JsonClaimValueTypes.JsonArray),
        ];
    }

    public static ValueTask<UserAuthVO> FromHttpContextAsync(HttpContext httpContext, bool parseToken = false)
    {
        if (httpContext.User.Identity?.IsAuthenticated != true)
        {
            throw new InvalidOperationException("Cannot parse a UserAuthVO from a non authenticated HttpContext.");
        }

        var claims = httpContext.User.Claims;
        var jwtOptions = httpContext.RequestServices.GetRequiredService<IOptions<JwtSignatureOptions>>().Value;

        var userId = claims
                            .Where(cl => cl.Type == "id")
                            .Select(cl => int.Parse(cl.Value))
                            .First();

        var nickname = claims
                            .Where(cl => cl.Type == "nickname")
                            .Select(cl => cl.Value)
                            .First();

        var username = claims
                            .Where(cl => cl.Type == "username")
                            .Select(cl => cl.Value)
                            .First();

        // Original call use a redis service here
        var permissions = claims
                            .Where(cl => cl.Type == "permissions")
                            .Select(cl => JsonSerializer.Deserialize<string[]>(cl.Value)!.Select(p => ToPermissionEnum(p)))
                            .First();

        var token = parseToken
            ? httpContext.Request.Cookies[jwtOptions.CookieName]
            : null;

        return ValueTask.FromResult(new UserAuthVO()
        {
            Id = userId,
            Nickname = nickname,
            Username = username,
            Permissions = permissions,
            Token = token,
        });
    }
}
