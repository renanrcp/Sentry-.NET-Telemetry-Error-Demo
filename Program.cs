using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using SentryTelemetryError;
using SentryTelemetryError.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.AddTracking();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddOutputCache();

builder.Services.AddJwtBearer(builder.Configuration);
builder.Services.AddScoped<IAuthorizationHandler, RequirePermissionsAuthorizationHandler>();

var app = builder.Build();

app.UseOutputCache();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();