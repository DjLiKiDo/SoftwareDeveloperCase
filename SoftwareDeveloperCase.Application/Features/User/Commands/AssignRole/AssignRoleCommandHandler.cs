using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Entities;

namespace SoftwareDeveloperCase.Application.Features.User.Commands.AssignRole
{
    public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, Guid>
    {
        private readonly ILogger<AssignRoleCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AssignRoleCommandHandler(ILogger<AssignRoleCommandHandler> logger, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

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
}
