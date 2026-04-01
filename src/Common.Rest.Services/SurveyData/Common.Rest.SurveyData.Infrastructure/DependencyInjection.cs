namespace Common.Rest.SurveyData.Infrastructure;

using Common.Rest.SurveyData.Infrastructure.Persistence;
using Common.Rest.SurveyData.Application.Interfaces;
using Common.Rest.Shared.Repository;
using Common.Rest.Shared.Resilience;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// DI extension methods for Infrastructure layer registration.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register EF Core
        services.AddDbContext<SurveyDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("SurveyDb");
            
            if (string.IsNullOrEmpty(connectionString) || connectionString.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
            {
                options.UseInMemoryDatabase("SurveyDb");
            }
            else
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                    sqlOptions.CommandTimeout(30);
                });
            }

            options.ConfigureWarnings(w => w.Default(WarningBehavior.Log));
        });

        // Register repositories
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<ISurveyRepository, SurveyRepository>();

        // Register Unit of Work using shared generic implementation
        services.AddScoped<IUnitOfWork, UnitOfWork>(provider => 
            new UnitOfWork(provider.GetRequiredService<SurveyDbContext>()));

        return services;
    }
}
