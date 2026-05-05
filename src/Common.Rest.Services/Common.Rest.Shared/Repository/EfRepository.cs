using Microsoft.EntityFrameworkCore;
using System;

namespace Common.Rest.Shared.Repository;

/// <summary>
/// Fully generic EF Core repository implementation.
/// </summary>
public class EfRepository<TEntity>(DbContext context) : IRepository<TEntity> where TEntity : class
{
    public readonly DbContext Context = context;
    public readonly DbSet<TEntity> DbSet = context.Set<TEntity>();

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await DbSet.FindAsync([id], ct);

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default)
        => await DbSet.AsNoTracking().ToListAsync(ct);

    public virtual async Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        => await DbSet.AsNoTracking().Where(predicate).ToListAsync(ct);

    public virtual async Task<IReadOnlyList<TEntity>> FindAsync(ISpecification<TEntity> specification, CancellationToken ct = default)
        => await DbSet.AsNoTracking().Where(specification.ToExpression()).ToListAsync(ct);

    public virtual async Task<(IReadOnlyList<TEntity> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        ISpecification<TEntity>? specification = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool descending = false,
        CancellationToken ct = default)
    {
        IQueryable<TEntity> query = DbSet.AsNoTracking();
        if (specification is not null)
            query = query.Where(specification.ToExpression());
        var totalCount = await query.CountAsync(ct);
        if (orderBy is not null)
            query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return (items, totalCount);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken ct = default)
        => await DbSet.AddAsync(entity, ct);

    public virtual void Update(TEntity entity)
        => DbSet.Update(entity);

    public virtual void Remove(TEntity entity)
        => DbSet.Remove(entity);

    public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        => await DbSet.AnyAsync(predicate, ct);

    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default)
        => predicate is null ? await DbSet.CountAsync(ct) : await DbSet.CountAsync(predicate, ct);
}
