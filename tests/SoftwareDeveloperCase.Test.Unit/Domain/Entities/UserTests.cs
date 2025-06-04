using FluentAssertions;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Domain.ValueObjects;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Domain.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_Default_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.Id.Should().Be(Guid.Empty);
        user.Name.Should().Be(string.Empty);
        user.Password.Should().Be(string.Empty);
        user.IsActive.Should().BeTrue();
        user.UserRoles.Should().NotBeNull().And.BeEmpty();
        user.TeamMemberships.Should().NotBeNull().And.BeEmpty();
        user.AssignedTasks.Should().NotBeNull().And.BeEmpty();
        user.TaskComments.Should().NotBeNull().And.BeEmpty();
        user.RefreshTokens.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Name_SetValidName_ShouldSetCorrectly()
    {
        // Arrange
        var user = new User();
        const string expectedName = "John Doe";

        // Act
        user.Name = expectedName;

        // Assert
        user.Name.Should().Be(expectedName);
    }

    [Fact]
    public void Email_SetValidEmail_ShouldSetCorrectly()
    {
        // Arrange
        var user = new User();
        var email = new Email("john.doe@example.com");

        // Act
        user.Email = email;

        // Assert
        user.Email.Should().Be(email);
        user.Email.Value.Should().Be("john.doe@example.com");
    }

    [Fact]
    public void Password_SetPassword_ShouldSetCorrectly()
    {
        // Arrange
        var user = new User();
        const string password = "SecurePassword123!";

        // Act
        user.Password = password;

        // Assert
        user.Password.Should().Be(password);
    }

    [Fact]
    public void IsActive_SetToFalse_ShouldUpdateCorrectly()
    {
        // Arrange
        var user = new User { IsActive = true };

        // Act
        user.IsActive = false;

        // Assert
        user.IsActive.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Name_SetInvalidName_ShouldAllowButBeEmpty(string? invalidName)
    {
        // Arrange
        var user = new User();

        // Act
        user.Name = invalidName ?? string.Empty;

        // Assert
        user.Name.Should().Be(invalidName ?? string.Empty);
    }

    [Fact]
    public void BaseEntityProperties_ShouldBeInherited()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.Id.Should().Be(Guid.Empty);
        user.CreatedBy.Should().BeNull();
        user.CreatedOn.Should().Be(default);
        user.LastModifiedBy.Should().BeNull();
        user.LastModifiedOn.Should().BeNull();
    }

    [Fact]
    public void Collections_ShouldBeInitializedAndMutable()
    {
        // Arrange
        var user = new User();

        // Act & Assert
        user.UserRoles.Should().NotBeNull();
        user.TeamMemberships.Should().NotBeNull();
        user.AssignedTasks.Should().NotBeNull();
        user.TaskComments.Should().NotBeNull();
        user.RefreshTokens.Should().NotBeNull();

        // Verify collections are mutable
        user.UserRoles.Should().BeAssignableTo<ICollection<SoftwareDeveloperCase.Domain.Entities.Identity.UserRole>>();
        user.TeamMemberships.Should().BeAssignableTo<ICollection<SoftwareDeveloperCase.Domain.Entities.Team.TeamMember>>();
        user.AssignedTasks.Should().BeAssignableTo<ICollection<SoftwareDeveloperCase.Domain.Entities.Task.Task>>();
        user.TaskComments.Should().BeAssignableTo<ICollection<SoftwareDeveloperCase.Domain.Entities.Task.TaskComment>>();
        user.RefreshTokens.Should().BeAssignableTo<ICollection<SoftwareDeveloperCase.Domain.Entities.Identity.RefreshToken>>();
    }
}
