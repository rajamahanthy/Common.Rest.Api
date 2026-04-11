namespace Common.Rest.Address.Infrastructure;

using Common.Rest.Address.Domain.Entities;
using Common.Rest.Address.Infrastructure.Persistence;
using Common.Rest.Shared.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        const string dbConnStrKey = "AddressDb";

        // ?? DbContext ???????????????????????????????????????????????????
        services.AddDbContext<AddressDocumentDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString(dbConnStrKey);
            if (string.IsNullOrEmpty(connectionString) || connectionString.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
            {
                options.UseInMemoryDatabase(dbConnStrKey);
            }
            else
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                });
            }
        });

        // ?? Register DbContext base class for repository injection ????????
        services.AddScoped<DbContext>(provider => provider.GetRequiredService<AddressDocumentDbContext>());

        // ?? Repositories (Generic) ??????????????????????????????????????
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        // ?? Unit of Work (Generic) ???????????????????????????????????????
        services.AddScoped<IUnitOfWork, UnitOfWork>(provider =>
            new UnitOfWork(provider.GetRequiredService<AddressDocumentDbContext>()));

        return services;
    }
}


