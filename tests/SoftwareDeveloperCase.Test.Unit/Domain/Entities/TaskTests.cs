using FluentAssertions;
using SoftwareDeveloperCase.Domain.Entities.Task;
using SoftwareDeveloperCase.Domain.Enums.Core;
using SoftwareDeveloperCase.Domain.ValueObjects;
using Xunit;
using TaskStatus = SoftwareDeveloperCase.Domain.Enums.Core.TaskStatus;
using TaskEntity = SoftwareDeveloperCase.Domain.Entities.Task.Task;

namespace SoftwareDeveloperCase.Test.Unit.Domain.Entities;

public class TaskTests
{
    [Fact]
    public void Constructor_Default_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var task = new TaskEntity();

        // Assert
        task.Id.Should().Be(Guid.Empty);
        task.Title.Should().Be(string.Empty);
        task.Description.Should().BeNull();
        task.Status.Should().Be(TaskStatus.Todo);
        task.Priority.Should().Be(Priority.Medium);
        task.EstimatedHours.Should().BeNull();
        task.ActualHours.Should().BeNull();
        task.DueDate.Should().BeNull();
        task.ProjectId.Should().Be(default(Guid));
        task.AssignedToId.Should().BeNull();
        task.ParentTaskId.Should().BeNull();
        task.SubTasks.Should().NotBeNull().And.BeEmpty();
        task.Comments.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Title_SetValidTitle_ShouldSetCorrectly()
    {
        // Arrange
        var task = new TaskEntity();
        const string expectedTitle = "Test Task";

        // Act
        task.Title = expectedTitle;

        // Assert
        task.Title.Should().Be(expectedTitle);
    }

    [Fact]
    public void Description_SetValidDescription_ShouldSetCorrectly()
    {
        // Arrange
        var task = new TaskEntity();
        const string expectedDescription = "This is a test task description";

        // Act
        task.Description = expectedDescription;

        // Assert
        task.Description.Should().Be(expectedDescription);
    }

    [Theory]
    [InlineData(TaskStatus.Todo)]
    [InlineData(TaskStatus.InProgress)]
    [InlineData(TaskStatus.InReview)]
    [InlineData(TaskStatus.Done)]
    [InlineData(TaskStatus.Blocked)]
    public void Status_SetValidStatus_ShouldSetCorrectly(TaskStatus status)
    {
        // Arrange
        var task = new TaskEntity();

        // Act
        task.Status = status;

        // Assert
        task.Status.Should().Be(status);
    }

    [Theory]
    [InlineData(Priority.Low)]
    [InlineData(Priority.Medium)]
    [InlineData(Priority.High)]
    [InlineData(Priority.Critical)]
    public void Priority_SetValidPriority_ShouldSetCorrectly(Priority priority)
    {
        // Arrange
        var task = new TaskEntity();

        // Act
        task.Priority = priority;

        // Assert
        task.Priority.Should().Be(priority);
    }

    [Theory]
    [InlineData(1.5)]
    [InlineData(8.0)]
    [InlineData(40.25)]
    public void EstimatedHours_SetValidHours_ShouldSetCorrectly(decimal hours)
    {
        // Arrange
        var task = new TaskEntity();

        // Act
        task.EstimatedHours = hours;

        // Assert
        task.EstimatedHours.Should().Be(hours);
    }

    [Theory]
    [InlineData(1.0)]
    [InlineData(7.5)]
    [InlineData(42.75)]
    public void ActualHours_SetValidHours_ShouldSetCorrectly(decimal hours)
    {
        // Arrange
        var task = new TaskEntity();

        // Act
        task.ActualHours = hours;

        // Assert
        task.ActualHours.Should().Be(hours);
    }

    [Fact]
    public void DueDate_SetValidDate_ShouldSetCorrectly()
    {
        // Arrange
        var task = new TaskEntity();
        var expectedDate = DateTime.Now.AddDays(7);

        // Act
        task.DueDate = expectedDate;

        // Assert
        task.DueDate.Should().Be(expectedDate);
    }

    [Fact]
    public void ProjectId_SetValidProjectId_ShouldSetCorrectly()
    {
        // Arrange
        var task = new TaskEntity();
        var expectedProjectId = Guid.NewGuid();

        // Act
        task.ProjectId = expectedProjectId;

        // Assert
        task.ProjectId.Should().Be(expectedProjectId);
    }

    [Fact]
    public void AssignedToId_SetValidUserId_ShouldSetCorrectly()
    {
        // Arrange
        var task = new TaskEntity();
        var expectedUserId = Guid.NewGuid();

        // Act
        task.AssignedToId = expectedUserId;

        // Assert
        task.AssignedToId.Should().Be(expectedUserId);
    }

    [Fact]
    public void ParentTaskId_SetValidParentId_ShouldSetCorrectly()
    {
        // Arrange
        var task = new TaskEntity();
        var expectedParentId = Guid.NewGuid();

        // Act
        task.ParentTaskId = expectedParentId;

        // Assert
        task.ParentTaskId.Should().Be(expectedParentId);
    }

    [Fact]
    public void Hierarchy_SetValidHierarchy_ShouldSetCorrectly()
    {
        // Arrange
        var task = new TaskEntity();
        var hierarchy = TaskHierarchy.CreateRoot(1);

        // Act
        task.Hierarchy = hierarchy;

        // Assert
        task.Hierarchy.Should().Be(hierarchy);
        task.Hierarchy.IsRoot.Should().BeTrue();
        task.Hierarchy.Level.Should().Be(0);
        task.Hierarchy.Order.Should().Be(1);
    }

    [Fact]
    public void TaskHierarchy_CreateChild_ShouldWorkCorrectly()
    {
        // Arrange
        var parentTask = new TaskEntity { Hierarchy = TaskHierarchy.CreateRoot(1) };
        var childTask = new TaskEntity();

        // Act
        childTask.Hierarchy = TaskHierarchy.CreateChild(parentTask.Hierarchy, 1);
        childTask.ParentTaskId = parentTask.Id;

        // Assert
        childTask.Hierarchy.Level.Should().Be(1);
        childTask.Hierarchy.Path.Should().Be("1.1");
        childTask.Hierarchy.IsRoot.Should().BeFalse();
        childTask.ParentTaskId.Should().Be(parentTask.Id);
    }

    [Fact]
    public void TaskHierarchy_MultiLevelNesting_ShouldWorkCorrectly()
    {
        // Arrange
        var rootTask = new TaskEntity { Hierarchy = TaskHierarchy.CreateRoot(1) };
        var childTask = new TaskEntity { Hierarchy = TaskHierarchy.CreateChild(rootTask.Hierarchy, 2) };
        var grandChildTask = new TaskEntity();

        // Act
        grandChildTask.Hierarchy = TaskHierarchy.CreateChild(childTask.Hierarchy, 3);

        // Assert
        grandChildTask.Hierarchy.Level.Should().Be(2);
        grandChildTask.Hierarchy.Path.Should().Be("1.2.3");
        grandChildTask.Hierarchy.IsRoot.Should().BeFalse();
    }

    [Fact]
    public void BaseEntityProperties_ShouldBeInherited()
    {
        // Arrange & Act
        var task = new TaskEntity();

        // Assert
        task.Id.Should().Be(Guid.Empty);
        task.CreatedBy.Should().BeNull();
        task.CreatedOn.Should().Be(default);
        task.LastModifiedBy.Should().BeNull();
        task.LastModifiedOn.Should().BeNull();
    }

    [Fact]
    public void Collections_ShouldBeInitializedAndMutable()
    {
        // Arrange
        var task = new TaskEntity();

        // Act & Assert
        task.SubTasks.Should().NotBeNull();
        task.Comments.Should().NotBeNull();
        task.SubTasks.Should().BeEmpty();
        task.Comments.Should().BeEmpty();
        task.SubTasks.Should().BeAssignableTo<ICollection<TaskEntity>>();
        task.Comments.Should().BeAssignableTo<ICollection<TaskComment>>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Title_SetInvalidTitle_ShouldAllowButBeEmpty(string? invalidTitle)
    {
        // Arrange
        var task = new TaskEntity();

        // Act
        task.Title = invalidTitle ?? string.Empty;

        // Assert
        task.Title.Should().Be(invalidTitle ?? string.Empty);
    }

    [Fact]
    public void Description_SetNull_ShouldAllowNull()
    {
        // Arrange
        var task = new TaskEntity { Description = "Some description" };

        // Act
        task.Description = null;

        // Assert
        task.Description.Should().BeNull();
    }

    [Fact]
    public void EstimatedHours_SetNull_ShouldAllowNull()
    {
        // Arrange
        var task = new TaskEntity { EstimatedHours = 8.0m };

        // Act
        task.EstimatedHours = null;

        // Assert
        task.EstimatedHours.Should().BeNull();
    }

    [Fact]
    public void ActualHours_SetNull_ShouldAllowNull()
    {
        // Arrange
        var task = new TaskEntity { ActualHours = 6.5m };

        // Act
        task.ActualHours = null;

        // Assert
        task.ActualHours.Should().BeNull();
    }
}
