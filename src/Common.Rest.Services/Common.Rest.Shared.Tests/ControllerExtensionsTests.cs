namespace Common.Rest.Shared.Tests;

[TestClass]
public class ControllerExtensionsTests
{
    [TestMethod]
    public void GetCorrelationId_WhenPresent_ReturnsCorrelationId()
    {
        var controller = new TestController();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controller.HttpContext.Items["CorrelationId"] = "test-corr-id";

        var result = controller.GetCorrelationId();

        Assert.AreEqual("test-corr-id", result);
    }

    [TestMethod]
    public void GetCorrelationId_WhenNotPresent_ReturnsNull()
    {
        var controller = new TestController();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var result = controller.GetCorrelationId();

        Assert.IsNull(result);
    }

    private class TestController : ControllerBase { }
}
