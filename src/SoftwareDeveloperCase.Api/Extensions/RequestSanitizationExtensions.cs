using Microsoft.AspNetCore.Builder;
using SoftwareDeveloperCase.Api.Middleware;

namespace SoftwareDeveloperCase.Api.Extensions;

/// <summary>
/// Extension methods for registering and using input sanitization middleware
/// </summary>
public static class RequestSanitizationExtensions
{
    /// <summary>
    /// Adds middleware to sanitize request parameters
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <returns>The application builder for method chaining</returns>
    public static IApplicationBuilder UseSanitizeRequestParameters(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestSanitizationMiddleware>();
    }
}
