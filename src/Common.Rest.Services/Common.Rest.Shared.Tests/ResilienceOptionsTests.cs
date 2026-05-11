namespace Common.Rest.Shared.Tests;

[TestClass]
public class ResilienceOptionsTests
{
    [TestMethod]
    public void DefaultValues_AreCorrect()
    {
        var options = new ResilienceOptions();

        Assert.AreEqual(3, options.MaxRetryAttempts);
        Assert.AreEqual(1.0, options.DelaySeconds);
        Assert.AreEqual("Exponential", options.BackoffType);
        Assert.IsTrue(options.UseJitter);
    }

    [TestMethod]
    public void CanSetAllProperties()
    {
        var options = new ResilienceOptions
        {
            MaxRetryAttempts = 5,
            DelaySeconds = 2.5,
            BackoffType = "Linear",
            UseJitter = false
        };

        Assert.AreEqual(5, options.MaxRetryAttempts);
        Assert.AreEqual(2.5, options.DelaySeconds);
        Assert.AreEqual("Linear", options.BackoffType);
        Assert.IsFalse(options.UseJitter);
    }

    [TestMethod]
    public void SectionName_IsCorrect()
    {
        Assert.AreEqual("Resilience", ResilienceOptions.SectionName);
    }
}
