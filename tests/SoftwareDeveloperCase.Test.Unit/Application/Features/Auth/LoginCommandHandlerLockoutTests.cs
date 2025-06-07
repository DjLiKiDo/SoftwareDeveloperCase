using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Application.Features.Auth.Commands.Login;
using SoftwareDeveloperCase.Application.Models;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using Xunit;
using Email = SoftwareDeveloperCase.Domain.ValueObjects.Email;

namespace SoftwareDeveloperCase.Test.Unit.Application.Features.Auth;

public class LoginCommandHandlerLockoutTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<IDateTimeService> _dateTimeServiceMock;
    private readonly Mock<ILogger<LoginCommandHandler>> _loggerMock;
    private readonly LoginCommandHandler _handler;
    private readonly DateTime _currentTime = new(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    public LoginCommandHandlerLockoutTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _passwordServiceMock = new Mock<IPasswordService>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _dateTimeServiceMock = new Mock<IDateTimeService>();
        _loggerMock = new Mock<ILogger<LoginCommandHandler>>();

        _handler = new LoginCommandHandler(
            _userRepositoryMock.Object,
            _refreshTokenRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordServiceMock.Object,
            _jwtTokenServiceMock.Object,
            _dateTimeServiceMock.Object,
            _loggerMock.Object);

        _dateTimeServiceMock.Setup(x => x.Now).Returns(_currentTime);
    }

    [Fact]
    public async Task Handle_WithLockedOutAccount_ShouldReturnFailure()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        var user = CreateTestUser(email);
        user.LockoutExpiresAt = _currentTime.AddMinutes(10); // Locked out for 10 more minutes

        var command = new LoginCommand(email, password);

        _userRepositoryMock.Setup(x => x.GetByEmailWithRolesAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Account is temporarily locked due to too many failed login attempts. Please try again later.");
    }

    [Fact]
    public async Task Handle_WithExpiredLockout_ShouldAllowLogin()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        var user = CreateTestUser(email);
        user.LockoutExpiresAt = _currentTime.AddMinutes(-10); // Lockout expired 10 minutes ago
        user.FailedLoginAttempts = 5;

        var command = new LoginCommand(email, password);

        _userRepositoryMock.Setup(x => x.GetByEmailWithRolesAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordServiceMock.Setup(x => x.VerifyPassword(password, user.Password))
            .Returns(true);
        _jwtTokenServiceMock.Setup(x => x.GenerateAccessTokenAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync("access-token");
        _jwtTokenServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");
        _jwtTokenServiceMock.Setup(x => x.GetJwtIdFromToken("access-token"))
            .Returns("jwt-id");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        user.FailedLoginAttempts.Should().Be(0); // Reset after successful login
        user.LockoutExpiresAt.Should().BeNull();
        user.LockedOutAt.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithIncorrectPassword_ShouldRecordFailedAttempt()
    {
        // Arrange
        var email = "test@example.com";
        var password = "wrongpassword";
        var user = CreateTestUser(email);
        user.FailedLoginAttempts = 2; // Already has 2 failed attempts

        var command = new LoginCommand(email, password);

        _userRepositoryMock.Setup(x => x.GetByEmailWithRolesAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordServiceMock.Setup(x => x.VerifyPassword(password, user.Password))
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invalid email or password");
        user.FailedLoginAttempts.Should().Be(3); // Incremented from 2 to 3
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithIncorrectPasswordReachingThreshold_ShouldLockAccount()
    {
        // Arrange
        var email = "test@example.com";
        var password = "wrongpassword";
        var user = CreateTestUser(email);
        user.FailedLoginAttempts = 4; // One attempt away from lockout

        var command = new LoginCommand(email, password);

        _userRepositoryMock.Setup(x => x.GetByEmailWithRolesAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordServiceMock.Setup(x => x.VerifyPassword(password, user.Password))
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invalid email or password");
        user.FailedLoginAttempts.Should().Be(5);
        user.LockedOutAt.Should().Be(_currentTime);
        user.LockoutExpiresAt.Should().Be(_currentTime.AddMinutes(15)); // Default 15 minutes
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithSuccessfulLogin_ShouldResetFailedAttempts()
    {
        // Arrange
        var email = "test@example.com";
        var password = "correctpassword";
        var user = CreateTestUser(email);
        user.FailedLoginAttempts = 3; // Had some failed attempts

        var command = new LoginCommand(email, password);

        _userRepositoryMock.Setup(x => x.GetByEmailWithRolesAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordServiceMock.Setup(x => x.VerifyPassword(password, user.Password))
            .Returns(true);
        _jwtTokenServiceMock.Setup(x => x.GenerateAccessTokenAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync("access-token");
        _jwtTokenServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");
        _jwtTokenServiceMock.Setup(x => x.GetJwtIdFromToken("access-token"))
            .Returns("jwt-id");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        user.FailedLoginAttempts.Should().Be(0); // Reset after successful login
        user.LockedOutAt.Should().BeNull();
        user.LockoutExpiresAt.Should().BeNull();
    }

    private static User CreateTestUser(string email)
    {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var role = new Role
        {
            Id = roleId,
            Name = "Developer",
            CreatedBy = "Test",
            CreatedOn = DateTime.UtcNow
        };

        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            Role = role,
            CreatedBy = "Test",
            CreatedOn = DateTime.UtcNow
        };

        var user = new User
        {
            Id = userId,
            Name = "Test User",
            Email = new Email(email),
            Password = "hashedpassword",
            IsActive = true,
            FailedLoginAttempts = 0,
            UserRoles = new List<UserRole> { userRole },
            CreatedBy = "Test",
            CreatedOn = DateTime.UtcNow
        };

        userRole.User = user;
        return user;
    }
}
