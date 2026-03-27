using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Address.Infrastructure.Persistence;

namespace Address.Infrastructure;

public class AddressDbContextFactory : IDesignTimeDbContextFactory<AddressDbContext>
{
    public AddressDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AddressDbContext>();
        var connectionString = configuration.GetConnectionString("AddressDb");

        optionsBuilder.UseSqlServer(connectionString);

        return new AddressDbContext(optionsBuilder.Options);
    }
}
