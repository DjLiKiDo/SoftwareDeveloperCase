using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoftwareDeveloperCase.Api.Extensions;
using SoftwareDeveloperCase.Application.Features.Tasks.DTOs;
using SoftwareDeveloperCase.Application.Models; // Changed from Common.Models

namespace SoftwareDeveloperCase.Api.Controllers.V1;

/// <summary>
/// Controller for managing tasks
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ILogger<TasksController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TasksController"/> class
    /// </summary>
    /// <param name="logger">The logger</param>
    public TasksController(ILogger<TasksController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets a paginated list of tasks with optional filtering
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="searchTerm">Optional search term for task title or description</param>
    /// <param name="status">Optional task status filter</param>
    /// <param name="priority">Optional priority filter</param>
    /// <param name="assignedUserId">Optional assigned user ID filter</param>
    /// <param name="projectId">Optional project ID filter</param>
    /// <param name="dueDateFrom">Optional due date range start</param>
    /// <param name="dueDateTo">Optional due date range end</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of tasks</returns>
    [HttpGet]
    [Authorize(Policy = "DeveloperOrManager")]
    [ProducesResponseType(typeof(PagedResult<TaskDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<PagedResult<TaskDto>>> GetTasks(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? status = null,
        [FromQuery] string? priority = null,
        [FromQuery] int? assignedUserId = null,
        [FromQuery] int? projectId = null,
        [FromQuery] DateTime? dueDateFrom = null,
        [FromQuery] DateTime? dueDateTo = null,
        CancellationToken cancellationToken = default)
    {
        _logger.SafeInformation("Getting tasks with pageNumber: {PageNumber}, pageSize: {PageSize}, searchTerm: {SearchTerm}, status: {Status}, priority: {Priority}, assignedUserId: {AssignedUserId}, projectId: {ProjectId}",
            pageNumber, pageSize, searchTerm, status, priority, assignedUserId, projectId);

        // TODO: Implement GetTasksQuery when available in Phase 5
        // var query = new GetTasksQuery(pageNumber, pageSize, searchTerm, status, priority, assignedUserId, projectId, dueDateFrom, dueDateTo);
        // var result = await _mediator.Send(query, cancellationToken);
        // return Ok(result);

        var mockResult = new PagedResult<TaskDto>(
            new List<TaskDto>(),
            pageNumber,
            pageSize,
            0);

        await Task.CompletedTask;
        return Ok(mockResult);
    }

    /// <summary>
    /// Gets a specific task by ID
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task details</returns>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "TaskRead")]
    [ProducesResponseType(typeof(TaskDetailDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<TaskDetailDto>> GetTask(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting task with ID: {TaskId}", id);

        // TODO: Implement GetTaskByIdQuery when available in Phase 5
        // var query = new GetTaskByIdQuery(id);
        // var result = await _mediator.Send(query, cancellationToken);
        // 
        // if (!result.IsSuccess)
        // {
        //     return NotFound(result.Error);
        // }
        // 
        // return Ok(result.Value);

        await Task.CompletedTask;
        return NotFound($"Task with ID {id} not found");
    }

    /// <summary>
    /// Creates a new task
    /// </summary>
    /// <param name="request">Task creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created task</returns>
    [HttpPost]
    [Authorize(Policy = "TaskCreate")]
    [ProducesResponseType(typeof(TaskDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        _logger.SafeInformation("Creating new task: {TaskTitle}", request.Title);

        // TODO: Implement CreateTaskCommand when available in Phase 5
        // var command = new CreateTaskCommand(request.Title, request.Description, request.ProjectId, request.AssignedUserId, request.Priority, request.ParentTaskId, request.DueDate);
        // var result = await _mediator.Send(command, cancellationToken);
        // 
        // if (!result.IsSuccess)
        // {
        //     return BadRequest(result.Error);
        // }
        // 
        // return CreatedAtAction(nameof(GetTask), new { id = result.Value.Id }, result.Value);

        await Task.CompletedTask;
        return BadRequest("Task creation not implemented yet");
    }

    /// <summary>
    /// Updates an existing task
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="request">Task update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated task</returns>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "TaskUpdate")]
    [ProducesResponseType(typeof(TaskDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<TaskDto>> UpdateTask(int id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating task with ID: {TaskId}", id);

        // TODO: Implement UpdateTaskCommand when available in Phase 5
        // var command = new UpdateTaskCommand(id, request.Title, request.Description, request.Status, request.Priority, request.AssignedUserId, request.DueDate);
        // var result = await _mediator.Send(command, cancellationToken);
        // 
        // if (!result.IsSuccess)
        // {
        //     if (result.Error.Contains("not found"))
        //         return NotFound(result.Error);
        //     return BadRequest(result.Error);
        // }
        // 
        // return Ok(result.Value);

        await Task.CompletedTask;
        return NotFound($"Task with ID {id} not found");
    }

    /// <summary>
    /// Updates task status
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="request">Status update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated task</returns>
    [HttpPatch("{id:int}/status")]
    [Authorize(Policy = "TaskUpdateStatus")]
    [ProducesResponseType(typeof(TaskDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<TaskDto>> UpdateTaskStatus(int id, UpdateTaskStatusRequest request, CancellationToken cancellationToken = default)
    {
        _logger.SafeInformation("Updating status for task with ID: {TaskId} to {Status}", id, request.Status);

        // TODO: Implement UpdateTaskStatusCommand when available in Phase 5
        // var command = new UpdateTaskStatusCommand(id, request.Status);
        // var result = await _mediator.Send(command, cancellationToken);
        // 
        // if (!result.IsSuccess)
        // {
        //     if (result.Error.Contains("not found"))
        //         return NotFound(result.Error);
        //     return BadRequest(result.Error);
        // }
        // 
        // return Ok(result.Value);

        await Task.CompletedTask;
        return NotFound($"Task with ID {id} not found");
    }

    /// <summary>
    /// Assigns a task to a user
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="request">Assignment request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated task</returns>
    [HttpPatch("{id:int}/assign")]
    [Authorize(Policy = "TaskAssign")]
    [ProducesResponseType(typeof(TaskDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<TaskDto>> AssignTask(int id, AssignTaskRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Assigning task with ID: {TaskId} to user {UserId}", id, request.UserId);

        // TODO: Implement AssignTaskCommand when available in Phase 5
        // var command = new AssignTaskCommand(id, request.UserId);
        // var result = await _mediator.Send(command, cancellationToken);
        // 
        // if (!result.IsSuccess)
        // {
        //     if (result.Error.Contains("not found"))
        //         return NotFound(result.Error);
        //     return BadRequest(result.Error);
        // }
        // 
        // return Ok(result.Value);

        await Task.CompletedTask;
        return NotFound($"Task with ID {id} not found");
    }

    /// <summary>
    /// Deletes a task
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> DeleteTask(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting task with ID: {TaskId}", id);

        // For testing purposes, simulate that task exists but check authorization first
        // This allows proper authorization testing before resource existence check

        // TODO: Implement DeleteTaskCommand when available in Phase 5
        // var command = new DeleteTaskCommand(id);
        // var result = await _mediator.Send(command, cancellationToken);
        // 
        // if (!result.IsSuccess)
        // {
        //     if (result.Error.Contains("not found"))
        //         return NotFound(result.Error);
        //     return BadRequest(result.Error);
        // }
        // 
        // return NoContent();

        // For now, simulate that the task exists for authorization testing
        // In a real implementation, this check would be done in the command handler
        await Task.CompletedTask;

        // If we get here, authorization passed, but task doesn't exist
        return NotFound($"Task with ID {id} not found");
    }

    /// <summary>
    /// Gets all comments for a specific task
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of task comments</returns>
    [HttpGet("{id:int}/comments")]
    [Authorize(Policy = "TaskRead")]
    [ProducesResponseType(typeof(PagedResult<TaskCommentDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<PagedResult<TaskCommentDto>>> GetTaskComments(
        int id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting comments for task ID: {TaskId} with pageNumber: {PageNumber}, pageSize: {PageSize}",
            id, pageNumber, pageSize);

        // TODO: Implement GetTaskCommentsQuery when available in Phase 5
        // var query = new GetTaskCommentsQuery(id, pageNumber, pageSize);
        // var result = await _mediator.Send(query, cancellationToken);
        // 
        // if (!result.IsSuccess)
        // {
        //     return NotFound(result.Error);
        // }
        // 
        // return Ok(result.Value);

        var mockResult = new PagedResult<TaskCommentDto>(
            new List<TaskCommentDto>(),
            pageNumber,
            pageSize,
            0);

        await Task.CompletedTask;
        return Ok(mockResult);
    }

    /// <summary>
    /// Adds a comment to a task
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="request">Comment creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created comment</returns>
    [HttpPost("{id:int}/comments")]
    [Authorize(Policy = "TaskAddComment")]
    [ProducesResponseType(typeof(TaskCommentDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<TaskCommentDto>> AddTaskComment(int id, CreateTaskCommentRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding comment to task ID: {TaskId}", id);

        // TODO: Implement CreateTaskCommentCommand when available in Phase 5
        // var command = new CreateTaskCommentCommand(id, request.Content);
        // var result = await _mediator.Send(command, cancellationToken);
        // 
        // if (!result.IsSuccess)
        // {
        //     if (result.Error.Contains("not found"))
        //         return NotFound(result.Error);
        //     return BadRequest(result.Error);
        // }
        // 
        // return CreatedAtAction(nameof(GetTaskComments), new { id }, result.Value);

        await Task.CompletedTask;
        return BadRequest("Comment creation not implemented yet");
    }

    /// <summary>
    /// Gets subtasks for a specific task
    /// </summary>
    /// <param name="id">Parent task ID</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of subtasks</returns>
    [HttpGet("{id:int}/subtasks")]
    [Authorize(Policy = "TaskRead")]
    [ProducesResponseType(typeof(PagedResult<TaskDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<PagedResult<TaskDto>>> GetSubtasks(
        int id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting subtasks for task ID: {TaskId} with pageNumber: {PageNumber}, pageSize: {PageSize}",
            id, pageNumber, pageSize);

        // TODO: Implement GetSubtasksQuery when available in Phase 5
        // var query = new GetSubtasksQuery(id, pageNumber, pageSize);
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
    /// Gets time tracking entries for a specific task
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of time entries</returns>
    [HttpGet("{id:int}/time-entries")]
    [Authorize(Policy = "TaskRead")]
    [ProducesResponseType(typeof(PagedResult<TimeEntryDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<PagedResult<TimeEntryDto>>> GetTaskTimeEntries(
        int id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting time entries for task ID: {TaskId} with pageNumber: {PageNumber}, pageSize: {PageSize}",
            id, pageNumber, pageSize);

        // TODO: Implement GetTaskTimeEntriesQuery when available in Phase 5
        // var query = new GetTaskTimeEntriesQuery(id, pageNumber, pageSize);
        // var result = await _mediator.Send(query, cancellationToken);
        // 
        // if (!result.IsSuccess)
        // {
        //     return NotFound(result.Error);
        // }
        // 
        // return Ok(result.Value);

        var mockResult = new PagedResult<TimeEntryDto>(
            new List<TimeEntryDto>(),
            pageNumber,
            pageSize,
            0);

        await Task.CompletedTask;
        return Ok(mockResult);
    }
}
