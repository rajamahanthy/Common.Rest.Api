
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
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [TestMethod]
    public async Task GetByReference_WithInvalidReference_ReturnsNotFoundAsync()
    {
        // Arrange
        var invalidReference = "INVALID-REF-12345";

        // Act
        var response = await _client.GetAsync($"/api/v1/surveys/reference/{invalidReference}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<SurveyDto> CreateTestSurveyAsync(string referenceNumber = "TEST-REF-001")
    {
        var createRequest = new CreateSurveyRequest
        {
            ReferenceNumber = referenceNumber,
            PropertyAddress = "10 Downing Street, London",
            PostCode = "SW1A 2AA",
            LocalAuthority = "Westminster",
            SurveyType = "Valuation",
            SurveyDate = DateTimeOffset.UtcNow.AddDays(-1),
            Surveyor = "Test Surveyor",
            Notes = "Integration test survey",
            AssessedValue = 500000m,
            FloorArea = 120.50m,
            FloorAreaUnit = "sqm",
            PropertyType = "Residential",
            PropertySubType = "Terraced"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/surveys", createRequest);
        return await response.Content.ReadFromJsonAsync<SurveyDto>() ?? throw new InvalidOperationException("Failed to create test survey");
    }
}