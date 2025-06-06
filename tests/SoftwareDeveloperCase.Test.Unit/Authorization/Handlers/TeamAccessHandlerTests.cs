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
using SoftwareDeveloperCase.Domain.Entities.Team;
using SoftwareDeveloperCase.Domain.Enums.Core;
using SoftwareDeveloperCase.Domain.Enums.Identity;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Authorization.Handlers;

/// <summary>
/// Unit tests for TeamAccessHandler
/// </summary>
public class TeamAccessHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<TeamAccessHandler>> _loggerMock;
    private readonly TeamAccessHandler _handler;
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<ITeamMemberRepository> _teamMemberRepositoryMock;

    public TeamAccessHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<TeamAccessHandler>>();
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _teamMemberRepositoryMock = new Mock<ITeamMemberRepository>();

        _unitOfWorkMock.Setup(x => x.TeamRepository).Returns(_teamRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.TeamMemberRepository).Returns(_teamMemberRepositoryMock.Object);

        _handler = new TeamAccessHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task HandleRequirementAsync_AdminUser_ShouldSucceed()
    {
        // Arrange
        var adminUserId = Guid.NewGuid();
        var user = CreateUser(adminUserId.ToString(), SystemRole.Admin);
        var team = CreateTeam();
        var requirement = new TeamAccessRequirement(TeamAccessRequirement.Operations.Read);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, team);

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
        var team = CreateTeam();
        var requirement = new TeamAccessRequirement(TeamAccessRequirement.Operations.Read);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, team);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_TeamLeader_ManageMembers_ShouldSucceed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateUser(userId.ToString(), SystemRole.Developer);
        var team = CreateTeam();
        var teamMember = CreateTeamMember(team.Id, userId, TeamRole.Leader);
        var requirement = new TeamAccessRequirement(TeamAccessRequirement.Operations.ManageMembers);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, team);

        _teamMemberRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<TeamMember, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { teamMember });

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_TeamMember_Delete_ShouldFail()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateUser(userId.ToString(), SystemRole.Developer);
        var team = CreateTeam();
        var teamMember = CreateTeamMember(team.Id, userId, TeamRole.Member);
        var requirement = new TeamAccessRequirement(TeamAccessRequirement.Operations.Delete);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, team);

        _teamMemberRepositoryMock
            .Setup(x => x.GetTeamMemberAsync(team.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teamMember);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_NonMember_Read_ShouldFail()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateUser(userId.ToString(), SystemRole.Developer);
        var team = CreateTeam();
        var requirement = new TeamAccessRequirement(TeamAccessRequirement.Operations.Read);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, team);

        _teamMemberRepositoryMock
            .Setup(x => x.GetTeamMemberAsync(team.Id, userId, It.IsAny<CancellationToken>()))
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

    private static Team CreateTeam()
    {
        return new Team
        {
            Id = Guid.NewGuid(),
            Name = "Test Team",
            Description = "Test Description",
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
