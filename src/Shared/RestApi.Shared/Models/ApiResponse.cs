namespace RestApi.Shared.Models;

/// <summary>
/// Standard API response wrapper for all microservices.
/// </summary>
public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<string>? Errors { get; init; }
    public string? CorrelationId { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    public static ApiResponse<T> Ok(T data, string? correlationId = null) => new()
    {
        Success = true,
        Data = data,
        CorrelationId = correlationId
    };

    public static ApiResponse<T> Fail(string message, IReadOnlyList<string>? errors = null, string? correlationId = null) => new()
    {
        Success = false,
        Message = message,
        Errors = errors,
        CorrelationId = correlationId
    };
}

/// <summary>
/// Paged response wrapper for list endpoints.
/// </summary>
public sealed class PagedApiResponse<T>
{
    public PagedApiResponse() { }
    public PagedApiResponse(IReadOnlyList<T> data, int page, int pageSize, int totalCount, string? correlationId = null)
    {
        Data = data;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
        CorrelationId = correlationId;
    }

    public bool Success { get; init; } = true;
    public IReadOnlyList<T> Data { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
    public string? CorrelationId { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}