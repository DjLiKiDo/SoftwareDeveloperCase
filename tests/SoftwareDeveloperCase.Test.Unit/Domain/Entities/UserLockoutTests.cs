using FluentAssertions;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Domain.ValueObjects;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Domain.Entities;

public class UserLockoutTests
{
    [Fact]
    public void IsLockedOut_WithNoLockout_ShouldReturnFalse()
    {
        // Arrange
        var user = CreateTestUser();
        var currentTime = DateTime.UtcNow;

        // Act
        var isLockedOut = user.IsLockedOut(currentTime);

        // Assert
        isLockedOut.Should().BeFalse();
    }

    [Fact]
    public void IsLockedOut_WithFutureLockoutExpiry_ShouldReturnTrue()
    {
        // Arrange
        var user = CreateTestUser();
        var currentTime = DateTime.UtcNow;
        user.LockoutExpiresAt = currentTime.AddMinutes(10);

        // Act
        var isLockedOut = user.IsLockedOut(currentTime);

        // Assert
        isLockedOut.Should().BeTrue();
    }

    [Fact]
    public void IsLockedOut_WithExpiredLockout_ShouldReturnFalse()
    {
        // Arrange
        var user = CreateTestUser();
        var currentTime = DateTime.UtcNow;
        user.LockoutExpiresAt = currentTime.AddMinutes(-10); // Past time

        // Act
        var isLockedOut = user.IsLockedOut(currentTime);

        // Assert
        isLockedOut.Should().BeFalse();
    }

    [Fact]
    public void RecordFailedLogin_WithLessThanMaxAttempts_ShouldIncrementCount()
    {
        // Arrange
        var user = CreateTestUser();
        var currentTime = DateTime.UtcNow;
        const int maxFailedAttempts = 5;

        // Act
        user.RecordFailedLogin(currentTime, maxFailedAttempts);

        // Assert
        user.FailedLoginAttempts.Should().Be(1);
        user.LockoutExpiresAt.Should().BeNull();
        user.LockedOutAt.Should().BeNull();
    }

    [Fact]
    public void RecordFailedLogin_WithMaxAttempts_ShouldLockoutAccount()
    {
        // Arrange
        var user = CreateTestUser();
        user.FailedLoginAttempts = 4; // Already at 4 attempts
        var currentTime = DateTime.UtcNow;
        const int maxFailedAttempts = 5;
        const int lockoutDurationMinutes = 15;

        // Act
        user.RecordFailedLogin(currentTime, maxFailedAttempts, lockoutDurationMinutes);

        // Assert
        user.FailedLoginAttempts.Should().Be(5);
        user.LockedOutAt.Should().Be(currentTime);
        user.LockoutExpiresAt.Should().Be(currentTime.AddMinutes(lockoutDurationMinutes));
    }

    [Fact]
    public void RecordFailedLogin_WithCustomMaxAttempts_ShouldRespectCustomValue()
    {
        // Arrange
        var user = CreateTestUser();
        user.FailedLoginAttempts = 2; // Already at 2 attempts
        var currentTime = DateTime.UtcNow;
        const int customMaxAttempts = 3;

        // Act
        user.RecordFailedLogin(currentTime, customMaxAttempts);

        // Assert
        user.FailedLoginAttempts.Should().Be(3);
        user.LockedOutAt.Should().Be(currentTime);
        user.LockoutExpiresAt.Should().Be(currentTime.AddMinutes(15)); // Default lockout duration
    }

    [Fact]
    public void RecordFailedLogin_WithCustomLockoutDuration_ShouldRespectCustomValue()
    {
        // Arrange
        var user = CreateTestUser();
        user.FailedLoginAttempts = 4; // Already at 4 attempts
        var currentTime = DateTime.UtcNow;
        const int maxFailedAttempts = 5;
        const int customLockoutDuration = 30;

        // Act
        user.RecordFailedLogin(currentTime, maxFailedAttempts, customLockoutDuration);

        // Assert
        user.FailedLoginAttempts.Should().Be(5);
        user.LockoutExpiresAt.Should().Be(currentTime.AddMinutes(customLockoutDuration));
    }

    [Fact]
    public void ResetFailedLoginAttempts_ShouldClearAllLockoutFields()
    {
        // Arrange
        var user = CreateTestUser();
        user.FailedLoginAttempts = 5;
        user.LockedOutAt = DateTime.UtcNow;
        user.LockoutExpiresAt = DateTime.UtcNow.AddMinutes(15);

        // Act
        user.ResetFailedLoginAttempts();

        // Assert
        user.FailedLoginAttempts.Should().Be(0);
        user.LockedOutAt.Should().BeNull();
        user.LockoutExpiresAt.Should().BeNull();
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, false)]
    [InlineData(3, false)]
    [InlineData(4, true)]
    [InlineData(5, true)]
    public void RecordFailedLogin_WithVariousAttemptCounts_ShouldLockoutOnlyAtThreshold(
        int initialAttempts, bool shouldLockout)
    {
        // Arrange
        var user = CreateTestUser();
        user.FailedLoginAttempts = initialAttempts;
        var currentTime = DateTime.UtcNow;
        const int maxFailedAttempts = 5;

        // Act
        user.RecordFailedLogin(currentTime, maxFailedAttempts);

        // Assert
        user.FailedLoginAttempts.Should().Be(initialAttempts + 1);
        if (shouldLockout)
        {
            user.LockedOutAt.Should().Be(currentTime);
            user.LockoutExpiresAt.Should().NotBeNull();
        }
        else
        {
            user.LockedOutAt.Should().BeNull();
            user.LockoutExpiresAt.Should().BeNull();
        }
    }

    [Fact]
    public void LockoutWorkflow_MultipleFailedLoginsFollowedByReset_ShouldWorkCorrectly()
    {
        // Arrange
        var user = CreateTestUser();
        var currentTime = DateTime.UtcNow;

        // Act & Assert - Multiple failed attempts
        for (int i = 1; i <= 4; i++)
        {
            user.RecordFailedLogin(currentTime);
            user.FailedLoginAttempts.Should().Be(i);
            user.IsLockedOut(currentTime).Should().BeFalse();
        }

        // Act & Assert - Final attempt should trigger lockout
        user.RecordFailedLogin(currentTime);
        user.FailedLoginAttempts.Should().Be(5);
        user.IsLockedOut(currentTime).Should().BeTrue();

        // Act & Assert - Reset should clear everything
        user.ResetFailedLoginAttempts();
        user.FailedLoginAttempts.Should().Be(0);
        user.IsLockedOut(currentTime).Should().BeFalse();
    }

    private static User CreateTestUser()
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = new Email("test@example.com"),
            Password = "hashedpassword",
            IsActive = true,
            CreatedBy = "Test",
            CreatedOn = DateTime.UtcNow
        };
    }
}