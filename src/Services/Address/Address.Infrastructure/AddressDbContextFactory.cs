using RestApi.Shared.Persistence;
using Address.Infrastructure.Persistence;

namespace Address.Infrastructure;

/// <summary>
/// Design-time factory for the AddressDbContext.
/// Inherits from the Shared base class to centralize configuration logic.
/// </summary>
public class AddressDbContextFactory : BaseDesignTimeDbContextFactory<AddressDbContext>
{
    public AddressDbContextFactory() : base("AddressDb")
    {
    }

    protected override AddressDbContext CreateContext(DbContextOptions<AddressDbContext> options)
    {
        return new AddressDbContext(options);
    }
}
