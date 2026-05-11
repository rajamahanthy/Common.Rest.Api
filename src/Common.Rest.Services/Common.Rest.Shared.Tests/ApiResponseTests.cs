namespace Common.Rest.Shared.Tests;

[TestClass]
public class ApiResponseTests
{
    [TestMethod]
    public void Ok_CreatesSuccessfulResponse()
    {
        var data = "test-data";
        var response = ApiResponse<string>.Ok(data, "corr-123");

        Assert.IsTrue(response.Success);
        Assert.AreEqual(data, response.Data);
        Assert.AreEqual("corr-123", response.CorrelationId);
        Assert.IsNull(response.Message);
        Assert.IsNull(response.Errors);
    }

    [TestMethod]
    public void Ok_WithoutCorrelationId_CorrelationIdIsNull()
    {
        var response = ApiResponse<string>.Ok("data");
        Assert.IsNull(response.CorrelationId);
    }

    [TestMethod]
    public void Fail_CreatesFailureResponse()
    {
        var errors = new[] { "error1", "error2" };
        var response = ApiResponse<string>.Fail("Something went wrong", errors, "corr-456");

        Assert.IsFalse(response.Success);
        Assert.AreEqual("Something went wrong", response.Message);
        Assert.AreEqual(2, response.Errors!.Count);
        Assert.AreEqual("corr-456", response.CorrelationId);
        Assert.IsNull(response.Data);
    }

    [TestMethod]
    public void Fail_WithoutErrors_ErrorsIsNull()
    {
        var response = ApiResponse<string>.Fail("fail");
        Assert.IsNull(response.Errors);
        Assert.IsNull(response.CorrelationId);
    }

    [TestMethod]
    public void Timestamp_IsSetToUtcNow()
    {
        var before = DateTimeOffset.UtcNow.AddSeconds(-1);
        var response = ApiResponse<string>.Ok("data");
        var after = DateTimeOffset.UtcNow.AddSeconds(1);

        Assert.IsTrue(response.Timestamp >= before);
        Assert.IsTrue(response.Timestamp <= after);
    }
}

[TestClass]
public class PagedApiResponseTests
{
    [TestMethod]
    public void ParameterizedConstructor_SetsProperties()
    {
        var data = new[] { "a", "b", "c" };
        var response = new PagedApiResponse<string>(data, 2, 10, 25, "corr-789");

        Assert.IsTrue(response.Success);
        Assert.AreEqual(3, response.Data.Count);
        Assert.AreEqual(2, response.Page);
        Assert.AreEqual(10, response.PageSize);
        Assert.AreEqual(25, response.TotalCount);
        Assert.AreEqual("corr-789", response.CorrelationId);
    }

    [TestMethod]
    public void ParameterlessConstructor_UsesDefaults()
    {
        var response = new PagedApiResponse<string>();

        Assert.IsTrue(response.Success);
        Assert.IsNotNull(response.Data);
        Assert.AreEqual(0, response.Data.Count);
        Assert.AreEqual(0, response.Page);
        Assert.AreEqual(0, response.PageSize);
        Assert.AreEqual(0, response.TotalCount);
        Assert.IsNull(response.CorrelationId);
    }

    [TestMethod]
    public void TotalPages_CalculatesCorrectly()
    {
        var response = new PagedApiResponse<string>([], 1, 10, 25);
        Assert.AreEqual(3, response.TotalPages);
    }

    [TestMethod]
    public void TotalPages_WhenPageSizeZero_ReturnsZero()
    {
        var response = new PagedApiResponse<string>([], 1, 0, 25);
        Assert.AreEqual(0, response.TotalPages);
    }

    [TestMethod]
    public void TotalPages_ExactDivision_ReturnsCorrectCount()
    {
        var response = new PagedApiResponse<string>([], 1, 10, 30);
        Assert.AreEqual(3, response.TotalPages);
    }
}

[TestClass]
public class PaginationResultTests
{
    [TestMethod]
    public void Constructor_SetsDataAndTotalCount()
    {
        var data = new[] { 1, 2, 3 };
        var result = new PaginationResult<int>(data, 100);

        Assert.AreEqual(3, result.Data.Count);
        Assert.AreEqual(100, result.TotalCount);
    }

    [TestMethod]
    public void Deconstruct_ReturnsCorrectValues()
    {
        var result = new PaginationResult<string>(["x", "y"], 50);
        var (data, total) = result;

        Assert.AreEqual(2, data.Count);
        Assert.AreEqual(50, total);
    }
}
