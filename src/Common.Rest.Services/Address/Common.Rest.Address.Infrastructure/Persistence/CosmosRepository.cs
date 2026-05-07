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
public class CosmosRepository : CosmosRepositoryBase<DocumentEntity<AddressEntity>>
{
    public CosmosRepository(Container container, ILogger<CosmosRepository> logger)
        : base(container, logger)
    {
    }

    public override string GetPartitionKey(DocumentEntity<AddressEntity> entity)
    {
        var partitionKey = entity.JsonData?.AddressInfo?.Postcode;
        if (string.IsNullOrEmpty(partitionKey))
            throw new ArgumentException("Postcode in JsonData.AddressInfo must be set before using Cosmos operations.", nameof(entity));
        return partitionKey;
    }

    public override dynamic ToCosmosItem(DocumentEntity<AddressEntity> entity)
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

    public override DocumentEntity<AddressEntity> FromCosmosItem(dynamic cosmosItem)
    {
        var json = JsonSerializer.Serialize(cosmosItem);
        var parsed = JsonSerializer.Deserialize<JsonElement>(json);

        return new DocumentEntity<AddressEntity> 
        {
            Id = Guid.Parse(parsed.GetProperty("id").GetString() ?? Guid.Empty.ToString()),
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
