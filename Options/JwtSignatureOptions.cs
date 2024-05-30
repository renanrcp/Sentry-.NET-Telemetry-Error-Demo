using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace SentryTelemetryError.Options;

public class JwtSignatureOptions : IOptions<JwtSignatureOptions>
{
    public string Issuer { get; } = "example.net";

    public string Audience { get; } = "example.net";

    public string CookieName { get; } = "example-manager-auth";

    public int ExpiresInHours { get; } = 6;

    public string IssuerSigningKey { get; set; } = "example";

    JwtSignatureOptions IOptions<JwtSignatureOptions>.Value => this;
}
