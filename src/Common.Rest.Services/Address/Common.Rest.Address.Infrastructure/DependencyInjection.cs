namespace Common.Rest.Address.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string dbConnStrKey = "AddressDb";
        return DependencyInjectionBase.AddInfra<AddressEntity>(services, configuration, dbConnStrKey);
    }
}

