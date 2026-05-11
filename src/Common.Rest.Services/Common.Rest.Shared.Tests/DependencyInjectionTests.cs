namespace Common.Rest.Shared.Tests;

[TestClass]
public class DependencyInjectionTests
{
    [TestMethod]
    public void AddInfrastructure_WithoutCosmosConfig_Throws()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder().Build();

        Assert.ThrowsException<InvalidOperationException>(() =>
            services.AddInfrastructure(config));
    }

    [TestMethod]
    public void AddInfrastructure_WithCosmosConfig_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CosmosDb:ConnectionString"] = "AccountEndpoint=https://localhost:8081/;AccountKey=j2sDDSjUiwI+BxAkhr7VNvmihfGpDw5kXguifnH0jzw=",
                ["CosmosDb:DatabaseName"] = "TestDb",
                ["CosmosDb:ContainerName"] = "TestContainer"
            })
            .Build();

        var result = services.AddInfrastructure(config);

        Assert.AreSame(services, result);
    }

    [TestMethod]
    public void AddInfrastructure_RegistersCosmosDbOptions()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CosmosDb:ConnectionString"] = "AccountEndpoint=https://localhost:8081/;AccountKey=j2sDDSjUiwI+BxAkhr7VNvmihfGpDw5kXguifnH0jzw=",
                ["CosmosDb:DatabaseName"] = "db",
                ["CosmosDb:ContainerName"] = "c"
            })
            .Build();

        var result = services.AddInfrastructure(config);

        Assert.AreSame(services, result);

        var optionsDescriptor = services.FirstOrDefault(s =>
            s.ServiceType == typeof(Microsoft.Extensions.Options.IConfigureOptions<CosmosDbOptions>));
        Assert.IsNotNull(optionsDescriptor);

        var cosmosClientDescriptor = services.FirstOrDefault(s =>
            s.ServiceType == typeof(CosmosClient));
        Assert.IsNotNull(cosmosClientDescriptor);
    }
}
