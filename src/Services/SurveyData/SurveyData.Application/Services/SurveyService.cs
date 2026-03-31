using RestApi.Shared.Exceptions;
using RestApi.Shared.Repository;

namespace SurveyData.Application.Services;

public sealed class SurveyService(
    ISurveyRepository surveyRepository,
    IUnitOfWork unitOfWork,
    ISurveyMappingService mappingService,
    ILogger<SurveyService> logger) : ISurveyService
{
    public async Task<SurveyDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var survey = await surveyRepository.GetWithDetailsAsync(id, ct)
                     ?? throw new NotFoundException(nameof(Survey), id);

        return mappingService.MapToSurveyDto(survey);
    }

    public async Task<SurveyDto> GetByReferenceAsync(string referenceNumber, CancellationToken ct = default)
    {
        var survey = await surveyRepository.GetByReferenceNumberAsync(referenceNumber, ct)
                     ?? throw new NotFoundException(nameof(Survey), referenceNumber);

        return mappingService.MapToSurveyDto(survey);
    }

    public async Task<(IReadOnlyList<SurveyDto> Items, int TotalCount)> SearchAsync(
        SurveySearchRequest request, CancellationToken ct = default)
    {
        System.Linq.Expressions.Expression<Func<Survey, bool>>? predicate = null;

        // Build a composite predicate from search filters
        if (!string.IsNullOrWhiteSpace(request.ReferenceNumber))
            predicate = s => s.ReferenceNumber.Contains(request.ReferenceNumber);
        if (!string.IsNullOrWhiteSpace(request.PostCode))
            predicate = CombineOr(predicate, s => s.PostCode != null && s.PostCode.Contains(request.PostCode));
        if (!string.IsNullOrWhiteSpace(request.LocalAuthority))
            predicate = CombineOr(predicate, s => s.LocalAuthority != null && s.LocalAuthority.Contains(request.LocalAuthority));
        if (!string.IsNullOrWhiteSpace(request.Status))
            predicate = CombineOr(predicate, s => s.Status == request.Status);
        if (!string.IsNullOrWhiteSpace(request.SurveyType))
            predicate = CombineOr(predicate, s => s.SurveyType == request.SurveyType);

        // Add soft-delete filter
        var basePredicate = predicate;
        predicate = basePredicate is null
            ? s => !s.IsDeleted
            : CombineAnd(basePredicate, s => !s.IsDeleted);

        var (items, totalCount) = await surveyRepository.GetPagedAsync(
            request.Page, request.PageSize, predicate, orderBy: s => s.CreatedAt,
            descending: request.Descending, ct: ct);

        return (items.Select(mappingService.MapToSurveyDto).ToList().AsReadOnly(), totalCount);
    }

    public async Task<SurveyDto> CreateAsync(CreateSurveyRequest request, string? userId = null, CancellationToken ct = default)
    {
        // Check for duplicate reference number
        if (await surveyRepository.ExistsAsync(s => s.ReferenceNumber == request.ReferenceNumber && !s.IsDeleted, ct))
            throw new ConflictException($"Survey with reference '{request.ReferenceNumber}' already exists.");

        var survey = mappingService.MapToSurvey(request);
        survey.CreatedBy = userId;

        await surveyRepository.AddAsync(survey, ct);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Created survey {SurveyId} with reference {Reference}", survey.Id, survey.ReferenceNumber);

        return mappingService.MapToSurveyDto(survey);
    }

    public async Task<SurveyDto> UpdateAsync(Guid id, UpdateSurveyRequest request, string? userId = null, CancellationToken ct = default)
    {
        var survey = await surveyRepository.GetWithDetailsAsync(id, ct)
                     ?? throw new NotFoundException(nameof(Survey), id);

        // Apply partial updates
        if (request.PropertyAddress is not null) survey.PropertyAddress = request.PropertyAddress;
        if (request.PostCode is not null) survey.PostCode = request.PostCode;
        if (request.LocalAuthority is not null) survey.LocalAuthority = request.LocalAuthority;
        if (request.SurveyType is not null) survey.SurveyType = request.SurveyType;
        if (request.SurveyDate.HasValue) survey.SurveyDate = request.SurveyDate.Value;
        if (request.Status is not null) survey.Status = request.Status;
        if (request.Surveyor is not null) survey.Surveyor = request.Surveyor;
        if (request.Notes is not null) survey.Notes = request.Notes;
        if (request.AssessedValue.HasValue) survey.AssessedValue = request.AssessedValue;
        if (request.FloorArea.HasValue) survey.FloorArea = request.FloorArea;
        if (request.FloorAreaUnit is not null) survey.FloorAreaUnit = request.FloorAreaUnit;
        if (request.PropertyType is not null) survey.PropertyType = request.PropertyType;
        if (request.PropertySubType is not null) survey.PropertySubType = request.PropertySubType;

        survey.UpdatedAt = DateTimeOffset.UtcNow;
        survey.UpdatedBy = userId;

        surveyRepository.Update(survey);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Updated survey {SurveyId}", survey.Id);

        return mappingService.MapToSurveyDto(survey);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var survey = await surveyRepository.GetByIdAsync(id, ct)
                     ?? throw new NotFoundException(nameof(Survey), id);

        // Soft delete
        survey.IsDeleted = true;
        survey.UpdatedAt = DateTimeOffset.UtcNow;
        surveyRepository.Update(survey);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Soft-deleted survey {SurveyId}", survey.Id);
    }

    // Helper methods for combining expressions
    private static System.Linq.Expressions.Expression<Func<Survey, bool>> CombineAnd(
        System.Linq.Expressions.Expression<Func<Survey, bool>> left,
        System.Linq.Expressions.Expression<Func<Survey, bool>> right)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(Survey));
        var body = System.Linq.Expressions.Expression.AndAlso(
            System.Linq.Expressions.Expression.Invoke(left, parameter),
            System.Linq.Expressions.Expression.Invoke(right, parameter));
        return System.Linq.Expressions.Expression.Lambda<Func<Survey, bool>>(body, parameter);
    }

    private static System.Linq.Expressions.Expression<Func<Survey, bool>>? CombineOr(
        System.Linq.Expressions.Expression<Func<Survey, bool>>? left,
        System.Linq.Expressions.Expression<Func<Survey, bool>> right)
    {
        if (left is null) return right;
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(Survey));
        var body = System.Linq.Expressions.Expression.AndAlso(
            System.Linq.Expressions.Expression.Invoke(left, parameter),
            System.Linq.Expressions.Expression.Invoke(right, parameter));
        return System.Linq.Expressions.Expression.Lambda<Func<Survey, bool>>(body, parameter);
    }
}
