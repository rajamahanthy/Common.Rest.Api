namespace Common.Rest.Shared.Tests;

[TestClass]
public class ValidationFilterTests
{
    private static ActionContext CreateActionContext()
    {
        return new ActionContext(
            new DefaultHttpContext(),
            new Microsoft.AspNetCore.Routing.RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
    }

    [TestMethod]
    public void OnActionExecuting_InvalidModelState_ReturnsBadRequest()
    {
        var filter = new ValidationFilter();
        var actionContext = CreateActionContext();
        actionContext.ModelState.AddModelError("Name", "Name is required");
        actionContext.ModelState.AddModelError("Email", "Email is invalid");

        var context = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            new object());

        filter.OnActionExecuting(context);

        Assert.IsInstanceOfType(context.Result, typeof(BadRequestObjectResult));
        var result = (BadRequestObjectResult)context.Result;
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);

        Assert.IsNotNull(result.Value);
    }

    [TestMethod]
    public void OnActionExecuting_ValidModelState_DoesNotSetResult()
    {
        var filter = new ValidationFilter();
        var actionContext = CreateActionContext();

        var context = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            new object());

        filter.OnActionExecuting(context);

        Assert.IsNull(context.Result);
    }

    [TestMethod]
    public void OnActionExecuted_DoesNothing()
    {
        var filter = new ValidationFilter();
        var actionContext = CreateActionContext();

        var context = new ActionExecutedContext(
            actionContext,
            new List<IFilterMetadata>(),
            new object());

        filter.OnActionExecuted(context);

        Assert.IsNull(context.Result);
    }
}

[TestClass]
public class ValidationExtensionsTests
{
    [TestMethod]
    public void AddStandardValidation_ConfiguresApiBehaviorOptions()
    {
        var services = new ServiceCollection();

        var result = services.AddStandardValidation();

        Assert.AreSame(services, result);

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<ApiBehaviorOptions>>();

        Assert.IsNotNull(options.Value.InvalidModelStateResponseFactory);
    }
}
