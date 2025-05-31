namespace SoftwareDeveloperCase.Api.Models.Requests;

public record CreateProjectRequest(
    string Name,
    string Description,
    int TeamId,
    DateTime? StartDate,
    DateTime? EndDate);
