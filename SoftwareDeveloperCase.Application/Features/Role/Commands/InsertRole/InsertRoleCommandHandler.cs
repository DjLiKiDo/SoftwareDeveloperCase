using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;

namespace SoftwareDeveloperCase.Application.Features.Role.Commands.InsertRole
{
    /// <summary>
    /// Handler for processing insert role commands
    /// </summary>
    public class InsertRoleCommandHandler : IRequestHandler<InsertRoleCommand, Guid>
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
        /// <returns>The identifier of the created role</returns>
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
