using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using RestApi.Shared.Repository;
using SurveyData.Application.DTOs;
using SurveyData.Application.Interfaces;
using SurveyData.Application.Mapping;
using SurveyData.Application.Services;
using SurveyData.Domain.Entities;

namespace SurveyData.Application.Tests;

[TestClass]
public class SurveyServiceTests
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<SurveyService> _logger;
    private readonly SurveyService _sut;

    public SurveyServiceTests()
    {
        _surveyRepository = Substitute.For<ISurveyRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _logger = Substitute.For<ILogger<SurveyService>>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<SurveyMappingProfile>());
        _mapper = config.CreateMapper();

        _sut = new SurveyService(_surveyRepository, _unitOfWork, _mapper, _logger);
    }

    [TestMethod]
    public async Task GetByIdAsync_WhenSurveyExists_ShouldReturnSurveyDto()
    {
        // Arrange
        var surveyId = Guid.NewGuid();
        var survey = new Survey { Id = surveyId, ReferenceNumber = "REF-123" };
        _surveyRepository.GetWithDetailsAsync(surveyId, Arg.Any<CancellationToken>()).Returns(survey);

        // Act
        var result = await _sut.GetByIdAsync(surveyId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(surveyId);
    }
}
