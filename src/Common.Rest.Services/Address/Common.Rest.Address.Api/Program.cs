var builder = WebApplication.CreateBuilder(args);

// ── Shared Standard Configuration ──────────────────────────────────────
builder.Logging.ClearProviders().AddConsole().AddDebug();
builder.Services.AddStandardOpenTelemetry(builder.Configuration);
builder.Services.AddStandardAuth(builder.Configuration, builder.Environment);
builder.Services.AddStandardApiVersioning();
builder.Services.AddStandardValidation();
builder.Services.AddStandardResilience(builder.Configuration);

// ── Application Services ────────────────────────────────────────────────
builder.Services.AddScoped<IAddressMappingService, AddressMappingService>();
builder.Services.AddScoped<IAddressService, AddressService>();

// ── Infrastructure ───────────────────────────────────────────────────
builder.Services.AddInfrastructure(builder.Configuration);


// ── Health Checks ───────────────────────────────────────────────────────
var healthChecks = builder.Services.AddHealthChecks();
healthChecks.AddCheck("cosmos", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Cosmos DB configured"), tags: ["ready"]);

// ── Controllers  ───────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// ── Middleware pipeline ─────────────────────────────────────────────────
app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/openapi"),
    appBuilder =>
    {
        appBuilder.UseMiddleware<CorrelationIdMiddleware>();
        appBuilder.UseMiddleware<ExceptionHandlingMiddleware>();
    });

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// ── Health check endpoints ──────────────────────────────────────────────
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

// ── OpenAPI endpoints ──────────────────────────────────────
app.MapOpenApi().AllowAnonymous();
app.MapControllers();

app.Logger.LogInformation("Address Service starting...");
app.Run();
