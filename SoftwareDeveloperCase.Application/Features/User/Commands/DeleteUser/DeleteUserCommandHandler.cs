using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Exceptions;

namespace SoftwareDeveloperCase.Application.Features.User.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly ILogger<DeleteUserCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserCommandHandler(ILogger<DeleteUserCommandHandler> logger, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var userToDelete = await _unitOfWork.UserRepository.GetByIdAsync(request.Id);

            if (userToDelete is null)
            {
                _logger.LogError($"Error retrieving entity from database. Entity not found --> Id: {request.Id}");
                throw new NotFoundException(nameof(Domain.Entities.User), request.Id);
            }

            _unitOfWork.UserRepository.Delete(userToDelete);

            await _unitOfWork.SaveChanges();

            _logger.LogInformation($"Entity deleted successfully --> Id: {request.Id}");

            return Unit.Value;
        }
    }
}
