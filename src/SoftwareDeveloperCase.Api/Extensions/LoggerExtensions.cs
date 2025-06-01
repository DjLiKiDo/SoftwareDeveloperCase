using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Services;

namespace SoftwareDeveloperCase.Api.Extensions;

/// <summary>
/// Extension methods for the ILogger interface to facilitate safe logging with sanitized inputs
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Logs information with sanitized user input to prevent log injection
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="message">The message template</param>
    /// <param name="userInput">User-provided input that will be sanitized before logging</param>
    public static void SafeInformation(this ILogger logger, string message, string? userInput)
    {
        var sanitizedInput = InputSanitizer.SanitizeForLogging(userInput)?.Replace("\n", "").Replace("\r", "");
        logger.LogInformation(message, sanitizedInput);
    }

    /// <summary>
    /// Logs warning with sanitized user input to prevent log injection
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="message">The message template</param>
    /// <param name="userInput">User-provided input that will be sanitized before logging</param>
    public static void SafeWarning(this ILogger logger, string message, string? userInput)
    {
        var sanitizedInput = InputSanitizer.SanitizeForLogging(userInput)?.Replace("\n", "").Replace("\r", "");
        logger.LogWarning(message, sanitizedInput);
    }

    /// <summary>
    /// Logs error with sanitized user input to prevent log injection
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="message">The message template</param>
    /// <param name="userInput">User-provided input that will be sanitized before logging</param>
    public static void SafeError(this ILogger logger, string message, string? userInput)
    {
        var sanitizedInput = InputSanitizer.SanitizeForLogging(userInput)?.Replace("\n", "").Replace("\r", "");
        logger.LogError(message, sanitizedInput);
    }

    /// <summary>
    /// Logs debug information with sanitized user input to prevent log injection
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="message">The message template</param>
    /// <param name="userInput">User-provided input that will be sanitized before logging</param>
    public static void SafeDebug(this ILogger logger, string message, string? userInput)
    {
        var sanitizedInput = InputSanitizer.SanitizeForLogging(userInput)?.Replace("\n", "").Replace("\r", "");
        logger.LogDebug(message, sanitizedInput);
    }
}
