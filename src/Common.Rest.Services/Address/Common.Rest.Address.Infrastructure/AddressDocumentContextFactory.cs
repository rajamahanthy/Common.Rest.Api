using Common.Rest.Address.Infrastructure.Persistence;
using Common.Rest.Shared.Domain;

namespace Common.Rest.Address.Infrastructure;

/// <summary>
/// Design-time factory for the AddressDbContext.
/// Inherits from the Shared base class to centralize configuration logic.
/// </summary>
public class AddressDocumentContextFactory : BaseDesignTimeDbContextFactory<AddressDocumentDbContext>
{
    public AddressDocumentContextFactory() : base("AddressDb")
    {
    }

    protected override AddressDocumentDbContext CreateContext(DbContextOptions<AddressDocumentDbContext> options)
    {
        return new AddressDocumentDbContext(options);
    }
}
