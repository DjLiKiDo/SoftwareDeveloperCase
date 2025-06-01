using System.Text.RegularExpressions;
using System.Web;
using Ganss.Xss;

namespace SoftwareDeveloperCase.Application.Services;

/// <summary>
/// Service for sanitizing user input to prevent security vulnerabilities such as XSS, SQL injection, etc.
/// </summary>
public static class InputSanitizer
{
    private static readonly HtmlSanitizer HtmlSanitizerInstance = new();

    static InputSanitizer()
    {
        // Configure the HTML sanitizer with safe tags and attributes
        HtmlSanitizerInstance.AllowedTags.Clear();
        HtmlSanitizerInstance.AllowedTags.Add("p");
        HtmlSanitizerInstance.AllowedTags.Add("br");
        HtmlSanitizerInstance.AllowedTags.Add("strong");
        HtmlSanitizerInstance.AllowedTags.Add("em");
        HtmlSanitizerInstance.AllowedTags.Add("u");
        HtmlSanitizerInstance.AllowedTags.Add("ul");
        HtmlSanitizerInstance.AllowedTags.Add("ol");
        HtmlSanitizerInstance.AllowedTags.Add("li");
        HtmlSanitizerInstance.AllowedTags.Add("a");
        HtmlSanitizerInstance.AllowedTags.Add("h1");
        HtmlSanitizerInstance.AllowedTags.Add("h2");
        HtmlSanitizerInstance.AllowedTags.Add("h3");
        HtmlSanitizerInstance.AllowedTags.Add("h4");
        HtmlSanitizerInstance.AllowedTags.Add("h5");
        HtmlSanitizerInstance.AllowedTags.Add("h6");
        
        HtmlSanitizerInstance.AllowedAttributes.Clear();
        HtmlSanitizerInstance.AllowedAttributes.Add("href");
        HtmlSanitizerInstance.AllowedAttributes.Add("target");
        
        // Ensure only http/https links are allowed
        HtmlSanitizerInstance.AllowedSchemes.Clear();
        HtmlSanitizerInstance.AllowedSchemes.Add("http");
        HtmlSanitizerInstance.AllowedSchemes.Add("https");
    }
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

        // Remove HTML tags first
        var withoutHtml = Regex.Replace(input, @"<[^>]*>", string.Empty, RegexOptions.Compiled);
        
        // Remove potentially dangerous characters, leaving only alphanumeric, spaces, and common punctuation
        return Regex.Replace(
            withoutHtml,
            @"[^\w\s\-.,?!:;()[\]{}@#$%&*+=/\\]",
            string.Empty,
            RegexOptions.Compiled);
    }

    /// <summary>
    /// Sanitizes a string intended for use as a filename following OWASP security best practices
    /// </summary>
    /// <param name="input">The input string to sanitize</param>
    /// <returns>Sanitized string safe for use as a filename or null if input was null</returns>
    public static string? SanitizeFileName(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var sanitized = input.Trim();

        // 1. Handle directory traversal patterns - replace with single underscore
        sanitized = Regex.Replace(sanitized, @"\.{2,}", "_", RegexOptions.Compiled);

        // 2. Remove or replace path separators completely (security critical)
        sanitized = sanitized.Replace('/', '_').Replace('\\', '_');

        // 3. Handle Windows reserved characters: < > : " | ? * and all control characters (0-31)
        // Using regex for better performance and readability
        sanitized = Regex.Replace(sanitized, @"[<>:""|?*\x00-\x1f\x7f]", "_", RegexOptions.Compiled);

        // 4. Handle Windows reserved names (case-insensitive)
        var reservedNames = new[] { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(sanitized);
        var extension = Path.GetExtension(sanitized);
        
        if (reservedNames.Contains(nameWithoutExtension.ToUpperInvariant()))
        {
            nameWithoutExtension = "_" + nameWithoutExtension;
            sanitized = nameWithoutExtension + extension;
        }

        // 5. Remove leading/trailing dots and spaces (Windows compatibility)
        sanitized = sanitized.Trim('.', ' ');

        // 6. Ensure the filename is not empty after sanitization
        if (string.IsNullOrEmpty(sanitized))
        {
            sanitized = "sanitized_file";
        }

        // 7. Limit length to 255 characters (most filesystem limit)
        if (sanitized.Length > 255)
        {
            var ext = Path.GetExtension(sanitized);
            var nameOnly = Path.GetFileNameWithoutExtension(sanitized);
            var maxNameLength = 255 - ext.Length;
            sanitized = nameOnly.Substring(0, Math.Max(1, maxNameLength)) + ext;
        }

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

        // Use the HtmlSanitizer library for proper HTML sanitization
        return HtmlSanitizerInstance.Sanitize(htmlInput);
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

        // Keep only valid email characters (including + for email aliases)
        return Regex.Replace(email, @"[^\w@.\-+]", string.Empty, RegexOptions.Compiled);
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

    /// <summary>
    /// Sanitizes a string input specifically for logging to prevent log injection attacks
    /// </summary>
    /// <param name="input">The input string to sanitize</param>
    /// <returns>Sanitized string safe for logging or null if input was null</returns>
    public static string? SanitizeForLogging(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // First apply basic string sanitization
        var sanitized = SanitizeString(input);
        
        if (string.IsNullOrEmpty(sanitized))
        {
            return sanitized;
        }

        // Remove newline characters that can be used for log injection
        sanitized = sanitized.Replace("\r\n", " ")
                             .Replace("\r", " ")
                             .Replace("\n", " ");

        // Remove other potential log injection vectors
        sanitized = sanitized.Replace("\t", " ");

        // Collapse multiple spaces into single space
        sanitized = Regex.Replace(sanitized, @"\s+", " ", RegexOptions.Compiled).Trim();

        return sanitized;
    }

    /// <summary>
    /// Sanitizes input to prevent LDAP injection attacks
    /// </summary>
    /// <param name="input">The input string to sanitize for LDAP queries</param>
    /// <returns>Sanitized string safe for LDAP queries or null if input was null</returns>
    public static string? SanitizeLdap(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Escape LDAP special characters
        var ldapEscapes = new Dictionary<char, string>
        {
            { '\\', @"\5c" },
            { '*', @"\2a" },
            { '(', @"\28" },
            { ')', @"\29" },
            { '\0', @"\00" }
        };

        var sanitized = input;
        foreach (var kvp in ldapEscapes)
        {
            sanitized = sanitized.Replace(kvp.Key.ToString(), kvp.Value);
        }

        return sanitized;
    }

    /// <summary>
    /// Sanitizes input to prevent JSON injection attacks
    /// </summary>
    /// <param name="input">The input string to sanitize for JSON</param>
    /// <returns>Sanitized string safe for JSON or null if input was null</returns>
    public static string? SanitizeJson(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Escape JSON special characters
        var sanitized = input
            .Replace("\\", "\\\\")  // Escape backslashes first
            .Replace("\"", "\\\"")  // Escape quotes
            .Replace("\b", "\\b")   // Escape backspace
            .Replace("\f", "\\f")   // Escape form feed
            .Replace("\n", "\\n")   // Escape newline
            .Replace("\r", "\\r")   // Escape carriage return
            .Replace("\t", "\\t");  // Escape tab

        // Remove or escape control characters
        sanitized = Regex.Replace(sanitized, @"[\x00-\x1f\x7f]", string.Empty, RegexOptions.Compiled);

        return sanitized;
    }

    /// <summary>
    /// Sanitizes input to prevent NoSQL injection attacks
    /// </summary>
    /// <param name="input">The input string to sanitize for NoSQL queries</param>
    /// <returns>Sanitized string safe for NoSQL queries or null if input was null</returns>
    public static string? SanitizeNoSql(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Remove potential NoSQL injection patterns
        var dangerousPatterns = new[]
        {
            @"\$where",
            @"\$regex",
            @"\$ne",
            @"\$gt",
            @"\$lt",
            @"\$in",
            @"\$nin",
            @"\$or",
            @"\$and",
            @"\$not",
            @"\$nor",
            @"\$exists",
            @"\$type",
            @"\$mod",
            @"\$all",
            @"\$size",
            @"\$elemMatch",
            @"javascript:",
            @"function\s*\(",
            @"eval\s*\(",
            @"setTimeout\s*\(",
            @"setInterval\s*\("
        };

        var sanitized = input;
        foreach (var pattern in dangerousPatterns)
        {
            sanitized = Regex.Replace(sanitized, pattern, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        return sanitized;
    }

    /// <summary>
    /// Sanitizes input to prevent command injection attacks
    /// </summary>
    /// <param name="input">The input string to sanitize for command execution</param>
    /// <returns>Sanitized string safe for command execution or null if input was null</returns>
    public static string? SanitizeCommand(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Remove or escape dangerous command characters
        var dangerousChars = new[] { ';', '&', '|', '`', '$', '(', ')', '<', '>', '\n', '\r' };
        
        var sanitized = input;
        foreach (var dangerousChar in dangerousChars)
        {
            sanitized = sanitized.Replace(dangerousChar.ToString(), string.Empty);
        }

        // Remove dangerous command patterns
        var dangerousPatterns = new[]
        {
            @"rm\s+",
            @"del\s+",
            @"format\s+",
            @"shutdown\s+",
            @"reboot\s+",
            @"sudo\s+",
            @"su\s+",
            @"chmod\s+",
            @"chown\s+",
            @"cat\s+/etc/",
            @"net\s+user",
            @"net\s+localgroup"
        };

        foreach (var pattern in dangerousPatterns)
        {
            sanitized = Regex.Replace(sanitized, pattern, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        return sanitized;
    }

    /// <summary>
    /// Sanitizes input to prevent template injection attacks
    /// </summary>
    /// <param name="input">The input string to sanitize for template engines</param>
    /// <returns>Sanitized string safe for template engines or null if input was null</returns>
    public static string? SanitizeTemplate(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Remove template engine syntax that could be dangerous
        var templatePatterns = new[]
        {
            @"\{\{.*?\}\}",      // Handlebars/Mustache
            @"\{%.*?%\}",        // Jinja2/Twig
            @"\{\{.*?\}\}",      // Angular
            @"<%.*?%>",          // ASP.NET/JSP
            @"@\{.*?\}",         // Razor
            @"\$\{.*?\}",        // Various template engines
            @"#\{.*?\}"          // Ruby ERB style
        };

        var sanitized = input;
        foreach (var pattern in templatePatterns)
        {
            sanitized = Regex.Replace(sanitized, pattern, string.Empty, RegexOptions.Compiled | RegexOptions.Singleline);
        }

        return sanitized;
    }
}
