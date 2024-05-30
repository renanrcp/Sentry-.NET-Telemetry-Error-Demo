using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
using Sentry;
using Sentry.OpenTelemetry;
using Sentry.Profiling;
using SentryTelemetryError.Options;
using SentryTelemetryError.Sentry;

namespace SentryTelemetryError;

public static class ServiceCollectionExtensions
{

    public static WebApplicationBuilder AddTracking(this WebApplicationBuilder builder)
    {
        builder.WebHost.UseSentry(options =>
        {
            options.UseOpenTelemetry();

            options.ExperimentalMetrics = new ExperimentalMetricsOptions
            {
                EnableCodeLocations = true
            };

            options.AddIntegration(new ProfilingIntegration(
                // During startup, wait up to 500ms to profile the app startup code.
                // This could make launching the app a bit slower so comment it out if you
                // prefer profiling to start asynchronously.
                TimeSpan.FromMilliseconds(500)
            ));
        });

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
                tracerProviderBuilder.AddSentry() // <-- Configure OpenTelemetry to send trace information to Sentry
                    .AddAspNetCoreInstrumentation() // <-- Adds ASP.NET Core telemetry sources
                    .AddHttpClientInstrumentation() // <-- Adds HttpClient telemetry sources
            );

        builder.Services.AddSingleton<ISentryUserFactory, ExampleSentryUserFactory>();

        return builder;
    }

    public static IServiceCollection AddJwtBearer(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSignatureOptions>((options) =>
        {
            options.IssuerSigningKey = configuration["MANAGER_KEY"]!;
        });

        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerSignatureOptions>();
        services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, PostConfigureJwtBearerSignatureOptions>();

        services.AddAuthorization();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer();

        return services;
    }
}
