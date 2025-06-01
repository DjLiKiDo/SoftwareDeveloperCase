using FluentAssertions;
using SoftwareDeveloperCase.Infrastructure.Services;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Infrastructure.Services;

public class PasswordServiceTests
{
    private readonly PasswordService _passwordService;

    public PasswordServiceTests()
    {
        _passwordService = new PasswordService();
    }

    [Fact]
    public void HashPassword_WithValidPassword_ShouldReturnHashedPassword()
    {
        // Arrange
        const string password = "MySecurePassword123!";

        // Act
        var hashedPassword = _passwordService.HashPassword(password);

        // Assert
        hashedPassword.Should().NotBeNullOrEmpty();
        hashedPassword.Should().NotBe(password);
        hashedPassword.Should().StartWith("$2a$");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void HashPassword_WithInvalidPassword_ShouldThrowArgumentException(string? password)
    {
        // Act & Assert
        var action = () => _passwordService.HashPassword(password!);
        action.Should().Throw<ArgumentException>()
            .WithParameterName("password");
    }

    [Fact]
    public void HashPassword_SamePasswordTwice_ShouldReturnDifferentHashes()
    {
        // Arrange
        const string password = "MySecurePassword123!";

        // Act
        var hash1 = _passwordService.HashPassword(password);
        var hash2 = _passwordService.HashPassword(password);

        // Assert
        hash1.Should().NotBe(hash2, "BCrypt should generate different salts for each hash");
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        const string password = "MySecurePassword123!";
        var hashedPassword = _passwordService.HashPassword(password);

        // Act
        var result = _passwordService.VerifyPassword(password, hashedPassword);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        const string correctPassword = "MySecurePassword123!";
        const string incorrectPassword = "WrongPassword456@";
        var hashedPassword = _passwordService.HashPassword(correctPassword);

        // Act
        var result = _passwordService.VerifyPassword(incorrectPassword, hashedPassword);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void VerifyPassword_WithInvalidPassword_ShouldThrowArgumentException(string? password)
    {
        // Arrange
        const string hashedPassword = "$2a$12$somevalidhash";

        // Act & Assert
        var action = () => _passwordService.VerifyPassword(password!, hashedPassword);
        action.Should().Throw<ArgumentException>()
            .WithParameterName("password");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void VerifyPassword_WithInvalidHashedPassword_ShouldThrowArgumentException(string? hashedPassword)
    {
        // Arrange
        const string password = "MySecurePassword123!";

        // Act & Assert
        var action = () => _passwordService.VerifyPassword(password, hashedPassword!);
        action.Should().Throw<ArgumentException>()
            .WithParameterName("hashedPassword");
    }

    [Fact]
    public void VerifyPassword_WithInvalidHash_ShouldReturnFalse()
    {
        // Arrange
        const string password = "MySecurePassword123!";
        const string invalidHash = "not-a-bcrypt-hash";

        // Act
        var result = _passwordService.VerifyPassword(password, invalidHash);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("plaintext", true)]
    [InlineData("", true)]
    [InlineData(null, true)]
    [InlineData("not-a-bcrypt-hash", true)]
    [InlineData("$2a$10$someoldhash", true)] // Lower work factor
    [InlineData("$2a$12$somevalidhash", false)] // Current work factor
    [InlineData("$2b$12$somevalidhash", false)] // Current work factor with different version
    [InlineData("$2y$12$somevalidhash", false)] // Current work factor with different version
    [InlineData("$2a$15$somehigherhash", false)] // Higher work factor
    public void NeedsRehash_ShouldReturnExpectedResult(string? hashedPassword, bool expectedNeedsRehash)
    {
        // Act
        var result = _passwordService.NeedsRehash(hashedPassword!);

        // Assert
        result.Should().Be(expectedNeedsRehash);
    }

    [Fact]
    public void HashPassword_ShouldUseWorkFactor12()
    {
        // Arrange
        const string password = "MySecurePassword123!";

        // Act
        var hashedPassword = _passwordService.HashPassword(password);

        // Assert
        hashedPassword.Should().Contain("$2a$12$", "hash should use work factor 12");
    }

    [Fact]
    public void PasswordWorkflow_CreateVerifyRehash_ShouldWorkCorrectly()
    {
        // Arrange
        const string password = "MySecurePassword123!";

        // Act - Hash password
        var hashedPassword = _passwordService.HashPassword(password);

        // Assert - Verify password works
        _passwordService.VerifyPassword(password, hashedPassword).Should().BeTrue();

        // Assert - Hash doesn't need rehashing (it's current)
        _passwordService.NeedsRehash(hashedPassword).Should().BeFalse();

        // Assert - Wrong password fails verification
        _passwordService.VerifyPassword("WrongPassword", hashedPassword).Should().BeFalse();
    }
}
