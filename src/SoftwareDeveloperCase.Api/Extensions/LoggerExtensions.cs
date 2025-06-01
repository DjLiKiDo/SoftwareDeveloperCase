using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Services;
using System.Linq;

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
        var sanitizedInput = InputSanitizer.SanitizeForLogging(userInput);
        logger.LogInformation(message, sanitizedInput);
    }

    /// <summary>
    /// Logs information with multiple parameters safely
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="message">The message template</param>
    /// <param name="args">Parameters for the message template</param>
    public static void SafeInformation(this ILogger logger, string message, params object?[] args)
    {
        // Sanitize any string arguments that might contain user input
        var sanitizedArgs = args.Select(arg => 
            arg is string str ? InputSanitizer.SanitizeForLogging(str) : arg).ToArray();
        logger.LogInformation(message, sanitizedArgs);
    }

    /// <summary>
    /// Logs warning with sanitized user input to prevent log injection
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="message">The message template</param>
    /// <param name="userInput">User-provided input that will be sanitized before logging</param>
    public static void SafeWarning(this ILogger logger, string message, string? userInput)
    {
        var sanitizedInput = InputSanitizer.SanitizeForLogging(userInput);
        logger.LogWarning(message, sanitizedInput);
    }

    /// <summary>
    /// Logs warning with multiple parameters safely
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="message">The message template</param>
    /// <param name="args">Parameters for the message template</param>
    public static void SafeWarning(this ILogger logger, string message, params object?[] args)
    {
        var sanitizedArgs = args.Select(arg => 
            arg is string str ? InputSanitizer.SanitizeForLogging(str) : arg).ToArray();
        logger.LogWarning(message, sanitizedArgs);
    }

    /// <summary>
    /// Logs error with sanitized user input to prevent log injection
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="message">The message template</param>
    /// <param name="userInput">User-provided input that will be sanitized before logging</param>
    public static void SafeError(this ILogger logger, string message, string? userInput)
    {
        var sanitizedInput = InputSanitizer.SanitizeForLogging(userInput);
        logger.LogError(message, sanitizedInput);
    }

    /// <summary>
    /// Logs error with multiple parameters safely
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="message">The message template</param>
    /// <param name="args">Parameters for the message template</param>
    public static void SafeError(this ILogger logger, string message, params object?[] args)
    {
        var sanitizedArgs = args.Select(arg => 
            arg is string str ? InputSanitizer.SanitizeForLogging(str) : arg).ToArray();
        logger.LogError(message, sanitizedArgs);
    }

    /// <summary>
    /// Logs debug information with sanitized user input to prevent log injection
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="message">The message template</param>
    /// <param name="userInput">User-provided input that will be sanitized before logging</param>
    public static void SafeDebug(this ILogger logger, string message, string? userInput)
    {
        var sanitizedInput = InputSanitizer.SanitizeForLogging(userInput);
        logger.LogDebug(message, sanitizedInput);
    }

    /// <summary>
    /// Logs debug information with multiple parameters safely
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="message">The message template</param>
    /// <param name="args">Parameters for the message template</param>
    public static void SafeDebug(this ILogger logger, string message, params object?[] args)
    {
        var sanitizedArgs = args.Select(arg => 
            arg is string str ? InputSanitizer.SanitizeForLogging(str) : arg).ToArray();
        logger.LogDebug(message, sanitizedArgs);
    }
}
