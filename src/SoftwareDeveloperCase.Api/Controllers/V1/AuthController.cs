using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoftwareDeveloperCase.Api.Controllers;
using SoftwareDeveloperCase.Api.Extensions;
using SoftwareDeveloperCase.Application.DTOs.Auth;
using SoftwareDeveloperCase.Application.Features.Auth.Commands.ChangePassword;
using SoftwareDeveloperCase.Application.Features.Auth.Commands.Login;
using SoftwareDeveloperCase.Application.Features.Auth.Commands.Logout;
using SoftwareDeveloperCase.Application.Features.Auth.Commands.RefreshToken;

namespace SoftwareDeveloperCase.Api.Controllers.V1;

/// <summary>
/// Controller for authentication operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class
    /// </summary>
    /// <param name="mediator">The mediator</param>
    /// <param name="logger">The logger</param>
    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user and returns JWT tokens
    /// </summary>
    /// <param name="request">The login request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Authentication response with tokens</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<AuthenticationResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.SafeInformation("Login attempt for email: {Email}", request.Email);

        var command = new LoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.SafeInformation("Login successful for user: {UserId}", result.Value?.User?.Id);
        }
        else
        {
            _logger.SafeWarning("Login failed for email: {Email}. Error: {Error}", request.Email, result.Error);
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Refreshes the access token using a refresh token
    /// </summary>
    /// <param name="request">The refresh token request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>New authentication response with tokens</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<AuthenticationResponse>> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Refresh token request received");

        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.SafeInformation("Refresh token successful for user: {UserId}", result.Value?.User?.Id);
        }
        else
        {
            _logger.SafeWarning("Refresh token failed. Error: {Error}", result.Error);
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Logs out the user by revoking their refresh token
    /// </summary>
    /// <param name="request">The logout request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>No content</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Logout(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        _logger.SafeInformation("Logout request for user: {UserId}", userId);

        try
        {
            var command = new LogoutCommand(userId, request.RefreshToken);
            await _mediator.Send(command, cancellationToken);

            _logger.SafeInformation("Logout successful for user: {UserId}", userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.SafeError("Error during logout for user: {UserId}. Error: {Error}", userId, ex.Message);
            return BadRequest(new { message = "An error occurred during logout" });
        }
    }

    /// <summary>
    /// Gets information about the currently authenticated user
    /// </summary>
    /// <returns>User information</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserInfo), 200)]
    [ProducesResponseType(401)]
    public IActionResult GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var nameClaim = User.FindFirst(ClaimTypes.Name)?.Value;
        var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
        var roleClaims = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var userInfo = new UserInfo
        {
            Id = userId,
            Name = nameClaim ?? string.Empty,
            Email = emailClaim ?? string.Empty,
            Roles = roleClaims
        };

        return Ok(userInfo);
    }

    /// <summary>
    /// Changes the password for the currently authenticated user
    /// </summary>
    /// <param name="request">The change password request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>No content if successful</returns>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        _logger.SafeInformation("Password change request for user: {UserId}", userId);

        // Create command from request and set user ID
        var command = new ChangePasswordCommand
        {
            UserId = userId,
            CurrentPassword = request.CurrentPassword,
            NewPassword = request.NewPassword,
            ConfirmPassword = request.ConfirmPassword
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.SafeInformation("Password changed successfully for user: {UserId}", userId);
        }
        else
        {
            _logger.SafeWarning("Password change failed for user: {UserId}. Error: {Error}", userId, result.Error);
        }

        return HandleResultAsAction(result);
    }
}
