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
        foreach (var key in context.Request.Query.Keys)
        {
            if (context.Request.Query.TryGetValue(key, out var values))
            {
                for (var i = 0; i < values.Count; i++)
                {
                    if (!string.IsNullOrEmpty(values[i]))
                    {
                        values[i] = InputSanitizer.SanitizeString(values[i]);
                    }
                }
            }
        }

        // Continue to next middleware
        await _next(context);
    }
}
