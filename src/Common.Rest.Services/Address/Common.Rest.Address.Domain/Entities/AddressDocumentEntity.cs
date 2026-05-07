namespace Common.Rest.Address.Domain.Entities;

using Common.Rest.Shared.Domain;

/// <summary>
/// Represents an address document stored in Cosmos DB.
/// 
/// Cosmos Document Structure:
/// {
///   "id": "guid-string",
///   "partitionKey": "partition-key-value",
///   "documentType": "Address",
///   "jsonData": { address entity content },
///   "createdAt": "utc-datetime",
///   "updatedAt": "utc-datetime",
///   "isDeleted": false,
///   "createdBy": "user-id",
///   "updatedBy": "user-id",
///   "rowVersion": "etag-bytes"
/// }
/// </summary>
public class AddressDocumentEntity : DocumentEntity<AddressEntity>
{
    /// <summary>
    /// Partition key for efficient Cosmos DB partitioning.
    /// </summary>
    public string? PartitionKey { get; set; }
}
