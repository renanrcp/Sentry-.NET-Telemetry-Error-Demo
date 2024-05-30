using Sentry;

namespace SentryTelemetryError.Sentry;

public class ExampleSentryUserFactory(IHttpContextAccessor httpContextAccessor, ILogger<ExampleSentryUserFactory> logger) : ISentryUserFactory
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger<ExampleSentryUserFactory> _logger = logger;

    public SentryUser? Create()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                _logger.LogInformation("HttpContext is null.");
                return null;
            }

            if (httpContext.User.Identity?.IsAuthenticated != true)
            {
                _logger.LogInformation("User is not authenticated.");
                return null;
            }

            var claims = httpContext.User.Claims;

            var penguinId = claims
                                .Where(cl => cl.Type == "id")
                                .Select(cl => cl.Value)
                                .First();

            var nickname = claims
                                .Where(cl => cl.Type == "nickname")
                                .Select(cl => cl.Value)
                                .First();

            var username = claims
                                .Where(cl => cl.Type == "username")
                                .Select(cl => cl.Value)
                                .First();

            _logger.LogInformation("Sucessfully parsed user for sentry.");

            return new SentryUser
            {
                Id = penguinId,
                Username = username,
                Other = new Dictionary<string, string>
                {
                    { "Nickname", nickname },
                },
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurried while parsing the user for sentry integration.");
            return null;
        }
    }
}