using System.Text.RegularExpressions;
using System.Web;

namespace SoftwareDeveloperCase.Application.Services;

/// <summary>
/// Service for sanitizing user input to prevent security vulnerabilities such as XSS, SQL injection, etc.
/// </summary>
public static class InputSanitizer
{
    /// <summary>
    /// Sanitizes a string input by removing potentially harmful content
    /// </summary>
    /// <param name="input">The input string to sanitize</param>
    /// <returns>Sanitized string or null if input was null</returns>
    public static string? SanitizeString(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Encode HTML entities to prevent XSS
        var sanitized = HttpUtility.HtmlEncode(input);

        // Remove any control characters
        sanitized = Regex.Replace(sanitized, @"[\p{C}]", string.Empty, RegexOptions.Compiled);

        return sanitized;
    }

    /// <summary>
    /// Sanitizes a string allowing only alphanumeric characters, spaces, and common punctuation
    /// </summary>
    /// <param name="input">The input string to sanitize</param>
    /// <returns>Sanitized string or null if input was null</returns>
    public static string? SanitizePlainText(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Remove potentially dangerous characters, leaving only alphanumeric, spaces, and common punctuation
        return Regex.Replace(
            input,
            @"[^\w\s\-.,?!:;()[\]{}@#$%&*+=/\\]",
            string.Empty,
            RegexOptions.Compiled);
    }

    /// <summary>
    /// Sanitizes a string intended for use as a filename
    /// </summary>
    /// <param name="input">The input string to sanitize</param>
    /// <returns>Sanitized string safe for use as a filename or null if input was null</returns>
    public static string? SanitizeFileName(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Replace potentially dangerous characters with underscores
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(input.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());

        // Prevent directory traversal attempts
        sanitized = sanitized.Replace("..", "_");

        return sanitized;
    }

    /// <summary>
    /// Sanitizes HTML content, keeping basic formatting tags while removing potentially harmful scripts and attributes
    /// </summary>
    /// <param name="htmlInput">The HTML input to sanitize</param>
    /// <returns>Sanitized HTML or null if input was null</returns>
    public static string? SanitizeHtml(string? htmlInput)
    {
        if (string.IsNullOrEmpty(htmlInput))
        {
            return htmlInput;
        }

        // This is a basic implementation
        // In a production environment, consider using a library like HtmlSanitizer
        
        // Remove script tags and their contents
        var sanitized = Regex.Replace(htmlInput, @"<script[^>]*>[\s\S]*?</script>", string.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        // Remove inline event handlers
        sanitized = Regex.Replace(sanitized, @"\s(on\w+)=[""][^""]*[""]", string.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        sanitized = Regex.Replace(sanitized, @"\s(on\w+)=[''][^'']*['']", string.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        // Remove iframe, object, embed tags
        sanitized = Regex.Replace(sanitized, @"<(iframe|object|embed)[^>]*>[\s\S]*?</\1>", string.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        // Remove potentially harmful attributes
        sanitized = Regex.Replace(sanitized, @"\s(href|src)\s*=\s*[""]javascript:[^""]*[""]", string.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        sanitized = Regex.Replace(sanitized, @"\s(href|src)\s*=\s*['']javascript:[^'']*['']", string.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        return sanitized;
    }

    /// <summary>
    /// Sanitizes SQL identifiers to prevent SQL injection in dynamic queries
    /// </summary>
    /// <param name="identifier">The SQL identifier to sanitize</param>
    /// <returns>Sanitized SQL identifier or null if input was null</returns>
    public static string? SanitizeSqlIdentifier(string? identifier)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            return identifier;
        }

        // Allow only alphanumeric characters and underscores
        return Regex.Replace(identifier, @"[^\w]", string.Empty, RegexOptions.Compiled);
    }

    /// <summary>
    /// Sanitizes an email address to ensure it's in a valid format
    /// </summary>
    /// <param name="email">The email address to sanitize</param>
    /// <returns>Sanitized email address or null if input was null or invalid</returns>
    public static string? SanitizeEmail(string? email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return email;
        }

        // First trim whitespace
        email = email.Trim();

        // Validate the email follows a basic pattern
        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase))
        {
            return null; // Invalid email format
        }

        // Keep only valid email characters
        return Regex.Replace(email, @"[^\w@.\-]", string.Empty, RegexOptions.Compiled);
    }

    /// <summary>
    /// Sanitizes a URL to prevent URL-based attacks
    /// </summary>
    /// <param name="url">The URL to sanitize</param>
    /// <returns>Sanitized URL or null if input was null or invalid</returns>
    public static string? SanitizeUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return url;
        }

        // First trim whitespace
        url = url.Trim();

        // Try to parse the URL to ensure it's valid
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult) || 
            (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
        {
            return null; // Invalid URL
        }

        return url;
    }

    /// <summary>
    /// Sanitizes a phone number to keep only digits, spaces, and common phone number symbols
    /// </summary>
    /// <param name="phoneNumber">The phone number to sanitize</param>
    /// <returns>Sanitized phone number or null if input was null</returns>
    public static string? SanitizePhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
        {
            return phoneNumber;
        }

        // Keep only digits, spaces, and common phone number symbols
        return Regex.Replace(phoneNumber, @"[^\d\s+\-().]", string.Empty, RegexOptions.Compiled);
    }
}
