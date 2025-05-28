using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;

namespace SoftwareDeveloperCase.Application.Features.Role.Commands.InsertRole
{
    public class InsertRoleCommandHandler : IRequestHandler<InsertRoleCommand, Guid>
    {
        private readonly ILogger<InsertRoleCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public InsertRoleCommandHandler(ILogger<InsertRoleCommandHandler> logger, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(InsertRoleCommand request, CancellationToken cancellationToken)
        {
            var role = _mapper.Map<Domain.Entities.Role>(request);

            _unitOfWork.RoleRepository.Insert(role);

            var result = await _unitOfWork.SaveChanges();

            if (result <= 0)
            {
                _logger.LogError("The role was not inserted");
                throw new Exception("The role was not inserted");
            }

            _logger.LogInformation($"New role registered (Id: {role.Id})");

            return role.Id;
        }
    }
}
