namespace SurveyData.Infrastructure.Integration.Tests;

/// <summary>
/// Integration tests for Survey Details endpoints.
/// These tests verify operations on survey detail items and their relationships to surveys.
/// </summary>
[TestClass]
public class SurveyDetailsIntegrationTests
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    [TestInitialize]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the normal database and use in-memory for testing
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<SurveyDbContext>));
                    
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<SurveyDbContext>(options =>
                        options.UseInMemoryDatabase($"SurveyDetailsIntegration_{Guid.NewGuid()}"));
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
    public async Task AddDetail_WithInvalidSurveyId_ReturnsNotFound_Async()
    {
        // Arrange
        var invalidSurveyId = Guid.NewGuid();
        var detailRequest = new CreateSurveyDetailRequest
        {
            Description = "Test Detail",
            Area = 10m
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/surveys/{invalidSurveyId}/details", detailRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    // ?? Helper Methods ??????????????????????????????????????????????????????

    private async Task<SurveyDto> CreateTestSurveyAsync(string referenceNumber = "TEST-DETAIL-001")
    {
        var createRequest = new CreateSurveyRequest
        {
            ReferenceNumber = referenceNumber,
            PropertyAddress = "Test Property Address",
            SurveyType = "Valuation",
            SurveyDate = DateTimeOffset.UtcNow.AddDays(-1)
        };

        var response = await _client.PostAsJsonAsync("/api/v1/surveys", createRequest);
        return await response.Content.ReadFromJsonAsync<SurveyDto>() ?? throw new InvalidOperationException("Failed to create test survey");
    }

    private async Task<SurveyDetailDto> AddDetailToSurveyAsync(Guid surveyId, string description = "Test Detail")
    {
        var detailRequest = new CreateSurveyDetailRequest
        {
            Description = description,
            AreaUnit = "sqm",
            Area = 25.75m,
            RatePerUnit = 400m,
            Value = 10300m
        };

        var response = await _client.PostAsJsonAsync($"/api/v1/surveys/{surveyId}/details", detailRequest);
        return await response.Content.ReadFromJsonAsync<SurveyDetailDto>() ?? throw new InvalidOperationException("Failed to add detail");
    }

    private async Task AddDetailsToSurveyAsync(Guid surveyId, int count)
    {
        for (int i = 0; i < count; i++)
        {
            await AddDetailToSurveyAsync(surveyId, $"Detail {i}");
        }
    }
}

/// <summary>
/// Stub classes for request DTOs (these should be in the Application layer)
/// </summary>
public class CreateSurveyDetailRequest
{
    public string Description { get; set; } = string.Empty;
    public string? AreaUnit { get; set; }
    public decimal? Area { get; set; }
    public decimal? RatePerUnit { get; set; }
    public decimal? Value { get; set; }
}

public class UpdateSurveyDetailRequest
{
    public string? Description { get; set; }
    public string? AreaUnit { get; set; }
    public decimal? Area { get; set; }
    public decimal? RatePerUnit { get; set; }
    public decimal? Value { get; set; }
}

public class SurveyDetailDto
{
    public Guid Id { get; set; }
    public Guid SurveyId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? AreaUnit { get; set; }
    public decimal? Area { get; set; }
    public decimal? RatePerUnit { get; set; }
    public decimal? Value { get; set; }
}
