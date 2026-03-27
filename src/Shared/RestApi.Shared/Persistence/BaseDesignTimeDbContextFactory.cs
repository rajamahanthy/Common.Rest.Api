using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RestApi.Shared.Persistence;

/// <summary>
/// A generic base class for EF Core design-time DbContext factories.
/// This centralizes the logic for reading connection strings from appsettings.json
/// across all microservices.
/// </summary>
/// <typeparam name="TContext">The type of the DbContext to create.</typeparam>
public abstract class BaseDesignTimeDbContextFactory<TContext> : IDesignTimeDbContextFactory<TContext>
    where TContext : DbContext
{
    private readonly string _connectionStringName;

    protected BaseDesignTimeDbContextFactory(string connectionStringName = "DefaultConnection")
    {
        _connectionStringName = connectionStringName;
    }

    public TContext CreateDbContext(string[] args)
    {
        // Try to find the appsettings.json in the current directory (startup project folder)
        var basePath = Directory.GetCurrentDirectory();
        
        // If run from solution root, we might need to find it differently, 
        // but dotnet ef tools usually set CWD to the project folder.
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString(_connectionStringName);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Connection string '{_connectionStringName}' not found in {basePath}/appsettings.json");
        }

        var optionsBuilder = new DbContextOptionsBuilder<TContext>();
        
        // Use SQL Server by default as it's the standard for this project's migrations
        optionsBuilder.UseSqlServer(connectionString);
        
        // Suppress common warnings that block design-time tools (like database update)
        optionsBuilder.ConfigureWarnings(w => w.Ignore(
            Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning,
            new EventId(10103)));

        return CreateContext(optionsBuilder.Options);
    }

    /// <summary>
    /// Factory method to create the specific DbContext instance.
    /// </summary>
    protected abstract TContext CreateContext(DbContextOptions<TContext> options);
}
