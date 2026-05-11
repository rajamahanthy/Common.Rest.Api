namespace Common.Rest.Shared.Repository;

/// <summary>
/// Unit of Work interface for coordinating repository transactions.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
    void MarkModified(string v, string partitionKey, dynamic cosmosItem);
    void MarkDeleted(string v, string partitionKey);
}
