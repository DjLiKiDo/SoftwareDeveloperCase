using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Features.User.Commands.AssignRole;
using SoftwareDeveloperCase.Domain.Entities;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Features.User.Commands;

public class AssignRoleCommandHandlerTests
{
    private readonly Mock<ILogger<AssignRoleCommandHandler>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUserRoleRepository> _mockUserRoleRepository;
    private readonly AssignRoleCommandHandler _handler;

    public AssignRoleCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<AssignRoleCommandHandler>>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRoleRepository = new Mock<IUserRoleRepository>();

        _mockUnitOfWork.Setup(x => x.UserRoleRepository).Returns(_mockUserRoleRepository.Object);

        _handler = new AssignRoleCommandHandler(_mockLogger.Object, _mockMapper.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ShouldAssignRoleSuccessfully_WhenValidCommandProvided()
    {
        // Arrange
        var userRoleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        
        var command = new AssignRoleCommand
        {
            UserId = userId,
            RoleId = roleId
        };

        var userRole = new UserRole
        {
            Id = userRoleId,
            UserId = userId,
            RoleId = roleId
        };

        _mockMapper.Setup(x => x.Map<UserRole>(command)).Returns(userRole);
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(userRoleId);
        _mockUserRoleRepository.Verify(x => x.Insert(userRole), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChanges(), Times.Once);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Role assigned (Id: {userRoleId})")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenSaveChangesFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        
        var command = new AssignRoleCommand
        {
            UserId = userId,
            RoleId = roleId
        };

        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId
        };

        _mockMapper.Setup(x => x.Map<UserRole>(command)).Returns(userRole);
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(0);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be("The role has not been assigned");
        
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("The role has not been assigned")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapCommandToUserRole_WhenCalled()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        
        var command = new AssignRoleCommand
        {
            UserId = userId,
            RoleId = roleId
        };

        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId
        };

        _mockMapper.Setup(x => x.Map<UserRole>(command)).Returns(userRole);
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockMapper.Verify(x => x.Map<UserRole>(command), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryMethods_InCorrectOrder()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        
        var command = new AssignRoleCommand
        {
            UserId = userId,
            RoleId = roleId
        };

        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId
        };
        
        var sequence = new MockSequence();

        _mockMapper.Setup(x => x.Map<UserRole>(command)).Returns(userRole);
        _mockUserRoleRepository.InSequence(sequence).Setup(x => x.Insert(userRole));
        _mockUnitOfWork.InSequence(sequence).Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockUserRoleRepository.Verify(x => x.Insert(userRole), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserRoleId_WhenRoleAssignedSuccessfully()
    {
        // Arrange
        var userRoleId = Guid.NewGuid();
        var command = new AssignRoleCommand
        {
            UserId = Guid.NewGuid(),
            RoleId = Guid.NewGuid()
        };

        var userRole = new UserRole { Id = userRoleId };
        _mockMapper.Setup(x => x.Map<UserRole>(command)).Returns(userRole);
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(userRoleId);
    }
}