using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
namespace Address.Infrastructure.Persistence;

public class AddressDbContext(DbContextOptions<AddressDbContext> options) : DbContext(options)
{
    public DbSet<AddressEntity> Addresses => Set<AddressEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var converter = new ValueConverter<AdditionalInfo, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
            v => JsonSerializer.Deserialize<AdditionalInfo>(v, (JsonSerializerOptions)null)
        );
        // Standard configuration for Address entity
        modelBuilder.Entity<AddressEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Uprn).IsRequired().HasMaxLength(20);
            entity.Property(e => e.SingleLineAddress).IsRequired().HasMaxLength(500);
            entity.Property(e => e.BuildingName).HasMaxLength(100);
            entity.Property(e => e.BuildingNumber).HasMaxLength(20);
            entity.Property(e => e.Street).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Locality).HasMaxLength(200);
            entity.Property(e => e.Town).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Postcode).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Country).IsRequired().HasMaxLength(100);

            // Audit Query Filter (Soft Delete)
            entity.HasQueryFilter(e => !e.IsDeleted);

            // Native JSON Column Mapping
            entity.Property(e => e.AdditionalInfoJson)
                  .HasConversion(converter)
                  .HasColumnType("NVARCHAR(MAX)");

            // Indices
            entity.HasIndex(e => e.Uprn).IsUnique();
            entity.HasIndex(e => e.Postcode);

            // Seed Data
            entity.HasData(
                new AddressEntity
                {
                    Id = Guid.Parse("f62e8b2a-8c88-4c8d-8d9e-1f2a3c4d5e6f"),
                    Uprn = "100023336491",
                    SingleLineAddress = "10 Downing St, London, SW1A 2AA",
                    BuildingName = "Prime Minister's Residence",
                    BuildingNumber = "10",
                    Street = "Downing St",
                    Locality = "Westminster",
                    Town = "London",
                    Postcode = "SW1A 2AA",
                    Country = "United Kingdom",
                    CreatedAt = new DateTimeOffset(2026, 3, 27, 0, 0, 0, TimeSpan.Zero),
                    AdditionalInfoJson = new AdditionalInfo
                    {
                        AddressLine1 = "10 Downing St",
                        AddressLine2 = "London",
                        AddressLine3 = "SW1A 2AA"
                    }
                },
                new AddressEntity
                {
                    Id = Guid.Parse("a1b2c3d4-e5f6-4a5b-6c7d-8e9f0a1b2c3d"),
                    Uprn = "100021234567",
                    SingleLineAddress = "1 High St, Edinburgh, EH1 1AA",
                    BuildingName = "Main Building",
                    BuildingNumber = "1",
                    Street = "High St",
                    Locality = "City Centre",
                    Town = "Edinburgh",
                    Postcode = "EH1 1AA",
                    Country = "United Kingdom",
                    CreatedAt = new DateTimeOffset(2026, 3, 27, 0, 0, 0, TimeSpan.Zero),
                    AdditionalInfoJson = new AdditionalInfo
                    {
                        AddressLine1 = "1 High St",
                        AddressLine2 = "Edinburgh",
                        AddressLine3 = "EH1 1AA"
                    }
                }
            );
        });
    }
}
