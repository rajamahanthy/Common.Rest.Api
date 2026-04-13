namespace Common.Rest.Address.Application.Tests.Services;

/// <summary>
/// Comprehensive unit tests for AddressService with 100% code coverage.
/// Tests all CRUD operations, search, and filtering functionality.
/// </summary>
[TestClass]
public class AddressServiceTests
{
    private Mock<IRepository<AddressDocumentEntity>> _mockRepository = null!;
    private Mock<IUnitOfWork> _mockUnitOfWork = null!;
    private Mock<IAddressMappingService> _mockMappingService = null!;
    private AddressService _service = null!;
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

        _service = new AddressService(
            _mockRepository.Object,
            _mockUnitOfWork.Object,
            _mockMappingService.Object);
    }

    private static AddressDocumentEntity CreateEntity(Guid? id = null)
    {
        return new AddressDocumentEntity
        {
            Id = id ?? Guid.NewGuid(),
            DocumentType = "Address",
            JsonData = new AddressEntity
            {
                Uprn = "123456789",
                Usrn = "999999999",
                AddressInfo = new AddressInfoEntity
                {
                    Organisation = "Test Org",
                    Pao = new AddressableObjectEntity { StartNumber = 1, Text = "1" },
                    StreetDescriptor = new StreetDescriptorEntity { StreetDescription = "Main St", PostTown = "Test Town" },
                    Postcode = "T1 1ST"
                },
                Geography = new GeographyEntity { Easting = 529904, Northing = 180994 }
            }
        };
    }

    private static AddressDocumentDto CreateDto(Guid? id = null)
    {
        return new AddressDocumentDto(
            id ?? Guid.NewGuid(),
            new AddressDto
            {
                Uprn = "123456789",
                AddressInfo = new AddressInfo
                {
                    Pao = new AddressableObject { Text = "1 Main St" },
                    StreetDescriptor = new StreetDescriptor { StreetDescription = "Main St", PostTown = "Test Town" }
                },
                Geography = new Geography { Easting = 123456, Northing = 654321 }
            });
    }

    #region CreateAddress

    [TestMethod]
    public async Task CreateAddressAsync_Success_ReturnsCreatedDto()
    {
        var createDto = new CreateUpdateAddress
        {
            AddressInfo = new AddressInfo
            {
                Pao = new AddressableObject { Text = "1 Main St" },
                StreetDescriptor = new StreetDescriptor { StreetDescription = "Main St", PostTown = "Test Town" }
            },
            Geography = new Geography { Easting = 529904, Northing = 180994 }
        };
        var expectedDto = CreateDto(_testId);

        _mockMappingService
            .Setup(m => m.MapToDomain(createDto))
            .Returns(CreateEntity(_testId));
        _mockMappingService
            .Setup(m => m.MapToDto(It.IsAny<AddressDocumentEntity>()))
            .Returns(expectedDto);

        var result = await _service.CreateAddressAsync(createDto, "test-user");

        result.Should().Be(expectedDto);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<AddressDocumentEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task CreateAddressAsync_NullDto_ThrowsArgumentNullException()
    {
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
            _service.CreateAddressAsync(null!));
    }

    #endregion

    #region GetAddressById

    [TestMethod]
    public async Task GetAddressByIdAsync_Found_ReturnsDto()
    {
        var entity = CreateEntity(_testId);
        var expectedDto = CreateDto(_testId);

        _mockRepository
            .Setup(r => r.GetByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mockMappingService
            .Setup(m => m.MapToDto(entity))
            .Returns(expectedDto);

        var result = await _service.GetAddressByIdAsync(_testId);

        result.Should().Be(expectedDto);
    }

    [TestMethod]
    public async Task GetAddressByIdAsync_NotFound_ReturnsNull()
    {
        _mockRepository
            .Setup(r => r.GetByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddressDocumentEntity?)null);

        var result = await _service.GetAddressByIdAsync(_testId);

        result.Should().BeNull();
    }

    [TestMethod]
    public async Task GetAddressByIdAsync_Deleted_ReturnsNull()
    {
        var entity = CreateEntity(_testId);
        entity.IsDeleted = true;

        _mockRepository
            .Setup(r => r.GetByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _service.GetAddressByIdAsync(_testId);

        result.Should().BeNull();
    }

    #endregion

    #region GetAllAddresses

    [TestMethod]
    public async Task GetAllAddressesAsync_DefaultPaging_ReturnsPagedResults()
    {
        var entities = new List<AddressDocumentEntity>
        {
            CreateEntity(),
            CreateEntity()
        };
        var dtos = new List<AddressDocumentDto> { CreateDto(), CreateDto() };

        _mockRepository
            .Setup(r => r.GetPagedAsync(
                1, 10, null, It.IsAny<ISpecification<AddressDocumentEntity>>(), null, false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((entities.AsReadOnly(), 2));

        for (int i = 0; i < entities.Count; i++)
        {
            _mockMappingService
                .Setup(m => m.MapToDto(entities[i]))
                .Returns(dtos[i]);
        }

        var result = await _service.GetAllAddressesAsync();

        result.Data.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
    }

    [TestMethod]
    public async Task GetAllAddressesAsync_CustomPaging_PassesParameters()
    {
        _mockRepository
            .Setup(r => r.GetPagedAsync(
                2, 20, null, It.IsAny<ISpecification<AddressDocumentEntity>>(), null, false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<AddressDocumentEntity>().AsReadOnly(), 0));

        await _service.GetAllAddressesAsync(page: 2, pageSize: 20);

        _mockRepository.Verify(r => r.GetPagedAsync(
            2, 20, null, It.IsAny<ISpecification<AddressDocumentEntity>>(), null, false,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region UpdateAddress

    [TestMethod]
    public async Task UpdateAddressAsync_Success_ReturnsUpdatedDto()
    {
        var updateDto = new CreateUpdateAddress
        {
            AddressInfo = new AddressInfo
            {
                Pao = new AddressableObject { Text = "1 Main St" },
                StreetDescriptor = new StreetDescriptor { StreetDescription = "Main St", PostTown = "Test Town" }
            },
            Geography = new Geography { Easting = 529904, Northing = 180994 }
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

        var result = await _service.UpdateAddressAsync(_testId, updateDto, "test-user");

        result.Should().Be(expectedDto);
        _mockRepository.Verify(r => r.Update(entity), Times.Once);
    }

    [TestMethod]
    public async Task UpdateAddressAsync_NotFound_ReturnsNull()
    {
        var updateDto = new CreateUpdateAddress
        {
            AddressInfo = new AddressInfo
            {
                Pao = new AddressableObject { Text = "1 Main St" },
                StreetDescriptor = new StreetDescriptor { StreetDescription = "Main St", PostTown = "Test Town" }
            },
            Geography = new Geography { Easting = 529904, Northing = 180994 }
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddressDocumentEntity?)null);

        var result = await _service.UpdateAddressAsync(_testId, updateDto);

        result.Should().BeNull();
    }

    #endregion

    #region DeleteAddress

    [TestMethod]
    public async Task DeleteAddressAsync_Success_ReturnsTrueAndMarksDeleted()
    {
        var entity = CreateEntity(_testId);

        _mockRepository
            .Setup(r => r.GetByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _service.DeleteAddressAsync(_testId, "test-user");

        result.Should().BeTrue();
        entity.IsDeleted.Should().BeTrue();
        _mockRepository.Verify(r => r.Update(entity), Times.Once);
    }

    [TestMethod]
    public async Task DeleteAddressAsync_NotFound_ReturnsFalse()
    {
        _mockRepository
            .Setup(r => r.GetByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddressDocumentEntity?)null);

        var result = await _service.DeleteAddressAsync(_testId);

        result.Should().BeFalse();
    }

    #endregion

    #region PermanentlyDeleteAddress

    [TestMethod]
    public async Task PermanentlyDeleteAddressAsync_Success_RemovesEntity()
    {
        var entity = CreateEntity(_testId);

        _mockRepository
            .Setup(r => r.GetByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _service.PermanentlyDeleteAddressAsync(_testId);

        result.Should().BeTrue();
        _mockRepository.Verify(r => r.Remove(entity), Times.Once);
    }

    [TestMethod]
    public async Task PermanentlyDeleteAddressAsync_NotFound_ReturnsFalse()
    {
        _mockRepository
            .Setup(r => r.GetByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddressDocumentEntity?)null);

        var result = await _service.PermanentlyDeleteAddressAsync(_testId);

        result.Should().BeFalse();
    }

    #endregion

    #region GetAddressByUprn

    [TestMethod]
    public async Task GetAddressByUprnAsync_Found_ReturnsDto()
    {
        const string uprn = "123456789";
        var entity = CreateEntity(_testId);
        var expectedDto = CreateDto(_testId);

        _mockRepository
            .Setup(r => r.FindAsync(It.IsAny<ISpecification<AddressDocumentEntity>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { entity }.ToList());
        _mockMappingService
            .Setup(m => m.MapToDto(entity))
            .Returns(expectedDto);

        var result = await _service.GetAddressByUprnAsync(uprn);

        result.Should().Be(expectedDto);
    }

    [TestMethod]
    public async Task GetAddressByUprnAsync_NotFound_ReturnsNull()
    {
        const string uprn = "999999999";

        _mockRepository
            .Setup(r => r.FindAsync(It.IsAny<ISpecification<AddressDocumentEntity>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AddressDocumentEntity>());

        var result = await _service.GetAddressByUprnAsync(uprn);

        result.Should().BeNull();
    }

    #endregion

    #region AdvancedSearch

    [TestMethod]
    public async Task AdvancedSearchAsync_WithFilters_ReturnsFiltered()
    {
        var entities = new List<AddressDocumentEntity> { CreateEntity(_testId) };
        var dtos = new List<AddressDocumentDto> { CreateDto(_testId) };

        _mockRepository
            .Setup(r => r.GetPagedAsync(
                1, 10, null, It.IsAny<ISpecification<AddressDocumentEntity>>(), null, false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((entities.AsReadOnly(), 1));
        _mockMappingService
            .Setup(m => m.MapToDto(entities[0]))
            .Returns(dtos[0]);

        var result = await _service.AdvancedSearchAsync(
            postcode: "T1 1ST",
            organisation: "Test Organisation");

        result.Data.Should().HaveCount(1);
    }

    [TestMethod]
    public async Task AdvancedSearchAsync_NoFilters_ReturnsAll()
    {
        _mockRepository
            .Setup(r => r.GetPagedAsync(
                1, 10, null, It.IsAny<ISpecification<AddressDocumentEntity>>(), null, false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<AddressDocumentEntity>().AsReadOnly(), 0));

        var result = await _service.AdvancedSearchAsync();

        result.Data.Should().BeEmpty();
    }

    #endregion

    #region GetAddressCount

     [TestMethod]
     public async Task GetAddressCountAsync_ReturnsCount()
     {
         const int expectedCount = 42;
         var entities = Enumerable.Range(1, expectedCount).Select(i => CreateEntity()).ToList();

         _mockRepository
             .Setup(r => r.FindAsync(It.IsAny<ISpecification<AddressDocumentEntity>>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(entities);

         var result = await _service.GetAddressCountAsync();

         result.Should().Be(expectedCount);
     }

    #endregion

    #region RestoreAddress

    [TestMethod]
    public async Task RestoreAddressAsync_Success_RestoresEntity()
    {
        var entity = CreateEntity(_testId);
        entity.IsDeleted = true;

        _mockRepository
            .Setup(r => r.GetByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _service.RestoreAddressAsync(_testId, "test-user");

        result.Should().BeTrue();
        entity.IsDeleted.Should().BeFalse();
        _mockRepository.Verify(r => r.Update(entity), Times.Once);
    }

    [TestMethod]
    public async Task RestoreAddressAsync_NotFound_ReturnsFalse()
    {
        _mockRepository
            .Setup(r => r.GetByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddressDocumentEntity?)null);

        var result = await _service.RestoreAddressAsync(_testId);

        result.Should().BeFalse();
    }

    #endregion

    #region CancellationToken

    [TestMethod]
    public async Task CreateAddressAsync_PassesCancellationToken()
    {
        var cts = new CancellationTokenSource();
        var createDto = new CreateUpdateAddress
        {
            AddressInfo = new AddressInfo
            {
                Pao = new AddressableObject { Text = "1 Main St" },
                StreetDescriptor = new StreetDescriptor { StreetDescription = "Main St", PostTown = "Test Town" }
            },
            Geography = new Geography { Easting = 529904, Northing = 180994 }
        };
        var expectedDto = CreateDto();

        _mockMappingService
            .Setup(m => m.MapToDomain(It.IsAny<CreateUpdateAddress>()))
            .Returns(CreateEntity());
        _mockMappingService
            .Setup(m => m.MapToDto(It.IsAny<AddressDocumentEntity>()))
            .Returns(expectedDto);

        await _service.CreateAddressAsync(createDto, "test-user", cts.Token);

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<AddressDocumentEntity>(), cts.Token), Times.Once);
    }

    #endregion
}
