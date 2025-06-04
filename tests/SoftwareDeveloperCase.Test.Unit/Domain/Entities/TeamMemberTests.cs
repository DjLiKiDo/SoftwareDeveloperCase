using FluentAssertions;
using SoftwareDeveloperCase.Domain.Entities.Team;
using SoftwareDeveloperCase.Domain.Enums.Core;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Domain.Entities;

public class TeamMemberTests
{
    [Fact]
    public void Constructor_Default_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var teamMember = new TeamMember();

        // Assert
        teamMember.Id.Should().Be(Guid.Empty);
        teamMember.TeamId.Should().Be(default(Guid));
        teamMember.UserId.Should().Be(default(Guid));
        teamMember.TeamRole.Should().Be(TeamRole.Member);
        teamMember.Status.Should().Be(MemberStatus.Active);
        teamMember.JoinedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        teamMember.LeftDate.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithParameters_ShouldInitializeCorrectly()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var teamRole = TeamRole.Leader;
        var status = MemberStatus.Active;

        // Act
        var teamMember = new TeamMember(teamId, userId, teamRole, status);

        // Assert
        teamMember.TeamId.Should().Be(teamId);
        teamMember.UserId.Should().Be(userId);
        teamMember.TeamRole.Should().Be(teamRole);
        teamMember.Status.Should().Be(status);
        teamMember.JoinedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        teamMember.LeftDate.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithDefaultParameters_ShouldUseDefaults()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var teamMember = new TeamMember(teamId, userId);

        // Assert
        teamMember.TeamId.Should().Be(teamId);
        teamMember.UserId.Should().Be(userId);
        teamMember.TeamRole.Should().Be(TeamRole.Member);
        teamMember.Status.Should().Be(MemberStatus.Active);
    }

    [Fact]
    public void MarkAsLeft_WhenCalled_ShouldSetStatusToInactiveAndLeftDate()
    {
        // Arrange
        var teamMember = new TeamMember
        {
            Status = MemberStatus.Active,
            LeftDate = null
        };

        // Act
        teamMember.MarkAsLeft();

        // Assert
        teamMember.Status.Should().Be(MemberStatus.Inactive);
        teamMember.LeftDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void Reactivate_WhenCalled_ShouldSetStatusToActiveAndClearLeftDate()
    {
        // Arrange
        var teamMember = new TeamMember
        {
            Status = MemberStatus.Inactive,
            LeftDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        teamMember.Reactivate();

        // Assert
        teamMember.Status.Should().Be(MemberStatus.Active);
        teamMember.LeftDate.Should().BeNull();
    }

    [Fact]
    public void PromoteToLeader_WhenCalled_ShouldSetTeamRoleToLeader()
    {
        // Arrange
        var teamMember = new TeamMember { TeamRole = TeamRole.Member };

        // Act
        teamMember.PromoteToLeader();

        // Assert
        teamMember.TeamRole.Should().Be(TeamRole.Leader);
    }

    [Fact]
    public void DemoteFromLeader_WhenCalled_ShouldSetTeamRoleToMember()
    {
        // Arrange
        var teamMember = new TeamMember { TeamRole = TeamRole.Leader };

        // Act
        teamMember.DemoteFromLeader();

        // Assert
        teamMember.TeamRole.Should().Be(TeamRole.Member);
    }

    [Fact]
    public void TeamMemberWorkflow_FullLifecycle_ShouldWorkCorrectly()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var teamMember = new TeamMember(teamId, userId);

        // Act & Assert - Initial state
        teamMember.Status.Should().Be(MemberStatus.Active);
        teamMember.TeamRole.Should().Be(TeamRole.Member);
        teamMember.LeftDate.Should().BeNull();

        // Promote to leader
        teamMember.PromoteToLeader();
        teamMember.TeamRole.Should().Be(TeamRole.Leader);

        // Mark as left
        teamMember.MarkAsLeft();
        teamMember.Status.Should().Be(MemberStatus.Inactive);
        teamMember.LeftDate.Should().NotBeNull();

        // Reactivate
        teamMember.Reactivate();
        teamMember.Status.Should().Be(MemberStatus.Active);
        teamMember.LeftDate.Should().BeNull();
        teamMember.TeamRole.Should().Be(TeamRole.Leader); // Should remain leader

        // Demote from leader
        teamMember.DemoteFromLeader();
        teamMember.TeamRole.Should().Be(TeamRole.Member);
    }

    [Fact]
    public void BaseEntityProperties_ShouldBeInherited()
    {
        // Arrange & Act
        var teamMember = new TeamMember();

        // Assert
        teamMember.Id.Should().Be(Guid.Empty);
        teamMember.CreatedBy.Should().BeNull();
        teamMember.CreatedOn.Should().Be(default);
        teamMember.LastModifiedBy.Should().BeNull();
        teamMember.LastModifiedOn.Should().BeNull();
    }

    [Theory]
    [InlineData(TeamRole.Member)]
    [InlineData(TeamRole.Leader)]
    public void TeamRole_SetValidRole_ShouldSetCorrectly(TeamRole role)
    {
        // Arrange
        var teamMember = new TeamMember();

        // Act
        teamMember.TeamRole = role;

        // Assert
        teamMember.TeamRole.Should().Be(role);
    }

    [Theory]
    [InlineData(MemberStatus.Active)]
    [InlineData(MemberStatus.Inactive)]
    [InlineData(MemberStatus.OnLeave)]
    public void Status_SetValidStatus_ShouldSetCorrectly(MemberStatus status)
    {
        // Arrange
        var teamMember = new TeamMember();

        // Act
        teamMember.Status = status;

        // Assert
        teamMember.Status.Should().Be(status);
    }

    [Fact]
    public void JoinedDate_SetValidDate_ShouldSetCorrectly()
    {
        // Arrange
        var teamMember = new TeamMember();
        var expectedDate = DateTime.UtcNow.AddDays(-30);

        // Act
        teamMember.JoinedDate = expectedDate;

        // Assert
        teamMember.JoinedDate.Should().Be(expectedDate);
    }

    [Fact]
    public void LeftDate_SetValidDate_ShouldSetCorrectly()
    {
        // Arrange
        var teamMember = new TeamMember();
        var expectedDate = DateTime.UtcNow.AddDays(-1);

        // Act
        teamMember.LeftDate = expectedDate;

        // Assert
        teamMember.LeftDate.Should().Be(expectedDate);
    }
}