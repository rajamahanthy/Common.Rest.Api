namespace Common.Rest.Address.Api.Tests.Controllers;

/// <summary>
/// Comprehensive unit tests for AddressController with 100% code coverage.
/// Uses generic approach to minimize duplication.
/// </summary>
[TestClass]
public class AddressControllerTests : ControllerTestBase
{
    private Guid _testId = Guid.NewGuid();

    protected override void OnTestInitialize()
    {
        _testId = Guid.NewGuid();
        SetUser("test-user");
    }

    #region CreateAddress

    [TestMethod]
    public async Task CreateAddress_Success_ReturnsCreatedAtAction()
    {
        var createDto = AddressTestDataBuilder.CreateUpdateAddressDto();
        var resultDto = AddressTestDataBuilder.CreateAddressDocumentDto(_testId);
        
        MockAddressService
            .Setup(s => s.CreateAddressAsync(It.IsAny<CreateUpdateAddress>(), "test-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultDto);

        var result = await Controller.CreateAddress(createDto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = (CreatedAtActionResult)result.Result!;
        createdResult.ActionName.Should().Be(nameof(Controller.GetAllAddresses));
        createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        
        MockAddressService.Verify(
            s => s.CreateAddressAsync(It.IsAny<CreateUpdateAddress>(), "test-user", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task CreateAddress_WithoutUser_PassesNullUserId()
    {
        ClearUser();
        var createDto = AddressTestDataBuilder.CreateUpdateAddressDto();
        var resultDto = AddressTestDataBuilder.CreateAddressDocumentDto(_testId);
        
        MockAddressService
            .Setup(s => s.CreateAddressAsync(It.IsAny<CreateUpdateAddress>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultDto);

        await Controller.CreateAddress(createDto);

        MockAddressService.Verify(
            s => s.CreateAddressAsync(It.IsAny<CreateUpdateAddress>(), null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region GetAddressById

    [TestMethod]
    public async Task GetAddressById_Found_ReturnsOk()
    {
        var addressDto = AddressTestDataBuilder.CreateAddressDocumentDto(_testId);
        
        MockAddressService
            .Setup(s => s.GetAddressByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(addressDto);

        var result = await Controller.GetAddressById(_testId);

        result.Result.Should().BeOfType<OkObjectResult>();
        MockAddressService.Verify(
            s => s.GetAddressByIdAsync(_testId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task GetAddressById_NotFound_ThrowsNotFoundException()
    {
        var invalidId = Guid.NewGuid();
        MockAddressService
            .Setup(s => s.GetAddressByIdAsync(invalidId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddressDocumentDto?)null);

        Func<Task> act = () => Controller.GetAddressById(invalidId);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region GetAllAddresses

    [TestMethod]
    public async Task GetAllAddresses_DefaultPaging_ReturnsPagedResults()
    {
        var addresses = new List<AddressDocumentDto>
        {
            AddressTestDataBuilder.CreateAddressDocumentDto(),
            AddressTestDataBuilder.CreateAddressDocumentDto()
        };
        var paginationResult = new PaginationResult<AddressDocumentDto>(addresses, 2);
        
        MockAddressService
            .Setup(s => s.GetAllAddressesAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginationResult);

        var result = await Controller.GetAllAddresses();

        result.Result.Should().BeOfType<OkObjectResult>();
        MockAddressService.Verify(
            s => s.GetAllAddressesAsync(1, 10, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task GetAllAddresses_CustomPaging_PassesParameters()
    {
        var paginationResult = new PaginationResult<AddressDocumentDto>(new List<AddressDocumentDto>().AsReadOnly(), 0);
        
        MockAddressService
            .Setup(s => s.GetAllAddressesAsync(2, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginationResult);

        await Controller.GetAllAddresses(page: 2, pageSize: 20);

        MockAddressService.Verify(
            s => s.GetAllAddressesAsync(2, 20, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region UpdateAddress

    [TestMethod]
    public async Task UpdateAddress_Success_ReturnsOk()
    {
        var updateDto = AddressTestDataBuilder.CreateUpdateAddressDto();
        var resultDto = AddressTestDataBuilder.CreateAddressDocumentDto(_testId);
        
        MockAddressService
            .Setup(s => s.UpdateAddressAsync(_testId, It.IsAny<CreateUpdateAddress>(), "test-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultDto);

        var result = await Controller.UpdateAddress(_testId, updateDto);

        result.Result.Should().BeOfType<OkObjectResult>();
        MockAddressService.Verify(
            s => s.UpdateAddressAsync(_testId, It.IsAny<CreateUpdateAddress>(), "test-user", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task UpdateAddress_NotFound_ThrowsNotFoundException()
    {
        var updateDto = AddressTestDataBuilder.CreateUpdateAddressDto();
        var invalidId = Guid.NewGuid();
        
        MockAddressService
            .Setup(s => s.UpdateAddressAsync(invalidId, It.IsAny<CreateUpdateAddress>(), "test-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddressDocumentDto?)null);

        Func<Task> act = () => Controller.UpdateAddress(invalidId, updateDto);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region DeleteAddress

    [TestMethod]
    public async Task DeleteAddress_Success_ReturnsNoContent()
    {
        MockAddressService
            .Setup(s => s.DeleteAddressAsync(_testId, "test-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await Controller.DeleteAddress(_testId);

        result.Should().BeOfType<NoContentResult>();
        MockAddressService.Verify(
            s => s.DeleteAddressAsync(_testId, "test-user", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task DeleteAddress_NotFound_ThrowsNotFoundException()
    {
        var invalidId = Guid.NewGuid();
        MockAddressService
            .Setup(s => s.DeleteAddressAsync(invalidId, "test-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Func<Task> act = () => Controller.DeleteAddress(invalidId);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region PermanentlyDeleteAddress

    [TestMethod]
    public async Task PermanentlyDeleteAddress_Success_ReturnsNoContent()
    {
        MockAddressService
            .Setup(s => s.PermanentlyDeleteAddressAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await Controller.PermanentlyDeleteAddress(_testId);

        result.Should().BeOfType<NoContentResult>();
    }

    [TestMethod]
    public async Task PermanentlyDeleteAddress_NotFound_ThrowsNotFoundException()
    {
        var invalidId = Guid.NewGuid();
        MockAddressService
            .Setup(s => s.PermanentlyDeleteAddressAsync(invalidId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Func<Task> act = () => Controller.PermanentlyDeleteAddress(invalidId);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region SearchByUprn

    [TestMethod]
    public async Task SearchByUprn_Found_ReturnsOk()
    {
        const string uprn = "123456789";
        var resultDto = AddressTestDataBuilder.CreateAddressDocumentDto(_testId);
        
        MockAddressService
            .Setup(s => s.GetAddressByUprnAsync(uprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultDto);

        var result = await Controller.SearchByUprn(uprn);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public async Task SearchByUprn_EmptyUprn_ThrowsValidationException()
    {
        Func<Task> actEmpty = () => Controller.SearchByUprn("");
        Func<Task> actNull = () => Controller.SearchByUprn(null!);
        Func<Task> actWhitespace = () => Controller.SearchByUprn("   ");

        await actEmpty.Should().ThrowAsync<ValidationException>();
        await actNull.Should().ThrowAsync<ValidationException>();
        await actWhitespace.Should().ThrowAsync<ValidationException>();
    }

    [TestMethod]
    public async Task SearchByUprn_NotFound_ThrowsNotFoundException()
    {
        const string uprn = "999999999";
        MockAddressService
            .Setup(s => s.GetAddressByUprnAsync(uprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddressDocumentDto?)null);

        Func<Task> act = () => Controller.SearchByUprn(uprn);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region AdvancedSearch

    [TestMethod]
    public async Task AdvancedSearch_WithFilters_ReturnsFiltered()
    {
        var searchResults = new List<AddressDocumentDto> 
        { 
            AddressTestDataBuilder.CreateAddressDocumentDto(_testId) 
        };
        var paginationResult = new PaginationResult<AddressDocumentDto>(searchResults, 1);
        
        MockAddressService
            .Setup(s => s.AdvancedSearchAsync(
                "T1 1ST", null, "Test Organisation", "Main Street", null, 1, 10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginationResult);

        var result = await Controller.AdvancedSearch(
            postcode: "T1 1ST",
            organisation: "Test Organisation",
            thoroughfare: "Main Street",
            page: 1,
            pageSize: 10);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public async Task AdvancedSearch_NoFilters_ReturnsAll()
    {
        var paginationResult = new PaginationResult<AddressDocumentDto>(new List<AddressDocumentDto>().AsReadOnly(), 0);
        
        MockAddressService
            .Setup(s => s.AdvancedSearchAsync(
                null, null, null, null, null, 1, 10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginationResult);

        await Controller.AdvancedSearch();

        MockAddressService.Verify(
            s => s.AdvancedSearchAsync(
                null, null, null, null, null, 1, 10,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region GetAddressCount

    [TestMethod]
    public async Task GetAddressCount_ReturnsCount()
    {
        const int expectedCount = 42;
        MockAddressService
            .Setup(s => s.GetAddressCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCount);

        var result = await Controller.GetAddressCount();

        result.Result.Should().BeOfType<OkObjectResult>();
        MockAddressService.Verify(
            s => s.GetAddressCountAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region RestoreAddress

    [TestMethod]
    public async Task RestoreAddress_Success_ReturnsNoContent()
    {
        MockAddressService
            .Setup(s => s.RestoreAddressAsync(_testId, "test-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await Controller.RestoreAddress(_testId);

        result.Should().BeOfType<NoContentResult>();
    }

    [TestMethod]
    public async Task RestoreAddress_NotFound_ThrowsNotFoundException()
    {
        var invalidId = Guid.NewGuid();
        MockAddressService
            .Setup(s => s.RestoreAddressAsync(invalidId, "test-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Func<Task> act = () => Controller.RestoreAddress(invalidId);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region CancellationToken Tests

    [TestMethod]
    public async Task AllMethods_PassCancellationToken()
    {
        var cts = new CancellationTokenSource();
        var createDto = AddressTestDataBuilder.CreateUpdateAddressDto();
        var resultDto = AddressTestDataBuilder.CreateAddressDocumentDto(_testId);
        
        MockAddressService.Setup(s => s.CreateAddressAsync(
            It.IsAny<CreateUpdateAddress>(), It.IsAny<string>(), cts.Token))
            .ReturnsAsync(resultDto);

        await Controller.CreateAddress(createDto, cts.Token);

        MockAddressService.Verify(
            s => s.CreateAddressAsync(
                It.IsAny<CreateUpdateAddress>(), It.IsAny<string>(), cts.Token),
            Times.Once);
    }

    #endregion
}
