
namespace SurveyData.Integration.Tests;

/// <summary>
/// Integration tests for Survey API endpoints.
/// These tests verify the full request/response lifecycle for the Surveys controller.
/// </summary>
[TestClass]
public class SurveysControllerIntegrationTests
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    [TestInitialize]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                // Override configuration to use InMemory database
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        { "ConnectionStrings:SurveyDb", "InMemory" }
                    });
                });
            });

        _client = _factory.CreateClient();
        
        // Add Bearer token for Authorization
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer test-token");
    }

    [TestCleanup]
    public void Teardown()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    [TestMethod]
    public async Task GetById_WithInvalidId_ReturnsNotFoundAsync()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/surveys/{invalidId}");

        // Assert
        Assert.Equals(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task GetByReference_WithInvalidReference_ReturnsNotFoundAsync()
    {
        // Arrange
        var invalidReference = "INVALID-REF-12345";

        // Act
        var response = await _client.GetAsync($"/api/v1/surveys/reference/{invalidReference}");

        // Assert
        Assert.Equals(HttpStatusCode.NotFound, response.StatusCode);
    }    
}