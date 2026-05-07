namespace Common.Rest.Address.Infrastructure.Persistence;

using Microsoft.Azure.Cosmos;
using Common.Rest.Address.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;

/// <summary>
/// Factory for creating and initializing Azure Cosmos DB client and containers.
/// </summary>
public interface ICosmosDbInitializer
{
    /// <summary>
    /// Initializes the Cosmos DB client and ensures database/container exist.
    /// </summary>
    Task InitializeAsync(CancellationToken ct = default);

    /// <summary>
    /// Gets the initialized Cosmos container.
    /// </summary>
    Container GetContainer();

    /// <summary>
    /// Gets the initialized Cosmos client.
    /// </summary>
    CosmosClient GetClient();
}

/// <summary>
/// Implementation of Cosmos DB initializer.
/// </summary>
public class CosmosDbInitializer : ICosmosDbInitializer
{
    private readonly CosmosDbOptions _options;
    private readonly ILogger<CosmosDbInitializer> _logger;
    private CosmosClient? _client;
    private Container? _container;

    public CosmosDbInitializer(CosmosDbOptions options, ILogger<CosmosDbInitializer> logger)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Initializing Cosmos DB client for database '{DatabaseName}' and container '{ContainerName}'...", 
                _options.DatabaseName, _options.ContainerName);

            // Create Cosmos client
            _client = new CosmosClient(
                _options.ConnectionString,
                new CosmosClientOptions
                {
                    ConnectionMode = ConnectionMode.Gateway
                });

            // Get or create database
            var databaseResponse = await _client.CreateDatabaseIfNotExistsAsync(
                _options.DatabaseName,
                cancellationToken: ct);
            var database = databaseResponse.Database;

            _logger.LogInformation("Database '{DatabaseName}' ready.", _options.DatabaseName);

            // Get or create container
            var containerProperties = new ContainerProperties
            {
                Id = _options.ContainerName,
                PartitionKeyPath = _options.PartitionKeyPath
            };

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                cancellationToken: ct);
            _container = containerResponse.Container;

            _logger.LogInformation("Container '{ContainerName}' ready with partition key '{PartitionKeyPath}'.", 
                _options.ContainerName, _options.PartitionKeyPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Cosmos DB.");
            throw;
        }
    }

    public Container GetContainer()
    {
        if (_container is null)
            throw new InvalidOperationException("Cosmos DB not initialized. Call InitializeAsync first.");
        return _container;
    }

    public CosmosClient GetClient()
    {
        if (_client is null)
            throw new InvalidOperationException("Cosmos DB not initialized. Call InitializeAsync first.");
        return _client;
    }
}
