using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Application.Models;

namespace SoftwareDeveloperCase.Application.Features.Auth.Commands.ChangePassword;

/// <summary>
/// Handler for change password command
/// </summary>
public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the ChangePasswordCommandHandler class
    /// </summary>
    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordService passwordService,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the change password command
    /// </summary>
    public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing password change request for user: {UserId}", request.UserId);

        // Get user from database
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Password change failed: User not found: {UserId}", request.UserId);
            return Result<bool>.NotFound("User not found");
        }

        // Check if user is active
        if (!user.IsActive)
        {
            _logger.LogWarning("Password change failed: User account is inactive: {UserId}", request.UserId);
            return Result<bool>.Failure("Account is inactive");
        }

        // Verify current password
        if (!_passwordService.VerifyPassword(request.CurrentPassword, user.Password))
        {
            _logger.LogWarning("Password change failed: Invalid current password for user: {UserId}", request.UserId);
            return Result<bool>.Failure("Current password is incorrect");
        }

        // Hash the new password
        var hashedNewPassword = _passwordService.HashPassword(request.NewPassword);

        // Update user password
        user.Password = hashedNewPassword;

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Password changed successfully for user: {UserId}", request.UserId);

        return Result<bool>.Success(true);
    }
}
