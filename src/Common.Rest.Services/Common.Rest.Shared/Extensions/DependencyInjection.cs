namespace Common.Rest.Shared.Extensions;

using Common.Rest.Shared.Persistence.Cosmos;
using Common.Rest.Shared.Repository;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CosmosDbOptions>(configuration.GetSection(CosmosDbOptions.SectionName));

        var cosmosOptions = configuration.GetSection(CosmosDbOptions.SectionName).Get<CosmosDbOptions>()
            ?? throw new InvalidOperationException($"Configuration section '{CosmosDbOptions.SectionName}' is required.");

        services.AddSingleton(new CosmosClient(cosmosOptions.ConnectionString));

        services.AddScoped(provider =>
        {
            var cosmosClient = provider.GetRequiredService<CosmosClient>();
            var database = cosmosClient.GetDatabase(cosmosOptions.DatabaseName);
            return database.GetContainer(cosmosOptions.ContainerName);
        });

        services.AddScoped<IUnitOfWork>(provider =>
        {
            var container = provider.GetRequiredService<Container>();
            var logger = provider.GetRequiredService<ILogger<UnitOfWork>>();
            return new UnitOfWork(container, logger);
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
}
