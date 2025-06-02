using FluentAssertions;
using SoftwareDeveloperCase.Application.Features.Projects.Queries.GetProjects;
using SoftwareDeveloperCase.Domain.Enums.Core;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Application.Features.Projects.Queries.GetProjects;

public class GetProjectsQueryTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var pageNumber = 2;
        var pageSize = 20;
        var searchTerm = "test project";
        var status = "Active";
        var teamId = Guid.NewGuid();
        var createdFrom = DateTime.UtcNow.AddDays(-10);
        var createdTo = DateTime.UtcNow.AddDays(-1);

        // Act
        var query = new GetProjectsQuery(pageNumber, pageSize, searchTerm, status, teamId, createdFrom, createdTo);

        // Assert
        query.PageNumber.Should().Be(pageNumber);
        query.PageSize.Should().Be(pageSize);
        query.SearchTerm.Should().Be(searchTerm);
        query.Status.Should().Be(ProjectStatus.Active);
        query.TeamId.Should().Be(teamId);
        query.CreatedFrom.Should().Be(createdFrom);
        query.CreatedTo.Should().Be(createdTo);
    }

    [Fact]
    public void Constructor_WithInvalidPageNumber_ShouldDefaultToOne()
    {
        // Arrange & Act
        var query = new GetProjectsQuery(0, 10);

        // Assert
        query.PageNumber.Should().Be(1);
    }

    [Fact]
    public void Constructor_WithInvalidPageSize_ShouldDefaultToTen()
    {
        // Arrange & Act
        var query = new GetProjectsQuery(1, 0);

        // Assert
        query.PageSize.Should().Be(10);
    }

    [Fact]
    public void Constructor_WithInvalidStatus_ShouldLeaveStatusNull()
    {
        // Arrange & Act
        var query = new GetProjectsQuery(1, 10, status: "InvalidStatus");

        // Assert
        query.Status.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithValidStatusString_ShouldParseCorrectly()
    {
        // Arrange & Act
        var queryActive = new GetProjectsQuery(1, 10, status: "Active");
        var queryPlanning = new GetProjectsQuery(1, 10, status: "planning"); // Case insensitive
        var queryCompleted = new GetProjectsQuery(1, 10, status: "COMPLETED");

        // Assert
        queryActive.Status.Should().Be(ProjectStatus.Active);
        queryPlanning.Status.Should().Be(ProjectStatus.Planning);
        queryCompleted.Status.Should().Be(ProjectStatus.Completed);
    }

    [Fact]
    public void DefaultConstructor_ShouldSetDefaultValues()
    {
        // Arrange & Act
        var query = new GetProjectsQuery();

        // Assert
        query.PageNumber.Should().Be(1);
        query.PageSize.Should().Be(10);
        query.SearchTerm.Should().BeNull();
        query.Status.Should().BeNull();
        query.TeamId.Should().BeNull();
        query.CreatedFrom.Should().BeNull();
        query.CreatedTo.Should().BeNull();
    }

    [Theory]
    [InlineData("", null)]
    [InlineData("   ", null)]
    [InlineData("Active", ProjectStatus.Active)]
    [InlineData("Planning", ProjectStatus.Planning)]
    [InlineData("OnHold", ProjectStatus.OnHold)]
    [InlineData("Completed", ProjectStatus.Completed)]
    [InlineData("Cancelled", ProjectStatus.Cancelled)]
    public void Constructor_WithDifferentStatusValues_ShouldHandleCorrectly(string statusInput, ProjectStatus? expectedStatus)
    {
        // Arrange & Act
        var query = new GetProjectsQuery(1, 10, status: statusInput);

        // Assert
        query.Status.Should().Be(expectedStatus);
    }

    [Fact]
    public void Constructor_WithNullStatus_ShouldLeaveStatusNull()
    {
        // Arrange & Act
        var query = new GetProjectsQuery(1, 10, status: null);

        // Assert
        query.Status.Should().BeNull();
    }
}