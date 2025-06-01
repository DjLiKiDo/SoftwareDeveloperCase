using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Exceptions;

namespace SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.DeleteUser;

/// <summary>
/// Handler for processing delete user commands
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Guid>
{
    private readonly ILogger<DeleteUserCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the DeleteUserCommandHandler class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="unitOfWork">The unit of work instance</param>
    public DeleteUserCommandHandler(ILogger<DeleteUserCommandHandler> logger, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the delete user command
    /// </summary>
    /// <param name="request">The delete user command</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The identifier of the deleted user</returns>
    public async Task<Guid> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userToDelete = await _unitOfWork.UserRepository.GetByIdAsync(request.Id);

        if (userToDelete is null)
        {
            _logger.LogError($"Error retrieving entity from database. Entity not found --> Id: {request.Id}");
            throw new NotFoundException("User", request.Id);
        }

        _unitOfWork.UserRepository.Delete(userToDelete);

        await _unitOfWork.SaveChanges();

        _logger.LogInformation($"Entity deleted successfully --> Id: {request.Id}");

        return request.Id;
    }
}
