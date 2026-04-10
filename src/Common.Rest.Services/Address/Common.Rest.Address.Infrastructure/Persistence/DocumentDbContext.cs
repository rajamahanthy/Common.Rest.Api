namespace Common.Rest.Address.Infrastructure.Persistence;

using Common.Rest.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Generic DbContext for managing document entities with JSON data storage.
/// Supports any TData type that can be serialized to/from JSON.
/// </summary>
/// <typeparam name="TData">The type of data to be stored in the JsonData field.</typeparam>
public class DocumentDbContext<TData>(DbContextOptions options) : DbContext(options) where TData : class
{
    /// <summary>
    /// Gets or sets the Documents DbSet for storing document entities with typed JSON data.
    /// </summary>
    public DbSet<DocumentEntity<TData>> Documents { get; set; } = null!;

    /// <summary>
    /// Configures the model for the database context with generic document entity mapping.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ?? Configure Documents Table ????????????????????????????????
        var documentsBuilder = modelBuilder.Entity<DocumentEntity<TData>>();

        documentsBuilder.ToTable("Documents");

        // Primary Key
        documentsBuilder.HasKey(d => d.Id);

        // Properties
        documentsBuilder.Property(d => d.Id)
            .ValueGeneratedNever()
            .IsRequired();

        documentsBuilder.Property(d => d.PartitionKey)
            .HasColumnType("NVARCHAR(100)")
            .IsRequired();

        documentsBuilder.Property(d => d.DocumentType)
            .HasColumnType("NVARCHAR(100)")
            .IsRequired();

        // Configure JsonData as JSON serialization with TData type
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        documentsBuilder.Property(d => d.JsonData)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired()
            .HasConversion(
                v => JsonSerializer.Serialize(v, jsonOptions),
                v => JsonSerializer.Deserialize<TData>(v, jsonOptions)!)
            .HasDefaultValue(Activator.CreateInstance<TData>());

        documentsBuilder.Property(d => d.CreatedAt)
            .HasColumnType("DATETIME2")
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .HasConversion(
                v => v.UtcDateTime,
                v => new DateTimeOffset(v, TimeSpan.Zero));

        documentsBuilder.Property(d => d.UpdatedAt)
            .HasColumnType("DATETIME2")
            .IsRequired(false)
            .HasConversion(
                v => v.HasValue ? v.Value.UtcDateTime : (DateTime?)null,
                v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : (DateTimeOffset?)null);

        documentsBuilder.Property(d => d.IsDeleted)
            .HasColumnType("BIT")
            .IsRequired()
            .HasDefaultValue(false);

        documentsBuilder.Property(d => d.CreatedBy)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired(false);

        documentsBuilder.Property(d => d.UpdatedBy)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired(false);

        // Concurrency Token
        documentsBuilder.Property(d => d.RowVersion)
            .IsRowVersion();

        // Indexes
        documentsBuilder.HasIndex(d => d.PartitionKey)
            .HasDatabaseName("IX_Documents_PartitionKey");

        documentsBuilder.HasIndex(d => d.DocumentType)
            .HasDatabaseName("IX_Documents_DocumentType");

        documentsBuilder.HasIndex(d => d.IsDeleted)
            .HasDatabaseName("IX_Documents_IsDeleted");

        documentsBuilder.HasIndex(d => new { d.PartitionKey, d.DocumentType })
            .HasDatabaseName("IX_Documents_PartitionKey_DocumentType");
    }
}
