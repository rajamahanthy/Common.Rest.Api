namespace Common.Rest.Address.Api.Extensions;

/// <summary>
/// Extension methods for controller operations.
/// </summary>
public static class ControllerExtensions
{
    /// <summary>
    /// Extracts the correlation ID from the HTTP context.
    /// The correlation ID is set by the CorrelationIdMiddleware and is used for distributed tracing.
    /// </summary>
    /// <param name="controller">The controller instance</param>
    /// <returns>The correlation ID as a string, or null if not available</returns>
    public static string? GetCorrelationId(this ControllerBase controller)
    {
        return controller.HttpContext.Items["CorrelationId"]?.ToString();
    }
}
