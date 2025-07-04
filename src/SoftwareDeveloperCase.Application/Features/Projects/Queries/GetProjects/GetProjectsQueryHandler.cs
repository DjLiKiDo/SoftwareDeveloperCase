using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Features.Projects.DTOs;
using SoftwareDeveloperCase.Application.Models;

namespace SoftwareDeveloperCase.Application.Features.Projects.Queries.GetProjects;

/// <summary>
/// Handler for processing get projects queries with pagination and filtering
/// </summary>
public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, Result<PagedResult<ProjectDto>>>
{
    private readonly ILogger<GetProjectsQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the GetProjectsQueryHandler
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="unitOfWork">The unit of work instance</param>
    public GetProjectsQueryHandler(
        ILogger<GetProjectsQueryHandler> logger,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Handles the get projects query with pagination and filtering
    /// </summary>
    /// <param name="request">The get projects query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paged result containing project DTOs wrapped in a Result</returns>
    public async Task<Result<PagedResult<ProjectDto>>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting projects with pageNumber: {PageNumber}, pageSize: {PageSize}, searchTerm: {SearchTerm}, status: {Status}, teamId: {TeamId}, createdFrom: {CreatedFrom}, createdTo: {CreatedTo}",
            request.PageNumber, request.PageSize, request.SearchTerm, request.Status, request.TeamId, request.CreatedFrom, request.CreatedTo);

        // Build the query
        var query = _unitOfWork.ProjectRepository.GetQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(searchTerm) ||
                (p.Description != null && p.Description.ToLower().Contains(searchTerm)));
        }

        if (request.Status.HasValue)
        {
            query = query.Where(p => p.Status == request.Status.Value);
        }

        if (request.TeamId.HasValue)
        {
            query = query.Where(p => p.TeamId == request.TeamId.Value);
        }

        // Apply date range filters
        if (request.CreatedFrom.HasValue)
        {
            query = query.Where(p => p.CreatedOn >= request.CreatedFrom.Value);
        }

        if (request.CreatedTo.HasValue)
        {
            query = query.Where(p => p.CreatedOn <= request.CreatedTo.Value);
        }

        // Apply sorting by creation date descending
        query = query.OrderByDescending(p => p.CreatedOn);

        // Get total count
        var totalCount = await _unitOfWork.ProjectRepository.CountAsync(query, cancellationToken);

        // Apply paging
        var skip = (request.PageNumber - 1) * request.PageSize;
        var projects = await _unitOfWork.ProjectRepository.GetPagedAsync(query, skip, request.PageSize, cancellationToken);

        // Map to DTOs
        var projectDtos = _mapper.Map<IEnumerable<ProjectDto>>(projects);

        // Create and return the paged result
        var pagedResult = new PagedResult<ProjectDto>(projectDtos, request.PageNumber, request.PageSize, totalCount);
        return Result<PagedResult<ProjectDto>>.Success(pagedResult);
    }
}
