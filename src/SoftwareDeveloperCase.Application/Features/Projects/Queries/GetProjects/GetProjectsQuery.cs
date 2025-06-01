using MediatR;
using SoftwareDeveloperCase.Application.Features.Projects.DTOs;
using SoftwareDeveloperCase.Application.Models;
using SoftwareDeveloperCase.Domain.Enums.Core;

namespace SoftwareDeveloperCase.Application.Features.Projects.Queries.GetProjects;

/// <summary>
/// Query to get a paginated list of projects with optional filtering
/// </summary>
public class GetProjectsQuery : IRequest<PagedResult<ProjectDto>>
{
    /// <summary>
    /// Gets or sets the page number (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets the optional search term to filter by name or description
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Gets or sets the optional status to filter projects
    /// </summary>
    public ProjectStatus? Status { get; set; }

    /// <summary>
    /// Gets or sets the optional team ID to filter projects
    /// </summary>
    public Guid? TeamId { get; set; }

    /// <summary>
    /// Initializes a new instance of the GetProjectsQuery class
    /// </summary>
    public GetProjectsQuery()
    {
    }

    /// <summary>
    /// Initializes a new instance of the GetProjectsQuery class with pagination and filtering parameters
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="searchTerm">Optional search term for project name or description</param>
    /// <param name="status">Optional project status filter</param>
    /// <param name="teamId">Optional team ID filter</param>
    public GetProjectsQuery(int pageNumber, int pageSize, string? searchTerm = null, string? status = null, Guid? teamId = null)
    {
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize < 1 ? 10 : pageSize;
        SearchTerm = searchTerm;

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ProjectStatus>(status, true, out var statusEnum))
        {
            Status = statusEnum;
        }

        TeamId = teamId;
    }
}
