using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace SentryTelemetryError.Options;

public class ConfigureJwtBearerSignatureOptions(IOptions<JwtSignatureOptions> jwtSignatureOptions) : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly IOptions<JwtSignatureOptions> _jwtSignatureOptions = jwtSignatureOptions;

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme)
        {
            return;
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = _jwtSignatureOptions.Value.Issuer,
            ValidAudience = _jwtSignatureOptions.Value.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSignatureOptions.Value.IssuerSigningKey)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
        };
    }

    public void Configure(JwtBearerOptions options)
    {
        Configure(string.Empty, options);
    }
}
