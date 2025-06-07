using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Models;
using SoftwareDeveloperCase.Application.Services;
using SoftwareDeveloperCase.Domain.Entities.Project;
using SoftwareDeveloperCase.Domain.Enums.Core;

namespace SoftwareDeveloperCase.Application.Features.Projects.Commands.CreateProject;

/// <summary>
/// Handler for processing create project commands
/// </summary>
public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Result<Guid>>
{
    private readonly ILogger<CreateProjectCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the CreateProjectCommandHandler
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="unitOfWork">The unit of work instance</param>
    public CreateProjectCommandHandler(ILogger<CreateProjectCommandHandler> logger, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Handles the create project command
    /// </summary>
    /// <param name="request">The create project command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The ID of the created project</returns>
    public async Task<Result<Guid>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new project with name: {ProjectName}", InputSanitizer.SanitizeForLogging(request.Name));

        // Validate team exists (this is also validated by the validator, but added here for safety)
        var teams = await _unitOfWork.TeamRepository.GetAsync(t => t.Id == request.TeamId, cancellationToken);
        if (!teams.Any())
        {
            return Result<Guid>.NotFound($"Team with ID {request.TeamId} not found");
        }

        // Validate unique project name within team (this is also validated by the validator, but added here for safety)
        var nameExists = await _unitOfWork.ProjectRepository
            .IsProjectNameExistsInTeamAsync(request.TeamId, request.Name, null, cancellationToken);
        if (nameExists)
        {
            return Result<Guid>.Failure($"Project name '{request.Name}' already exists in the specified team");
        }

        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            TeamId = request.TeamId,
            Status = ProjectStatus.Planning,
            Priority = request.Priority
        };

        var createdProject = await _unitOfWork.ProjectRepository.InsertAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Project created successfully with ID: {ProjectId}", createdProject.Id);

        // TODO: Publish ProjectCreatedEvent when domain event infrastructure is available
        // TODO: Add user permission validation when current user service is available
        // TODO: Assign creator as project member when team membership logic is available

        return Result<Guid>.Success(createdProject.Id);
    }
}
