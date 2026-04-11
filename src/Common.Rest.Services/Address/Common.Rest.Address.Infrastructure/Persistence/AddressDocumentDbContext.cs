namespace Common.Rest.Address.Infrastructure.Persistence;

using Common.Rest.Address.Domain.Entities;
using Common.Rest.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// DbContext specifically for managing Address documents with computed columns for efficient JSON querying.
/// </summary>
public class AddressDocumentDbContext(DbContextOptions<AddressDocumentDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the Address Documents DbSet.
    /// </summary>
    public DbSet<AddressDocumentEntity> AddressDocuments { get; set; } = null!;

    /// <summary>
    /// Configures the model for the database context with address-specific mappings including computed columns.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure AddressDocuments Table
        var addressDocumentsBuilder = modelBuilder.Entity<AddressDocumentEntity>();

        addressDocumentsBuilder.ToTable("Documents");

        // Primary Key
        addressDocumentsBuilder.HasKey(d => d.Id);

        // Properties
        addressDocumentsBuilder.Property(d => d.Id)
            .ValueGeneratedNever()
            .IsRequired();

        addressDocumentsBuilder.Property(d => d.PartitionKey)
            .HasColumnType("NVARCHAR(100)")
            .IsRequired();

        addressDocumentsBuilder.Property(d => d.DocumentType)
            .HasColumnType("NVARCHAR(100)")
            .IsRequired();

        // Configure JsonData as JSON serialization with AddressEntity type
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        addressDocumentsBuilder.Property(d => d.JsonData)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired()
            .HasConversion(
                v => JsonSerializer.Serialize(v, jsonOptions),
                v => JsonSerializer.Deserialize<AddressEntity>(v, jsonOptions)!)
            .HasDefaultValue(Activator.CreateInstance<AddressEntity>());

        addressDocumentsBuilder.Property(d => d.CreatedAt)
            .HasColumnType("DATETIME2")
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .HasConversion(
                v => v.UtcDateTime,
                v => new DateTimeOffset(v, TimeSpan.Zero));

        addressDocumentsBuilder.Property(d => d.UpdatedAt)
            .HasColumnType("DATETIME2")
            .IsRequired(false)
            .HasConversion(
                v => v.HasValue ? v.Value.UtcDateTime : (DateTime?)null,
                v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : (DateTimeOffset?)null);

        addressDocumentsBuilder.Property(d => d.IsDeleted)
            .HasColumnType("BIT")
            .IsRequired()
            .HasDefaultValue(false);

        addressDocumentsBuilder.Property(d => d.CreatedBy)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired(false);

        addressDocumentsBuilder.Property(d => d.UpdatedBy)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired(false);

        // Concurrency Token
        addressDocumentsBuilder.Property(d => d.RowVersion)
            .IsRowVersion();

        // Computed Columns
        addressDocumentsBuilder.Property(d => d.UprnIndex)
            .HasComputedColumnSql("JSON_VALUE([JsonData], '$.uprn')", stored: true)
            .IsRequired(false);

        addressDocumentsBuilder.Property(d => d.PostcodeIndex)
            .HasComputedColumnSql("JSON_VALUE([JsonData], '$.address.postcode')", stored: true)
            .IsRequired(false);

        addressDocumentsBuilder.Property(d => d.PostTownIndex)
            .HasComputedColumnSql("JSON_VALUE([JsonData], '$.address.streetDescriptor.postTown')", stored: true)
            .IsRequired(false);

        addressDocumentsBuilder.Property(d => d.OrganisationIndex)
            .HasComputedColumnSql("JSON_VALUE([JsonData], '$.address.organisation')", stored: true)
            .IsRequired(false);

        addressDocumentsBuilder.Property(d => d.ThoroughfareIndex)
            .HasComputedColumnSql("JSON_VALUE([JsonData], '$.address.streetDescriptor.streetDescription')", stored: true)
            .IsRequired(false);

        addressDocumentsBuilder.Property(d => d.LocalityIndex)
            .HasComputedColumnSql("JSON_VALUE([JsonData], '$.address.streetDescriptor.locality')", stored: true)
            .IsRequired(false);

        addressDocumentsBuilder.Property(d => d.DependentLocalityIndex)
            .HasComputedColumnSql("JSON_VALUE([JsonData], '$.address.streetDescriptor.dependentLocality')", stored: true)
            .IsRequired(false);

        // Indexes
        addressDocumentsBuilder.HasIndex(d => d.PartitionKey)
            .HasDatabaseName("IX_Documents_PartitionKey");

        addressDocumentsBuilder.HasIndex(d => d.DocumentType)
            .HasDatabaseName("IX_Documents_DocumentType");

        addressDocumentsBuilder.HasIndex(d => d.IsDeleted)
            .HasDatabaseName("IX_Documents_IsDeleted");

        addressDocumentsBuilder.HasIndex(d => new { d.PartitionKey, d.DocumentType })
            .HasDatabaseName("IX_Documents_PartitionKey_DocumentType");

        // Indexes on computed columns for efficient querying
        addressDocumentsBuilder.HasIndex(d => d.UprnIndex)
            .HasDatabaseName("IX_Documents_UprnIndex");

        addressDocumentsBuilder.HasIndex(d => d.PostcodeIndex)
            .HasDatabaseName("IX_Documents_PostcodeIndex");

        addressDocumentsBuilder.HasIndex(d => d.PostTownIndex)
            .HasDatabaseName("IX_Documents_PostTownIndex");

        addressDocumentsBuilder.HasIndex(d => d.OrganisationIndex)
            .HasDatabaseName("IX_Documents_OrganisationIndex");

        addressDocumentsBuilder.HasIndex(d => d.ThoroughfareIndex)
            .HasDatabaseName("IX_Documents_ThoroughfareIndex");

        addressDocumentsBuilder.HasIndex(d => d.LocalityIndex)
            .HasDatabaseName("IX_Documents_LocalityIndex");

        addressDocumentsBuilder.HasIndex(d => d.DependentLocalityIndex)
            .HasDatabaseName("IX_Documents_DependentLocalityIndex");
    }
}
