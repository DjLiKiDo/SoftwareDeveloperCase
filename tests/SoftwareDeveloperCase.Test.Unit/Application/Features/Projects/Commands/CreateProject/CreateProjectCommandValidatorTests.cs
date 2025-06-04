using FluentAssertions;
using Moq;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Core;
using SoftwareDeveloperCase.Application.Features.Projects.Commands.CreateProject;
using SoftwareDeveloperCase.Domain.Entities.Project;
using SoftwareDeveloperCase.Domain.Entities.Team;
using SoftwareDeveloperCase.Domain.Enums.Core;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandValidatorTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IProjectRepository> _mockProjectRepository;
    private readonly Mock<ITeamRepository> _mockTeamRepository;
    private readonly CreateProjectCommandValidator _validator;

    public CreateProjectCommandValidatorTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockProjectRepository = new Mock<IProjectRepository>();
        _mockTeamRepository = new Mock<ITeamRepository>();

        _mockUnitOfWork.Setup(x => x.ProjectRepository).Returns(_mockProjectRepository.Object);
        _mockUnitOfWork.Setup(x => x.TeamRepository).Returns(_mockTeamRepository.Object);

        _validator = new CreateProjectCommandValidator(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Validate_WithValidCommand_ShouldPass()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var command = new CreateProjectCommand
        {
            Name = "Test Project",
            Description = "Test Description",
            TeamId = teamId,
            Priority = Priority.Medium,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        _mockTeamRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Team, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team> { new Team { Id = teamId } });

        _mockProjectRepository.Setup(x => x.IsProjectNameExistsInTeamAsync(teamId, command.Name, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public async Task Validate_WithInvalidName_ShouldFail(string? name)
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Name = name!,
            TeamId = Guid.NewGuid()
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProjectCommand.Name));
    }

    [Theory]
    [InlineData("AB")] // Too short (2 chars)
    [InlineData("A")] // Too short (1 char)
    public async Task Validate_WithTooShortName_ShouldFail(string name)
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Name = name,
            TeamId = Guid.NewGuid()
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProjectCommand.Name) && e.ErrorMessage.Contains("3 and 100"));
    }

    [Fact]
    public async Task Validate_WithTooLongName_ShouldFail()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Name = new string('A', 101), // 101 characters
            TeamId = Guid.NewGuid()
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProjectCommand.Name) && e.ErrorMessage.Contains("3 and 100"));
    }

    [Fact]
    public async Task Validate_WithTooLongDescription_ShouldFail()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var command = new CreateProjectCommand
        {
            Name = "Valid Project Name",
            Description = new string('A', 1001), // 1001 characters
            TeamId = teamId
        };

        _mockTeamRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Team, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team> { new Team { Id = teamId } });

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProjectCommand.Description) && e.ErrorMessage.Contains("1000"));
    }

    [Fact]
    public async Task Validate_WithEmptyTeamId_ShouldFail()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Name = "Test Project",
            TeamId = Guid.Empty
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProjectCommand.TeamId));
    }

    [Fact]
    public async Task Validate_WithNonExistentTeam_ShouldFail()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var command = new CreateProjectCommand
        {
            Name = "Test Project",
            TeamId = teamId
        };

        _mockTeamRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Team, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team>()); // No teams found

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProjectCommand.TeamId) && e.ErrorMessage.Contains("does not exist"));
    }

    [Fact]
    public async Task Validate_WithDuplicateProjectNameInTeam_ShouldFail()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var command = new CreateProjectCommand
        {
            Name = "Existing Project",
            TeamId = teamId
        };

        _mockTeamRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Team, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team> { new Team { Id = teamId } });

        _mockProjectRepository.Setup(x => x.IsProjectNameExistsInTeamAsync(teamId, command.Name, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true); // Project name exists

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProjectCommand.Name) && e.ErrorMessage.Contains("already exists"));
    }

    [Fact]
    public async Task Validate_WithEndDateBeforeStartDate_ShouldFail()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var startDate = DateTime.UtcNow;
        var command = new CreateProjectCommand
        {
            Name = "Test Project",
            TeamId = teamId,
            StartDate = startDate,
            EndDate = startDate.AddDays(-1) // End date before start date
        };

        _mockTeamRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Team, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team> { new Team { Id = teamId } });

        _mockProjectRepository.Setup(x => x.IsProjectNameExistsInTeamAsync(teamId, command.Name, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProjectCommand.EndDate) && e.ErrorMessage.Contains("after start date"));
    }

    [Fact]
    public async Task Validate_WithNullEndDate_ShouldPass()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var command = new CreateProjectCommand
        {
            Name = "Test Project",
            TeamId = teamId,
            StartDate = DateTime.UtcNow,
            EndDate = null // Null end date should be valid
        };

        _mockTeamRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Team, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team> { new Team { Id = teamId } });

        _mockProjectRepository.Setup(x => x.IsProjectNameExistsInTeamAsync(teamId, command.Name, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(Priority.Low)]
    [InlineData(Priority.Medium)]
    [InlineData(Priority.High)]
    [InlineData(Priority.Critical)]
    public async Task Validate_WithValidPriority_ShouldPass(Priority priority)
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var command = new CreateProjectCommand
        {
            Name = "Test Project",
            TeamId = teamId,
            Priority = priority
        };

        _mockTeamRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Team, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team> { new Team { Id = teamId } });

        _mockProjectRepository.Setup(x => x.IsProjectNameExistsInTeamAsync(teamId, command.Name, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}