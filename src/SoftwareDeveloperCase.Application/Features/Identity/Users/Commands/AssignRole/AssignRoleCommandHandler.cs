using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Entities.Identity;

namespace SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.AssignRole;

/// <summary>
/// Handler for processing assign role to user commands
/// </summary>
public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, Guid>
{
    private readonly ILogger<AssignRoleCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the AssignRoleCommandHandler class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="unitOfWork">The unit of work instance</param>
    public AssignRoleCommandHandler(ILogger<AssignRoleCommandHandler> logger, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the assign role command
    /// </summary>
    /// <param name="request">The assign role command</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The identifier of the created user role assignment</returns>
    public async Task<Guid> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var userRole = _mapper.Map<UserRole>(request);

        _unitOfWork.UserRoleRepository.Insert(userRole);

        var result = await _unitOfWork.SaveChanges();

        if (result <= 0)
        {
            _logger.LogError("The role has not been assigned");
            throw new Exception("The role has not been assigned");
        }

        _logger.LogInformation($"Role assigned (Id: {userRole.Id})");

        return userRole.Id;
    }
}
