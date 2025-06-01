using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Application.DTOs.Auth;
using SoftwareDeveloperCase.Application.Exceptions;
using SoftwareDeveloperCase.Domain.Entities.Identity;

namespace SoftwareDeveloperCase.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Handler for refresh token command
/// </summary>
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthenticationResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IDateTimeService _dateTimeService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenCommandHandler"/> class
    /// </summary>
    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService,
        IDateTimeService dateTimeService,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _dateTimeService = dateTimeService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the refresh token command
    /// </summary>
    public async Task<AuthenticationResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing refresh token request");

        // Get the refresh token
        var refreshToken = await _refreshTokenRepository.GetActiveRefreshTokenAsync(request.RefreshToken, cancellationToken);
        if (refreshToken == null)
        {
            _logger.LogWarning("Refresh token not found or expired");
            throw new AuthenticationException("Invalid refresh token");
        }

        // Get user with roles
        var user = await _userRepository.GetByEmailWithRolesAsync(refreshToken.User.Email.ToString(), cancellationToken);
        if (user == null || !user.IsActive)
        {
            _logger.LogWarning("User not found or inactive for refresh token");
            throw new AuthenticationException("User not found or inactive");
        }

        // Revoke the old refresh token
        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = _dateTimeService.Now;
        await _refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);

        // Generate new tokens
        var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user, cancellationToken);
        var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        // Create new refresh token entity
        var newRefreshToken = new Domain.Entities.Identity.RefreshToken
        {
            Token = newRefreshTokenValue,
            UserId = user.Id,
            ExpiresAt = _dateTimeService.Now.AddDays(7), // 7 days as per requirements
            JwtId = _jwtTokenService.GetJwtIdFromToken(accessToken),
            CreatedBy = user.Email.ToString(),
            CreatedOn = _dateTimeService.Now
        };

        // Save new refresh token
        await _refreshTokenRepository.InsertAsync(newRefreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Refresh token successfully processed for user: {UserId}", user.Id);

        // Return authentication response
        return new AuthenticationResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshTokenValue,
            ExpiresAt = _dateTimeService.Now.AddMinutes(15), // 15 minutes as per requirements
            User = new UserInfo
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email.ToString(),
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            }
        };
    }
}