using SentryTelemetryError.Enums;

namespace SentryTelemetryError.Responses;

public class LoginResponse
{
    public required int Id { get; init; }

    public required string Nickname { get; init; }

    public required string Username { get; init; }

    public required IEnumerable<Permission> Permissions { get; init; }
}
