using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Models;
using SoftwareDeveloperCase.Domain.Entities.Identity;

namespace SoftwareDeveloperCase.Application.Features.Identity.Roles.Commands.AssignPermission;

/// <summary>
/// Handler for processing assign permission to role commands
/// </summary>
public class AssignPermissionCommandHandler : IRequestHandler<AssignPermissionCommand, Result<Guid>>
{
    private readonly ILogger<AssignPermissionCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the AssignPermissionCommandHandler class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="unitOfWork">The unit of work instance</param>
    public AssignPermissionCommandHandler(ILogger<AssignPermissionCommandHandler> logger, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the assign permission command
    /// </summary>
    /// <param name="request">The assign permission command</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Result containing the identifier of the created role permission assignment</returns>
    public async Task<Result<Guid>> Handle(AssignPermissionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning permission {PermissionId} to role {RoleId}", request.PermissionId, request.RoleId);

        var rolePermission = _mapper.Map<RolePermission>(request);

        _unitOfWork.RolePermissionRepository.Insert(rolePermission);

        var result = await _unitOfWork.SaveChanges();

        if (result <= 0)
        {
            _logger.LogError("The permission has not been assigned to role {RoleId}", request.RoleId);
            return Result<Guid>.Failure("The permission has not been assigned");
        }

        _logger.LogInformation("Permission assigned successfully with ID: {RolePermissionId}", rolePermission.Id);

        return Result<Guid>.Success(rolePermission.Id);
    }
}
