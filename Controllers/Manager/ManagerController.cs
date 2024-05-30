using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SentryTelemetryError.Enums;
using SentryTelemetryError.Options;
using SentryTelemetryError.Requests;
using SentryTelemetryError.Responses;
using SentryTelemetryError.VOs;

namespace SentryTelemetryError.Controllers.Manager;

public class ManagerController : AuthorizeController
{
    [HttpGet("info")]
    public async ValueTask<IActionResult> InfoAsync()
    {
        var userAuth = await UserAuthVO.FromHttpContextAsync(HttpContext, false);

        return Ok(userAuth);
    }

    [AllowAnonymous]
    [HttpGet("permission")]
    public async ValueTask<IActionResult> GetPermissionsAsync()
    {
        if (HttpContext.User.Identity?.IsAuthenticated != true)
        {
            return Ok(Array.Empty<Permission>());
        }

        var userAuth = await UserAuthVO.FromHttpContextAsync(HttpContext, false);

        return Ok(userAuth.Permissions);
    }

    // Original project this call is async
    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult LoginAsync(
        [FromServices] IOptions<JwtSignatureOptions> jwtSignatureOptions,
        [FromBody] LoginRequest loginRequest)
    {
        // Original project uses an authentication service.
        var userAuth = new UserAuthVO
        {
            Id = loginRequest.Id,
            Nickname = loginRequest.Nickname,
            Username = loginRequest.Username,
            Permissions = loginRequest.Permissions,
        };

        // Original project uses a token generator.
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(userAuth.ToClaims()),
            Expires = DateTime.UtcNow.AddHours(jwtSignatureOptions.Value.ExpiresInHours),
            Issuer = jwtSignatureOptions.Value.Issuer,
            Audience = jwtSignatureOptions.Value.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSignatureOptions.Value.IssuerSigningKey)
                ),
                SecurityAlgorithms.HmacSha256
            )
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var stringToken = tokenHandler.WriteToken(token);

        userAuth.Token = stringToken;

        Response.Cookies.Append(jwtSignatureOptions.Value.CookieName, userAuth.Token!, new CookieOptions()
        {
            Expires = DateTimeOffset.Now.AddHours(jwtSignatureOptions.Value.ExpiresInHours),
            IsEssential = true,
            Secure = true,
        });

        return Ok(new LoginResponse
        {
            Id = userAuth.Id,
            Nickname = userAuth.Nickname,
            Username = userAuth.Username,
            Permissions = userAuth.Permissions,
        });
    }
}
