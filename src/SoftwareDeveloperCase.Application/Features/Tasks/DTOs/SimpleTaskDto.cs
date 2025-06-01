#nullable enable

namespace SoftwareDeveloperCase.Application.Features.Tasks.DTOs;

/// <summary>
/// Simple DTO for basic task information
/// </summary>
/// <param name="Id">The unique identifier of the task</param>
/// <param name="Title">The title of the task</param>
/// <param name="Description">The detailed description of the task</param>
/// <param name="Status">The current status of the task</param>
/// <param name="Priority">The priority level of the task</param>
/// <param name="ProjectId">The ID of the project this task belongs to</param>
/// <param name="AssignedUserId">Optional ID of the user assigned to the task</param>
/// <param name="DueDate">Optional due date for the task completion</param>
/// <param name="CreatedAt">The date and time when the task was created</param>
/// <param name="UpdatedAt">Optional date and time when the task was last updated</param>
public record SimpleTaskDto(
    int Id,
    string Title,
    string Description,
    string Status,
    string Priority,
    int ProjectId,
    int? AssignedUserId,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
