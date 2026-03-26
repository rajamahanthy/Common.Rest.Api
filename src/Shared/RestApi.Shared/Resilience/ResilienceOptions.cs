using Polly;

namespace RestApi.Shared.Resilience;

/// <summary>
/// Configuration model for shared resilience strategies.
/// Can be bound from appsettings.json section "Resilience".
/// </summary>
public class ResilienceOptions
{
    public const string SectionName = "Resilience";

    public int MaxRetryAttempts { get; set; } = 3;
    public double DelaySeconds { get; set; } = 1.0;
    public string BackoffType { get; set; } = "Exponential"; // Linear, Constant, Exponential
    public bool UseJitter { get; set; } = true;
}
