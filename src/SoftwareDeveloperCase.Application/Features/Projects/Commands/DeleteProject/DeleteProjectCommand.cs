using MediatR;
using System;

namespace SoftwareDeveloperCase.Application.Features.Projects.Commands.DeleteProject;

/// <summary>
/// Command to delete a project
/// </summary>
public class DeleteProjectCommand : IRequest<bool>
{
    /// <summary>
    /// Gets or sets the project ID to delete
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Initializes a new instance of the DeleteProjectCommand class
    /// </summary>
    public DeleteProjectCommand()
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the DeleteProjectCommand class with a project ID
    /// </summary>
    /// <param name="id">Project ID to delete</param>
    public DeleteProjectCommand(Guid id)
    {
        Id = id;
    }
}
