namespace Common.Rest.Address.Infrastructure;

using Common.Rest.Address.Infrastructure.Persistence;
using Common.Rest.Shared.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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
