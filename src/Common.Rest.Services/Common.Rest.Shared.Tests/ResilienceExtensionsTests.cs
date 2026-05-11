namespace Common.Rest.Shared.Tests;

[TestClass]
public class ResilienceExtensionsTests
{
    [TestMethod]
    public void AddStandardInternalResilience_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Resilience:MaxRetryAttempts"] = "3",
                ["Resilience:DelaySeconds"] = "1",
                ["Resilience:BackoffType"] = "Exponential",
                ["Resilience:UseJitter"] = "true"
            })
            .Build();

        var result = services.AddStandardInternalResilience(config);

        Assert.AreSame(services, result);
    }

    [TestMethod]
    public void AddStandardInternalResilience_WithoutConfig_UsesDefaults()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder().Build();

        var result = services.AddStandardInternalResilience(config);

        Assert.AreSame(services, result);
    }
}
