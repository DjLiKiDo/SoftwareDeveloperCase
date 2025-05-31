using MediatR;
using SoftwareDeveloperCase.Application.DTOs.Projects;
using System;

namespace SoftwareDeveloperCase.Application.Features.Projects.Queries.GetProjectById;

/// <summary>
/// Query to get a project by its ID
/// </summary>
public class GetProjectByIdQuery : IRequest<ProjectDto>
{
    /// <summary>
    /// Gets or sets the project ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Initializes a new instance of the GetProjectByIdQuery class
    /// </summary>
    /// <param name="id">Project ID</param>
    public GetProjectByIdQuery(Guid id)
    {
        Id = id;
    }
}
