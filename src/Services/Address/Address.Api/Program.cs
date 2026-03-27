using Address.Application.Mapping;
using Address.Application.Services;
using Address.Infrastructure;
using RestApi.Shared.Extensions;
using RestApi.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ── Shared Standard Configuration ──────────────────────────────────────
builder.Logging.ClearProviders().AddConsole().AddDebug();
builder.Services.AddStandardOpenTelemetry(builder.Configuration);
builder.Services.AddStandardAuth(builder.Configuration, builder.Environment);
builder.Services.AddStandardApiVersioning();
builder.Services.AddStandardValidation();
builder.Services.AddStandardResilience(builder.Configuration);

// ── Application Services ────────────────────────────────────────────────
builder.Services.AddAutoMapper(typeof(AddressMappingProfile).Assembly);
builder.Services.AddScoped<IAddressService, AddressService>();

// ── Infrastructure ───────────────────────────────────────────────────
builder.Services.AddInfrastructure(builder.Configuration);


// ── Health Checks ───────────────────────────────────────────────────────
var healthChecks = builder.Services.AddHealthChecks();
var dbConn = builder.Configuration.GetConnectionString("AddressDb");
if (!string.IsNullOrEmpty(dbConn) && !dbConn.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
{
    healthChecks.AddCheck("sql", new RestApi.Shared.Health.SqlHealthCheck(dbConn), tags: ["ready"]);
}
else
{
    healthChecks.AddCheck("mock-db", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Using InMemory DB"), tags: ["ready"]);
}

// ── Controllers & Swagger ───────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Address API",
        Version = "v1",
        Description = "Microservice for managing standardized address records."
    });
});

var app = builder.Build();

// ── Middleware pipeline ─────────────────────────────────────────────────
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Address API v1"));
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// ── Health check endpoints ──────────────────────────────────────────────
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

app.MapControllers();

app.Logger.LogInformation("Address Service starting...");
app.Run();
