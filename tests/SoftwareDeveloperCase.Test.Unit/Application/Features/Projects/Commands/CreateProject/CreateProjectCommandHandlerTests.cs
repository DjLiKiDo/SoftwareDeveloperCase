using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Core;
using SoftwareDeveloperCase.Application.Exceptions;
using SoftwareDeveloperCase.Application.Features.Projects.Commands.CreateProject;
using SoftwareDeveloperCase.Domain.Entities.Project;
using SoftwareDeveloperCase.Domain.Entities.Team;
using SoftwareDeveloperCase.Domain.Enums.Core;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandHandlerTests
{
    private readonly Mock<ILogger<CreateProjectCommandHandler>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IProjectRepository> _mockProjectRepository;
    private readonly Mock<ITeamRepository> _mockTeamRepository;
    private readonly CreateProjectCommandHandler _handler;

    public CreateProjectCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<CreateProjectCommandHandler>>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockProjectRepository = new Mock<IProjectRepository>();
        _mockTeamRepository = new Mock<ITeamRepository>();

        _mockUnitOfWork.Setup(x => x.ProjectRepository).Returns(_mockProjectRepository.Object);
        _mockUnitOfWork.Setup(x => x.TeamRepository).Returns(_mockTeamRepository.Object);

        _handler = new CreateProjectCommandHandler(
            _mockLogger.Object,
            _mockMapper.Object,
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateProjectSuccessfully()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var command = new CreateProjectCommand
        {
            Name = "Test Project",
            Description = "Test Description",
            TeamId = teamId,
            Priority = Priority.High,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        var teams = new List<Team> { new Team { Id = teamId, Name = "Test Team" } };
        _mockTeamRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Team, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);

        _mockProjectRepository.Setup(x => x.IsProjectNameExistsInTeamAsync(teamId, command.Name, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var createdProject = new Project
        {
            Id = projectId,
            Name = command.Name,
            Description = command.Description,
            TeamId = teamId,
            Status = ProjectStatus.Planning,
            Priority = command.Priority
        };

        _mockProjectRepository.Setup(x => x.InsertAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdProject);

        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(projectId);
        
        _mockProjectRepository.Verify(x => x.InsertAsync(It.Is<Project>(p => 
            p.Name == command.Name &&
            p.Description == command.Description &&
            p.TeamId == teamId &&
            p.Status == ProjectStatus.Planning &&
            p.Priority == command.Priority), It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentTeam_ShouldThrowNotFoundException()
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

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => 
            _handler.Handle(command, CancellationToken.None));

        exception.Message.Should().Contain($"Team with ID {teamId} not found");
        
        _mockProjectRepository.Verify(x => x.InsertAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithDuplicateProjectName_ShouldThrowBusinessRuleViolationException()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var command = new CreateProjectCommand
        {
            Name = "Existing Project",
            TeamId = teamId
        };

        var teams = new List<Team> { new Team { Id = teamId, Name = "Test Team" } };
        _mockTeamRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Team, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);

        _mockProjectRepository.Setup(x => x.IsProjectNameExistsInTeamAsync(teamId, command.Name, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true); // Project name exists

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => 
            _handler.Handle(command, CancellationToken.None));

        exception.Message.Should().Contain($"Project name '{command.Name}' already exists in the specified team");
        
        _mockProjectRepository.Verify(x => x.InsertAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldSetStatusToPlanningByDefault()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var command = new CreateProjectCommand
        {
            Name = "Test Project",
            TeamId = teamId,
            Priority = Priority.Medium
        };

        var teams = new List<Team> { new Team { Id = teamId } };
        _mockTeamRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Team, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);

        _mockProjectRepository.Setup(x => x.IsProjectNameExistsInTeamAsync(teamId, command.Name, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var createdProject = new Project { Id = projectId };
        _mockProjectRepository.Setup(x => x.InsertAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdProject);

        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockProjectRepository.Verify(x => x.InsertAsync(It.Is<Project>(p => 
            p.Status == ProjectStatus.Planning), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldAssignCorrectTeamId()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var command = new CreateProjectCommand
        {
            Name = "Test Project",
            TeamId = teamId
        };

        var teams = new List<Team> { new Team { Id = teamId } };
        _mockTeamRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Team, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);

        _mockProjectRepository.Setup(x => x.IsProjectNameExistsInTeamAsync(teamId, command.Name, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var createdProject = new Project { Id = projectId };
        _mockProjectRepository.Setup(x => x.InsertAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdProject);

        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockProjectRepository.Verify(x => x.InsertAsync(It.Is<Project>(p => 
            p.TeamId == teamId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogInformationMessages()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var command = new CreateProjectCommand
        {
            Name = "Test Project",
            TeamId = teamId
        };

        var teams = new List<Team> { new Team { Id = teamId } };
        _mockTeamRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Team, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);

        _mockProjectRepository.Setup(x => x.IsProjectNameExistsInTeamAsync(teamId, command.Name, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var createdProject = new Project { Id = projectId };
        _mockProjectRepository.Setup(x => x.InsertAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdProject);

        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Creating new project")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Project created successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(Priority.Low)]
    [InlineData(Priority.Medium)]
    [InlineData(Priority.High)]
    [InlineData(Priority.Critical)]
    public async Task Handle_ShouldRespectProvidedPriority(Priority priority)
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var command = new CreateProjectCommand
        {
            Name = "Test Project",
            TeamId = teamId,
            Priority = priority
        };

        var teams = new List<Team> { new Team { Id = teamId } };
        _mockTeamRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Team, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);

        _mockProjectRepository.Setup(x => x.IsProjectNameExistsInTeamAsync(teamId, command.Name, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var createdProject = new Project { Id = projectId };
        _mockProjectRepository.Setup(x => x.InsertAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdProject);

        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockProjectRepository.Verify(x => x.InsertAsync(It.Is<Project>(p => 
            p.Priority == priority), It.IsAny<CancellationToken>()), Times.Once);
    }
}