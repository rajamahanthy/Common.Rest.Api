namespace Common.Rest.Shared.Tests;

[TestClass]
public class ServiceCollectionExtensionsTests
{
    [TestMethod]
    public void AddStandardResilience_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var result = services.AddStandardResilience(config);

        Assert.AreSame(services, result);
    }

    [TestMethod]
    public void AddStandardApiVersioning_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddStandardApiVersioning();

        Assert.AreSame(services, result);
    }

    [TestMethod]
    public void AddStandardOpenTelemetry_WithoutConnectionString_ReturnsServices()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder().Build();

        var result = services.AddStandardOpenTelemetry(config);

        Assert.AreSame(services, result);
    }

    [TestMethod]
    public void AddStandardAuth_DevelopmentEnvironment_AddsTestAuth()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder().Build();
        var env = new Mock<IWebHostEnvironment>();
        env.Setup(e => e.EnvironmentName).Returns("Development");

        var result = services.AddStandardAuth(config, env.Object);

        Assert.AreSame(services, result);
    }

    [TestMethod]
    public void AddStandardAuth_ProductionEnvironment_AddsAzureAdAuth()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder().Build();
        var env = new Mock<IWebHostEnvironment>();
        env.Setup(e => e.EnvironmentName).Returns("Production");

        var result = services.AddStandardAuth(config, env.Object);

        Assert.AreSame(services, result);
    }
}
