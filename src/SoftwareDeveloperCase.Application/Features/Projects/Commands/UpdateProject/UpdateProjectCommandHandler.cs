using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Exceptions;
using SoftwareDeveloperCase.Application.Models;

namespace SoftwareDeveloperCase.Application.Features.Projects.Commands.UpdateProject;

/// <summary>
/// Handler for processing update project commands
/// </summary>
public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Result<bool>>
{
    private readonly ILogger<UpdateProjectCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the UpdateProjectCommandHandler
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="unitOfWork">The unit of work instance</param>
    public UpdateProjectCommandHandler(
        ILogger<UpdateProjectCommandHandler> logger,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Handles the update project command
    /// </summary>
    /// <param name="request">The update project command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result indicating success or failure</returns>
    public async Task<Result<bool>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating project with ID: {ProjectId}", request.Id);

        var project = await _unitOfWork.ProjectRepository.GetByIdAsync(request.Id, cancellationToken);
        if (project == null)
        {
            _logger.LogWarning("Project not found with ID: {ProjectId}", request.Id);
            return Result<bool>.NotFound($"Project with ID {request.Id} not found");
        }

        // Update project properties
        project.Name = request.Name;
        project.Description = request.Description;
        project.Status = request.Status;
        project.Priority = request.Priority;

        await _unitOfWork.ProjectRepository.UpdateAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Project updated successfully with ID: {ProjectId}", project.Id);

        return Result<bool>.Success(true);
    }
}
