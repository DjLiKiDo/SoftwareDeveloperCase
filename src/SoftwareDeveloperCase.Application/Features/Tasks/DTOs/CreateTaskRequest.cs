#nullable enable

namespace SoftwareDeveloperCase.Application.Features.Tasks.DTOs;

/// <summary>
/// Request DTO for creating a new task
/// </summary>
/// <param name="Title">The title of the task</param>
/// <param name="Description">The detailed description of the task</param>
/// <param name="ProjectId">The ID of the project this task belongs to</param>
/// <param name="AssignedUserId">Optional ID of the user assigned to the task</param>
/// <param name="Priority">The priority level of the task</param>
/// <param name="ParentTaskId">Optional ID of the parent task if this is a subtask</param>
/// <param name="DueDate">Optional due date for the task</param>
public record CreateTaskRequest(
    string Title,
    string Description,
    int ProjectId,
    int? AssignedUserId,
    string Priority,
    int? ParentTaskId,
    DateTime? DueDate);
