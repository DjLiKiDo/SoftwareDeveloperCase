#nullable enable

namespace SoftwareDeveloperCase.Application.Features.Tasks.DTOs;

/// <summary>
/// DTO for detailed task information
/// </summary>
/// <param name="Id">The unique identifier of the task</param>
/// <param name="Title">The title of the task</param>
/// <param name="Description">The detailed description of the task</param>
/// <param name="Status">The current status of the task</param>
/// <param name="Priority">The priority level of the task</param>
/// <param name="ProjectId">The ID of the project this task belongs to</param>
/// <param name="ProjectName">The name of the project this task belongs to</param>
/// <param name="AssignedUserId">Optional ID of the user assigned to the task</param>
/// <param name="AssignedUserName">Optional name of the user assigned to the task</param>
/// <param name="ParentTaskId">Optional ID of the parent task if this is a subtask</param>
/// <param name="ParentTaskTitle">Optional title of the parent task</param>
/// <param name="DueDate">Optional due date for the task completion</param>
/// <param name="CreatedAt">The date and time when the task was created</param>
/// <param name="UpdatedAt">Optional date and time when the task was last updated</param>
/// <param name="Subtasks">List of subtasks belonging to this task</param>
/// <param name="RecentComments">List of recent comments on this task</param>
/// <param name="TotalTimeLogged">Total time logged for this task</param>
/// <param name="CommentCount">Total number of comments on this task</param>
public record TaskDetailDto(
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
    string? ParentTaskTitle,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<SimpleTaskDto> Subtasks,
    List<TaskCommentDto> RecentComments,
    TimeSpan TotalTimeLogged,
    int CommentCount);
