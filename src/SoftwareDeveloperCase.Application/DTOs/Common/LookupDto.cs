namespace SoftwareDeveloperCase.Application.DTOs.Common;

/// <summary>
/// Generic DTO for lookup values (reference data)
/// </summary>
public class LookupDto
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the display name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}
