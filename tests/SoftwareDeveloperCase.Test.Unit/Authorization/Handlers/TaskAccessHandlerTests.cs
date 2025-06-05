using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Moq;
using SoftwareDeveloperCase.Api.Authorization.Handlers;
using SoftwareDeveloperCase.Api.Authorization.Requirements;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Domain.Enums;
using Xunit;
using TaskEntity = SoftwareDeveloperCase.Domain.Entities.Task.Task;

namespace SoftwareDeveloperCase.Test.Unit.Authorization.Handlers;

/// <summary>
/// Unit tests for TaskAccessHandler
/// </summary>
public class TaskAccessHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<TaskAccessHandler>> _loggerMock;
    private readonly TaskAccessHandler _handler;
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<ITeamMemberRepository> _teamMemberRepositoryMock;

    public TaskAccessHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<TaskAccessHandler>>();
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _teamMemberRepositoryMock = new Mock<ITeamMemberRepository>();

        _unitOfWorkMock.Setup(x => x.TaskRepository).Returns(_taskRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.TeamMemberRepository).Returns(_teamMemberRepositoryMock.Object);

        _handler = new TaskAccessHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task HandleRequirementAsync_AdminUser_ShouldSucceed()
    {
        // Arrange
        var user = CreateUser("admin-user-id", SystemRole.Admin);
        var task = CreateTask();
        var requirement = new TaskAccessRequirement(TaskAccessRequirement.Operations.Read);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, task);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_AssignedUser_UpdateStatus_ShouldSucceed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateUser(userId.ToString(), SystemRole.Developer);
        var task = CreateTask(assignedTo: userId);
        var requirement = new TaskAccessRequirement(TaskAccessRequirement.Operations.UpdateStatus);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, task);

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(task.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_TeamMember_ReadTask_ShouldSucceed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateUser(userId.ToString(), SystemRole.Developer);
        var task = CreateTask();
        var teamMember = CreateTeamMember(task.Project.TeamId, userId, TeamRole.Member);
        var requirement = new TaskAccessRequirement(TaskAccessRequirement.Operations.Read);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, task);

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(task.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _teamMemberRepositoryMock
            .Setup(x => x.GetTeamMemberByUserIdAndTeamIdAsync(userId, task.Project.TeamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teamMember);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_NonAssignedUser_UpdateStatus_ShouldFail()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var user = CreateUser(userId.ToString(), SystemRole.Developer);
        var task = CreateTask(assignedTo: otherUserId);
        var teamMember = CreateTeamMember(task.Project.TeamId, userId, TeamRole.Member);
        var requirement = new TaskAccessRequirement(TaskAccessRequirement.Operations.UpdateStatus);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, task);

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(task.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _teamMemberRepositoryMock
            .Setup(x => x.GetTeamMemberByUserIdAndTeamIdAsync(userId, task.Project.TeamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teamMember);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_TeamLeader_Assign_ShouldSucceed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateUser(userId.ToString(), SystemRole.Developer);
        var task = CreateTask();
        var teamMember = CreateTeamMember(task.Project.TeamId, userId, TeamRole.Leader);
        var requirement = new TaskAccessRequirement(TaskAccessRequirement.Operations.Assign);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, task);

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(task.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _teamMemberRepositoryMock
            .Setup(x => x.GetTeamMemberByUserIdAndTeamIdAsync(userId, task.Project.TeamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teamMember);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_NonTeamMember_Read_ShouldFail()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateUser(userId.ToString(), SystemRole.Developer);
        var task = CreateTask();
        var requirement = new TaskAccessRequirement(TaskAccessRequirement.Operations.Read);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, task);

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(task.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _teamMemberRepositoryMock
            .Setup(x => x.GetTeamMemberByUserIdAndTeamIdAsync(userId, task.Project.TeamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeamMember?)null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    private static ClaimsPrincipal CreateUser(string userId, SystemRole role)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, role.ToString())
        }));
    }

    private static TaskEntity CreateTask(Guid? assignedTo = null)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Test Project",
            Description = "Test Description",
            TeamId = Guid.NewGuid(),
            Status = ProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(3),
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return new TaskEntity
        {
            Id = Guid.NewGuid(),
            Title = "Test Task",
            Description = "Test Description",
            ProjectId = project.Id,
            Project = project,
            AssignedTo = assignedTo,
            Status = TaskStatus.Todo,
            Priority = Priority.Medium,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private static TeamMember CreateTeamMember(Guid teamId, Guid userId, TeamRole role)
    {
        return new TeamMember
        {
            Id = Guid.NewGuid(),
            TeamId = teamId,
            UserId = userId,
            Role = role,
            Status = MemberStatus.Active,
            JoinedDate = DateTime.UtcNow,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
