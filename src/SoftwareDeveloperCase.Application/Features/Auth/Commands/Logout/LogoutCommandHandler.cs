using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Application.Contracts.Persistence;

namespace SoftwareDeveloperCase.Application.Features.Auth.Commands.Logout;

/// <summary>
/// Handler for logout command
/// </summary>
public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LogoutCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogoutCommandHandler"/> class
    /// </summary>
    public LogoutCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        ILogger<LogoutCommandHandler> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Handles the logout command
    /// </summary>
    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing logout request for user: {UserId}", request.UserId);

        // If a specific refresh token is provided, revoke only that token
        if (!string.IsNullOrEmpty(request.RefreshToken))
        {
            var refreshToken = await _refreshTokenRepository.GetActiveRefreshTokenAsync(request.RefreshToken, cancellationToken);
            if (refreshToken != null && refreshToken.UserId == request.UserId)
            {
                refreshToken.IsRevoked = true;
                refreshToken.RevokedAt = DateTime.UtcNow;
                await _refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);
            }
        }
        else
        {
            // Revoke all refresh tokens for the user
            await _refreshTokenRepository.RevokeAllUserRefreshTokensAsync(request.UserId, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Logout successful for user: {UserId}", request.UserId);
    }
}