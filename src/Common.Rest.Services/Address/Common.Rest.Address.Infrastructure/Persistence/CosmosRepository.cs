namespace Common.Rest.Address.Infrastructure.Persistence;

using Azure.Cosmos;
using Common.Rest.Address.Domain.Entities;
using Common.Rest.Shared.Repository;
using Common.Rest.Shared.Specification;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Text.Json;

/// <summary>
/// Cosmos DB repository implementation for AddressDocumentEntity.
/// Replaces EF Core repository with direct Cosmos SDK usage.
/// </summary>
public class CosmosRepository : IRepository<AddressDocumentEntity>
{
    private readonly Container _container;
    private readonly ILogger<CosmosRepository> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public CosmosRepository(Container container, ILogger<CosmosRepository> logger)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AddressDocumentEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            _logger.LogDebug("Fetching document with Id: {Id}", id);

            // Cosmos requires partition key for point reads. 
            // Since we don't always have postcode when reading by ID only, we must use query.
            var query = _container.GetItemQueryIterator<AddressDocumentEntity>(
                new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
                    .WithParameter("@id", id.ToString()));

            await foreach (var page in query.AsPages())
            {
                foreach (var item in page.Values)
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

    public async Task<IReadOnlyList<AddressDocumentEntity>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            _logger.LogDebug("Fetching all documents");

            var query = _container.GetItemQueryIterator<AddressDocumentEntity>(
                new QueryDefinition("SELECT * FROM c"));

            var items = new List<AddressDocumentEntity>();
            await foreach (var page in query.AsPages())
            {
                items.AddRange(page.Values);
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

    public async Task<IReadOnlyList<AddressDocumentEntity>> FindAsync(
        Expression<Func<AddressDocumentEntity, bool>> predicate,
        CancellationToken ct = default)
    {
        try
        {
            _logger.LogDebug("Finding documents with predicate");

            // Note: Cosmos doesn't support LINQ expressions like SQL.
            // We fetch all and filter in-memory. For large datasets, use SQL queries instead.
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

    public async Task<IReadOnlyList<AddressDocumentEntity>> FindAsync(
        ISpecification<AddressDocumentEntity> specification,
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

    public async Task<(IReadOnlyList<AddressDocumentEntity> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        Expression<Func<AddressDocumentEntity, bool>>? predicate = null,
        ISpecification<AddressDocumentEntity>? specification = null,
        Expression<Func<AddressDocumentEntity, object>>? orderBy = null,
        bool descending = false,
        CancellationToken ct = default)
    {
        try
        {
            _logger.LogDebug("Fetching paged documents: page={Page}, pageSize={PageSize}", page, pageSize);

            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            // Fetch all matching items (optimization: could use Cosmos paging tokens)
            IEnumerable<AddressDocumentEntity> query = await GetAllAsync(ct);

            // Apply specification filter
            if (specification != null)
            {
                var expression = specification.ToExpression();
                var compiled = expression.Compile();
                query = query.Where(compiled);
            }

            // Apply predicate filter
            if (predicate != null)
            {
                var compiled = predicate.Compile();
                query = query.Where(compiled);
            }

            // Apply sorting
            if (orderBy != null)
            {
                var compiled = orderBy.Compile();
                query = descending
                    ? query.OrderByDescending(compiled)
                    : query.OrderBy(compiled);
            }

            var totalCount = query.Count();

            // Apply pagination
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

    public async Task AddAsync(AddressDocumentEntity entity, CancellationToken ct = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity);
            if (string.IsNullOrEmpty(entity.PartitionKey))
                throw new ArgumentException("PartitionKey (postcode) must be set before adding to Cosmos.", nameof(entity));

            _logger.LogDebug("Adding document with Id: {Id}, PartitionKey: {PartitionKey}", 
                entity.Id, entity.PartitionKey);

            // Convert to dynamic object for Cosmos, preserving all properties
            var cosmosItem = ToCosmosItem(entity);

            await _container.CreateItemAsync(
                cosmosItem,
                new PartitionKey(entity.PartitionKey),
                cancellationToken: ct);

            _logger.LogInformation("Document added successfully. Id: {Id}", entity.Id);
        }
        catch (CosmosException ex) when (ex.Status == 409)
        {
            _logger.LogWarning(ex, "Document already exists. Id: {Id}", entity.Id);
            throw new InvalidOperationException($"Document with Id {entity.Id} already exists.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding document with Id: {Id}", entity.Id);
            throw;
        }
    }

    public void Update(AddressDocumentEntity entity)
    {
        // In Cosmos, updates are handled via ReplaceItemAsync during SaveChanges.
        // This is a no-op for compatibility with IRepository contract.
        // The actual update happens in UnitOfWork.SaveChangesAsync.
    }

    public void Remove(AddressDocumentEntity entity)
    {
        // In Cosmos, deletions are handled via DeleteItemAsync during SaveChanges.
        // This is a no-op for compatibility with IRepository contract.
        // The actual deletion happens in UnitOfWork.SaveChangesAsync.
    }

    public async Task<bool> ExistsAsync(Expression<Func<AddressDocumentEntity, bool>> predicate, CancellationToken ct = default)
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

    public async Task<int> CountAsync(Expression<Func<AddressDocumentEntity, bool>>? predicate = null, CancellationToken ct = default)
    {
        try
        {
            _logger.LogDebug("Counting documents");

            var query = _container.GetItemQueryIterator<dynamic>(
                new QueryDefinition("SELECT VALUE COUNT(1) FROM c"));

            var totalCount = 0;
            await foreach (var page in query.AsPages())
            {
                if (page.Values.First() is int count)
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

    /// <summary>
    /// Internal helper to parse Cosmos item back to entity (used for testing/serialization).
    /// </summary>
    internal static AddressDocumentEntity FromCosmosItem(dynamic cosmosItem)
    {
        var json = JsonSerializer.Serialize(cosmosItem);
        var parsed = JsonSerializer.Deserialize<JsonElement>(json);

        return new AddressDocumentEntity
        {
            Id = Guid.Parse(parsed.GetProperty("id").GetString() ?? Guid.Empty.ToString()),
            PartitionKey = parsed.GetProperty("postcode").GetString() ?? string.Empty,
            DocumentType = parsed.GetProperty("documentType").GetString() ?? string.Empty,
            JsonData = JsonSerializer.Deserialize<AddressEntity>(
                parsed.GetProperty("jsonData").GetRawText(),
                JsonOptions) ?? throw new InvalidOperationException("Unable to deserialize AddressEntity from jsonData"),
            UprnIndex = parsed.TryGetProperty("uprn", out JsonElement uprn) ? uprn.GetString() : null,
            PostcodeIndex = parsed.TryGetProperty("postcodeIndex", out JsonElement postcode) ? postcode.GetString() : null,
            PostTownIndex = parsed.TryGetProperty("postTownIndex", out JsonElement town) ? town.GetString() : null,
            OrganisationIndex = parsed.TryGetProperty("organisationIndex", out JsonElement org) ? org.GetString() : null,
            ThoroughfareIndex = parsed.TryGetProperty("thoroughfareIndex", out JsonElement thor) ? thor.GetString() : null,
            LocalityIndex = parsed.TryGetProperty("localityIndex", out JsonElement local) ? local.GetString() : null,
            DependentLocalityIndex = parsed.TryGetProperty("dependentLocalityIndex", out JsonElement depLocal) ? depLocal.GetString() : null,
            CreatedAt = parsed.GetProperty("createdAt").GetDateTimeOffset(),
            UpdatedAt = parsed.TryGetProperty("updatedAt", out JsonElement updated) ? updated.GetDateTimeOffset() : null,
            IsDeleted = parsed.GetProperty("isDeleted").GetBoolean(),
            CreatedBy = parsed.TryGetProperty("createdBy", out JsonElement creator) ? creator.GetString() : null,
            UpdatedBy = parsed.TryGetProperty("updatedBy", out JsonElement updater) ? updater.GetString() : null,
            RowVersion = parsed.TryGetProperty("rowVersion", out JsonElement rowVersion) 
                ? Convert.FromBase64String(rowVersion.GetString() ?? string.Empty) 
                : null
        };
    }
}
