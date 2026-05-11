namespace Common.Rest.Shared.Tests;

[TestClass]
public class CorrelationIdMiddlewareTests
{
    [TestMethod]
    public async Task InvokeAsync_WhenHeaderPresent_UsesExistingCorrelationId()
    {
        var existingId = "existing-corr-id";
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Correlation-ID"] = existingId;

        var loggerMock = new Mock<ILogger<CorrelationIdMiddleware>>();
        var nextCalled = false;

        var middleware = new CorrelationIdMiddleware(
            ctx =>
            {
                nextCalled = true;
                Assert.AreEqual(existingId, ctx.Items["CorrelationId"]);
                return Task.CompletedTask;
            },
            loggerMock.Object);

        await middleware.InvokeAsync(context);

        Assert.IsTrue(nextCalled);
        Assert.AreEqual(existingId, context.Items["CorrelationId"]);
    }

    [TestMethod]
    public async Task InvokeAsync_WhenHeaderMissing_GeneratesNewCorrelationId()
    {
        var context = new DefaultHttpContext();
        var loggerMock = new Mock<ILogger<CorrelationIdMiddleware>>();
        string? generatedId = null;

        var middleware = new CorrelationIdMiddleware(
            ctx =>
            {
                generatedId = ctx.Items["CorrelationId"]?.ToString();
                return Task.CompletedTask;
            },
            loggerMock.Object);

        await middleware.InvokeAsync(context);

        Assert.IsNotNull(generatedId);
        Assert.AreNotEqual("", generatedId);
        Assert.AreEqual(generatedId, context.Items["CorrelationId"]);
    }

    [TestMethod]
    public async Task InvokeAsync_SetsResponseHeaderOnStarting()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Correlation-ID"] = "test-corr-id";
        var loggerMock = new Mock<ILogger<CorrelationIdMiddleware>>();

        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask, loggerMock.Object);
        await middleware.InvokeAsync(context);

        Assert.AreEqual("test-corr-id", context.Items["CorrelationId"]);
    }

    [TestMethod]
    public async Task InvokeAsync_LogsCorrelationIdInScope()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Correlation-ID"] = "log-test-id";
        var loggerMock = new Mock<ILogger<CorrelationIdMiddleware>>();

        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask, loggerMock.Object);
        await middleware.InvokeAsync(context);

        Assert.AreEqual("log-test-id", context.Items["CorrelationId"]);
    }
}
