using FluentAssertions;
using SoftwareDeveloperCase.Domain.Entities.Team;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Domain.Entities;

public class TeamTests
{
    [Fact]
    public void Constructor_Default_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var team = new Team();

        // Assert
        team.Id.Should().Be(Guid.Empty);
        team.Name.Should().Be(string.Empty);
        team.Description.Should().BeNull();
        team.TeamMembers.Should().NotBeNull().And.BeEmpty();
        team.Projects.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Name_SetValidName_ShouldSetCorrectly()
    {
        // Arrange
        var team = new Team();
        const string expectedName = "Development Team";

        // Act
        team.Name = expectedName;

        // Assert
        team.Name.Should().Be(expectedName);
    }

    [Fact]
    public void Description_SetValidDescription_ShouldSetCorrectly()
    {
        // Arrange
        var team = new Team();
        const string expectedDescription = "This is a development team description";

        // Act
        team.Description = expectedDescription;

        // Assert
        team.Description.Should().Be(expectedDescription);
    }

    [Fact]
    public void Description_SetNull_ShouldAllowNull()
    {
        // Arrange
        var team = new Team { Description = "Some description" };

        // Act
        team.Description = null;

        // Assert
        team.Description.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Name_SetInvalidName_ShouldAllowButBeEmpty(string? invalidName)
    {
        // Arrange
        var team = new Team();

        // Act
        team.Name = invalidName ?? string.Empty;

        // Assert
        team.Name.Should().Be(invalidName ?? string.Empty);
    }

    [Fact]
    public void BaseEntityProperties_ShouldBeInherited()
    {
        // Arrange & Act
        var team = new Team();

        // Assert
        team.Id.Should().Be(Guid.Empty);
        team.CreatedBy.Should().BeNull();
        team.CreatedOn.Should().Be(default);
        team.LastModifiedBy.Should().BeNull();
        team.LastModifiedOn.Should().BeNull();
    }

    [Fact]
    public void Collections_ShouldBeInitializedAndMutable()
    {
        // Arrange
        var team = new Team();

        // Act & Assert
        team.TeamMembers.Should().NotBeNull();
        team.Projects.Should().NotBeNull();
        team.TeamMembers.Should().BeEmpty();
        team.Projects.Should().BeEmpty();
        team.TeamMembers.Should().BeAssignableTo<ICollection<TeamMember>>();
        team.Projects.Should().BeAssignableTo<ICollection<SoftwareDeveloperCase.Domain.Entities.Project.Project>>();
    }
}