using SoftwareDeveloperCase.Api.Models;
using SoftwareDeveloperCase.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace SoftwareDeveloperCase.Api.Middleware;

/// <summary>
/// Global exception handling middleware that catches all unhandled exceptions
/// and returns consistent error responses.
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionHandlingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger for recording exceptions.</param>
    /// <param name="environment">The web host environment for determining error detail level.</param>
    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Invokes the middleware to handle exceptions.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing the request. TraceId: {TraceId}",
                context.TraceIdentifier);

            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles the exception and creates an appropriate HTTP response.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            ValidationException validationEx => new ErrorResponse
            {
                Title = "Validation Failed",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = "One or more validation errors occurred.",
                TraceId = context.TraceIdentifier,
                Errors = validationEx.Errors
            },
            NotFoundException notFoundEx => new ErrorResponse
            {
                Title = "Resource Not Found",
                Status = (int)HttpStatusCode.NotFound,
                Detail = notFoundEx.Message,
                TraceId = context.TraceIdentifier
            },
            ArgumentNullException argumentNullEx => new ErrorResponse
            {
                Title = "Invalid Argument",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = _environment.IsDevelopment() ? argumentNullEx.Message : "Required parameter is missing.",
                TraceId = context.TraceIdentifier
            },
            ArgumentException argumentEx => new ErrorResponse
            {
                Title = "Invalid Argument",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = _environment.IsDevelopment() ? argumentEx.Message : "Invalid request parameters.",
                TraceId = context.TraceIdentifier
            },
            UnauthorizedAccessException => new ErrorResponse
            {
                Title = "Unauthorized",
                Status = (int)HttpStatusCode.Unauthorized,
                Detail = "Access denied.",
                TraceId = context.TraceIdentifier
            },
            _ => new ErrorResponse
            {
                Title = "Internal Server Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = _environment.IsDevelopment()
                    ? exception.Message
                    : "An internal server error occurred. Please try again later.",
                TraceId = context.TraceIdentifier
            }
        };

        context.Response.StatusCode = response.Status;

        _logger.LogWarning("Returning error response: {Status} - {Title} - TraceId: {TraceId}",
            response.Status, response.Title, response.TraceId);

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}
