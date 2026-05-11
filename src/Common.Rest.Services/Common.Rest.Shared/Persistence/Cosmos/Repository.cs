namespace Common.Rest.Shared.Persistence.Cosmos;

using Common.Rest.Shared.Domain;
using Common.Rest.Shared.Repository;
using Common.Rest.Shared.Specification;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Text.Json;

/// <summary>
/// Generic Cosmos DB repository implementation supporting any DocumentEntity.
/// Provides CRUD, query, and pagination operations for Cosmos DB containers.
/// Serves as the base class for entity-specific repository implementations.
/// </summary>
public abstract class Repository<T> : IRepository<T> where T : class
{
    protected readonly Container _container;
    protected readonly ILogger<Repository<T>> _logger;
    protected readonly IUnitOfWork _unitOfWork;

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    protected Repository(Container container, ILogger<Repository<T>> logger, IUnitOfWork unitOfWork = null)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _unitOfWork = unitOfWork;
    }

    public string GetPartitionKey(T entity)
        => (entity as BaseEntity)?.PartitionKey ?? string.Empty;

    public dynamic ToCosmosItem(T entity)
        => entity!;

    public T FromCosmosItem(dynamic cosmosItem)
        => (T)cosmosItem;

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            _logger.LogDebug("Fetching document with Id: {Id}", id);

            var query = _container.GetItemQueryIterator<T>(
                new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
                    .WithParameter("@id", id.ToString()));

            while (query.HasMoreResults)
            {
                var page = await query.ReadNextAsync();
                foreach (var item in page)
                {
                    if (item != null)
                    {
                        _logger.LogDebug("Found document with Id: {Id}", id);
                        return item;
                    }
                }
            }

            _logger.LogDebug("Document with Id: {Id} not found.", id);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching document with Id: {Id}", id);
            throw;
        }
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            _logger.LogDebug("Fetching all documents");

            var query = _container.GetItemQueryIterator<T>(
                new QueryDefinition("SELECT * FROM c"));

            var items = new List<T>();
            while (query.HasMoreResults)
            {
                var page = await query.ReadNextAsync();
                items.AddRange(page);
            }

            _logger.LogDebug("Fetched {Count} documents", items.Count);
            return items.AsReadOnly();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all documents");
            throw;
        }
    }

    public async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default)
    {
        try
        {
            _logger.LogDebug("Finding documents with predicate");

            var allItems = await GetAllAsync(ct);
            var compiled = predicate.Compile();
            var filtered = allItems.Where(compiled).ToList();

            _logger.LogDebug("Found {Count} documents matching predicate", filtered.Count);
            return filtered.AsReadOnly();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding documents");
            throw;
        }
    }

    public async Task<IReadOnlyList<T>> FindAsync(
        ISpecification<T> specification,
        CancellationToken ct = default)
    {
        try
        {
            _logger.LogDebug("Finding documents with specification");

            var allItems = await GetAllAsync(ct);
            var expression = specification.ToExpression();
            var compiled = expression.Compile();
            var filtered = allItems.Where(compiled).ToList();

            _logger.LogDebug("Found {Count} documents matching specification", filtered.Count);
            return filtered.AsReadOnly();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding documents with specification");
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
            _logger.LogDebug("Fetching paged documents: page={Page}, pageSize={PageSize}", page, pageSize);

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

            _logger.LogDebug("Fetched {Count} items for page {Page}, total count: {TotalCount}", 
                items.Count, page, totalCount);

            return (items.AsReadOnly(), totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching paged documents");
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

            _logger.LogDebug("Adding document with PartitionKey: {PartitionKey}", partitionKey);

            var cosmosItem = ToCosmosItem(entity);

            await _container.CreateItemAsync(
                cosmosItem,
                new PartitionKey(partitionKey),
                cancellationToken: ct);

            _logger.LogInformation("Document added successfully.");
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            _logger.LogWarning(ex, "Document already exists.");
            throw new InvalidOperationException($"Document already exists.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding document");
            throw;
        }
    }

    public virtual void Update(T entity)
    {
        var partitionKey = GetPartitionKey(entity);
        var cosmosItem = ToCosmosItem(entity);
        var entityId = (entity as BaseEntity)?.Id;
        if (entityId.HasValue)
        {
            _unitOfWork.MarkModified(entityId.ToString()!, partitionKey, cosmosItem);
        }
    }

    public virtual void Remove(T entity)
    {
        var partitionKey = GetPartitionKey(entity);
        var entityId = (entity as BaseEntity)?.Id;
        if (entityId.HasValue)
        {
            _unitOfWork.MarkDeleted(entityId.ToString()!, partitionKey);
        }
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        try
        {
            _logger.LogDebug("Checking if any document matches predicate");

            var items = await FindAsync(predicate, ct);
            var exists = items.Any();

            _logger.LogDebug("Document exists: {Exists}", exists);
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking document existence");
            throw;
        }
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
    {
        try
        {
            _logger.LogDebug("Counting documents");

            var query = _container.GetItemQueryIterator<dynamic>(
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

            _logger.LogDebug("Total document count: {Count}", totalCount);
            return totalCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting documents");
            throw;
        }
    }
}
