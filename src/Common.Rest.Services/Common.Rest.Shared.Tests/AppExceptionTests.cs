namespace Common.Rest.Shared.Tests;

[TestClass]
public class AppExceptionTests
{
    [TestMethod]
    public void AppException_DefaultStatusCode_IsInternalServerError()
    {
        var ex = new AppException("test");
        Assert.AreEqual(HttpStatusCode.InternalServerError, ex.StatusCode);
        Assert.AreEqual("test", ex.Message);
    }

    [TestMethod]
    public void AppException_CustomStatusCode_SetsCorrectly()
    {
        var ex = new AppException("bad request", HttpStatusCode.BadRequest);
        Assert.AreEqual(HttpStatusCode.BadRequest, ex.StatusCode);
    }

    [TestMethod]
    public void AppException_WithInnerException_SetsInner()
    {
        var inner = new InvalidOperationException("inner");
        var ex = new AppException("outer", HttpStatusCode.InternalServerError, inner);
        Assert.AreSame(inner, ex.InnerException);
    }

    [TestMethod]
    public void NotFoundException_FormatsMessage()
    {
        var ex = new NotFoundException("User", 42);
        Assert.AreEqual("User with key '42' was not found.", ex.Message);
        Assert.AreEqual(HttpStatusCode.NotFound, ex.StatusCode);
    }

    [TestMethod]
    public void ConflictException_SetsMessageAndStatusCode()
    {
        var ex = new ConflictException("Resource already exists");
        Assert.AreEqual("Resource already exists", ex.Message);
        Assert.AreEqual(HttpStatusCode.Conflict, ex.StatusCode);
    }

    [TestMethod]
    public void ValidationException_DefaultErrors_IsEmpty()
    {
        var ex = new ValidationException("invalid");
        Assert.AreEqual("invalid", ex.Message);
        Assert.AreEqual(HttpStatusCode.BadRequest, ex.StatusCode);
        Assert.IsNotNull(ex.ValidationErrors);
        Assert.AreEqual(0, ex.ValidationErrors.Count);
    }

    [TestMethod]
    public void ValidationException_WithErrors_StoresThem()
    {
        var errors = new[] { "Name is required", "Email is invalid" };
        var ex = new ValidationException("Validation failed", errors);
        Assert.AreEqual(2, ex.ValidationErrors.Count);
        Assert.AreEqual("Name is required", ex.ValidationErrors[0]);
    }

    [TestMethod]
    public void ForbiddenException_DefaultMessage()
    {
        var ex = new ForbiddenException();
        Assert.AreEqual("You do not have permission to perform this action.", ex.Message);
        Assert.AreEqual(HttpStatusCode.Forbidden, ex.StatusCode);
    }

    [TestMethod]
    public void ForbiddenException_CustomMessage()
    {
        var ex = new ForbiddenException("Access denied");
        Assert.AreEqual("Access denied", ex.Message);
    }
}
