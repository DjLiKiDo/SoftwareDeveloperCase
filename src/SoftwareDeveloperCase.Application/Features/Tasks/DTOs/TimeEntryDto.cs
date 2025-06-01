#nullable enable

namespace SoftwareDeveloperCase.Application.Features.Tasks.DTOs;

/// <summary>
/// DTO for time entry records
/// </summary>
/// <param name="Id">The unique identifier of the time entry</param>
/// <param name="TaskId">The ID of the task this time entry is for</param>
/// <param name="TaskTitle">The title of the task</param>
/// <param name="UserId">The ID of the user who logged the time</param>
/// <param name="UserName">The name of the user who logged the time</param>
/// <param name="StartTime">When the time tracking started</param>
/// <param name="EndTime">When the time tracking ended (null if still running)</param>
/// <param name="Duration">The total duration of the time entry</param>
/// <param name="Description">Optional description of work performed</param>
/// <param name="CreatedAt">When this time entry was created in the system</param>
public record TimeEntryDto(
    int Id,
    int TaskId,
    string TaskTitle,
    int UserId,
    string UserName,
    DateTime StartTime,
    DateTime? EndTime,
    TimeSpan Duration,
    string? Description,
    DateTime CreatedAt);
