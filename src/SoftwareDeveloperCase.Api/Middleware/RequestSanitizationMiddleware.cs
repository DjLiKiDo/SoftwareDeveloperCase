using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SoftwareDeveloperCase.Application.Services;

namespace SoftwareDeveloperCase.Api.Middleware;

/// <summary>
/// Middleware to sanitize query string parameters and form data
/// </summary>
public class RequestSanitizationMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the RequestSanitizationMiddleware
    /// </summary>
    /// <param name="next">The next middleware in the pipeline</param>
    public RequestSanitizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Processes an HTTP request to sanitize query string and form data
    /// </summary>
    /// <param name="context">The HTTP context for the request</param>
    public async Task InvokeAsync(HttpContext context)
    {
        // Sanitize query string parameters
        var sanitizedQuery = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
        foreach (var kvp in context.Request.Query)
        {
            var sanitizedValues = new List<string>();
            foreach (var value in kvp.Value)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    sanitizedValues.Add(InputSanitizer.SanitizeString(value) ?? string.Empty);
                }
                else
                {
                    sanitizedValues.Add(value ?? string.Empty);
                }
            }
            sanitizedQuery[kvp.Key] = new Microsoft.Extensions.Primitives.StringValues(sanitizedValues.ToArray());
        }

        // Replace the query collection with sanitized values
        context.Request.Query = new Microsoft.AspNetCore.Http.QueryCollection(sanitizedQuery);

        // Continue to next middleware
        await _next(context);
    }
}
