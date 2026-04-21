using Common.Rest.Shared.Repository;
using Common.Rest.SurveyData.Application.Services;

namespace Common.Rest.SurveyData.Application.Tests;

[TestClass]
public class SurveyServiceTests
{
    private readonly Mock<ISurveyRepository> _surveyRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<SurveyService>> _loggerMock;
    private readonly ISurveyMappingService _mappingService;
    private readonly SurveyService _sut;

    public SurveyServiceTests()
    {
        _surveyRepositoryMock = new Mock<ISurveyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<SurveyService>>();
        _mappingService = new SurveyMappingService();

        _sut = new SurveyService(_surveyRepositoryMock.Object, _unitOfWorkMock.Object, _mappingService, _loggerMock.Object);
    }

    [TestMethod]
    public async Task GetByIdAsync_WhenSurveyExists_ShouldReturnSurveyDto()
    {
        // Arrange
        var surveyId = Guid.NewGuid();
        var survey = new Survey { Id = surveyId, ReferenceNumber = "REF-123" };
        _surveyRepositoryMock.Setup(r => r.GetWithDetailsAsync(surveyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(survey);

        // Act
        var result = await _sut.GetByIdAsync(surveyId);

        // Assert
        Assert.IsNotNull(result); 
        Assert.AreEqual(surveyId, result.Id);
    }
}

