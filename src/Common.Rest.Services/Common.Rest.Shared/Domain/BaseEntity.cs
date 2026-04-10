namespace Common.Rest.Shared.Domain;

/// <summary>
/// Base entity with common audit fields shared across all microservices.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? PartitionKey { get; set; } = Convert.ToString(Guid.NewGuid());
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}


