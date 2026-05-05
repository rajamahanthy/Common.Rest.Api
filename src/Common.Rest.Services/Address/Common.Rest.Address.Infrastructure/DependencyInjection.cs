namespace Common.Rest.Address.Infrastructure;

using Common.Rest.Address.Domain.Entities;
using Common.Rest.Address.Infrastructure.Configuration;
using Common.Rest.Address.Infrastructure.Persistence;
using Common.Rest.Shared.Repository;
using Azure.Cosmos;
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

        // ── Register Cosmos Initializer ──────────────────────────────
        services.AddSingleton<ICosmosDbInitializer>(provider =>
        {
            var logger = provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<CosmosDbInitializer>>();
            return new CosmosDbInitializer(cosmosOptions, logger);
        });

        // ── Initialize Cosmos on startup ─────────────────────────────
        services.AddHostedService<CosmosDbInitializationService>();

        // ── Register Cosmos Container (lazy-loaded from initializer) ─
        services.AddScoped(provider =>
        {
            var initializer = provider.GetRequiredService<ICosmosDbInitializer>();
            return initializer.GetContainer();
        });

        // ── Register Repositories ────────────────────────────────────
        //services.AddScoped(typeof(IRepository<>), provider =>
        //{
        //    // Generic factory for IRepository<T>
        //    // For now, only AddressDocumentEntity is supported directly.
        //    throw new NotImplementedException("Use CosmosRepository for AddressDocumentEntity.");
        //});

        services.AddScoped<IRepository<AddressDocumentEntity>>(provider =>
        {
            var container = provider.GetRequiredService<Container>();
            var logger = provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<CosmosRepository>>();
            return new CosmosRepository(container, logger);
        });

        // ── Register Document Synchronizer ──────────────────────────
        services.AddScoped<IAddressDocumentSynchronizer, AddressDocumentSynchronizer>();

        // ── Register Unit of Work ────────────────────────────────────
        services.AddScoped<IUnitOfWork>(provider =>
        {
            var container = provider.GetRequiredService<Container>();
            var logger = provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<CosmosUnitOfWork>>();
            return new CosmosUnitOfWork(container, logger);
        });

        return services;
    }
}
