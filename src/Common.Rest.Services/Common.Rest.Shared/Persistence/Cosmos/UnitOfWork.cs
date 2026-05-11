namespace Common.Rest.Shared.Persistence.Cosmos;

using Common.Rest.Shared.Repository;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Net;

/// <summary>
/// Generic Cosmos DB Unit of Work implementation for transaction management.
/// Tracks added, modified, and deleted entities and applies changes to Cosmos.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly Container _container;
    private readonly ILogger<UnitOfWork> _logger;

    // Track entities for batch operations
    private readonly List<(string Id, string PartitionKey)> _deletedEntities = [];
    private readonly List<(string Id, string PartitionKey, dynamic Item)> _modifiedEntities = [];

    public UnitOfWork(Container container, ILogger<UnitOfWork> logger)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Register an entity as modified with its Cosmos representation.
    /// </summary>
    public void MarkModified(string id, string partitionKey, dynamic cosmosItem)
    {
        if (!_modifiedEntities.Any(e => e.Id == id))
            _modifiedEntities.Add((id, partitionKey, cosmosItem));
    }

    /// <summary>
    /// Register an entity as deleted.
    /// </summary>
    public void MarkDeleted(string id, string partitionKey)
    {
        if (!_deletedEntities.Any(e => e.Id == id))
            _deletedEntities.Add((id, partitionKey));
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var changeCount = 0;

        try
        {
            // Process deletions
            foreach (var (id, partitionKey) in _deletedEntities)
            {
                if (string.IsNullOrEmpty(partitionKey))
                {
                    _logger.LogWarning("Cannot delete item without PartitionKey. Id: {Id}", id);
                    continue;
                }

                _logger.LogDebug("Deleting document. Id: {Id}, PartitionKey: {PartitionKey}", id, partitionKey);

                try
                {
                    await _container.DeleteItemAsync<dynamic>(
                        id,
                        new PartitionKey(partitionKey),
                        cancellationToken: ct);
                    changeCount++;
                    _logger.LogInformation("Document deleted. Id: {Id}", id);
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning(ex, "Document to delete not found. Id: {Id}", id);
                }
            }

            // Process modifications
            foreach (var (id, partitionKey, cosmosItem) in _modifiedEntities)
            {
                if (string.IsNullOrEmpty(partitionKey))
                {
                    _logger.LogWarning("Cannot update item without PartitionKey. Id: {Id}", id);
                    continue;
                }

                _logger.LogDebug("Updating document. Id: {Id}, PartitionKey: {PartitionKey}", id, partitionKey);

                try
                {
                    await _container.ReplaceItemAsync(
                        cosmosItem,
                        id,
                        new PartitionKey(partitionKey),
                        cancellationToken: ct);
                    changeCount++;
                    _logger.LogInformation("Document updated. Id: {Id}", id);
                }
                catch (CosmosException ex)
                {
                    _logger.LogError(ex, "Error updating document. Id: {Id}", id);
                    throw;
                }
            }

            _logger.LogInformation("SaveChangesAsync completed. Changes: {ChangeCount}", changeCount);
            return changeCount;
        }
        finally
        {
            // Clear tracking collections after save
            _modifiedEntities.Clear();
            _deletedEntities.Clear();
        }
    }

    public Task BeginTransactionAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Transaction support is limited in Cosmos. Using session consistency.");
        return Task.CompletedTask;
    }

    public Task CommitTransactionAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Implicit commit via SaveChangesAsync.");
        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        _modifiedEntities.Clear();
        _deletedEntities.Clear();
        _logger.LogInformation("Transaction rolled back. Pending changes cleared.");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _modifiedEntities.Clear();
        _deletedEntities.Clear();
    }
}