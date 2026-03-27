using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Polly.Retry;

namespace RestApi.Shared.Resilience;

/// <summary>
/// Centralized resilience policies and configuration for all microservices.
/// </summary>
public static class ResilienceExtensions
{
    /// <summary>
    /// Adds a standard resilience handler for HttpClients. 
    /// Includes standard Retry, Circuit Breaker, Timeout, and Hedging.
    /// </summary>
    public static IHttpClientBuilder AddStandardHttpResilience(this IHttpClientBuilder builder)
    {
        builder.AddStandardResilienceHandler();
        return builder;
    }

    /// <summary>
    /// Configures a custom, shared internal resilience pipeline (e.g., for DB or cache access).
    /// </summary>
    public static IServiceCollection AddStandardInternalResilience(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new ResilienceOptions();
        configuration.GetSection(ResilienceOptions.SectionName).Bind(options);

        services.AddResiliencePipeline("default-db-retry", builder =>
        {
            builder.AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = options.MaxRetryAttempts,
                BackoffType = Enum.TryParse<DelayBackoffType>(options.BackoffType, true, out var bt) ? bt : DelayBackoffType.Exponential,
                UseJitter = options.UseJitter,
                Delay = TimeSpan.FromSeconds(options.DelaySeconds)
            });
        });

        return services;
    }
}
