using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Application.Exceptions;
using SoftwareDeveloperCase.Application.Features.Auth.Commands.Login;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using SoftwareDeveloperCase.Domain.ValueObjects;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Application.Features.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<IDateTimeService> _dateTimeServiceMock;
    private readonly Mock<ILogger<LoginCommandHandler>> _loggerMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
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

        _dateTimeServiceMock.Setup(x => x.Now).Returns(new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnAuthenticationResponse()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        var user = CreateTestUser(email);
        var accessToken = "access-token";
        var refreshToken = "refresh-token";
        var jwtId = Guid.NewGuid().ToString();

        var command = new LoginCommand(email, password);

        _userRepositoryMock.Setup(x => x.GetByEmailWithRolesAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordServiceMock.Setup(x => x.VerifyPassword(password, user.Password))
            .Returns(true);
        _jwtTokenServiceMock.Setup(x => x.GenerateAccessTokenAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessToken);
        _jwtTokenServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns(refreshToken);
        _jwtTokenServiceMock.Setup(x => x.GetJwtIdFromToken(accessToken))
            .Returns(jwtId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be(accessToken);
        result.RefreshToken.Should().Be(refreshToken);
        result.User.Email.Should().Be(email);
        result.User.Id.Should().Be(user.Id);

        _refreshTokenRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ShouldThrowAuthenticationException()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var password = "password123";
        var command = new LoginCommand(email, password);

        _userRepositoryMock.Setup(x => x.GetByEmailWithRolesAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AuthenticationException>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be("Invalid email or password");
    }

    [Fact]
    public async Task Handle_WithInactiveUser_ShouldThrowAuthenticationException()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        var user = CreateTestUser(email);
        user.IsActive = false;

        var command = new LoginCommand(email, password);

        _userRepositoryMock.Setup(x => x.GetByEmailWithRolesAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AuthenticationException>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be("Account is inactive");
    }

    [Fact]
    public async Task Handle_WithInvalidPassword_ShouldThrowAuthenticationException()
    {
        // Arrange
        var email = "test@example.com";
        var password = "wrongpassword";
        var user = CreateTestUser(email);

        var command = new LoginCommand(email, password);

        _userRepositoryMock.Setup(x => x.GetByEmailWithRolesAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordServiceMock.Setup(x => x.VerifyPassword(password, user.Password))
            .Returns(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AuthenticationException>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be("Invalid email or password");
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
            Email = new Domain.ValueObjects.Email(email),
            Password = "hashedpassword",
            IsActive = true,
            UserRoles = new List<UserRole> { userRole },
            CreatedBy = "Test",
            CreatedOn = DateTime.UtcNow
        };

        userRole.User = user;
        return user;
    }
}
