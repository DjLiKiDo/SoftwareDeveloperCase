using System.Diagnostics;

namespace SoftwareDeveloperCase.Api.Middleware;

/// <summary>
/// Middleware to add request correlation ID for tracking requests across services
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;
    private const string CorrelationIdHeaderName = "X-Correlation-ID";

    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationIdMiddleware"/> class
    /// </summary>
    /// <param name="next">The next middleware in the pipeline</param>
    /// <param name="logger">The logger</param>
    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Processes an HTTP request to add correlation ID
    /// </summary>
    /// <param name="context">The HTTP context</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);

        // Add to response headers
        context.Response.Headers.Append(CorrelationIdHeaderName, correlationId);

        // Add to logging scope
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        });

        // Set in Activity for distributed tracing
        Activity.Current?.SetTag("correlation.id", correlationId);

        await _next(context);
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        // Try to get from request headers first
        if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationId) &&
            !string.IsNullOrEmpty(correlationId))
        {
            return correlationId.ToString();
        }

        // Generate new correlation ID
        return Guid.NewGuid().ToString();
    }
}
