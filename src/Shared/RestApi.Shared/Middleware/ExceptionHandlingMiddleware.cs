using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using RestApi.Shared.Exceptions;

namespace RestApi.Shared.Middleware;

/// <summary>
/// Centralized exception handling middleware for all microservices.
/// Maps AppException subtypes to appropriate HTTP status codes.
/// </summary>
public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IHostEnvironment environment)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.Items["CorrelationId"]?.ToString();

        var (statusCode, message, errors) = exception switch
        {
            Exceptions.ValidationException ve => (HttpStatusCode.BadRequest, ve.Message, ve.ValidationErrors),
            NotFoundException nf => (HttpStatusCode.NotFound, nf.Message, (IReadOnlyList<string>?)null),
            ConflictException ce => (HttpStatusCode.Conflict, ce.Message, null),
            ForbiddenException fe => (HttpStatusCode.Forbidden, fe.Message, null),
            AppException ae => (ae.StatusCode, ae.Message, null),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.", null)
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception. CorrelationId: {CorrelationId}", correlationId);
        }
        else
        {
            logger.LogWarning(exception, "Handled exception ({StatusCode}). CorrelationId: {CorrelationId}", (int)statusCode, correlationId);
        }

        var response = ApiResponse<object>.Fail(
            environment.IsDevelopment() ? exception.Message : message,
            errors,
            correlationId);

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
