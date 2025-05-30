using System.Text.Json.Serialization;
using System.Text.Json;

namespace SoftwareDeveloperCase.Test.Unit.Models;

/// <summary>
/// Test model to deserialize ProblemDetails responses for testing.
/// This matches the RFC 7807 ProblemDetails format.
/// </summary>
public class TestProblemDetails
{
    /// <summary>
    /// A URI reference that identifies the problem type.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// A short, human-readable summary of the problem type.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// The HTTP status code.
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// A human-readable explanation specific to this occurrence.
    /// </summary>
    public string? Detail { get; set; }

    /// <summary>
    /// A URI reference that identifies the specific occurrence.
    /// </summary>
    public string? Instance { get; set; }

    /// <summary>
    /// Gets or sets the trace identifier for debugging purposes.
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// Gets or sets validation errors if applicable.
    /// </summary>
    public IDictionary<string, string[]>? Errors { get; set; }
}
