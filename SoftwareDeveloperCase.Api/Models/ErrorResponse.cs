namespace SoftwareDeveloperCase.Api.Models;

/// <summary>
/// Represents a consistent error response structure for API errors.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Gets or sets the error title.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the HTTP status code.
    /// </summary>
    public required int Status { get; set; }

    /// <summary>
    /// Gets or sets the error detail message.
    /// </summary>
    public required string Detail { get; set; }

    /// <summary>
    /// Gets or sets the trace identifier for debugging purposes.
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// Gets or sets validation errors if applicable.
    /// </summary>
    public IDictionary<string, string[]>? Errors { get; set; }
}
