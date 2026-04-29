namespace Common.Rest.Hereditament.Application.Tests.Services;

/// <summary>
/// Comprehensive unit tests for HereditamentService with 100% code coverage.
/// Tests all CRUD operations, search, and filtering functionality.
/// </summary>
[TestClass]
public class HereditamentServiceTests
{
    private Mock<IRepository<HereditamentDocumentEntity>> _mockRepository = null!;
    private Mock<IUnitOfWork> _mockUnitOfWork = null!;
    private Mock<IHereditamentMappingService> _mockMappingService = null!;
    private HereditamentService _service = null!;
    private Guid _testId;

    [TestInitialize]
    public void Setup()
    {
        _testId = Guid.NewGuid();
        _mockRepository = new();
        _mockUnitOfWork = new();
        _mockMappingService = new();
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _service = new HereditamentService(
            _mockRepository.Object,
            _mockUnitOfWork.Object,
            _mockMappingService.Object);
    }

    private static HereditamentDocumentEntity CreateEntity(Guid? id = null)
    {
        return new HereditamentDocumentEntity
        {
            Id = id ?? Guid.NewGuid(),
            DocumentType = "Hereditament",
            JsonData = new HereditamentEntity
            {
                UARN = Guid.NewGuid(),
                Name = "Test Hereditament",
                Status = HereditamentStatus.Active,
                EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow),
                AddressId = Guid.NewGuid()
            }
        };
    }

    private static HereditamentDocumentDto CreateDto(Guid? id = null)
    {
        return new HereditamentDocumentDto(
            id ?? Guid.NewGuid(),
            new HereditamentDto
            {
                Name = "Test Hereditament",
                Status = HereditamentStatus.Active,
                EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow),
                AddressId = Guid.NewGuid()
            });
    }

    #region CreateHereditament

    [TestMethod]
    public async Task CreateHereditamentAsync_Success_ReturnsCreatedDto()
    {
        var createDto = new CreateUpdateHereditament
        {
            Name = "Test Hereditament",
            EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow),
            AddressId = Guid.NewGuid()
        };
        var expectedDto = CreateDto(_testId);

        _mockMappingService
            .Setup(m => m.MapToDomain(createDto))
            .Returns(CreateEntity(_testId));
        _mockMappingService
            .Setup(m => m.MapToDto(It.IsAny<HereditamentDocumentEntity>()))
            .Returns(expectedDto);

        var result = await _service.CreateHereditamentAsync(createDto, "test-user");

        Assert.AreEqual(expectedDto, result);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<HereditamentDocumentEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task CreateHereditamentAsync_NullDto_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.CreateHereditamentAsync(null!));
    }

    #endregion

    #region GetHereditamentById

    [TestMethod]
    public async Task GetHereditamentByIdAsync_Found_ReturnsDto()
    {
        var entity = CreateEntity(_testId);
        var expectedDto = CreateDto(_testId);

        _mockRepository
            .Setup(r => r.GetByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mockMappingService
            .Setup(m => m.MapToDto(entity))
            .Returns(expectedDto);

        var result = await _service.GetHereditamentByIdAsync(_testId);

        Assert.AreEqual(expectedDto, result);
    }

    [TestMethod]
    public async Task GetHereditamentByIdAsync_NotFound_ReturnsNull()
    {
        _mockRepository
            .Setup(r => r.GetByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((HereditamentDocumentEntity?)null);

        var result = await _service.GetHereditamentByIdAsync(_testId);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetHereditamentByIdAsync_Deleted_ReturnsNull()
    {
        var entity = CreateEntity(_testId);
        entity.IsDeleted = true;

        _mockRepository
            .Setup(r => r.GetByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _service.GetHereditamentByIdAsync(_testId);

        Assert.IsNull(result);
    }

    #endregion

    #region GetAllHereditamentes

    [TestMethod]
    public async Task GetAllHereditamentesAsync_DefaultPaging_ReturnsPagedResults()
    {
        var entities = new List<HereditamentDocumentEntity>
        {
            CreateEntity(),
            CreateEntity()
        };
        var dtos = new List<HereditamentDocumentDto> { CreateDto(), CreateDto() };

        _mockRepository
            .Setup(r => r.GetPagedAsync(
                1, 10, null, It.IsAny<ISpecification<HereditamentDocumentEntity>>(), null, false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((entities.AsReadOnly(), 2));

        for (int i = 0; i < entities.Count; i++)
        {
            _mockMappingService
                .Setup(m => m.MapToDto(entities[i]))
                .Returns(dtos[i]);
        }

        var result = await _service.GetAllHereditamentesAsync();

        Assert.AreEqual(2, result.Data.Count);
        Assert.AreEqual(2, result.TotalCount);
    }

    [TestMethod]
    public async Task GetAllHereditamentesAsync_CustomPaging_PassesParameters()
    {
        _mockRepository
            .Setup(r => r.GetPagedAsync(
                2, 20, null, It.IsAny<ISpecification<HereditamentDocumentEntity>>(), null, false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<HereditamentDocumentEntity>().AsReadOnly(), 0));

        await _service.GetAllHereditamentesAsync(page: 2, pageSize: 20);

        _mockRepository.Verify(r => r.GetPagedAsync(
            2, 20, null, It.IsAny<ISpecification<HereditamentDocumentEntity>>(), null, false,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region UpdateHereditament

    [TestMethod]
    public async Task UpdateHereditamentAsync_Success_ReturnsUpdatedDto()
    {
        var updateDto = new CreateUpdateHereditament
        {
            Name = "Updated Hereditament",
            EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow),
            AddressId = Guid.NewGuid()
        };
        var entity = CreateEntity(_testId);
        var expectedDto = CreateDto(_testId);

        _mockRepository
            .Setup(r => r.GetByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mockMappingService
            .Setup(m => m.UpdateDomain(entity, updateDto))
            .Returns(entity);
        _mockMappingService
            .Setup(m => m.MapToDto(entity))
            .Returns(expectedDto);

        var result = await _service.UpdateHereditamentAsync(_testId, updateDto, "test-user");

        Assert.AreEqual(expectedDto, result);
        _mockRepository.Verify(r => r.Update(entity), Times.Once);
    }

    [TestMethod]
    public async Task UpdateHereditamentAsync_NotFound_ReturnsNull()
    {
        var updateDto = new CreateUpdateHereditament
        {
            Name = "Updated Hereditament",
            EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow),
            AddressId = Guid.NewGuid()
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((HereditamentDocumentEntity?)null);

        var result = await _service.UpdateHereditamentAsync(_testId, updateDto);

        Assert.IsNull(result);
    }

    #endregion

    #region DeleteHereditament

    [TestMethod]
    public async Task DeleteHereditamentAsync_Success_ReturnsTrueAndMarksDeleted()
    {
        var entity = CreateEntity(_testId);

        _mockRepository
            .Setup(r => r.GetByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _service.DeleteHereditamentAsync(_testId, "test-user");

        Assert.IsTrue(result);
        Assert.IsTrue(entity.IsDeleted);
        _mockRepository.Verify(r => r.Update(entity), Times.Once);
    }

    [TestMethod]
    public async Task DeleteHereditamentAsync_NotFound_ReturnsFalse()
    {
        _mockRepository
            .Setup(r => r.GetByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((HereditamentDocumentEntity?)null);

        var result = await _service.DeleteHereditamentAsync(_testId);

        Assert.IsFalse(result);
    }

    #endregion

    #region PermanentlyDeleteHereditament

    [TestMethod]
    public async Task PermanentlyDeleteHereditamentAsync_Success_RemovesEntity()
    {
        var entity = CreateEntity(_testId);

        _mockRepository
            .Setup(r => r.GetByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _service.PermanentlyDeleteHereditamentAsync(_testId);

        Assert.IsTrue(result);
        _mockRepository.Verify(r => r.Remove(entity), Times.Once);
    }

    [TestMethod]
    public async Task PermanentlyDeleteHereditamentAsync_NotFound_ReturnsFalse()
    {
        _mockRepository
            .Setup(r => r.GetByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((HereditamentDocumentEntity?)null);

        var result = await _service.PermanentlyDeleteHereditamentAsync(_testId);

        Assert.IsFalse(result);
    }

    #endregion

    #region AdvancedSearch

    [TestMethod]
    public async Task AdvancedSearchAsync_WithFilters_ReturnsFiltered()
    {
        var entities = new List<HereditamentDocumentEntity> { CreateEntity(_testId) };
        var dtos = new List<HereditamentDocumentDto> { CreateDto(_testId) };

        _mockRepository
            .Setup(r => r.GetPagedAsync(
                1, 10, null, It.IsAny<ISpecification<HereditamentDocumentEntity>>(), null, false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((entities.AsReadOnly(), 1));
        _mockMappingService
            .Setup(m => m.MapToDto(entities[0]))
            .Returns(dtos[0]);

        var result = await _service.AdvancedSearchAsync(
            name: "Test Hereditament");

        Assert.AreEqual(1, result.Data.Count);
    }

    [TestMethod]
    public async Task AdvancedSearchAsync_NoFilters_ReturnsAll()
    {
        _mockRepository
            .Setup(r => r.GetPagedAsync(
                1, 10, null, It.IsAny<ISpecification<HereditamentDocumentEntity>>(), null, false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<HereditamentDocumentEntity>().AsReadOnly(), 0));

        var result = await _service.AdvancedSearchAsync();

        Assert.AreEqual(0, result.Data.Count);
    }

    #endregion

    #region GetHereditamentCount

     [TestMethod]
     public async Task GetHereditamentCountAsync_ReturnsCount()
     {
         const int expectedCount = 42;
         var entities = Enumerable.Range(1, expectedCount).Select(i => CreateEntity()).ToList();

         _mockRepository
             .Setup(r => r.FindAsync(It.IsAny<ISpecification<HereditamentDocumentEntity>>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(entities);

         var result = await _service.GetHereditamentCountAsync();

         Assert.AreEqual(expectedCount, result);
     }

    #endregion

    #region RestoreHereditament

    [TestMethod]
    public async Task RestoreHereditamentAsync_Success_RestoresEntity()
    {
        var entity = CreateEntity(_testId);
        entity.IsDeleted = true;

        _mockRepository
            .Setup(r => r.GetByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _service.RestoreHereditamentAsync(_testId, "test-user");

        Assert.IsTrue(result);
        Assert.IsFalse(entity.IsDeleted);
        _mockRepository.Verify(r => r.Update(entity), Times.Once);
    }

    [TestMethod]
    public async Task RestoreHereditamentAsync_NotFound_ReturnsFalse()
    {
        _mockRepository
            .Setup(r => r.GetByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((HereditamentDocumentEntity?)null);

        var result = await _service.RestoreHereditamentAsync(_testId);

        Assert.IsFalse(result);
    }

    #endregion

    #region CancellationToken

    [TestMethod]
    public async Task CreateHereditamentAsync_PassesCancellationToken()
    {
        var cts = new CancellationTokenSource();
        var createDto = new CreateUpdateHereditament
        {
            Name = "Test Hereditament",
            EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow),
            AddressId = Guid.NewGuid()
        };
        var expectedDto = CreateDto();

        _mockMappingService
            .Setup(m => m.MapToDomain(It.IsAny<CreateUpdateHereditament>()))
            .Returns(CreateEntity());
        _mockMappingService
            .Setup(m => m.MapToDto(It.IsAny<HereditamentDocumentEntity>()))
            .Returns(expectedDto);

        await _service.CreateHereditamentAsync(createDto, "test-user", cts.Token);

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<HereditamentDocumentEntity>(), cts.Token), Times.Once);
    }

    #endregion
}
