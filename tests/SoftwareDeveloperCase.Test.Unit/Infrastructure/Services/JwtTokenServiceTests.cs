using FluentAssertions;
using Microsoft.Extensions.Options;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Application.Models;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using SoftwareDeveloperCase.Domain.ValueObjects;
using SoftwareDeveloperCase.Infrastructure.Services;
using Moq;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Infrastructure.Services;

public class JwtTokenServiceTests
{
    private readonly Mock<IDateTimeService> _dateTimeServiceMock;
    private readonly JwtSettings _jwtSettings;
    private readonly JwtTokenService _jwtTokenService;

    public JwtTokenServiceTests()
    {
        _dateTimeServiceMock = new Mock<IDateTimeService>();
        _dateTimeServiceMock.Setup(x => x.Now).Returns(DateTime.UtcNow);

        _jwtSettings = new JwtSettings
        {
            SecretKey = "test-secret-key-that-is-long-enough-for-hmac-256-algorithm",
            Issuer = "test-issuer",
            Audience = "test-audience",
            AccessTokenExpirationMinutes = 15,
            RefreshTokenExpirationDays = 7
        };

        var options = Options.Create(_jwtSettings);
        _jwtTokenService = new JwtTokenService(options, _dateTimeServiceMock.Object);
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_ShouldGenerateValidToken()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        var token = await _jwtTokenService.GenerateAccessTokenAsync(user);

        // Assert
        token.Should().NotBeNullOrEmpty();

        // Validate token structure (JWT has 3 parts separated by dots)
        var tokenParts = token.Split('.');
        tokenParts.Should().HaveCount(3);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldGenerateValidToken()
    {
        // Act
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Assert
        refreshToken.Should().NotBeNullOrEmpty();
        refreshToken.Length.Should().BeGreaterThan(20); // Base64 encoded should be reasonably long
    }

    [Fact]
    public async Task ValidateToken_WithValidToken_ShouldReturnClaimsPrincipal()
    {
        // Arrange
        var user = CreateTestUser();
        var token = await _jwtTokenService.GenerateAccessTokenAsync(user);

        // Act
        var principal = _jwtTokenService.ValidateToken(token);

        // Assert
        principal.Should().NotBeNull();
        principal!.Identity!.IsAuthenticated.Should().BeTrue();
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ShouldReturnNull()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act
        var principal = _jwtTokenService.ValidateToken(invalidToken);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public async Task GetUserIdFromToken_WithValidToken_ShouldReturnUserId()
    {
        // Arrange
        var user = CreateTestUser();
        var token = await _jwtTokenService.GenerateAccessTokenAsync(user);

        // Act
        var userId = _jwtTokenService.GetUserIdFromToken(token);

        // Assert
        userId.Should().Be(user.Id.ToString());
    }

    [Fact]
    public async Task GetJwtIdFromToken_WithValidToken_ShouldReturnJwtId()
    {
        // Arrange
        var user = CreateTestUser();
        var token = await _jwtTokenService.GenerateAccessTokenAsync(user);

        // Act
        var jwtId = _jwtTokenService.GetJwtIdFromToken(token);

        // Assert
        jwtId.Should().NotBeNullOrEmpty();
        Guid.TryParse(jwtId, out _).Should().BeTrue();
    }

    private static User CreateTestUser()
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
            Email = new SoftwareDeveloperCase.Domain.ValueObjects.Email("test@example.com"),
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
