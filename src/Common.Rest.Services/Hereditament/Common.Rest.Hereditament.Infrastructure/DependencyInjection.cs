namespace Common.Rest.Hereditament.Infrastructure;

using Common.Rest.Hereditament.Domain.Entities;
using Common.Rest.Hereditament.Infrastructure.Persistence;
using Common.Rest.Shared.Repository;
using Common.Rest.Shared.Persistence.Cosmos;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // ── Bind Cosmos Configuration ────────────────────────────────
        services.Configure<CosmosDbOptions>(configuration.GetSection(CosmosDbOptions.SectionName));

        var cosmosOptions = configuration.GetSection(CosmosDbOptions.SectionName).Get<CosmosDbOptions>()
            ?? throw new InvalidOperationException($"Configuration section '{CosmosDbOptions.SectionName}' is required.");

        // ── Register Cosmos Client ───────────────────────────────────
        services.AddSingleton(new CosmosClient(cosmosOptions.ConnectionString));

        // ── Register Cosmos Container ────────────────────────────────
        services.AddScoped(provider =>
        {
            var cosmosClient = provider.GetRequiredService<CosmosClient>();
            var database = cosmosClient.GetDatabase(cosmosOptions.DatabaseName);
            return database.GetContainer(cosmosOptions.ContainerName);
        });

        // ── Register Unit of Work ────────────────────────────────────
        services.AddScoped<IUnitOfWork>(provider =>
        {
            var container = provider.GetRequiredService<Container>();
            var logger = provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<CosmosUnitOfWork>>();
            return new CosmosUnitOfWork(container, logger);
        });

        // ── Register Repository ──────────────────────────────────────
        services.AddScoped<IRepository< DocumentEntity<HereditamentEntity>>>(provider =>
        {
            var container = provider.GetRequiredService<Container>();
            var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
            var logger = provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<CosmosRepository>>();
            return new CosmosRepository(container, logger, unitOfWork);
        });

        return services;
    }
}



