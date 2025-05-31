namespace SoftwareDeveloperCase.Api.Models.Responses;

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
