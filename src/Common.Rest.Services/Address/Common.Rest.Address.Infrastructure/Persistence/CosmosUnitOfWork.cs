namespace Common.Rest.Address.Infrastructure.Persistence;

using Azure.Cosmos;
using Common.Rest.Address.Domain.Entities;
using Common.Rest.Shared.Repository;
using Microsoft.Extensions.Logging;

/// <summary>
/// Cosmos DB Unit of Work implementation for transaction management.
/// Tracks added, modified, and deleted entities and applies changes to Cosmos.
/// </summary>
public class CosmosUnitOfWork : IUnitOfWork
{
    private readonly Container _container;
    private readonly ILogger<CosmosUnitOfWork> _logger;

    // Track entities for batch operations
    private readonly List<AddressDocumentEntity> _addedEntities = [];
    private readonly List<AddressDocumentEntity> _modifiedEntities = [];
    private readonly List<AddressDocumentEntity> _deletedEntities = [];

    public CosmosUnitOfWork(Container container, ILogger<CosmosUnitOfWork> logger)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Register an entity as added.
    /// </summary>
    public void MarkAdded(AddressDocumentEntity entity)
    {
        if (!_addedEntities.Contains(entity))
            _addedEntities.Add(entity);
    }

    /// <summary>
    /// Register an entity as modified.
    /// </summary>
    public void MarkModified(AddressDocumentEntity entity)
    {
        if (!_modifiedEntities.Contains(entity))
            _modifiedEntities.Add(entity);
    }

    /// <summary>
    /// Register an entity as deleted.
    /// </summary>
    public void MarkDeleted(AddressDocumentEntity entity)
    {
        if (!_deletedEntities.Contains(entity))
            _deletedEntities.Add(entity);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var changeCount = 0;

        try
        {
            // Process deletions
            foreach (var entity in _deletedEntities)
            {
                if (string.IsNullOrEmpty(entity.PartitionKey))
                {
                    _logger.LogWarning("Cannot delete entity without PartitionKey. Id: {Id}", entity.Id);
                    continue;
                }

                _logger.LogDebug("Deleting document. Id: {Id}, PartitionKey: {PartitionKey}", 
                    entity.Id, entity.PartitionKey);

                try
                {
                    await _container.DeleteItemAsync<AddressDocumentEntity>(
                        entity.Id.ToString(),
                        new PartitionKey(entity.PartitionKey),
                        cancellationToken: ct);
                    changeCount++;
                    _logger.LogInformation("Document deleted. Id: {Id}", entity.Id);
                }
                catch (CosmosException ex) when (ex.Status == 404)
                {
                    _logger.LogWarning(ex, "Document to delete not found. Id: {Id}", entity.Id);
                }
            }

            // Process modifications
            foreach (var entity in _modifiedEntities)
            {
                if (string.IsNullOrEmpty(entity.PartitionKey))
                {
                    _logger.LogWarning("Cannot update entity without PartitionKey. Id: {Id}", entity.Id);
                    continue;
                }

                _logger.LogDebug("Updating document. Id: {Id}, PartitionKey: {PartitionKey}", 
                    entity.Id, entity.PartitionKey);

                try
                {
                    var cosmosItem = ToCosmosItem(entity);
                    await _container.ReplaceItemAsync(
                        cosmosItem,
                        entity.Id.ToString(),
                        new PartitionKey(entity.PartitionKey),
                        cancellationToken: ct);
                    changeCount++;
                    _logger.LogInformation("Document updated. Id: {Id}", entity.Id);
                }
                catch (CosmosException ex)
                {
                    _logger.LogError(ex, "Error updating document. Id: {Id}", entity.Id);
                    throw;
                }
            }

            // Note: Added entities are handled by CosmosRepository.AddAsync directly,
            // not via this SaveChangesAsync. However, we track them for audit purposes.

            _logger.LogInformation("SaveChangesAsync completed. Changes: {ChangeCount}", changeCount);
            return changeCount;
        }
        finally
        {
            // Clear tracking collections after save
            _addedEntities.Clear();
            _modifiedEntities.Clear();
            _deletedEntities.Clear();
        }
    }

    public Task BeginTransactionAsync(CancellationToken ct = default)
    {
        // Cosmos doesn't support multi-document ACID transactions across partitions.
        // For single-partition transactions, use session consistency.
        _logger.LogInformation("Transaction support is limited in Cosmos. Using session consistency.");
        return Task.CompletedTask;
    }

    public Task CommitTransactionAsync(CancellationToken ct = default)
    {
        // Cosmos commit is implicit with SaveChangesAsync.
        _logger.LogInformation("Implicit commit via SaveChangesAsync.");
        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        // Clear any pending changes
        _addedEntities.Clear();
        _modifiedEntities.Clear();
        _deletedEntities.Clear();
        _logger.LogInformation("Transaction rolled back. Pending changes cleared.");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _addedEntities.Clear();
        _modifiedEntities.Clear();
        _deletedEntities.Clear();
    }

    /// <summary>
    /// Converts an AddressDocumentEntity to a Cosmos-compatible dynamic object.
    /// </summary>
    private static dynamic ToCosmosItem(AddressDocumentEntity entity)
    {
        return new
        {
            id = entity.Id.ToString(),
            postcode = entity.PartitionKey,
            documentType = entity.DocumentType,
            jsonData = entity.JsonData,
            uprn = entity.UprnIndex,
            postcodeIndex = entity.PostcodeIndex,
            postTownIndex = entity.PostTownIndex,
            organisationIndex = entity.OrganisationIndex,
            thoroughfareIndex = entity.ThoroughfareIndex,
            localityIndex = entity.LocalityIndex,
            dependentLocalityIndex = entity.DependentLocalityIndex,
            createdAt = entity.CreatedAt,
            updatedAt = entity.UpdatedAt,
            isDeleted = entity.IsDeleted,
            createdBy = entity.CreatedBy,
            updatedBy = entity.UpdatedBy,
            rowVersion = entity.RowVersion
        };
    }
}
