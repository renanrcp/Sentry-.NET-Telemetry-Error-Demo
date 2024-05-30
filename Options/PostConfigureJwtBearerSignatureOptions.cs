using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace SentryTelemetryError.Options;

public class PostConfigureJwtBearerSignatureOptions(IOptions<JwtSignatureOptions> jwtSignatureOptions) : IPostConfigureOptions<JwtBearerOptions>
{
    private readonly IOptions<JwtSignatureOptions> _jwtSignatureOptions = jwtSignatureOptions;

    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme)
        {
            return;
        }

        options.Events ??= new();

        options.Events.OnMessageReceived = (ctx) =>
        {
            var token = ctx.Request.Cookies[_jwtSignatureOptions.Value.CookieName];

            ctx.Token = token;

            return Task.CompletedTask;
        };
    }
}