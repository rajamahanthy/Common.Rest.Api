using Asp.Versioning;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using OpenTelemetry.Trace;
using RestApi.Shared.Middleware;
using RestApi.Shared.Resilience;
using System.Security.Claims;
using OpenTelemetry.Instrumentation.EntityFrameworkCore;

namespace RestApi.Shared.Extensions;

/// <summary>
/// Centralized ServiceCollection extensions to provide consistency across all microservices.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures the standard resilience policies.
    /// </summary>
    public static IServiceCollection AddStandardResilience(this IServiceCollection services, IConfiguration configuration)
    {
        return ResilienceExtensions.AddStandardInternalResilience(services, configuration);
    }
    /// <summary>
    /// Configures standardized API versioning.
    /// </summary>
    public static IServiceCollection AddStandardApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Api-Version"));
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    /// <summary>
    /// Configures standardized OpenTelemetry with Azure Monitor and EF Core instrumentation.
    /// </summary>
    public static IServiceCollection AddStandardOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["ApplicationInsights:ConnectionString"];
        if (!string.IsNullOrEmpty(connectionString))
        {
            services.AddOpenTelemetry()
                .UseAzureMonitor(options => options.ConnectionString = connectionString)
                .WithTracing(tracing => tracing.AddEntityFrameworkCoreInstrumentation());
        }

        return services;
    }

    /// <summary>
    /// Configures Microsoft Identity Auth with a mock fallback for Local Development.
    /// </summary>
    public static IServiceCollection AddStandardAuth(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
        }
        else
        {
            services.AddMicrosoftIdentityWebApiAuthentication(configuration, "AzureAd");
        }

        return services;
    }

    /// <summary>
    /// Mock authentication handler for local development.
    /// </summary>
    private class TestAuthHandler(
        Microsoft.Extensions.Options.IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        System.Text.Encodings.Web.UrlEncoder encoder)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { 
                new Claim(ClaimTypes.Name, "Local User"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}

// Ensure you have the Microsoft.Identity.Web NuGet package installed in your project.
// You can install it via NuGet Package Manager or with the following command:
// dotnet add package Microsoft.Identity.Web
