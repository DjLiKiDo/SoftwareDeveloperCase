namespace SoftwareDeveloperCase.Application.DTOs.Common;

/// <summary>
/// Base DTO for auditable entities
/// </summary>
public abstract class AuditableDto
{
    /// <summary>
    /// Gets or sets the entity identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who created the entity
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last updated the entity
    /// </summary>
    public Guid? UpdatedBy { get; set; }
}
