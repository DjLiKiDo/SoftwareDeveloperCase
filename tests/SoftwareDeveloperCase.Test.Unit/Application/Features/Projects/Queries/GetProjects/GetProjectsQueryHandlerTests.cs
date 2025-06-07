using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Core;
using SoftwareDeveloperCase.Application.Features.Projects.DTOs;
using SoftwareDeveloperCase.Application.Features.Projects.Queries.GetProjects;
using SoftwareDeveloperCase.Domain.Entities.Project;
using SoftwareDeveloperCase.Domain.Enums.Core;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Application.Features.Projects.Queries.GetProjects;

public class GetProjectsQueryHandlerTests
{
    private readonly Mock<ILogger<GetProjectsQueryHandler>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IProjectRepository> _mockProjectRepository;
    private readonly GetProjectsQueryHandler _handler;

    public GetProjectsQueryHandlerTests()
    {
        _mockLogger = new Mock<ILogger<GetProjectsQueryHandler>>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockProjectRepository = new Mock<IProjectRepository>();

        _mockUnitOfWork.Setup(u => u.ProjectRepository).Returns(_mockProjectRepository.Object);

        _handler = new GetProjectsQueryHandler(_mockLogger.Object, _mockMapper.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldReturnPagedResult()
    {
        // Arrange
        var query = new GetProjectsQuery(1, 10);
        var projects = new List<Project>
        {
            new() { Id = Guid.NewGuid(), Name = "Project 1", Status = ProjectStatus.Active, CreatedOn = DateTime.UtcNow.AddDays(-5) },
            new() { Id = Guid.NewGuid(), Name = "Project 2", Status = ProjectStatus.Planning, CreatedOn = DateTime.UtcNow.AddDays(-3) }
        };
        var projectDtos = new List<ProjectDto>
        {
            new() { Id = projects[0].Id, Name = "Project 1", Status = ProjectStatus.Active },
            new() { Id = projects[1].Id, Name = "Project 2", Status = ProjectStatus.Planning }
        };

        var mockQueryable = projects.AsQueryable();
        _mockProjectRepository.Setup(r => r.GetQueryable()).Returns(mockQueryable);
        _mockProjectRepository.Setup(r => r.CountAsync(It.IsAny<IQueryable<Project>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects.Count);
        _mockProjectRepository.Setup(r => r.GetPagedAsync(It.IsAny<IQueryable<Project>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects.AsReadOnly());
        _mockMapper.Setup(m => m.Map<IEnumerable<ProjectDto>>(It.IsAny<IEnumerable<Project>>()))
            .Returns(projectDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Items.Should().HaveCount(2);
        result.Value.PageNumber.Should().Be(1);
        result.Value.PageSize.Should().Be(10);
        result.Value.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WithDateRangeFilter_ShouldApplyCorrectFiltering()
    {
        // Arrange
        var fromDate = DateTime.UtcNow.AddDays(-10);
        var toDate = DateTime.UtcNow.AddDays(-1);
        var query = new GetProjectsQuery(1, 10, createdFrom: fromDate, createdTo: toDate);

        var projects = new List<Project>
        {
            new() { Id = Guid.NewGuid(), Name = "Project 1", CreatedOn = DateTime.UtcNow.AddDays(-5) },
            new() { Id = Guid.NewGuid(), Name = "Project 2", CreatedOn = DateTime.UtcNow.AddDays(-15) } // Should be filtered out
        };

        var mockQueryable = projects.AsQueryable();
        _mockProjectRepository.Setup(r => r.GetQueryable()).Returns(mockQueryable);
        _mockProjectRepository.Setup(r => r.CountAsync(It.IsAny<IQueryable<Project>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1); // Only one project should remain after filtering
        _mockProjectRepository.Setup(r => r.GetPagedAsync(It.IsAny<IQueryable<Project>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Project> { projects[0] }.AsReadOnly());
        _mockMapper.Setup(m => m.Map<IEnumerable<ProjectDto>>(It.IsAny<IEnumerable<Project>>()))
            .Returns(new List<ProjectDto> { new() { Id = projects[0].Id, Name = "Project 1" } });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.TotalCount.Should().Be(1);

        // Verify that the repository was called with the correct parameters
        _mockProjectRepository.Verify(r => r.CountAsync(
            It.IsAny<IQueryable<Project>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithStatusFilter_ShouldApplyStatusFiltering()
    {
        // Arrange
        var query = new GetProjectsQuery(1, 10, status: "Active");
        var projects = new List<Project>
        {
            new() { Id = Guid.NewGuid(), Name = "Project 1", Status = ProjectStatus.Active, CreatedOn = DateTime.UtcNow.AddDays(-5) },
            new() { Id = Guid.NewGuid(), Name = "Project 2", Status = ProjectStatus.Planning, CreatedOn = DateTime.UtcNow.AddDays(-3) }
        };

        var mockQueryable = projects.AsQueryable();
        _mockProjectRepository.Setup(r => r.GetQueryable()).Returns(mockQueryable);
        _mockProjectRepository.Setup(r => r.CountAsync(It.IsAny<IQueryable<Project>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mockProjectRepository.Setup(r => r.GetPagedAsync(It.IsAny<IQueryable<Project>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Project> { projects[0] }.AsReadOnly());
        _mockMapper.Setup(m => m.Map<IEnumerable<ProjectDto>>(It.IsAny<IEnumerable<Project>>()))
            .Returns(new List<ProjectDto> { new() { Id = projects[0].Id, Name = "Project 1", Status = ProjectStatus.Active } });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.TotalCount.Should().Be(1);
        result.Value.Items.First().Status.Should().Be(ProjectStatus.Active);
    }

    [Fact]
    public async Task Handle_ShouldOrderByCreatedOnDescending()
    {
        // Arrange
        var query = new GetProjectsQuery(1, 10);
        var projects = new List<Project>
        {
            new() { Id = Guid.NewGuid(), Name = "Project 1", CreatedOn = DateTime.UtcNow.AddDays(-5) },
            new() { Id = Guid.NewGuid(), Name = "Project 2", CreatedOn = DateTime.UtcNow.AddDays(-3) },
            new() { Id = Guid.NewGuid(), Name = "Project 3", CreatedOn = DateTime.UtcNow.AddDays(-1) }
        };

        var mockQueryable = projects.AsQueryable();
        _mockProjectRepository.Setup(r => r.GetQueryable()).Returns(mockQueryable);
        _mockProjectRepository.Setup(r => r.CountAsync(It.IsAny<IQueryable<Project>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects.Count);
        _mockProjectRepository.Setup(r => r.GetPagedAsync(It.IsAny<IQueryable<Project>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects.OrderByDescending(p => p.CreatedOn).ToList().AsReadOnly());
        _mockMapper.Setup(m => m.Map<IEnumerable<ProjectDto>>(It.IsAny<IEnumerable<Project>>()))
            .Returns(projects.OrderByDescending(p => p.CreatedOn).Select(p => new ProjectDto { Id = p.Id, Name = p.Name }).ToList());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Items.Should().HaveCount(3);

        // Verify that ordering is applied correctly
        _mockProjectRepository.Verify(r => r.GetPagedAsync(
            It.IsAny<IQueryable<Project>>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

}
