namespace SoftwareDeveloperCase.Api.Models.Responses;

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
