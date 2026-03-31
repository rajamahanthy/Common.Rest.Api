namespace SurveyData.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext for the Survey Data microservice.
/// </summary>
public sealed class SurveyDbContext(DbContextOptions<SurveyDbContext> options) : DbContext(options)
{
    public DbSet<Survey> Surveys => Set<Survey>();
    public DbSet<SurveyDetail> SurveyDetails => Set<SurveyDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var converter = new ValueConverter<SurveyInfo?, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
            v => JsonSerializer.Deserialize<SurveyInfo?>(v, (JsonSerializerOptions)null!)
        );

        modelBuilder.Entity<Survey>(entity =>
        {
            entity.ToTable("Surveys");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ReferenceNumber).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.ReferenceNumber).IsUnique();
            entity.Property(e => e.PropertyAddress).HasMaxLength(500).IsRequired();
            entity.Property(e => e.PostCode).HasMaxLength(20);
            entity.Property(e => e.LocalAuthority).HasMaxLength(200);
            entity.Property(e => e.SurveyType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Draft");
            entity.Property(e => e.Surveyor).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(4000);
            entity.Property(e => e.AssessedValue).HasColumnType("decimal(18,2)");
            entity.Property(e => e.FloorArea).HasColumnType("decimal(18,4)");
            entity.Property(e => e.FloorAreaUnit).HasMaxLength(20);
            entity.Property(e => e.PropertyType).HasMaxLength(100);
            entity.Property(e => e.PropertySubType).HasMaxLength(100);
            entity.Property(e => e.SurveyJson)
                  .HasConversion(converter)
                  .HasColumnType("NVARCHAR(MAX)");
            entity.Property(e => e.CreatedBy).HasMaxLength(200);
            entity.Property(e => e.UpdatedBy).HasMaxLength(200);

            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasMany(e => e.Details)
                  .WithOne(d => d.Survey)
                  .HasForeignKey(d => d.SurveyId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasData(
                new Survey
                {
                    Id = Guid.Parse("f62e8b2a-8c88-4c8d-8d9e-1f2a3c4d5e6f"),
                    ReferenceNumber = "100023336491",
                    PropertyAddress = "10 Downing St, London, SW1A 2AA",
                    PostCode = "SW1A 2AA",
                    LocalAuthority = "Westminster",
                    SurveyType = "Valuation",
                    SurveyDate = new DateTimeOffset(2025, 3, 27, 0, 0, 0, TimeSpan.Zero),
                    Status = "Draft",
                    Surveyor = "John Doe",
                    Notes = "This is a test survey",
                    AssessedValue = 1000000m,
                    FloorArea = 1000m,
                    FloorAreaUnit = "sqft",
                    PropertyType = "House",
                    PropertySubType = "Detached",
                    SurveyJson = new SurveyInfo
                    {
                        Uprn = "100023336491",
                        SingleLineAddress = "10 Downing St, London, SW1A 2AA",
                        BuildingName = "Prime Minister's Residence",
                        BuildingNumber = "10",
                        Street = "Downing St",
                        Town = "London",
                        Postcode = "SW1A 2AA"
                    }
                }
            );

            entity.HasData(new Survey
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-4a5b-6c7d-8e9f0a1b2c3d"),
                ReferenceNumber = "100021234567",
                PropertyAddress = "1 High St, Edinburgh, EH1 1AA",
                PostCode = "EH1 1AA",
                LocalAuthority = "Edinburgh",
                SurveyType = "Valuation",
                SurveyDate = DateTimeOffset.UtcNow,
                Status = "Draft",
                Surveyor = "John Doe",
                Notes = "This is a test survey",
                AssessedValue = 1000000m,
                FloorArea = 1000m,
                FloorAreaUnit = "sqft",
                PropertyType = "House",
                PropertySubType = "Detached",
                SurveyJson = new SurveyInfo
                {
                    Uprn = "100021234567",
                    SingleLineAddress = "1 High St, Edinburgh, EH1 1AA",
                    BuildingName = "Main Building",
                    BuildingNumber = "1",
                    Street = "High St",
                    Town = "Edinburgh",
                    Postcode = "EH1 1AA"
                }
            });
        });

        modelBuilder.Entity<SurveyDetail>(entity =>
        {
            entity.ToTable("SurveyDetails");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Description).HasMaxLength(500).IsRequired();
            entity.Property(e => e.AreaUnit).HasMaxLength(20);
            entity.Property(e => e.Area).HasColumnType("decimal(18,4)");
            entity.Property(e => e.RatePerUnit).HasColumnType("decimal(18,4)");
            entity.Property(e => e.Value).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreatedBy).HasMaxLength(200);
            entity.Property(e => e.UpdatedBy).HasMaxLength(200);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }
}
