using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Application.DTOs.Auth;
using SoftwareDeveloperCase.Application.Exceptions;
using SoftwareDeveloperCase.Application.Services;

namespace SoftwareDeveloperCase.Application.Features.Auth.Commands.Login;

/// <summary>
/// Handler for login command
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IDateTimeService _dateTimeService;
    private readonly ILogger<LoginCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginCommandHandler"/> class
    /// </summary>
    public LoginCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IPasswordService passwordService,
        IJwtTokenService jwtTokenService,
        IDateTimeService dateTimeService,
        ILogger<LoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _jwtTokenService = jwtTokenService;
        _dateTimeService = dateTimeService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the login command
    /// </summary>
    public async Task<AuthenticationResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Sanitize and validate email specifically for authentication
        var sanitizedEmail = InputSanitizer.SanitizeEmail(request.Email);
        if (sanitizedEmail == null)
        {
            throw new AuthenticationException("Invalid email format");
        }

        var logSafeEmail = InputSanitizer.SanitizeForLogging(sanitizedEmail);
        _logger.LogInformation("Processing login request for email: {Email}", logSafeEmail);

        // Get user by email with roles
        var user = await _userRepository.GetByEmailWithRolesAsync(sanitizedEmail, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Login failed: User not found for email: {Email}", logSafeEmail);
            throw new AuthenticationException("Invalid email or password");
        }

        // Check if user is active
        if (!user.IsActive)
        {
            _logger.LogWarning("Login failed: User account is inactive for email: {Email}", logSafeEmail);
            throw new AuthenticationException("Account is inactive");
        }

        // Check if user account is locked out
        if (user.IsLockedOut(_dateTimeService.Now))
        {
            _logger.LogWarning("Login failed: User account is locked out for email: {Email}", logSafeEmail);
            throw new AuthenticationException("Account is temporarily locked due to too many failed login attempts. Please try again later.");
        }

        // Verify password (use original password, not sanitized)
        if (!_passwordService.VerifyPassword(request.Password, user.Password))
        {
            _logger.LogWarning("Login failed: Invalid password for email: {Email}", logSafeEmail);

            // Record failed login attempt
            user.RecordFailedLogin(_dateTimeService.Now);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            throw new AuthenticationException("Invalid email or password");
        }

        // Reset failed login attempts on successful authentication
        user.ResetFailedLoginAttempts();

        // Generate tokens
        var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user, cancellationToken);
        var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        // Create refresh token entity
        var refreshToken = new Domain.Entities.Identity.RefreshToken
        {
            Token = refreshTokenValue,
            UserId = user.Id,
            ExpiresAt = _dateTimeService.Now.AddDays(7), // 7 days as per requirements
            JwtId = _jwtTokenService.GetJwtIdFromToken(accessToken),
            CreatedBy = user.Email.ToString(),
            CreatedOn = _dateTimeService.Now
        };

        // Save refresh token
        await _refreshTokenRepository.InsertAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Login successful for user: {UserId}", user.Id);

        // Return authentication response
        return new AuthenticationResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
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
