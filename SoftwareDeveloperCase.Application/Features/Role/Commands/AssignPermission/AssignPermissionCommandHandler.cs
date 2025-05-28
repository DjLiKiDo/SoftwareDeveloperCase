using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Entities;

namespace SoftwareDeveloperCase.Application.Features.Role.Commands.AssignPermission
{
    public class AssignPermissionCommandHandler : IRequestHandler<AssignPermissionCommand, Guid>
    {
        private readonly ILogger<AssignPermissionCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AssignPermissionCommandHandler(ILogger<AssignPermissionCommandHandler> logger, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

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
