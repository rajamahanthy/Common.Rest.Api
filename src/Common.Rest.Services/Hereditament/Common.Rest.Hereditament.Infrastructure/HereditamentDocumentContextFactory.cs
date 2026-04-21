using Common.Rest.Hereditament.Infrastructure.Persistence;
using Common.Rest.Shared.Domain;

namespace Common.Rest.Hereditament.Infrastructure;

/// <summary>
/// Design-time factory for the HereditamentDbContext.
/// Inherits from the Shared base class to centralize configuration logic.
/// </summary>
public class HereditamentDocumentContextFactory : BaseDesignTimeDbContextFactory<HereditamentDocumentDbContext>
{
    public HereditamentDocumentContextFactory() : base("HereditamentDb")
    {
    }

    protected override HereditamentDocumentDbContext CreateContext(DbContextOptions<HereditamentDocumentDbContext> options)
    {
        return new HereditamentDocumentDbContext(options);
    }
}
