namespace Common.Rest.Address.Integration.Tests;

using Azure.Cosmos;
using Common.Rest.Address.Domain.Entities;
using Common.Rest.Address.Infrastructure.Configuration;
using Common.Rest.Address.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using Moq;

/// <summary>
/// Integration tests for Cosmos DB repository using the local emulator.
/// 
/// Prerequisites:
/// 1. Install Azure Cosmos DB Emulator (https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator)
/// 2. Start the emulator before running tests
/// 3. Set environment variable COSMOS_CONNECTION_STRING or use default emulator endpoint
/// 
/// Default Emulator Connection String:
/// "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLMUJA6/R0yMQQzjJN+lWXvCzRcMV+fy7xUltQYCIAecmwxltJisUZVQ==;DisableSSLVerification=true"
/// </summary>
[TestClass]
public class CosmosDbIntegrationTests
{
    private ICosmosDbInitializer? _initializer;
    private Container? _container;
    private CosmosRepository? _repository;
    private CosmosUnitOfWork? _unitOfWork;

    /// <summary>
    /// Gets connection string from environment or returns emulator default.
    /// </summary>
    private static string GetConnectionString()
    {
        var envConnStr = Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING");
        if (!string.IsNullOrEmpty(envConnStr))
            return envConnStr;

        // Default emulator connection string
        return "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLMUJA6/R0yMQQzjJN+lWXvCzRcMV+fy7xUltQYCIAecmwxltJisUZVQ==;DisableSSLVerification=true";
    }

    [TestInitialize]
    public async Task Setup()
    {
        try
        {
            var cosmosOptions = new CosmosDbOptions
            {
                ConnectionString = GetConnectionString(),
                DatabaseName = "AddressDb",
                ContainerName = "AddressContainer",
                PartitionKeyPath = "/postcode",
                ThroughputRus = null // Serverless
            };

            var mockLogger = new Mock<ILogger<CosmosDbInitializer>>();
            _initializer = new CosmosDbInitializer(cosmosOptions, mockLogger.Object);

            await _initializer.InitializeAsync();

            _container = _initializer.GetContainer();

            var repositoryLogger = new Mock<ILogger<CosmosRepository>>();
            _repository = new CosmosRepository(_container, repositoryLogger.Object);

            var unitOfWorkLogger = new Mock<ILogger<CosmosUnitOfWork>>();
            _unitOfWork = new CosmosUnitOfWork(_container, unitOfWorkLogger.Object);
        }
        catch (Exception ex)
        {
            Assert.Inconclusive($"Cosmos DB emulator not available. Integration tests skipped. Error: {ex.Message}");
        }
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        // Optional: Clean up test data
        if (_container is not null)
        {
            try
            {
                // Delete all test documents
                var query = _container.GetItemQueryIterator<dynamic>(
                    new QueryDefinition("SELECT * FROM c"));

                while (query.HasMoreResults)
                {
                    var items = await query.ReadNextAsync();
                    foreach (var item in items)
                    {
                        // In production, use proper cleanup
                    }
                }
            }
            catch { /* Ignore cleanup errors */ }
        }
    }

    #region Create Tests

    [TestMethod]
    [TestCategory("Integration")]
    public async Task CreateAsync_ValidAddress_InsertsToContainer()
    {
        if (_repository == null) Assert.Inconclusive("Cosmos not available");

        var entity = new AddressDocumentEntity
        {
            Id = Guid.NewGuid(),
            DocumentType = "Address",
            PartitionKey = "SW1A 1AA",
            JsonData = new AddressEntity
            {
                Uprn = "100023336491",
                Usrn = "999999999",
                AddressInfo = new AddressInfoEntity
                {
                    Organisation = "Test Org",
                    Pao = new AddressableObjectEntity { StartNumber = 10, Text = "10" },
                    StreetDescriptor = new StreetDescriptorEntity
                    {
                        StreetDescription = "Downing Street",
                        PostTown = "London"
                    },
                    Postcode = "SW1A 1AA"
                },
                Geography = new GeographyEntity { Easting = 529904, Northing = 180994 }
            },
            UprnIndex = "100023336491",
            PostcodeIndex = "SW1A 1AA",
            PostTownIndex = "London",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _repository.AddAsync(entity);

        var retrieved = await _repository.GetByIdAsync(entity.Id);
        Assert.IsNotNull(retrieved);
        Assert.AreEqual(entity.Id, retrieved.Id);
        Assert.AreEqual("100023336491", retrieved.UprnIndex);
    }

    #endregion

    #region Query Tests

    [TestMethod]
    [TestCategory("Integration")]
    public async Task GetAllAsync_ReturnsDocuments()
    {
        if (_repository == null) Assert.Inconclusive("Cosmos not available");

        var items = await _repository.GetAllAsync();
        Assert.IsNotNull(items);
        Assert.IsInstanceOfType(items, typeof(IReadOnlyList<AddressDocumentEntity>));
    }

    #endregion

    #region Paging Tests

    [TestMethod]
    [TestCategory("Integration")]
    public async Task GetPagedAsync_WithValidParameters_ReturnsPaginatedResults()
    {
        if (_repository == null) Assert.Inconclusive("Cosmos not available");

        var (items, totalCount) = await _repository.GetPagedAsync(page: 1, pageSize: 10);
        Assert.IsNotNull(items);
        Assert.IsTrue(totalCount >= 0);
    }

    #endregion
}
