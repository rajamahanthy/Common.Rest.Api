using Common.Rest.Address.Infrastructure.Persistence;
using Common.Rest.Shared.Domain;

namespace Common.Rest.Address.Infrastructure;

/// <summary>
/// Design-time factory for the AddressDbContext.
/// Inherits from the Shared base class to centralize configuration logic.
/// </summary>
public class DocumentContextFactory : BaseDesignTimeDbContextFactory<DocumentDbContext<AddressEntity>>
{
    public DocumentContextFactory() : base("AddressDb")
    {
    }

    protected override DocumentDbContext<AddressEntity> CreateContext(DbContextOptions<DocumentDbContext<AddressEntity>> options)
    {
        return new DocumentDbContext<AddressEntity>(options);
    }
}
