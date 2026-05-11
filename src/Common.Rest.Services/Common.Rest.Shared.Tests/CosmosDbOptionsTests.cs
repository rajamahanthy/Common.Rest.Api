namespace Common.Rest.Shared.Tests;

[TestClass]
public class CosmosDbOptionsTests
{
    [TestMethod]
    public void SectionName_IsCorrect()
    {
        Assert.AreEqual("CosmosDb", CosmosDbOptions.SectionName);
    }

    [TestMethod]
    public void DefaultValues_AreCorrect()
    {
        var options = new CosmosDbOptions
        {
            ConnectionString = "conn-str",
            DatabaseName = "db",
            ContainerName = "container"
        };

        Assert.AreEqual("/jsonData/AddressInfo/Postcode", options.PartitionKeyPath);
        Assert.IsNull(options.ThroughputRus);
        Assert.IsTrue(options.EnableConnectionSharing);
        Assert.AreEqual(9, options.MaxRetryAttemptsOnThrottledRequests);
        Assert.AreEqual(30, options.MaxRetryWaitTimeInSeconds);
    }

    [TestMethod]
    public void RequiredProperties_CanBeSet()
    {
        var options = new CosmosDbOptions
        {
            ConnectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6y...==",
            DatabaseName = "AddressDb",
            ContainerName = "AddressContainer2"
        };

        Assert.AreEqual("AccountEndpoint=https://localhost:8081/;AccountKey=C2y6y...==", options.ConnectionString);
        Assert.AreEqual("AddressDb", options.DatabaseName);
        Assert.AreEqual("AddressContainer2", options.ContainerName);
    }

    [TestMethod]
    public void OptionalProperties_CanBeOverridden()
    {
        var options = new CosmosDbOptions
        {
            ConnectionString = "conn",
            DatabaseName = "db",
            ContainerName = "c",
            PartitionKeyPath = "/custom/path",
            ThroughputRus = 400,
            EnableConnectionSharing = false,
            MaxRetryAttemptsOnThrottledRequests = 5,
            MaxRetryWaitTimeInSeconds = 15
        };

        Assert.AreEqual("/custom/path", options.PartitionKeyPath);
        Assert.AreEqual(400, options.ThroughputRus);
        Assert.IsFalse(options.EnableConnectionSharing);
        Assert.AreEqual(5, options.MaxRetryAttemptsOnThrottledRequests);
        Assert.AreEqual(15, options.MaxRetryWaitTimeInSeconds);
    }
}
