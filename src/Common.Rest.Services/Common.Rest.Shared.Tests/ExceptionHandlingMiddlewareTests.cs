namespace Common.Rest.Shared.Tests;

[TestClass]
public class ExceptionHandlingMiddlewareTests
{
    private static Mock<IHostEnvironment> CreateMockEnvironment(bool isDevelopment = false)
    {
        var env = new Mock<IHostEnvironment>();
        env.Setup(e => e.EnvironmentName).Returns(isDevelopment ? "Development" : "Production");
        return env;
    }

    private static ExceptionHandlingMiddleware CreateMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
        => new(next, logger, env);

    private static DefaultHttpContext CreateContext(string? correlationId = null)
    {
        var context = new DefaultHttpContext();
        context.Items["CorrelationId"] = correlationId;
        context.Response.Body = new MemoryStream();
        return context;
    }

    [TestMethod]
    public async Task InvokeAsync_NoException_CallsNext()
    {
        var nextCalled = false;
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var env = CreateMockEnvironment();
        var context = CreateContext();

        var middleware = CreateMiddleware(
            ctx => { nextCalled = true; return Task.CompletedTask; },
            loggerMock.Object,
            env.Object);

        await middleware.InvokeAsync(context);

        Assert.IsTrue(nextCalled);
    }

    [TestMethod]
    public async Task InvokeAsync_ValidationException_ReturnsBadRequest()
    {
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var env = CreateMockEnvironment();
        var context = CreateContext("corr-1");

        var middleware = CreateMiddleware(
            _ => throw new ValidationException("Invalid input", ["Field X is wrong"]),
            loggerMock.Object,
            env.Object);

        await middleware.InvokeAsync(context);

        Assert.AreEqual((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        Assert.AreEqual("application/json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<JsonElement>(body);

        Assert.IsFalse(response.GetProperty("success").GetBoolean());
    }

    [TestMethod]
    public async Task InvokeAsync_NotFoundException_ReturnsNotFound()
    {
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var env = CreateMockEnvironment();
        var context = CreateContext("corr-2");

        var middleware = CreateMiddleware(
            _ => throw new NotFoundException("Resource", 42),
            loggerMock.Object,
            env.Object);

        await middleware.InvokeAsync(context);

        Assert.AreEqual((int)HttpStatusCode.NotFound, context.Response.StatusCode);
    }

    [TestMethod]
    public async Task InvokeAsync_ConflictException_ReturnsConflict()
    {
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var env = CreateMockEnvironment();
        var context = CreateContext("corr-3");

        var middleware = CreateMiddleware(
            _ => throw new ConflictException("Already exists"),
            loggerMock.Object,
            env.Object);

        await middleware.InvokeAsync(context);

        Assert.AreEqual((int)HttpStatusCode.Conflict, context.Response.StatusCode);
    }

    [TestMethod]
    public async Task InvokeAsync_ForbiddenException_ReturnsForbidden()
    {
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var env = CreateMockEnvironment();
        var context = CreateContext("corr-4");

        var middleware = CreateMiddleware(
            _ => throw new ForbiddenException("No access"),
            loggerMock.Object,
            env.Object);

        await middleware.InvokeAsync(context);

        Assert.AreEqual((int)HttpStatusCode.Forbidden, context.Response.StatusCode);
    }

    [TestMethod]
    public async Task InvokeAsync_GenericAppException_UsesItsStatusCode()
    {
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var env = CreateMockEnvironment();
        var context = CreateContext("corr-5");

        var middleware = CreateMiddleware(
            _ => throw new AppException("Custom error", HttpStatusCode.TooManyRequests),
            loggerMock.Object,
            env.Object);

        await middleware.InvokeAsync(context);

        Assert.AreEqual(429, context.Response.StatusCode);
    }

    [TestMethod]
    public async Task InvokeAsync_UnhandledException_ReturnsInternalServerError()
    {
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var env = CreateMockEnvironment();
        var context = CreateContext("corr-6");

        var middleware = CreateMiddleware(
            _ => throw new InvalidOperationException("Unexpected"),
            loggerMock.Object,
            env.Object);

        await middleware.InvokeAsync(context);

        Assert.AreEqual((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<JsonElement>(body);

        Assert.AreEqual("An unexpected error occurred.", response.GetProperty("message").GetString());
    }

    [TestMethod]
    public async Task InvokeAsync_DevelopmentMode_ReturnsExceptionMessage()
    {
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var env = CreateMockEnvironment(isDevelopment: true);
        var context = CreateContext("corr-dev");

        var middleware = CreateMiddleware(
            _ => throw new InvalidOperationException("Detailed error info"),
            loggerMock.Object,
            env.Object);

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<JsonElement>(body);

        Assert.AreEqual("Detailed error info", response.GetProperty("message").GetString());
    }

    [TestMethod]
    public async Task InvokeAsync_ResponseHasCorrelationId()
    {
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var env = CreateMockEnvironment();
        var context = CreateContext("corr-789");

        var middleware = CreateMiddleware(
            _ => throw new NotFoundException("Item", 1),
            loggerMock.Object,
            env.Object);

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<JsonElement>(body);

        Assert.AreEqual("corr-789", response.GetProperty("correlationId").GetString());
    }
}
