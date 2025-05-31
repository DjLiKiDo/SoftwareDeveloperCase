using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SoftwareDeveloperCase.Application.Features.Projects.Commands.DeleteProject;

/// <summary>
/// Handler for processing delete project commands
/// </summary>
public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, bool>
{
    private readonly ILogger<DeleteProjectCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the DeleteProjectCommandHandler
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="unitOfWork">The unit of work instance</param>
    public DeleteProjectCommandHandler(ILogger<DeleteProjectCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Handles the delete project command
    /// </summary>
    /// <param name="request">The delete project command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the project was deleted successfully</returns>
    public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting project with ID: {ProjectId}", request.Id);

        var project = await _unitOfWork.ProjectRepository.GetByIdAsync(request.Id, cancellationToken);
        if (project == null)
        {
            _logger.LogWarning("Project not found with ID: {ProjectId}", request.Id);
            throw new NotFoundException($"Project with ID {request.Id} not found");
        }

        // Check if there are associated tasks
        var associatedTasks = await _unitOfWork.TaskRepository.GetAsync(t => t.ProjectId == request.Id, cancellationToken);
        if (associatedTasks.Any())
        {
            _logger.LogWarning("Cannot delete project with ID: {ProjectId} as it has associated tasks", request.Id);
            throw new BusinessRuleViolationException("Cannot delete project as it has associated tasks");
        }

        await _unitOfWork.ProjectRepository.DeleteAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Project deleted successfully with ID: {ProjectId}", request.Id);

        return true;
    }
}
