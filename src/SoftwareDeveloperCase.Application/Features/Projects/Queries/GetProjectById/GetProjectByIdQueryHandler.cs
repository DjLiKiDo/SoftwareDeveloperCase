using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.DTOs.Projects;
using SoftwareDeveloperCase.Application.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SoftwareDeveloperCase.Application.Features.Projects.Queries.GetProjectById;

/// <summary>
/// Handler for processing get project by ID queries
/// </summary>
public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto>
{
    private readonly ILogger<GetProjectByIdQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the GetProjectByIdQueryHandler
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="unitOfWork">The unit of work instance</param>
    public GetProjectByIdQueryHandler(
        ILogger<GetProjectByIdQueryHandler> logger,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Handles the get project by ID query
    /// </summary>
    /// <param name="request">The get project by ID query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The project DTO</returns>
    public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting project with ID: {ProjectId}", request.Id);

        var project = await _unitOfWork.ProjectRepository.GetByIdAsync(request.Id, cancellationToken);
        if (project == null)
        {
            _logger.LogWarning("Project not found with ID: {ProjectId}", request.Id);
            throw new NotFoundException($"Project with ID {request.Id} not found");
        }

        var projectDto = _mapper.Map<ProjectDto>(project);
        
        // If there's a team associated with the project, get the team name
        if (project.TeamId != Guid.Empty)
        {
            var team = await _unitOfWork.TeamRepository.GetByIdAsync(project.TeamId, cancellationToken);
            if (team != null)
            {
                projectDto.TeamName = team.Name;
            }
        }

        return projectDto;
    }
}
