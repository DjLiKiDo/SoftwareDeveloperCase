using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Application.Models;
using UserEntity = SoftwareDeveloperCase.Domain.Entities.User;

namespace SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.UpdateUser;

/// <summary>
/// Handler for processing update user commands
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<bool>>
{
    private readonly ILogger<UpdateUserCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;

    /// <summary>
    /// Initializes a new instance of the UpdateUserCommandHandler class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="unitOfWork">The unit of work instance</param>
    /// <param name="passwordService">The password service for hashing passwords</param>
    public UpdateUserCommandHandler(ILogger<UpdateUserCommandHandler> logger, IMapper mapper, IUnitOfWork unitOfWork, IPasswordService passwordService)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
    }

    /// <summary>
    /// Handles the update user command
    /// </summary>
    /// <param name="request">The update user command</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Result indicating success or failure</returns>
    public async Task<Result<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating user with ID: {UserId}", request.Id);

        var userToUpdate = await _unitOfWork.UserRepository.GetByIdAsync(request.Id);

        if (userToUpdate is null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", request.Id);
            return Result<bool>.NotFound($"User with ID {request.Id} not found");
        }

        // Handle password update separately to ensure it gets hashed
        var passwordToUpdate = request.Password;
        request.Password = null; // Temporarily clear to prevent mapper from setting it

        _mapper.Map(request, userToUpdate, typeof(UpdateUserCommand), typeof(UserEntity));

        // Hash and set password if provided
        if (!string.IsNullOrWhiteSpace(passwordToUpdate))
        {
            userToUpdate.Password = _passwordService.HashPassword(passwordToUpdate);
        }

        _unitOfWork.UserRepository.Update(userToUpdate);

        await _unitOfWork.SaveChanges();

        _logger.LogInformation("User updated successfully with ID: {UserId}", userToUpdate.Id);

        return Result<bool>.Success(true);
    }
}
