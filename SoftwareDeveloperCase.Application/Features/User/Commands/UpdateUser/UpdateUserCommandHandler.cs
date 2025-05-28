using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Exceptions;

namespace SoftwareDeveloperCase.Application.Features.User.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
    {
        private readonly ILogger<UpdateUserCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserCommandHandler(ILogger<UpdateUserCommandHandler> logger, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var userToUpdate = await _unitOfWork.UserRepository.GetByIdAsync(request.Id);

            if (userToUpdate is null)
            {
                _logger.LogError($"Error retrieving entity from database. Entity not found --> Id: {request.Id}");
                throw new NotFoundException(nameof(Domain.Entities.User), request.Id);
            }

            _mapper.Map(request, userToUpdate, typeof(UpdateUserCommand), typeof(Domain.Entities.User));

            _unitOfWork.UserRepository.Update(userToUpdate);

            await _unitOfWork.SaveChanges();

            _logger.LogInformation($"Entity updated successfully --> Id: {userToUpdate.Id}");

            return Unit.Value;
        }
    }
}
