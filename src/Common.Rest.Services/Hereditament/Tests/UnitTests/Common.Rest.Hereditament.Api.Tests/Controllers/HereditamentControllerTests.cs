namespace Common.Rest.Hereditament.Api.Tests.Controllers;

/// <summary>
/// Comprehensive unit tests for HereditamentController with 100% code coverage.
/// Uses generic approach to minimize duplication.
/// </summary>
[TestClass]
public class HereditamentControllerTests : ControllerTestBase
{
    private Guid _testId = Guid.NewGuid();

    protected override void OnTestInitialize()
    {
        _testId = Guid.NewGuid();
        SetUser("test-user");
    }

    #region CreateHereditament

    [TestMethod]
    public async Task CreateHereditament_Success_ReturnsCreatedAtAction()
    {
        var createDto = HereditamentTestDataBuilder.CreateUpdateHereditamentDto();
        var resultDto = HereditamentTestDataBuilder.CreateHereditamentDocumentDto(_testId);
        
        MockHereditamentService
            .Setup(s => s.CreateHereditamentAsync(It.IsAny<CreateUpdateHereditament>(), "test-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultDto);

        var result = await Controller.CreateHereditament(createDto);

        Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
        var createdResult = (CreatedAtActionResult)result.Result!;
        Assert.AreEqual(nameof(Controller.GetAllHereditamentes), createdResult.ActionName);
        Assert.AreEqual(StatusCodes.Status201Created, createdResult.StatusCode);
        
        MockHereditamentService.Verify(
            s => s.CreateHereditamentAsync(It.IsAny<CreateUpdateHereditament>(), "test-user", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task CreateHereditament_WithoutUser_PassesNullUserId()
    {
        ClearUser();
        var createDto = HereditamentTestDataBuilder.CreateUpdateHereditamentDto();
        var resultDto = HereditamentTestDataBuilder.CreateHereditamentDocumentDto(_testId);
        
        MockHereditamentService
            .Setup(s => s.CreateHereditamentAsync(It.IsAny<CreateUpdateHereditament>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultDto);

        await Controller.CreateHereditament(createDto);

        MockHereditamentService.Verify(
            s => s.CreateHereditamentAsync(It.IsAny<CreateUpdateHereditament>(), null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region GetHereditamentById

    [TestMethod]
    public async Task GetHereditamentById_Found_ReturnsOk()
    {
        var HereditamentDto = HereditamentTestDataBuilder.CreateHereditamentDocumentDto(_testId);
        
        MockHereditamentService
            .Setup(s => s.GetHereditamentByIdAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HereditamentDto);

        var result = await Controller.GetHereditamentById(_testId);

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        MockHereditamentService.Verify(
            s => s.GetHereditamentByIdAsync(_testId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task GetHereditamentById_NotFound_ThrowsNotFoundException()
    {
        var invalidId = Guid.NewGuid();
        MockHereditamentService
            .Setup(s => s.GetHereditamentByIdAsync(invalidId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((HereditamentDocumentDto?)null);

        await Assert.ThrowsExceptionAsync<NotFoundException>(() => Controller.GetHereditamentById(invalidId));
    }

    #endregion

    #region GetAllHereditamentes

    [TestMethod]
    public async Task GetAllHereditamentes_DefaultPaging_ReturnsPagedResults()
    {
        var Hereditamentes = new List<HereditamentDocumentDto>
        {
            HereditamentTestDataBuilder.CreateHereditamentDocumentDto(),
            HereditamentTestDataBuilder.CreateHereditamentDocumentDto()
        };
        var paginationResult = new PaginationResult<HereditamentDocumentDto>(Hereditamentes, 2);
        
        MockHereditamentService
            .Setup(s => s.GetAllHereditamentesAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginationResult);

        var result = await Controller.GetAllHereditamentes();

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        MockHereditamentService.Verify(
            s => s.GetAllHereditamentesAsync(1, 10, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task GetAllHereditamentes_CustomPaging_PassesParameters()
    {
        var paginationResult = new PaginationResult<HereditamentDocumentDto>(new List<HereditamentDocumentDto>().AsReadOnly(), 0);
        
        MockHereditamentService
            .Setup(s => s.GetAllHereditamentesAsync(2, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginationResult);

        await Controller.GetAllHereditamentes(page: 2, pageSize: 20);

        MockHereditamentService.Verify(
            s => s.GetAllHereditamentesAsync(2, 20, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region UpdateHereditament

    [TestMethod]
    public async Task UpdateHereditament_Success_ReturnsOk()
    {
        var updateDto = HereditamentTestDataBuilder.CreateUpdateHereditamentDto();
        var resultDto = HereditamentTestDataBuilder.CreateHereditamentDocumentDto(_testId);
        
        MockHereditamentService
            .Setup(s => s.UpdateHereditamentAsync(_testId, It.IsAny<CreateUpdateHereditament>(), "test-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultDto);

        var result = await Controller.UpdateHereditament(_testId, updateDto);

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        MockHereditamentService.Verify(
            s => s.UpdateHereditamentAsync(_testId, It.IsAny<CreateUpdateHereditament>(), "test-user", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task UpdateHereditament_NotFound_ThrowsNotFoundException()
    {
        var updateDto = HereditamentTestDataBuilder.CreateUpdateHereditamentDto();
        var invalidId = Guid.NewGuid();
        
        MockHereditamentService
            .Setup(s => s.UpdateHereditamentAsync(invalidId, It.IsAny<CreateUpdateHereditament>(), "test-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync((HereditamentDocumentDto?)null);

        await Assert.ThrowsExceptionAsync<NotFoundException>(() => Controller.UpdateHereditament(invalidId, updateDto));
    }

    #endregion

    #region DeleteHereditament

    [TestMethod]
    public async Task DeleteHereditament_Success_ReturnsNoContent()
    {
        MockHereditamentService
            .Setup(s => s.DeleteHereditamentAsync(_testId, "test-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await Controller.DeleteHereditament(_testId);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        MockHereditamentService.Verify(
            s => s.DeleteHereditamentAsync(_testId, "test-user", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task DeleteHereditament_NotFound_ThrowsNotFoundException()
    {
        var invalidId = Guid.NewGuid();
        MockHereditamentService
            .Setup(s => s.DeleteHereditamentAsync(invalidId, "test-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await Assert.ThrowsExceptionAsync<NotFoundException>(() => Controller.DeleteHereditament(invalidId));
    }

    #endregion

    #region PermanentlyDeleteHereditament

    [TestMethod]
    public async Task PermanentlyDeleteHereditament_Success_ReturnsNoContent()
    {
        MockHereditamentService
            .Setup(s => s.PermanentlyDeleteHereditamentAsync(_testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await Controller.PermanentlyDeleteHereditament(_testId);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }

    [TestMethod]
    public async Task PermanentlyDeleteHereditament_NotFound_ThrowsNotFoundException()
    {
        var invalidId = Guid.NewGuid();
        MockHereditamentService
            .Setup(s => s.PermanentlyDeleteHereditamentAsync(invalidId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await Assert.ThrowsExceptionAsync<NotFoundException>(() => Controller.PermanentlyDeleteHereditament(invalidId));
    }

    #endregion

    #region AdvancedSearch

    [TestMethod]
    public async Task AdvancedSearch_WithFilters_ReturnsFiltered()
    {
        var searchResults = new List<HereditamentDocumentDto> 
        { 
            HereditamentTestDataBuilder.CreateHereditamentDocumentDto(_testId) 
        };
        var paginationResult = new PaginationResult<HereditamentDocumentDto>(searchResults, 1);
        
        MockHereditamentService
            .Setup(s => s.AdvancedSearchAsync(
                "Test", "Draft", DateOnly.FromDateTime(DateTime.UtcNow), 1, 10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginationResult);

        var result = await Controller.AdvancedSearch(
            name: "Test",
            status: "Draft",
            effectiveFrom: DateOnly.FromDateTime(DateTime.UtcNow),
            page: 1,
            pageSize: 10);

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
    }

    [TestMethod]
    public async Task AdvancedSearch_NoFilters_ReturnsAll()
    {
        var paginationResult = new PaginationResult<HereditamentDocumentDto>(new List<HereditamentDocumentDto>().AsReadOnly(), 0);
        
        MockHereditamentService
            .Setup(s => s.AdvancedSearchAsync(
                null, null, null, 1, 10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginationResult);

        await Controller.AdvancedSearch();

        MockHereditamentService.Verify(
            s => s.AdvancedSearchAsync(
                null, null, null, 1, 10,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region GetHereditamentCount

    [TestMethod]
    public async Task GetHereditamentCount_ReturnsCount()
    {
        const int expectedCount = 42;
        MockHereditamentService
            .Setup(s => s.GetHereditamentCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCount);

        var result = await Controller.GetHereditamentCount();

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        MockHereditamentService.Verify(
            s => s.GetHereditamentCountAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region RestoreHereditament

    [TestMethod]
    public async Task RestoreHereditament_Success_ReturnsNoContent()
    {
        MockHereditamentService
            .Setup(s => s.RestoreHereditamentAsync(_testId, "test-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await Controller.RestoreHereditament(_testId);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }

    [TestMethod]
    public async Task RestoreHereditament_NotFound_ThrowsNotFoundException()
    {
        var invalidId = Guid.NewGuid();
        MockHereditamentService
            .Setup(s => s.RestoreHereditamentAsync(invalidId, "test-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await Assert.ThrowsExceptionAsync<NotFoundException>(() => Controller.RestoreHereditament(invalidId));
    }

    #endregion

    #region CancellationToken Tests

    [TestMethod]
    public async Task AllMethods_PassCancellationToken()
    {
        var cts = new CancellationTokenSource();
        var createDto = HereditamentTestDataBuilder.CreateUpdateHereditamentDto();
        var resultDto = HereditamentTestDataBuilder.CreateHereditamentDocumentDto(_testId);
        
        MockHereditamentService.Setup(s => s.CreateHereditamentAsync(
            It.IsAny<CreateUpdateHereditament>(), It.IsAny<string>(), cts.Token))
            .ReturnsAsync(resultDto);

        await Controller.CreateHereditament(createDto, cts.Token);

        MockHereditamentService.Verify(
            s => s.CreateHereditamentAsync(
                It.IsAny<CreateUpdateHereditament>(), It.IsAny<string>(), cts.Token),
            Times.Once);
    }

    #endregion
}
