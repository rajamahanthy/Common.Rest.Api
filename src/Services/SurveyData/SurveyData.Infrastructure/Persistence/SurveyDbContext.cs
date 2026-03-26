using Microsoft.EntityFrameworkCore;
using SurveyData.Domain.Entities;

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
            entity.Property(e => e.CreatedBy).HasMaxLength(200);
            entity.Property(e => e.UpdatedBy).HasMaxLength(200);

            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasMany(e => e.Details)
                  .WithOne(d => d.Survey)
                  .HasForeignKey(d => d.SurveyId)
                  .OnDelete(DeleteBehavior.Cascade);
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
