namespace Common.Rest.Shared.Persistence.Cosmos;

using Common.Rest.Shared.Repository;

/// <summary>
/// Cosmos DB specific repository interface extending the generic repository.
/// Provides methods for Cosmos-specific serialization and document management.
/// </summary>
public interface ICosmosRepository<T> : IRepository<T> where T : class
{
    /// <summary>
    /// Gets the partition key value for an entity.
    /// </summary>
    string GetPartitionKey(T entity);

    /// <summary>
    /// Converts entity to Cosmos-compatible format.
    /// </summary>
    dynamic ToCosmosItem(T entity);

    /// <summary>
    /// Converts Cosmos item back to entity.
    /// </summary>
    T FromCosmosItem(dynamic cosmosItem);
}

/// <summary>
/// Marker interface for document data types used by Cosmos repositories.
/// </summary>
public interface IDocumentData
{
}
