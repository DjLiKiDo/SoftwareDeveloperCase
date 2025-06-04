using FluentAssertions;
using SoftwareDeveloperCase.Domain.Events.Core;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Domain.Events;

public class TaskAssignedEventTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateSuccessfully()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        const string taskTitle = "Test Task";
        var projectId = Guid.NewGuid();
        var assignedToUserId = Guid.NewGuid();
        var assignedByUserId = Guid.NewGuid();
        var assignedAt = DateTime.UtcNow;

        // Act
        var eventArgs = new TaskAssignedEvent(taskId, taskTitle, projectId, assignedToUserId, assignedByUserId, assignedAt);

        // Assert
        eventArgs.TaskId.Should().Be(taskId);
        eventArgs.TaskTitle.Should().Be(taskTitle);
        eventArgs.ProjectId.Should().Be(projectId);
        eventArgs.AssignedToUserId.Should().Be(assignedToUserId);
        eventArgs.AssignedByUserId.Should().Be(assignedByUserId);
        eventArgs.AssignedAt.Should().Be(assignedAt);
    }

    [Fact]
    public void Constructor_NullAssignedByUserId_ShouldAllowNull()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        const string taskTitle = "Test Task";
        var projectId = Guid.NewGuid();
        var assignedToUserId = Guid.NewGuid();
        Guid? assignedByUserId = null;
        var assignedAt = DateTime.UtcNow;

        // Act
        var eventArgs = new TaskAssignedEvent(taskId, taskTitle, projectId, assignedToUserId, assignedByUserId, assignedAt);

        // Assert
        eventArgs.TaskId.Should().Be(taskId);
        eventArgs.TaskTitle.Should().Be(taskTitle);
        eventArgs.ProjectId.Should().Be(projectId);
        eventArgs.AssignedToUserId.Should().Be(assignedToUserId);
        eventArgs.AssignedByUserId.Should().BeNull();
        eventArgs.AssignedAt.Should().Be(assignedAt);
    }

    [Fact]
    public void Equality_SameValues_ShouldBeEqual()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        const string taskTitle = "Test Task";
        var projectId = Guid.NewGuid();
        var assignedToUserId = Guid.NewGuid();
        var assignedByUserId = Guid.NewGuid();
        var assignedAt = DateTime.UtcNow;

        var event1 = new TaskAssignedEvent(taskId, taskTitle, projectId, assignedToUserId, assignedByUserId, assignedAt);
        var event2 = new TaskAssignedEvent(taskId, taskTitle, projectId, assignedToUserId, assignedByUserId, assignedAt);

        // Act & Assert
        event1.Should().Be(event2);
    }

    [Fact]
    public void Equality_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var taskId1 = Guid.NewGuid();
        var taskId2 = Guid.NewGuid();
        const string taskTitle = "Test Task";
        var projectId = Guid.NewGuid();
        var assignedToUserId = Guid.NewGuid();
        var assignedByUserId = Guid.NewGuid();
        var assignedAt = DateTime.UtcNow;

        var event1 = new TaskAssignedEvent(taskId1, taskTitle, projectId, assignedToUserId, assignedByUserId, assignedAt);
        var event2 = new TaskAssignedEvent(taskId2, taskTitle, projectId, assignedToUserId, assignedByUserId, assignedAt);

        // Act & Assert
        event1.Should().NotBe(event2);
    }
}
