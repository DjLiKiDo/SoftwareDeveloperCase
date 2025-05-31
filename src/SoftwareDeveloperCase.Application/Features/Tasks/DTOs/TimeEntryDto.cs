#nullable enable

namespace SoftwareDeveloperCase.Application.Features.Tasks.DTOs;

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
