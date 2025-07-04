using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoftwareDeveloperCase.Api.Extensions;
using SoftwareDeveloperCase.Application.Features.Projects.Commands.CreateProject;
using SoftwareDeveloperCase.Application.Features.Projects.Commands.DeleteProject;
using SoftwareDeveloperCase.Application.Features.Projects.Commands.UpdateProject;
using SoftwareDeveloperCase.Application.Features.Projects.Queries.GetProjectById;
using SoftwareDeveloperCase.Application.Features.Projects.Queries.GetProjects;
using SoftwareDeveloperCase.Application.Models;
using CreateTaskRequest = SoftwareDeveloperCase.Application.Features.Tasks.DTOs.CreateTaskRequest;
using ProjectDto = SoftwareDeveloperCase.Application.Features.Projects.DTOs.ProjectDto;
using ProjectStatisticsDto = SoftwareDeveloperCase.Application.Features.Projects.DTOs.ProjectStatisticsDto;
using TaskDto = SoftwareDeveloperCase.Application.Features.Tasks.DTOs.TaskDto;

namespace SoftwareDeveloperCase.Api.Controllers.V1;

/// <summary>
/// Controller for managing projects
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ProjectsController : BaseController
{
    private readonly ILogger<ProjectsController> _logger;
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectsController"/> class
    /// </summary>
    /// <param name="logger">The logger</param>
    /// <param name="mediator">The mediator for handling commands and queries</param>
    public ProjectsController(ILogger<ProjectsController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Gets a paginated list of projects with optional filtering
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="searchTerm">Optional search term for project name or description</param>
    /// <param name="status">Optional project status filter</param>
    /// <param name="teamId">Optional team ID filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of projects</returns>
    [HttpGet]
    [Authorize(Policy = "DeveloperOrManager")]
    [ProducesResponseType(typeof(PagedResult<ProjectDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<PagedResult<ProjectDto>>> GetProjects(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? status = null,
        [FromQuery] Guid? teamId = null,
        CancellationToken cancellationToken = default)
    {
        // Use safe logging extension to prevent log injection
        _logger.SafeInformation("Getting projects with searchTerm: {SearchTerm}", searchTerm);

        // Log other parameters safely
        _logger.SafeInformation("Getting projects with pageNumber: {PageNumber}, pageSize: {PageSize}, status: {Status}, teamId: {TeamId}",
            pageNumber, pageSize, status ?? "null", teamId?.ToString() ?? "null");

        var query = new GetProjectsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            Status = status != null ? Enum.Parse<Domain.Enums.Core.ProjectStatus>(status) : null,
            TeamId = teamId
        };

        var result = await _mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Gets a specific project by ID
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Project details</returns>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "ProjectRead")]
    [ProducesResponseType(typeof(ProjectDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetProject(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting project with ID: {ProjectId}", id);

        var query = new GetProjectByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        return HandleResultAsAction(result);
    }

    /// <summary>
    /// Creates a new project
    /// </summary>
    /// <param name="request">Project creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created project</returns>
    [HttpPost]
    [Authorize(Policy = "ManagerOrAdmin")]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> CreateProject(CreateProjectCommand request, CancellationToken cancellationToken = default)
    {
        _logger.SafeInformation("Creating new project: {ProjectName}", request.Name);

        var result = await _mediator.Send(request, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetProject), new { id = result.Value }, result.Value);
        }

        return HandleResultAsAction(result);
    }

    /// <summary>
    /// Updates an existing project
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <param name="request">Project update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated project</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ProjectUpdate")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> UpdateProject(Guid id, UpdateProjectCommand request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating project with ID: {ProjectId}", id);

        // Ensure the ID in the route matches the ID in the request body
        if (id != request.Id)
        {
            return BadRequest("The project ID in the route must match the ID in the request body.");
        }

        var result = await _mediator.Send(request, cancellationToken);
        return HandleResultAsAction(result);
    }

    /// <summary>
    /// Deletes a project
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> DeleteProject(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting project with ID: {ProjectId}", id);

        var command = new DeleteProjectCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return HandleResultAsAction(result);
    }

    /// <summary>
    /// Gets all tasks for a specific project
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="status">Optional task status filter</param>
    /// <param name="assignedUserId">Optional assigned user ID filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of tasks for the project</returns>
    [HttpGet("{id:guid}/tasks")]
    [Authorize(Policy = "ProjectRead")]
    [ProducesResponseType(typeof(PagedResult<TaskDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<PagedResult<TaskDto>>> GetProjectTasks(
        Guid id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] Guid? assignedUserId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.SafeInformation("Getting tasks for project ID: {ProjectId} with pageNumber: {PageNumber}, pageSize: {PageSize}, status: {Status}, assignedUserId: {AssignedUserId}",
            id, pageNumber, pageSize, status, assignedUserId);

        // TODO: Implement GetProjectTasksQuery when available in Phase 5
        // var query = new GetProjectTasksQuery(id, pageNumber, pageSize, status, assignedUserId);
        // var result = await _mediator.Send(query, cancellationToken);
        // 
        // if (!result.IsSuccess)
        // {
        //     return NotFound(result.Error);
        // }
        // 
        // return Ok(result.Value);

        var mockResult = new PagedResult<TaskDto>(
            new List<TaskDto>(),
            pageNumber,
            pageSize,
            0);

        await Task.CompletedTask;
        return Ok(mockResult);
    }

    /// <summary>
    /// Creates a new task in the project
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <param name="request">Task creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created task</returns>
    [HttpPost("{id:guid}/tasks")]
    [Authorize(Policy = "ProjectManageTasks")]
    [ProducesResponseType(typeof(TaskDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<TaskDto>> CreateProjectTask(Guid id, CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new task in project ID: {ProjectId}", id);

        // TODO: Implement CreateTaskCommand when available in Phase 5
        // var command = new CreateTaskCommand(request.Title, request.Description, id, request.AssignedUserId, request.Priority, request.ParentTaskId);
        // var result = await _mediator.Send(command, cancellationToken);
        // 
        // if (!result.IsSuccess)
        // {
        //     if (result.Error.Contains("not found"))
        //         return NotFound(result.Error);
        //     return BadRequest(result.Error);
        // }
        // 
        // return CreatedAtAction("GetTask", "Tasks", new { id = result.Value.Id }, result.Value);

        await Task.CompletedTask;
        return BadRequest("Task creation not implemented yet");
    }

    /// <summary>
    /// Gets project statistics and metrics
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Project statistics</returns>
    [HttpGet("{id:guid}/statistics")]
    [Authorize(Policy = "ProjectRead")]
    [ProducesResponseType(typeof(ProjectStatisticsDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ProjectStatisticsDto>> GetProjectStatistics(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting statistics for project ID: {ProjectId}", id);

        // TODO: Implement GetProjectStatisticsQuery when available in Phase 5
        // var query = new GetProjectStatisticsQuery(id);
        // var result = await _mediator.Send(query, cancellationToken);
        // 
        // if (!result.IsSuccess)
        // {
        //     return NotFound(result.Error);
        // }
        // 
        // return Ok(result.Value);

        await Task.CompletedTask;
        return NotFound($"Project with ID {id} not found");
    }

    /// <summary>
    /// Searches for projects by keyword in name or description
    /// </summary>
    /// <param name="keyword">The search keyword</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching projects</returns>
    [HttpGet("search")]
    [Authorize(Policy = "DeveloperOrManager")]
    [ProducesResponseType(typeof(List<ProjectDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> SearchProjects(
        [FromQuery] string keyword,
        CancellationToken cancellationToken = default)
    {
        // Use safe logging extension to prevent log injection
        _logger.SafeInformation("Searching projects with keyword: {Keyword}", keyword);

        // Create a custom search query 
        var query = new GetProjectsQuery
        {
            PageNumber = 1,
            PageSize = 100, // Returning all matches
            SearchTerm = keyword
            // We're intentionally not filtering by status or team here
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsSuccess && result.Value != null)
        {
            return Ok(result.Value.Items);
        }

        return HandleResultAsAction(result);
    }
}
