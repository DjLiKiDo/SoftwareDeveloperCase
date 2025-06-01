using MediatR;
using System;

namespace SoftwareDeveloperCase.Application.Features.Projects.Commands.CreateProject;

/// <summary>
/// Command to create a new project
/// </summary>
public class CreateProjectCommand : IRequest<Guid>
{
    /// <summary>
    /// Gets or sets the name of the project.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the project.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the team ID for the project.
    /// </summary>
    public Guid TeamId { get; set; }

    /// <summary>
    /// Gets or sets the project priority.
    /// </summary>
    public Domain.Enums.Core.Priority Priority { get; set; } = Domain.Enums.Core.Priority.Medium;

    /// <summary>
    /// Gets or sets the project start date.
    /// </summary>
    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the project end date.
    /// </summary>
    public DateTime? EndDate { get; set; }
}
