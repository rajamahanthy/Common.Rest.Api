namespace Common.Rest.Address.Domain.Entities;

using Common.Rest.Shared.Domain;

/// <summary>
/// Represents an address document stored in Cosmos DB with denormalized search fields.
/// 
/// Cosmos Document Structure:
/// {
///   "id": "guid-string",
///   "postcode": "partition-key-value",
///   "documentType": "Address",
///   "jsonData": { address entity content },
///   "uprn": "denormalized for search",
///   "postcodeIndex": "denormalized for search",
///   "postTownIndex": "denormalized for search",
///   "organisationIndex": "denormalized for search",
///   "thoroughfareIndex": "denormalized for search",
///   "localityIndex": "denormalized for search",
///   "dependentLocalityIndex": "denormalized for search",
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
    /// Partition key: postcode for efficient Cosmos DB partitioning.
    /// </summary>
    public string? PartitionKey { get; set; }

    /// <summary>
    /// Denormalized UPRN from JsonData for efficient querying and indexing.
    /// Sourced from: JsonData.Uprn
    /// </summary>
    public string? UprnIndex { get; set; }

    /// <summary>
    /// Denormalized Postcode from JsonData for efficient querying and indexing.
    /// Sourced from: JsonData.AddressInfo.Postcode
    /// Equals PartitionKey when properly synchronized.
    /// </summary>
    public string? PostcodeIndex { get; set; }

    /// <summary>
    /// Denormalized PostTown from JsonData for efficient querying and indexing.
    /// Sourced from: JsonData.AddressInfo.StreetDescriptor.PostTown
    /// </summary>
    public string? PostTownIndex { get; set; }

    /// <summary>
    /// Denormalized Organisation from JsonData for efficient querying and indexing.
    /// Sourced from: JsonData.AddressInfo.Organisation
    /// </summary>
    public string? OrganisationIndex { get; set; }

    /// <summary>
    /// Denormalized Thoroughfare (StreetDescription) from JsonData for efficient querying and indexing.
    /// Sourced from: JsonData.AddressInfo.StreetDescriptor.StreetDescription
    /// </summary>
    public string? ThoroughfareIndex { get; set; }

    /// <summary>
    /// Denormalized Locality from JsonData for efficient querying and indexing.
    /// Sourced from: JsonData.AddressInfo.StreetDescriptor.Locality
    /// </summary>
    public string? LocalityIndex { get; set; }

    /// <summary>
    /// Denormalized DependentLocality from JsonData for efficient querying and indexing.
    /// Sourced from: JsonData.AddressInfo.StreetDescriptor.DependentLocality
    /// </summary>
    public string? DependentLocalityIndex { get; set; }
}
