namespace Common.Rest.Shared.Dtos;

/// <summary>
/// Generic pagination result containing data and total count.
/// Used by service layer to return paginated results with explicit total count metadata.
/// </summary>
/// <typeparam name="T">The type of items in the data collection.</typeparam>
/// <param name="Data">The collection of paginated items.</param>
/// <param name="TotalCount">The total count of items before pagination.</param>
public sealed record PaginationResult<T>(IReadOnlyList<T> Data, int TotalCount);
