using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Entities;

namespace SoftwareDeveloperCase.Application.Features.Role.Commands.AssignPermission
{
    /// <summary>
    /// Handler for processing assign permission to role commands
    /// </summary>
    public class AssignPermissionCommandHandler : IRequestHandler<AssignPermissionCommand, Guid>
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
        /// <returns>The identifier of the created role permission assignment</returns>
        public async Task<Guid> Handle(AssignPermissionCommand request, CancellationToken cancellationToken)
        {
            var rolePermission = _mapper.Map<RolePermission>(request);

            _unitOfWork.RolePermissionRepository.Insert(rolePermission);

            var result = await _unitOfWork.SaveChanges();

            if (result <= 0)
            {
                _logger.LogError("The permission has not been assigned");
                throw new Exception("The permission has not been assigned");
            }

            _logger.LogInformation($"Permission assigned (Id: {rolePermission.Id})");

            return rolePermission.Id;
        }
    }
}
