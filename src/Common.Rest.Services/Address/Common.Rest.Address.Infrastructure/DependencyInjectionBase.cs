namespace Common.Rest.Address.Infrastructure
{
    public static class DependencyInjectionBase
    {

        public static IServiceCollection AddInfra<T>(IServiceCollection services, IConfiguration configuration, string dbConnStrKey)
            where T : class
        {
            // ── DbContext ───────────────────────────────────────────────────
            services.AddDbContext<DocumentDbContext<T>>(options =>
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

            // ── Register DbContext base class for repository injection ────────
            services.AddScoped<DbContext>(provider => provider.GetRequiredService<DocumentDbContext<T>>());

            // ── Repositories (Generic) ──────────────────────────────────────
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

            // ── Unit of Work (Generic) ───────────────────────────────────────
            services.AddScoped<IUnitOfWork, UnitOfWork>(provider =>
                new UnitOfWork(provider.GetRequiredService<DocumentDbContext<T>>()));

            return services;
        }
    }
}