namespace Common.Rest.Address.Infrastructure.Configuration;

/// <summary>
/// Configuration options for Azure Cosmos DB connection and container settings.
/// </summary>
public class CosmosDbOptions
{
    public const string SectionName = "CosmosDb";

    /// <summary>
    /// Connection string or endpoint URI for Cosmos DB.
    /// For emulator: "AccountEndpoint=https://localhost:8081/;AccountKey=..."
    /// For Azure: "AccountEndpoint=https://{account}.documents.azure.com:443/;AccountKey=..."
    /// </summary>
    public required string ConnectionString { get; set; }

    /// <summary>
    /// Database name (e.g., "AddressDb").
    /// </summary>
    public required string DatabaseName { get; set; }

    /// <summary>
    /// Container name (e.g., "AddressContainer").
    /// </summary>
    public required string ContainerName { get; set; }

    /// <summary>
    /// Partition key path (e.g., "/postcode").
    /// </summary>
    public string PartitionKeyPath { get; set; } = "/postcode";

    /// <summary>
    /// Request units per second for provisioned throughput (null = serverless).
    /// </summary>
    public int? ThroughputRus { get; set; }

    /// <summary>
    /// Enable connection pooling (recommended for production).
    /// </summary>
    public bool EnableConnectionSharing { get; set; } = true;

    /// <summary>
    /// Maximum retry attempts for transient failures.
    /// </summary>
    public int MaxRetryAttemptsOnThrottledRequests { get; set; } = 9;

    /// <summary>
    /// Maximum wait time in seconds for retry on throttled requests.
    /// </summary>
    public int MaxRetryWaitTimeInSeconds { get; set; } = 30;
}
