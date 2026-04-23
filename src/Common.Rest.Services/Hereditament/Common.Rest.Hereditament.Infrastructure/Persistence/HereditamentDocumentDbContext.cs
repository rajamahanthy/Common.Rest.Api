namespace Common.Rest.Hereditament.Infrastructure.Persistence;

using Common.Rest.Hereditament.Domain.Entities;
using Common.Rest.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// DbContext specifically for managing Hereditament documents with computed columns for efficient JSON querying.
/// </summary>
public class HereditamentDocumentDbContext(DbContextOptions<HereditamentDocumentDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the Hereditament Documents DbSet.
    /// </summary>
    public DbSet<HereditamentDocumentEntity> HereditamentDocuments { get; set; } = null!;

    /// <summary>
    /// Configures the model for the database context with Hereditament-specific mappings including computed columns.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure HereditamentDocuments Table
        var HereditamentDocumentsBuilder = modelBuilder.Entity<HereditamentDocumentEntity>();

        HereditamentDocumentsBuilder.ToTable("Documents");

        // Primary Key
        HereditamentDocumentsBuilder.HasKey(d => d.Id);

        // Properties
        HereditamentDocumentsBuilder.Property(d => d.Id)
            .ValueGeneratedNever()
            .IsRequired();

        HereditamentDocumentsBuilder.Property(d => d.PartitionKey)
            .HasColumnType("NVARCHAR(100)")
            .IsRequired();

        HereditamentDocumentsBuilder.Property(d => d.DocumentType)
            .HasColumnType("NVARCHAR(100)")
            .IsRequired();

        // Configure JsonData as JSON serialization with HereditamentEntity type
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        HereditamentDocumentsBuilder.Property(d => d.JsonData)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired()
            .HasConversion(
                v => JsonSerializer.Serialize(v, jsonOptions),
                v => JsonSerializer.Deserialize<HereditamentEntity>(v, jsonOptions)!)
            .HasDefaultValue(Activator.CreateInstance<HereditamentEntity>());

        HereditamentDocumentsBuilder.Property(d => d.CreatedAt)
            .HasColumnType("DATETIME2")
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .HasConversion(
                v => v.UtcDateTime,
                v => new DateTimeOffset(v, TimeSpan.Zero));

        HereditamentDocumentsBuilder.Property(d => d.UpdatedAt)
            .HasColumnType("DATETIME2")
            .IsRequired(false)
            .HasConversion(
                v => v.HasValue ? v.Value.UtcDateTime : (DateTime?)null,
                v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : (DateTimeOffset?)null);

        HereditamentDocumentsBuilder.Property(d => d.IsDeleted)
            .HasColumnType("BIT")
            .IsRequired()
            .HasDefaultValue(false);

        HereditamentDocumentsBuilder.Property(d => d.CreatedBy)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired(false);

        HereditamentDocumentsBuilder.Property(d => d.UpdatedBy)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired(false);

        // Concurrency Token
        HereditamentDocumentsBuilder.Property(d => d.RowVersion)
            .IsRowVersion();

        // Computed Columns
        HereditamentDocumentsBuilder.Property(d => d.NameIndex)
            .HasComputedColumnSql("JSON_VALUE([JsonData], '$.Hereditament.name')", stored: true)
            .IsRequired(false);

        HereditamentDocumentsBuilder.Property(d => d.StatusIndex)
            .HasComputedColumnSql("JSON_VALUE([JsonData], '$.Hereditament.status')", stored: true)
            .IsRequired(false);

        HereditamentDocumentsBuilder.Property(d => d.EffectiveFromIndex)
            .HasComputedColumnSql("JSON_VALUE([JsonData], '$.Hereditament.effectiveFrom')", stored: true)
            .IsRequired(false);

        // Indexes
        HereditamentDocumentsBuilder.HasIndex(d => d.PartitionKey)
            .HasDatabaseName("IX_Documents_PartitionKey");

        HereditamentDocumentsBuilder.HasIndex(d => d.DocumentType)
            .HasDatabaseName("IX_Documents_DocumentType");

        HereditamentDocumentsBuilder.HasIndex(d => d.IsDeleted)
            .HasDatabaseName("IX_Documents_IsDeleted");

        HereditamentDocumentsBuilder.HasIndex(d => new { d.PartitionKey, d.DocumentType })
            .HasDatabaseName("IX_Documents_PartitionKey_DocumentType");

        // Indexes on computed columns for efficient querying
        HereditamentDocumentsBuilder.HasIndex(d => d.NameIndex)
            .HasDatabaseName("IX_Documents_NameIndex");

        HereditamentDocumentsBuilder.HasIndex(d => d.StatusIndex)
            .HasDatabaseName("IX_Documents_StatusIndex");

        HereditamentDocumentsBuilder.HasIndex(d => d.EffectiveFromIndex)
            .HasDatabaseName("IX_Documents_EffectiveFromIndex");
    }
}
