namespace SoftwareDeveloperCase.Application.Features.Projects.DTOs;

/// <summary>
/// Data transfer object for project statistics
/// </summary>
public record ProjectStatisticsDto(
    Guid Id,
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
