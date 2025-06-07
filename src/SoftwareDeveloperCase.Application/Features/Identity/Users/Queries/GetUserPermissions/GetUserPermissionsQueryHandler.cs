using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Models;

namespace SoftwareDeveloperCase.Application.Features.Identity.Users.Queries.GetUserPermissions;

/// <summary>
/// Handler for processing get user permissions queries
/// </summary>
public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, Result<List<PermissionDto>>>
{
    private readonly ILogger<GetUserPermissionsQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the GetUserPermissionsQueryHandler class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="unitOfWork">The unit of work instance</param>
    public GetUserPermissionsQueryHandler(ILogger<GetUserPermissionsQueryHandler> logger, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the get user permissions query
    /// </summary>
    /// <param name="request">The get user permissions query</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Result containing a list of permissions assigned to the user</returns>
    public async Task<Result<List<PermissionDto>>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting permissions for user with ID: {UserId}", request.UserId);

        var user = await _unitOfWork.UserRepository
            .GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", request.UserId);
            return Result<List<PermissionDto>>.NotFound($"User with ID {request.UserId} not found");
        }

        var userRoles = await _unitOfWork.UserRoleRepository
            .GetAsync(ur => ur.UserId.Equals(request.UserId), cancellationToken);

        var roleIds = userRoles
            .Select(ur => ur.RoleId);

        var rolePermissions = await _unitOfWork.RolePermissionRepository
            .GetAsync(rp => roleIds.Contains(rp.RoleId), cancellationToken);

        var permissionIds = rolePermissions
            .GroupBy(rp => rp.PermissionId)
            .Select(rpg => rpg.Key);

        var userPermissions = await _unitOfWork.PermissionRepository
            .GetAsync(p => permissionIds.Contains(p.Id), cancellationToken);

        var permissionDtos = _mapper.Map<List<PermissionDto>>(userPermissions);
        return Result<List<PermissionDto>>.Success(permissionDtos);
    }
}
