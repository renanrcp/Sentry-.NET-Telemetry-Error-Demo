using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SentryTelemetryError.Enums;

namespace SentryTelemetryError.Requests;

public class LoginRequest
{
    [Required]
    public required string Username { get; init; }

    [Required]
    public required string Password { get; init; }

    // These properties above doesn't exists in the original request.
    [Required]
    public required string Nickname { get; init; }

    [Required]
    public required int Id { get; init; }

    [Required]
    public required IReadOnlyCollection<Permission> Permissions { get; init; }
}
