using Address.Infrastructure.Persistence;
using RestApi.Shared.Repository;

namespace Address.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // ── DbContext ───────────────────────────────────────────────────
        services.AddDbContext<AddressDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("AddressDb");
            if (string.IsNullOrEmpty(connectionString) || connectionString.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
            {
                options.UseInMemoryDatabase("AddressDb");
            }
            else
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                });
            }
        });
        
        // ── Register DbContext base class for repository injection ────────
        services.AddScoped<DbContext>(provider => provider.GetRequiredService<AddressDbContext>());

        // ── Repositories (Generic) ──────────────────────────────────────
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        
        // ── Unit of Work (Generic) ───────────────────────────────────────
        services.AddScoped<IUnitOfWork, UnitOfWork>(provider => 
            new UnitOfWork(provider.GetRequiredService<AddressDbContext>()));

        return services;
    }
}
