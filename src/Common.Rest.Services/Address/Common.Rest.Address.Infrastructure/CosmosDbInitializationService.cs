namespace Common.Rest.Address.Infrastructure;

using Common.Rest.Address.Infrastructure.Persistence;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/// <summary>
/// Hosted service that initializes Cosmos DB on application startup.
/// Ensures database and container are created before the app handles requests.
/// </summary>
public class CosmosDbInitializationService : IHostedService
{
    private readonly ICosmosDbInitializer _initializer;
    private readonly ILogger<CosmosDbInitializationService> _logger;

    public CosmosDbInitializationService(ICosmosDbInitializer initializer, ILogger<CosmosDbInitializationService> logger)
    {
        _initializer = initializer ?? throw new ArgumentNullException(nameof(initializer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting Cosmos DB initialization...");
            await _initializer.InitializeAsync(cancellationToken);
            _logger.LogInformation("Cosmos DB initialized successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Cosmos DB. Application startup will continue, but requests may fail.");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cosmos DB initialization service stopping.");
        return Task.CompletedTask;
    }
}
