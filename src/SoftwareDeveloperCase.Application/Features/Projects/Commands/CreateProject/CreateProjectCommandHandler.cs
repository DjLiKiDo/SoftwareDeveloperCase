using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Services;
using SoftwareDeveloperCase.Domain.Entities.Project;
using SoftwareDeveloperCase.Domain.Enums.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SoftwareDeveloperCase.Application.Features.Projects.Commands.CreateProject;

/// <summary>
/// Handler for processing create project commands
/// </summary>
public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Guid>
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
    public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new project with name: {ProjectName}", InputSanitizer.SanitizeForLogging(request.Name));

        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            Status = ProjectStatus.Planning,
            Priority = request.Priority
        };

        var createdProject = await _unitOfWork.ProjectRepository.InsertAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Project created successfully with ID: {ProjectId}", createdProject.Id);

        return createdProject.Id;
    }
}
