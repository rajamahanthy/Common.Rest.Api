
namespace Address.Integration.Tests;

/// <summary>
/// Integration tests for Address API endpoints.
/// These tests verify the full request/response lifecycle for the Addresses controller.
/// </summary>
[TestClass]
public class AddressesControllerIntegrationTests
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDatabaseName = null!;

    [TestInitialize]
    public void Setup()
    {
        _testDatabaseName = $"AddressIntegration_{Guid.NewGuid()}";
        
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the normal database and use in-memory for testing
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AddressDbContext>));
                    
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<AddressDbContext>(options =>
                        options.UseInMemoryDatabase(_testDatabaseName));
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
    public async Task GetPaged_WithNoFilter_ReturnsAddressesAsync()
    {
        // Arrange
        var expectedCount = 2;
        await SeedTestAddressesAsync(expectedCount);

        // Act
        var response = await _client.GetAsync("/api/v1/addresses");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<PagedApiResponse<AddressDto>>();
        content.Data.Should().HaveCount(expectedCount);
        content.Page.Should().Be(1);
        content.PageSize.Should().Be(10);
        content.TotalCount.Should().Be(expectedCount);
    }

    private async Task SeedTestAddressesAsync(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var createRequest = new CreateAddressRequest(
                $"1000{i}",
                i == 0 ? "10 Downing Street, London" : $"{i} Test Street, London",
                i == 0 ? "Prime Minister's Residence" : $"Test Building {i}",
                i.ToString(),
                i == 0 ? "Downing Street" : "Test Street",
                "Test Locality",
                "London",
                $"W1A {i:0000}",
                "United Kingdom"
            );

            var response = await _client.PostAsJsonAsync("/api/v1/addresses", createRequest);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Failed to seed address: {response.StatusCode} - {errorContent}");
            }
        }
    }
}

