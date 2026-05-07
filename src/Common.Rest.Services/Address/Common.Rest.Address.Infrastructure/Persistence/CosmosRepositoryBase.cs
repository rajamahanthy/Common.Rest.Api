namespace Common.Rest.Address.Infrastructure.Persistence;

using Common.Rest.Shared.Domain;
using Common.Rest.Shared.Repository;
using Common.Rest.Shared.Specification;
using Common.Rest.Shared.Persistence.Cosmos;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Text.Json;

/// <summary>
/// Generic Cosmos DB repository base implementation supporting any DocumentEntity.
/// Provides CRUD, query, and pagination operations for Cosmos DB containers.
/// </summary>
public abstract class CosmosRepositoryBase<T> : ICosmosRepository<T> where T : class
{
    protected readonly Container Container;
    protected readonly ILogger<CosmosRepositoryBase<T>> Logger;

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    protected CosmosRepositoryBase(Container container, ILogger<CosmosRepositoryBase<T>> logger)
    {
        Container = container ?? throw new ArgumentNullException(nameof(container));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the partition key value for an entity. Must be implemented by derived classes.
    /// </summary>
    public abstract string GetPartitionKey(T entity);

    /// <summary>
    /// Converts entity to Cosmos-compatible dynamic object. Must be implemented by derived classes.
    /// </summary>
    public abstract dynamic ToCosmosItem(T entity);

    /// <summary>
    /// Converts Cosmos item back to entity. Must be implemented by derived classes.
    /// </summary>
    public abstract T FromCosmosItem(dynamic cosmosItem);

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            Logger.LogDebug("Fetching document with Id: {Id}", id);

            var query = Container.GetItemQueryIterator<T>(
                new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
                    .WithParameter("@id", id.ToString()));

            while (query.HasMoreResults)
            {
                var page = await query.ReadNextAsync();
                foreach (var item in page)
                {
                    if (item != null)
                    {
                        Logger.LogDebug("Found document with Id: {Id}", id);
                        return item;
                    }
                }
            }

            Logger.LogDebug("Document with Id: {Id} not found.", id);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching document with Id: {Id}", id);
            throw;
        }
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            Logger.LogDebug("Fetching all documents");

            var query = Container.GetItemQueryIterator<T>(
                new QueryDefinition("SELECT * FROM c"));

            var items = new List<T>();
            while (query.HasMoreResults)
            {
                var page = await query.ReadNextAsync();
                items.AddRange(page);
            }

            Logger.LogDebug("Fetched {Count} documents", items.Count);
            return items.AsReadOnly();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching all documents");
            throw;
        }
    }

    public async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default)
    {
        try
        {
            Logger.LogDebug("Finding documents with predicate");

            var allItems = await GetAllAsync(ct);
            var compiled = predicate.Compile();
            var filtered = allItems.Where(compiled).ToList();

            Logger.LogDebug("Found {Count} documents matching predicate", filtered.Count);
            return filtered.AsReadOnly();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error finding documents");
            throw;
        }
    }

    public async Task<IReadOnlyList<T>> FindAsync(
        ISpecification<T> specification,
        CancellationToken ct = default)
    {
        try
        {
            Logger.LogDebug("Finding documents with specification");

            var allItems = await GetAllAsync(ct);
            var expression = specification.ToExpression();
            var compiled = expression.Compile();
            var filtered = allItems.Where(compiled).ToList();

            Logger.LogDebug("Found {Count} documents matching specification", filtered.Count);
            return filtered.AsReadOnly();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error finding documents with specification");
            throw;
        }
    }

    public async Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        ISpecification<T>? specification = null,
        Expression<Func<T, object>>? orderBy = null,
        bool descending = false,
        CancellationToken ct = default)
    {
        try
        {
            Logger.LogDebug("Fetching paged documents: page={Page}, pageSize={PageSize}", page, pageSize);

            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            IEnumerable<T> query = await GetAllAsync(ct);

            if (specification != null)
            {
                var expression = specification.ToExpression();
                var compiled = expression.Compile();
                query = query.Where(compiled);
            }

            if (predicate != null)
            {
                var compiled = predicate.Compile();
                query = query.Where(compiled);
            }

            if (orderBy != null)
            {
                var compiled = orderBy.Compile();
                query = descending
                    ? query.OrderByDescending(compiled)
                    : query.OrderBy(compiled);
            }

            var totalCount = query.Count();

            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            Logger.LogDebug("Fetched {Count} items for page {Page}, total count: {TotalCount}", 
                items.Count, page, totalCount);

            return (items.AsReadOnly(), totalCount);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching paged documents");
            throw;
        }
    }

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity);

            var partitionKey = GetPartitionKey(entity);
            if (string.IsNullOrEmpty(partitionKey))
                throw new ArgumentException("PartitionKey must be set before adding to Cosmos.", nameof(entity));

            Logger.LogDebug("Adding document with PartitionKey: {PartitionKey}", partitionKey);

            var cosmosItem = ToCosmosItem(entity);

            await Container.CreateItemAsync(
                cosmosItem,
                new PartitionKey(partitionKey),
                cancellationToken: ct);

            Logger.LogInformation("Document added successfully.");
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            Logger.LogWarning(ex, "Document already exists.");
            throw new InvalidOperationException($"Document already exists.", ex);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error adding document");
            throw;
        }
    }

    public void Update(T entity)
    {
        // In Cosmos, updates are handled via ReplaceItemAsync during SaveChanges.
        // This is a no-op for compatibility with IRepository contract.
    }

    public void Remove(T entity)
    {
        // In Cosmos, deletions are handled via DeleteItemAsync during SaveChanges.
        // This is a no-op for compatibility with IRepository contract.
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        try
        {
            Logger.LogDebug("Checking if any document matches predicate");

            var items = await FindAsync(predicate, ct);
            var exists = items.Any();

            Logger.LogDebug("Document exists: {Exists}", exists);
            return exists;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking document existence");
            throw;
        }
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
    {
        try
        {
            Logger.LogDebug("Counting documents");

            var query = Container.GetItemQueryIterator<dynamic>(
                new QueryDefinition("SELECT VALUE COUNT(1) FROM c"));

            var totalCount = 0;
            while (query.HasMoreResults)
            {
                var page = await query.ReadNextAsync();
                if (page.First() is int count)
                    totalCount = count;
            }

            if (predicate != null)
            {
                var items = await FindAsync(predicate, ct);
                return items.Count;
            }

            Logger.LogDebug("Total document count: {Count}", totalCount);
            return totalCount;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error counting documents");
            throw;
        }
    }
}
