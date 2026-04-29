namespace Common.Rest.Address.Api.Tests.Fixtures;

/// <summary>
/// Generic base class for controller unit tests with reusable setup and helper methods.
/// </summary>
[TestClass]
public abstract class ControllerTestBase
{
    protected Mock<IAddressService> MockAddressService { get; private set; } = null!;
    protected AddressController Controller { get; private set; } = null!;

    [TestInitialize]
    public void Initialize()
    {
        MockAddressService = new();
        Controller = new AddressController(MockAddressService.Object);
        
        var httpContext = CreateHttpContext();
        Controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        
        OnTestInitialize();
    }

    /// <summary>
    /// Override to add test-specific initialization logic.
    /// </summary>
    protected virtual void OnTestInitialize() { }

    /// <summary>
    /// Creates a real HttpContext with CorrelationId for test usage.
    /// Uses DefaultHttpContext to allow proper User claim setup.
    /// </summary>
    private static HttpContext CreateHttpContext()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Items["CorrelationId"] = "test-correlation-id";
        return httpContext;
    }

    /// <summary>
    /// Sets up controller user claims.
    /// </summary>
    protected void SetUser(string userId)
    {
        var claims = new List<Claim> { new Claim("sub", userId) };
        var identity = new ClaimsIdentity(claims, "test");
        Controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
    }

    /// <summary>
    /// Clears the controller user claims (simulates unauthenticated request).
    /// </summary>
    protected void ClearUser()
    {
        Controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();
    }
}
