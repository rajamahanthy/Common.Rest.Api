var builder = WebApplication.CreateBuilder(args);

// ── Shared Standard Configuration ──────────────────────────────────────
builder.Logging.ClearProviders().AddConsole().AddDebug();
builder.Services.AddStandardOpenTelemetry(builder.Configuration);
builder.Services.AddStandardAuth(builder.Configuration, builder.Environment);
builder.Services.AddStandardApiVersioning();
builder.Services.AddStandardValidation();
builder.Services.AddStandardResilience(builder.Configuration);

// ── Application Services ────────────────────────────────────────────────
builder.Services.AddScoped<ISurveyMappingService, SurveyMappingService>();
builder.Services.AddScoped<ISurveyService, SurveyService>();

// ── Infrastructure (EF Core, Repositories, UoW) ────────────────────────
builder.Services.AddInfrastructure(builder.Configuration);


// ── Health Checks ───────────────────────────────────────────────────────
var healthChecks = builder.Services.AddHealthChecks();
var dbConn = builder.Configuration.GetConnectionString("SurveyDb");
if (!string.IsNullOrEmpty(dbConn) && !dbConn.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
{
    healthChecks.AddCheck("sql", new SqlHealthCheck(dbConn), tags: ["ready"]);
}
else
{
    // Local dev fallback health check
    healthChecks.AddCheck("mock-db", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Using InMemory DB"), tags: ["ready"]);
}

// ── Controllers ───────────────────────────────────────────────
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
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false // liveness: always return healthy
});

app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapOpenApi().AllowAnonymous();
app.MapControllers();

app.Logger.LogInformation("Survey Data Service starting...");
app.Run();


