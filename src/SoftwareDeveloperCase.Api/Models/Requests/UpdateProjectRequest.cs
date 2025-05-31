namespace SoftwareDeveloperCase.Api.Models.Requests;

public record UpdateProjectRequest(
    string Name,
    string Description,
    string Status,
    DateTime? StartDate,
    DateTime? EndDate);
