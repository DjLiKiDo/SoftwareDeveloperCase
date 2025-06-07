using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Models;

namespace SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.DeleteUser;

/// <summary>
/// Handler for processing delete user commands
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<bool>>
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
    /// <returns>Result indicating success or failure</returns>
    public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting user with ID: {UserId}", request.Id);

        var userToDelete = await _unitOfWork.UserRepository.GetByIdAsync(request.Id);

        if (userToDelete is null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", request.Id);
            return Result<bool>.NotFound($"User with ID {request.Id} not found");
        }

        _unitOfWork.UserRepository.Delete(userToDelete);

        await _unitOfWork.SaveChanges();

        _logger.LogInformation("User deleted successfully with ID: {UserId}", request.Id);

        return Result<bool>.Success(true);
    }
}
