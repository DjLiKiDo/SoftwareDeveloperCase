#nullable enable

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SoftwareDeveloperCase.Application.Models;

namespace SoftwareDeveloperCase.Api.Middleware;

/// <summary>
/// Middleware that transforms Result{T} responses into appropriate HTTP responses
/// </summary>
public class ResultResponseMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResultResponseMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the ResultResponseMiddleware class
    /// </summary>
    /// <param name="next">The next middleware in the pipeline</param>
    /// <param name="logger">The logger</param>
    /// <param name="environment">The web host environment</param>
    public ResultResponseMiddleware(RequestDelegate next, ILogger<ResultResponseMiddleware> logger, IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Invokes the middleware
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        // Only process if response hasn't been started and we have a Result response
        if (!context.Response.HasStarted && context.Items.TryGetValue("Result", out var result) && result != null)
        {
            await HandleResultAsync(context, result);
        }
    }

    /// <summary>
    /// Handles the Result response and transforms it to appropriate HTTP response
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <param name="result">The result object</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task HandleResultAsync(HttpContext context, object result)
    {
        context.Response.ContentType = "application/json";

        switch (result)
        {
            case Result baseResult:
                await HandleBaseResultAsync(context, baseResult);
                break;
            default:
                // Handle generic Result<T>
                await HandleGenericResultAsync(context, result);
                break;
        }
    }

    /// <summary>
    /// Handles base Result responses
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <param name="result">The base result</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task HandleBaseResultAsync(HttpContext context, Result result)
    {
        if (result.IsSuccess)
        {
            context.Response.StatusCode = 204; // No Content

            if (result.Warnings?.Any() == true)
            {
                var successResponse = new
                {
                    success = true,
                    warnings = result.Warnings
                };

                var jsonResponse = JsonSerializer.Serialize(successResponse, GetJsonOptions());
                await context.Response.WriteAsync(jsonResponse);
            }
            return;
        }

        await HandleFailureResultAsync(context, result);
    }

    /// <summary>
    /// Handles generic Result{T} responses
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <param name="result">The generic result</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task HandleGenericResultAsync(HttpContext context, object result)
    {
        var resultType = result.GetType();
        var isSuccessProperty = resultType.GetProperty("IsSuccess");
        var valueProperty = resultType.GetProperty("Value");
        var errorProperty = resultType.GetProperty("Error");
        var validationErrorsProperty = resultType.GetProperty("ValidationErrors");
        var warningsProperty = resultType.GetProperty("Warnings");

        var isSuccess = (bool)(isSuccessProperty?.GetValue(result) ?? false);

        if (isSuccess)
        {
            var value = valueProperty?.GetValue(result);
            var warnings = warningsProperty?.GetValue(result) as IList<string>;

            if (value != null)
            {
                context.Response.StatusCode = 200; // OK

                object responseObject = value;

                if (warnings?.Any() == true)
                {
                    responseObject = new
                    {
                        data = value,
                        warnings = warnings
                    };
                }

                var jsonResponse = JsonSerializer.Serialize(responseObject, GetJsonOptions());
                await context.Response.WriteAsync(jsonResponse);
            }
            else
            {
                context.Response.StatusCode = 204; // No Content

                if (warnings?.Any() == true)
                {
                    var successResponse = new
                    {
                        success = true,
                        warnings = warnings
                    };

                    var jsonResponse = JsonSerializer.Serialize(successResponse, GetJsonOptions());
                    await context.Response.WriteAsync(jsonResponse);
                }
            }
            return;
        }

        // Handle failure
        var error = errorProperty?.GetValue(result) as string;
        var validationErrors = validationErrorsProperty?.GetValue(result) as IDictionary<string, string[]>;
        var resultWarnings = warningsProperty?.GetValue(result) as IList<string>;

        var baseResult = validationErrors?.Any() == true
            ? Result.Invalid(validationErrors, resultWarnings)
            : Result.Failure(error ?? "An error occurred", resultWarnings);
        await HandleFailureResultAsync(context, baseResult);
    }

    /// <summary>
    /// Handles failure results and creates appropriate error responses
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <param name="result">The failed result</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task HandleFailureResultAsync(HttpContext context, Result result)
    {
        var problemDetails = CreateProblemDetails(context, result);

        context.Response.StatusCode = problemDetails.Status ?? 500;

        _logger.LogWarning("Returning error response: {Status} - {Title} - TraceId: {TraceId}",
            problemDetails.Status, problemDetails.Title, problemDetails.Extensions["traceId"]);

        var jsonResponse = JsonSerializer.Serialize(problemDetails, GetJsonOptions());
        await context.Response.WriteAsync(jsonResponse);
    }

    /// <summary>
    /// Creates a ProblemDetails object from a failed Result
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <param name="result">The failed result</param>
    /// <returns>A ProblemDetails object</returns>
    private ProblemDetails CreateProblemDetails(HttpContext context, Result result)
    {
        // Determine the type of error and appropriate status code
        var (statusCode, title, type) = DetermineErrorType(result);

        var problemDetails = new ProblemDetails
        {
            Type = type,
            Title = title,
            Status = statusCode,
            Detail = result.Error ?? "An error occurred",
            Instance = context.Request.Path,
            Extensions =
            {
                ["traceId"] = context.TraceIdentifier
            }
        };

        // Add validation errors if present
        if (result.ValidationErrors?.Any() == true)
        {
            problemDetails.Extensions["errors"] = result.ValidationErrors;
        }

        // Add warnings if present
        if (result.Warnings?.Any() == true)
        {
            problemDetails.Extensions["warnings"] = result.Warnings;
        }

        return problemDetails;
    }

    /// <summary>
    /// Determines the error type, status code, and title based on the result
    /// </summary>
    /// <param name="result">The failed result</param>
    /// <returns>A tuple containing status code, title, and type</returns>
    private static (int statusCode, string title, string type) DetermineErrorType(Result result)
    {
        // Check for validation errors
        if (result.ValidationErrors?.Any() == true)
        {
            return (400, "Validation Failed", "https://tools.ietf.org/html/rfc7231#section-6.5.1");
        }

        // Check for not found based on error message patterns
        if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true ||
            result.Error?.Contains("does not exist", StringComparison.OrdinalIgnoreCase) == true)
        {
            return (404, "Resource Not Found", "https://tools.ietf.org/html/rfc7231#section-6.5.4");
        }

        // Check for business rule violations based on common patterns
        if (result.Error?.Contains("cannot", StringComparison.OrdinalIgnoreCase) == true ||
            result.Error?.Contains("not allowed", StringComparison.OrdinalIgnoreCase) == true ||
            result.Error?.Contains("invalid operation", StringComparison.OrdinalIgnoreCase) == true ||
            result.Error?.Contains("business rule", StringComparison.OrdinalIgnoreCase) == true)
        {
            return (422, "Business Rule Violation", "https://tools.ietf.org/html/rfc4918#section-11.2");
        }

        // Default to bad request
        return (400, "Bad Request", "https://tools.ietf.org/html/rfc7231#section-6.5.1");
    }

    /// <summary>
    /// Gets JSON serialization options
    /// </summary>
    /// <returns>JsonSerializerOptions</returns>
    private JsonSerializerOptions GetJsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };
    }
}
