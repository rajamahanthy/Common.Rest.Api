using RestApi.Shared.Health;
using RestApi.Shared.Middleware;
using RestApi.Shared.Extensions;
using SurveyData.Application.Interfaces;
using SurveyData.Application.Mapping;
using SurveyData.Application.Services;
using SurveyData.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ── Shared Standard Configuration ──────────────────────────────────────
builder.Logging.ClearProviders().AddConsole().AddDebug();
builder.Services.AddStandardOpenTelemetry(builder.Configuration);
builder.Services.AddStandardAuth(builder.Configuration, builder.Environment);
builder.Services.AddStandardApiVersioning();
builder.Services.AddStandardValidation();
builder.Services.AddStandardResilience(builder.Configuration);

// ── Application Services ────────────────────────────────────────────────
builder.Services.AddAutoMapper(typeof(SurveyMappingProfile).Assembly);
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

// ── Controllers & Swagger ───────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Survey Data API",
        Version = "v1",
        Description = "Microservice for managing survey data records."
    });
});

var app = builder.Build();

// ── Middleware pipeline ─────────────────────────────────────────────────
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Survey Data API v1"));
}

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

app.MapControllers();

app.Logger.LogInformation("Survey Data Service starting...");
app.Run();


