using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoftwareDeveloperCase.Application.Models;
using SoftwareDeveloperCase.Application.DTOs.Projects;
using SoftwareDeveloperCase.Application.Features.Projects.Commands.CreateProject;
using SoftwareDeveloperCase.Application.Features.Projects.Commands.UpdateProject;
using SoftwareDeveloperCase.Application.Features.Projects.Commands.DeleteProject;
using SoftwareDeveloperCase.Application.Features.Projects.Queries.GetProjectById;
using SoftwareDeveloperCase.Application.Features.Projects.Queries.GetProjects;

namespace SoftwareDeveloperCase.Api.Controllers.Core;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly ILogger<ProjectsController> _logger;
    private readonly IMediator _mediator;

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
        _logger.LogInformation("Getting projects with pageNumber: {PageNumber}, pageSize: {PageSize}, searchTerm: {SearchTerm}, status: {Status}, teamId: {TeamId}",
            pageNumber, pageSize, searchTerm, status, teamId);

        var query = new GetProjectsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            Status = status != null ? Enum.Parse<Domain.Enums.Core.ProjectStatus>(status) : null,
            TeamId = teamId
        };
        
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific project by ID
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Project details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ProjectDto>> GetProject(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting project with ID: {ProjectId}", id);

        await Task.CompletedTask;
        return NotFound($"Project with ID {id} not found");
    }

    /// <summary>
    /// Creates a new project
    /// </summary>
    /// <param name="request">Project creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created project</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<Guid>> CreateProject(CreateProjectCommand request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new project: {ProjectName}", request.Name);

        try
        {
            var projectId = await _mediator.Send(request, cancellationToken);
            return CreatedAtAction(nameof(GetProject), new { id = projectId }, projectId);
        }
        catch (Exception ex) when (ex is Application.Exceptions.ValidationException || ex is Application.Exceptions.BusinessRuleViolationException)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing project
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <param name="request">Project update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated project</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<bool>> UpdateProject(Guid id, UpdateProjectCommand request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating project with ID: {ProjectId}", id);
        
        // Ensure the ID in the route matches the ID in the request body
        if (id != request.Id)
        {
            return BadRequest("The project ID in the route must match the ID in the request body.");
        }

        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (Application.Exceptions.NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex) when (ex is Application.Exceptions.ValidationException || ex is Application.Exceptions.BusinessRuleViolationException)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a project
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> DeleteProject(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting project with ID: {ProjectId}", id);

        try
        {
            var command = new DeleteProjectCommand(id);
            var result = await _mediator.Send(command, cancellationToken);
            
            if (result)
            {
                return NoContent();
            }
            
            return BadRequest("Failed to delete project");
        }
        catch (Application.Exceptions.NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Application.Exceptions.BusinessRuleViolationException ex)
        {
            return BadRequest(ex.Message);
        }
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
    [ProducesResponseType(typeof(PagedResult<TaskDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<PagedResult<TaskDto>>> GetProjectTasks(
        int id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] int? assignedUserId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting tasks for project ID: {ProjectId} with pageNumber: {PageNumber}, pageSize: {PageSize}, status: {Status}, assignedUserId: {AssignedUserId}",
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
    [HttpPost("{id:int}/tasks")]
    [ProducesResponseType(typeof(TaskDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<TaskDto>> CreateProjectTask(int id, CreateTaskRequest request, CancellationToken cancellationToken = default)
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
    [HttpGet("{id:int}/statistics")]
    [ProducesResponseType(typeof(ProjectStatisticsDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ProjectStatisticsDto>> GetProjectStatistics(int id, CancellationToken cancellationToken = default)
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
}

// Request DTOs (these will be moved to Application layer in Phase 5)
public record CreateProjectRequest(
    string Name,
    string Description,
    int TeamId,
    DateTime? StartDate,
    DateTime? EndDate);

public record UpdateProjectRequest(
    string Name,
    string Description,
    string Status,
    DateTime? StartDate,
    DateTime? EndDate);

public record CreateTaskRequest(
    string Title,
    string Description,
    int? AssignedUserId,
    string Priority,
    int? ParentTaskId);

// Response DTOs (these will be moved to Application layer in Phase 5)
public record ProjectDto(
    int Id,
    string Name,
    string Description,
    string Status,
    int TeamId,
    string TeamName,
    DateTime? StartDate,
    DateTime? EndDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int TaskCount,
    int CompletedTaskCount);

public record TaskDto(
    int Id,
    string Title,
    string Description,
    string Status,
    string Priority,
    int ProjectId,
    string ProjectName,
    int? AssignedUserId,
    string? AssignedUserName,
    int? ParentTaskId,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record ProjectStatisticsDto(
    int Id,
    string Name,
    int TotalTasks,
    int CompletedTasks,
    int InProgressTasks,
    int TodoTasks,
    int BlockedTasks,
    decimal CompletionPercentage,
    TimeSpan TotalTimeLogged,
    int ActiveMembers,
    DateTime? LastActivity);
