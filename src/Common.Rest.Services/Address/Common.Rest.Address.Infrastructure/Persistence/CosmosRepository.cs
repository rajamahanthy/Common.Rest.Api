namespace Common.Rest.Address.Infrastructure.Persistence;

using Common.Rest.Address.Domain.Entities;
using Common.Rest.Shared.Persistence.Cosmos;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Text.Json;

/// <summary>
/// Address-specific Cosmos DB repository implementation.
/// Extends the generic CosmosRepositoryBase with Address entity serialization logic.
/// </summary>
public class CosmosRepository : CosmosRepositoryBase<AddressDocumentEntity>
{
    public CosmosRepository(Container container, ILogger<CosmosRepository> logger)
        : base(container, logger)
    {
    }

    public override string GetPartitionKey(AddressDocumentEntity entity)
    {
        if (string.IsNullOrEmpty(entity.PartitionKey))
            throw new ArgumentException("PartitionKey must be set before using Cosmos operations.", nameof(entity));
        return entity.PartitionKey;
    }

    public override dynamic ToCosmosItem(AddressDocumentEntity entity)
    {
        return new
        {
            id = entity.Id.ToString(),
            partitionKey = entity.PartitionKey,
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

    public override AddressDocumentEntity FromCosmosItem(dynamic cosmosItem)
    {
        var json = JsonSerializer.Serialize(cosmosItem);
        var parsed = JsonSerializer.Deserialize<JsonElement>(json);

        return new AddressDocumentEntity
        {
            Id = Guid.Parse(parsed.GetProperty("id").GetString() ?? Guid.Empty.ToString()),
            PartitionKey = parsed.GetProperty("partitionKey").GetString() ?? string.Empty,
            DocumentType = parsed.GetProperty("documentType").GetString() ?? string.Empty,
            JsonData = JsonSerializer.Deserialize<AddressEntity>(
                parsed.GetProperty("jsonData").GetRawText(),
                JsonOptions) ?? throw new InvalidOperationException("Unable to deserialize AddressEntity from jsonData"),
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
