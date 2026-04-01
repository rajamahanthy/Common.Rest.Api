namespace Common.Rest.SurveyData.Infrastructure;

using Common.Rest.SurveyData.Infrastructure.Persistence;
using Common.Rest.Shared.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

/// <summary>
/// Design-time factory for the SurveyDbContext.
/// Inherits from the Shared base class to centralize configuration logic.
/// </summary>
public class SurveyDbContextFactory : BaseDesignTimeDbContextFactory<SurveyDbContext>
{
    public SurveyDbContextFactory() : base("SurveyDb")
    {
    }

    protected override SurveyDbContext CreateContext(DbContextOptions<SurveyDbContext> options)
    {
        return new SurveyDbContext(options);
    }
}
