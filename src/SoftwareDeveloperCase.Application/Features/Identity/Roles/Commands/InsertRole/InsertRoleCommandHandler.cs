using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Models;
using RoleEntity = SoftwareDeveloperCase.Domain.Entities.Role;

namespace SoftwareDeveloperCase.Application.Features.Identity.Roles.Commands.InsertRole;

/// <summary>
/// Handler for processing insert role commands
/// </summary>
public class InsertRoleCommandHandler : IRequestHandler<InsertRoleCommand, Result<Guid>>
{
    private readonly ILogger<InsertRoleCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the InsertRoleCommandHandler class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="unitOfWork">The unit of work instance</param>
    public InsertRoleCommandHandler(ILogger<InsertRoleCommandHandler> logger, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the insert role command
    /// </summary>
    /// <param name="request">The insert role command</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Result containing the identifier of the created role</returns>
    public async Task<Result<Guid>> Handle(InsertRoleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new role with name: {RoleName}", request.Name);

        var role = _mapper.Map<RoleEntity>(request);

        _unitOfWork.RoleRepository.Insert(role);

        var result = await _unitOfWork.SaveChanges();

        if (result <= 0)
        {
            _logger.LogError("The role was not inserted");
            return Result<Guid>.Failure("Failed to create role");
        }

        _logger.LogInformation("New role registered with ID: {RoleId}", role.Id);

        return Result<Guid>.Success(role.Id);
    }
}
