using SoftwareDeveloperCase.Domain.Common;
using SoftwareDeveloperCase.Domain.Enums.Core;
using TaskEntity = SoftwareDeveloperCase.Domain.Entities.Task.Task;

namespace SoftwareDeveloperCase.Domain.Entities.Project;

/// <summary>
/// Represents a project in the system.
/// </summary>
public class Project : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the project.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the project.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the project status.
    /// </summary>
    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;

    /// <summary>
    /// Gets or sets the project priority.
    /// </summary>
    public Priority Priority { get; set; } = Priority.Medium;

    // TODO: Re-enable ProjectTimeline once compilation issue is resolved
    // /// <summary>
    // /// Gets or sets the project timeline.
    // /// </summary>
    // public ProjectTimeline Timeline { get; set; } = null!;

    /// <summary>
    /// Gets or sets the team identifier assigned to this project.
    /// </summary>
    public Guid TeamId { get; set; }

    /// <summary>
    /// Gets or sets the team assigned to this project.
    /// </summary>
    public Entities.Team.Team Team { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of tasks in this project.
    /// </summary>
    public ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
}
