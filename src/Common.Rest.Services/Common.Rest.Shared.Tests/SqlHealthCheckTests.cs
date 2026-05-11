namespace Common.Rest.Shared.Tests;

[TestClass]
public class SqlHealthCheckTests
{
    [TestMethod]
    public async Task CheckHealthAsync_ReturnsHealthy()
    {
        var healthCheck = new SqlHealthCheck("Server=localhost;Database=test;");
        var context = new HealthCheckContext();

        var result = await healthCheck.CheckHealthAsync(context);

        Assert.AreEqual(HealthCheckResult.Healthy().Status, result.Status);
        Assert.AreEqual("SQL Database is reachable.", result.Description);
    }
}
