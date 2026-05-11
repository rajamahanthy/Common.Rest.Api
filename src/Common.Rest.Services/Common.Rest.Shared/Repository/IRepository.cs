namespace Common.Rest.Shared.Repository;

/// <summary>
/// Generic repository interface for the Repository pattern.
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<IReadOnlyList<T>> FindAsync(ISpecification<T> specification, CancellationToken ct = default);
    Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        ISpecification<T>? specification = null,
        Expression<Func<T, object>>? orderBy = null,
        bool descending = false,
        CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Remove(T entity);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);


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

public interface IDocumentData
{
}