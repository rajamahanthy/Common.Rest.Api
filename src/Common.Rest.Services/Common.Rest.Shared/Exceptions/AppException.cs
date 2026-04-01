using System.Net;

namespace Common.Rest.Shared.Exceptions;

/// <summary>
/// Base application exception with HTTP status code mapping.
/// </summary>
public class AppException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError, Exception? innerException = null)
    : Exception(message, innerException)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}

public class NotFoundException(string entity, object key)
    : AppException($"{entity} with key '{key}' was not found.", HttpStatusCode.NotFound);

public class ConflictException(string message)
    : AppException(message, HttpStatusCode.Conflict);

public class ValidationException(string message, IReadOnlyList<string>? errors = null)
    : AppException(message, HttpStatusCode.BadRequest)
{
    public IReadOnlyList<string> ValidationErrors { get; } = errors ?? [];
}

public class ForbiddenException(string message = "You do not have permission to perform this action.")
    : AppException(message, HttpStatusCode.Forbidden);
