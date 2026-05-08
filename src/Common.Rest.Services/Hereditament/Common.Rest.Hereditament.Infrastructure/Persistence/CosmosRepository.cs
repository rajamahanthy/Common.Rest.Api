namespace Common.Rest.Hereditament.Infrastructure.Persistence;

using Common.Rest.Hereditament.Domain.Entities;
using Common.Rest.Shared.Repository;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Text.Json;

/// <summary>
/// Hereditament-specific Cosmos DB repository implementation.
/// Extends the generic CosmosRepositoryBase with Hereditament entity serialization logic.
/// </summary>
public class CosmosRepository : CosmosRepositoryBase< DocumentEntity<HereditamentEntity>>
{
    private readonly IUnitOfWork? _unitOfWork;

    public CosmosRepository(Container container, ILogger<CosmosRepository> logger, IUnitOfWork? unitOfWork = null)
        : base(container, logger)
    {
        _unitOfWork = unitOfWork;
    }

    public override string GetPartitionKey( DocumentEntity<HereditamentEntity> entity)
    {
        var partitionKey = entity.JsonData?.AddressId?.ToString();
        if (string.IsNullOrEmpty(partitionKey))
            throw new ArgumentException("UARN in JsonData must be set before using Cosmos operations.", nameof(entity));
        return partitionKey;
    }

    public override dynamic ToCosmosItem( DocumentEntity<HereditamentEntity> entity)
    {
        return new
        {
            id = entity.Id.ToString(),
            documentType = entity.DocumentType,
            jsonData = entity.JsonData,
            createdAt = entity.CreatedAt,
            updatedAt = entity.UpdatedAt,
            isDeleted = entity.IsDeleted,
            createdBy = entity.CreatedBy,
            updatedBy = entity.UpdatedBy,
            rowVersion = entity.RowVersion
        };
    }

    public override  DocumentEntity<HereditamentEntity> FromCosmosItem(dynamic cosmosItem)
    {
        var json = JsonSerializer.Serialize(cosmosItem);
        var parsed = JsonSerializer.Deserialize<JsonElement>(json);

        return new  DocumentEntity<HereditamentEntity>
        {
            Id = Guid.Parse(parsed.GetProperty("id").GetString() ?? Guid.Empty.ToString()),
            DocumentType = parsed.GetProperty("documentType").GetString() ?? string.Empty,
            JsonData = JsonSerializer.Deserialize<HereditamentEntity>(
                parsed.GetProperty("jsonData").GetRawText(),
                JsonOptions) ?? throw new InvalidOperationException("Unable to deserialize HereditamentEntity from jsonData"),
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

    public override void Update( DocumentEntity<HereditamentEntity> entity)
    {
        if (_unitOfWork is CosmosUnitOfWork cosmosUnitOfWork)
        {
            var partitionKey = GetPartitionKey(entity);
            var cosmosItem = ToCosmosItem(entity);
            cosmosUnitOfWork.MarkModified(entity.Id.ToString(), partitionKey, cosmosItem);
        }
    }

    public override void Remove( DocumentEntity<HereditamentEntity> entity)
    {
        if (_unitOfWork is CosmosUnitOfWork cosmosUnitOfWork)
        {
            var partitionKey = GetPartitionKey(entity);
            cosmosUnitOfWork.MarkDeleted(entity.Id.ToString(), partitionKey);
        }
    }
}
