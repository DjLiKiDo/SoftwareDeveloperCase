using System.Linq.Expressions;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Moq;
using SoftwareDeveloperCase.Api.Authorization.Handlers;
using SoftwareDeveloperCase.Api.Authorization.Requirements;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Core;
using SoftwareDeveloperCase.Domain.Entities.Project;
using SoftwareDeveloperCase.Domain.Entities.Team;
using SoftwareDeveloperCase.Domain.Enums.Core;
using SoftwareDeveloperCase.Domain.Enums.Identity;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Authorization.Handlers;

/// <summary>
/// Unit tests for ProjectAccessHandler
/// </summary>
public class ProjectAccessHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<ProjectAccessHandler>> _loggerMock;
    private readonly ProjectAccessHandler _handler;
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<ITeamMemberRepository> _teamMemberRepositoryMock;

    public ProjectAccessHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<ProjectAccessHandler>>();
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _teamMemberRepositoryMock = new Mock<ITeamMemberRepository>();

        _unitOfWorkMock.Setup(x => x.ProjectRepository).Returns(_projectRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.TeamMemberRepository).Returns(_teamMemberRepositoryMock.Object);

        _handler = new ProjectAccessHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task HandleRequirementAsync_AdminUser_ShouldSucceed()
    {
        // Arrange
        var adminUserId = Guid.NewGuid();
        var user = CreateUser(adminUserId.ToString(), SystemRole.Admin);
        var project = CreateProject();
        var requirement = new ProjectAccessRequirement(ProjectAccessRequirement.Operations.Read);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, project);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_ManagerUser_ReadOperation_ShouldSucceed()
    {
        // Arrange
        var managerUserId = Guid.NewGuid();
        var user = CreateUser(managerUserId.ToString(), SystemRole.Manager);
        var project = CreateProject();
        var requirement = new ProjectAccessRequirement(ProjectAccessRequirement.Operations.Read);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, project);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_TeamMember_ReadProject_ShouldSucceed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateUser(userId.ToString(), SystemRole.Developer);
        var project = CreateProject();
        var teamMember = CreateTeamMember(project.TeamId, userId, TeamRole.Member);
        var requirement = new ProjectAccessRequirement(ProjectAccessRequirement.Operations.Read);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, project);

        _teamMemberRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<TeamMember, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { teamMember });

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_NonTeamMember_Delete_ShouldFail()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateUser(userId.ToString(), SystemRole.Developer);
        var project = CreateProject();
        var requirement = new ProjectAccessRequirement(ProjectAccessRequirement.Operations.Delete);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, project);

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _teamMemberRepositoryMock
            .Setup(x => x.GetTeamMemberAsync(project.TeamId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeamMember?)null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_TeamLeader_ManageTasks_ShouldSucceed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateUser(userId.ToString(), SystemRole.Developer);
        var project = CreateProject();
        var teamMember = CreateTeamMember(project.TeamId, userId, TeamRole.Leader);
        var requirement = new ProjectAccessRequirement(ProjectAccessRequirement.Operations.ManageTasks);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, project);

        _teamMemberRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<TeamMember, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { teamMember });

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    private static ClaimsPrincipal CreateUser(string userId, SystemRole role)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, role.ToString())
        }));
    }

    private static Project CreateProject()
    {
        return new Project
        {
            Id = Guid.NewGuid(),
            Name = "Test Project",
            Description = "Test Description",
            TeamId = Guid.NewGuid(),
            Status = ProjectStatus.Active,
            CreatedBy = "system",
            CreatedOn = DateTime.UtcNow,
            LastModifiedBy = "system",
            LastModifiedOn = DateTime.UtcNow
        };
    }

    private static TeamMember CreateTeamMember(Guid teamId, Guid userId, TeamRole role)
    {
        return new TeamMember
        {
            Id = Guid.NewGuid(),
            TeamId = teamId,
            UserId = userId,
            TeamRole = role,
            Status = MemberStatus.Active,
            JoinedDate = DateTime.UtcNow,
            CreatedBy = "system",
            CreatedOn = DateTime.UtcNow,
            LastModifiedBy = "system",
            LastModifiedOn = DateTime.UtcNow
        };
    }
}
