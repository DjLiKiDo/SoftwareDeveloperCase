using FluentAssertions;
using SoftwareDeveloperCase.Domain.Entities.Project;
using SoftwareDeveloperCase.Domain.Enums.Core;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Domain.Entities;

public class ProjectTests
{
    [Fact]
    public void Constructor_Default_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var project = new Project();

        // Assert
        project.Id.Should().Be(Guid.Empty);
        project.Name.Should().Be(string.Empty);
        project.Description.Should().BeNull();
        project.Status.Should().Be(ProjectStatus.Planning);
        project.Priority.Should().Be(Priority.Medium);
        project.TeamId.Should().Be(default(Guid));
        project.Tasks.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Name_SetValidName_ShouldSetCorrectly()
    {
        // Arrange
        var project = new Project();
        const string expectedName = "Test Project";

        // Act
        project.Name = expectedName;

        // Assert
        project.Name.Should().Be(expectedName);
    }

    [Fact]
    public void Description_SetValidDescription_ShouldSetCorrectly()
    {
        // Arrange
        var project = new Project();
        const string expectedDescription = "This is a test project description";

        // Act
        project.Description = expectedDescription;

        // Assert
        project.Description.Should().Be(expectedDescription);
    }

    [Theory]
    [InlineData(ProjectStatus.Planning)]
    [InlineData(ProjectStatus.Active)]
    [InlineData(ProjectStatus.OnHold)]
    [InlineData(ProjectStatus.Completed)]
    [InlineData(ProjectStatus.Cancelled)]
    public void Status_SetValidStatus_ShouldSetCorrectly(ProjectStatus status)
    {
        // Arrange
        var project = new Project();

        // Act
        project.Status = status;

        // Assert
        project.Status.Should().Be(status);
    }

    [Theory]
    [InlineData(Priority.Low)]
    [InlineData(Priority.Medium)]
    [InlineData(Priority.High)]
    [InlineData(Priority.Critical)]
    public void Priority_SetValidPriority_ShouldSetCorrectly(Priority priority)
    {
        // Arrange
        var project = new Project();

        // Act
        project.Priority = priority;

        // Assert
        project.Priority.Should().Be(priority);
    }

    [Fact]
    public void TeamId_SetValidTeamId_ShouldSetCorrectly()
    {
        // Arrange
        var project = new Project();
        var expectedTeamId = Guid.NewGuid();

        // Act
        project.TeamId = expectedTeamId;

        // Assert
        project.TeamId.Should().Be(expectedTeamId);
    }

    [Fact]
    public void ProjectStatusTransitions_ValidTransitions_ShouldWork()
    {
        // Arrange
        var project = new Project();

        // Act & Assert - Planning to Active
        project.Status = ProjectStatus.Planning;
        project.Status = ProjectStatus.Active;
        project.Status.Should().Be(ProjectStatus.Active);

        // Active to OnHold
        project.Status = ProjectStatus.OnHold;
        project.Status.Should().Be(ProjectStatus.OnHold);

        // OnHold back to Active
        project.Status = ProjectStatus.Active;
        project.Status.Should().Be(ProjectStatus.Active);

        // Active to Completed
        project.Status = ProjectStatus.Completed;
        project.Status.Should().Be(ProjectStatus.Completed);
    }

    [Fact]
    public void ProjectStatusTransitions_CancelledFromAnyStatus_ShouldWork()
    {
        // Arrange
        var project = new Project();

        // Act & Assert - From Planning to Cancelled
        project.Status = ProjectStatus.Planning;
        project.Status = ProjectStatus.Cancelled;
        project.Status.Should().Be(ProjectStatus.Cancelled);

        // From Active to Cancelled
        project.Status = ProjectStatus.Active;
        project.Status = ProjectStatus.Cancelled;
        project.Status.Should().Be(ProjectStatus.Cancelled);

        // From OnHold to Cancelled
        project.Status = ProjectStatus.OnHold;
        project.Status = ProjectStatus.Cancelled;
        project.Status.Should().Be(ProjectStatus.Cancelled);
    }

    [Fact]
    public void BaseEntityProperties_ShouldBeInherited()
    {
        // Arrange & Act
        var project = new Project();

        // Assert
        project.Id.Should().Be(Guid.Empty);
        project.CreatedBy.Should().BeNull();
        project.CreatedOn.Should().Be(default);
        project.LastModifiedBy.Should().BeNull();
        project.LastModifiedOn.Should().BeNull();
    }

    [Fact]
    public void Tasks_Collection_ShouldBeInitializedAndMutable()
    {
        // Arrange
        var project = new Project();

        // Act & Assert
        project.Tasks.Should().NotBeNull();
        project.Tasks.Should().BeEmpty();
        project.Tasks.Should().BeAssignableTo<ICollection<SoftwareDeveloperCase.Domain.Entities.Task.Task>>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Name_SetInvalidName_ShouldAllowButBeEmpty(string? invalidName)
    {
        // Arrange
        var project = new Project();

        // Act
        project.Name = invalidName ?? string.Empty;

        // Assert
        project.Name.Should().Be(invalidName ?? string.Empty);
    }

    [Fact]
    public void Description_SetNull_ShouldAllowNull()
    {
        // Arrange
        var project = new Project { Description = "Some description" };

        // Act
        project.Description = null;

        // Assert
        project.Description.Should().BeNull();
    }
}