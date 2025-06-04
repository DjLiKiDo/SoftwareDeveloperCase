using FluentAssertions;
using SoftwareDeveloperCase.Domain.Events.Core;
using SoftwareDeveloperCase.Domain.Enums.Core;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Domain.Events;

public class ProjectCreatedEventTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateSuccessfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        const string projectName = "Test Project";
        var teamId = Guid.NewGuid();
        const Priority priority = Priority.High;
        var createdAt = DateTime.UtcNow;

        // Act
        var eventArgs = new ProjectCreatedEvent(projectId, projectName, teamId, priority, createdAt);

        // Assert
        eventArgs.ProjectId.Should().Be(projectId);
        eventArgs.ProjectName.Should().Be(projectName);
        eventArgs.TeamId.Should().Be(teamId);
        eventArgs.Priority.Should().Be(priority);
        eventArgs.CreatedAt.Should().Be(createdAt);
    }

    [Theory]
    [InlineData(Priority.Low)]
    [InlineData(Priority.Medium)]
    [InlineData(Priority.High)]
    [InlineData(Priority.Critical)]
    public void Constructor_AllPriorityValues_ShouldCreateSuccessfully(Priority priority)
    {
        // Arrange
        var projectId = Guid.NewGuid();
        const string projectName = "Test Project";
        var teamId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // Act
        var eventArgs = new ProjectCreatedEvent(projectId, projectName, teamId, priority, createdAt);

        // Assert
        eventArgs.Priority.Should().Be(priority);
    }

    [Fact]
    public void Equality_SameValues_ShouldBeEqual()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        const string projectName = "Test Project";
        var teamId = Guid.NewGuid();
        const Priority priority = Priority.Medium;
        var createdAt = DateTime.UtcNow;

        var event1 = new ProjectCreatedEvent(projectId, projectName, teamId, priority, createdAt);
        var event2 = new ProjectCreatedEvent(projectId, projectName, teamId, priority, createdAt);

        // Act & Assert
        event1.Should().Be(event2);
    }

    [Fact]
    public void Equality_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var projectId1 = Guid.NewGuid();
        var projectId2 = Guid.NewGuid();
        const string projectName = "Test Project";
        var teamId = Guid.NewGuid();
        const Priority priority = Priority.Medium;
        var createdAt = DateTime.UtcNow;

        var event1 = new ProjectCreatedEvent(projectId1, projectName, teamId, priority, createdAt);
        var event2 = new ProjectCreatedEvent(projectId2, projectName, teamId, priority, createdAt);

        // Act & Assert
        event1.Should().NotBe(event2);
    }
}

public class TeamCreatedEventTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateSuccessfully()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        const string teamName = "Development Team";
        var createdAt = DateTime.UtcNow;

        // Act
        var eventArgs = new TeamCreatedEvent(teamId, teamName, createdAt);

        // Assert
        eventArgs.TeamId.Should().Be(teamId);
        eventArgs.TeamName.Should().Be(teamName);
        eventArgs.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void Equality_SameValues_ShouldBeEqual()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        const string teamName = "Development Team";
        var createdAt = DateTime.UtcNow;

        var event1 = new TeamCreatedEvent(teamId, teamName, createdAt);
        var event2 = new TeamCreatedEvent(teamId, teamName, createdAt);

        // Act & Assert
        event1.Should().Be(event2);
    }

    [Fact]
    public void Equality_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var teamId1 = Guid.NewGuid();
        var teamId2 = Guid.NewGuid();
        const string teamName = "Development Team";
        var createdAt = DateTime.UtcNow;

        var event1 = new TeamCreatedEvent(teamId1, teamName, createdAt);
        var event2 = new TeamCreatedEvent(teamId2, teamName, createdAt);

        // Act & Assert
        event1.Should().NotBe(event2);
    }
}