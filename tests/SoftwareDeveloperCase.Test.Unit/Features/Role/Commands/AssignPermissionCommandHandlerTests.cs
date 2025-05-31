using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Features.Identity.Roles.Commands.AssignPermission;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace SoftwareDeveloperCase.Test.Unit.Features.Role.Commands;

public class AssignPermissionCommandHandlerTests
{
    private readonly Mock<ILogger<AssignPermissionCommandHandler>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRolePermissionRepository> _mockRolePermissionRepository;
    private readonly AssignPermissionCommandHandler _handler;

    public AssignPermissionCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<AssignPermissionCommandHandler>>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockRolePermissionRepository = new Mock<IRolePermissionRepository>();

        _mockUnitOfWork.Setup(x => x.RolePermissionRepository).Returns(_mockRolePermissionRepository.Object);

        _handler = new AssignPermissionCommandHandler(_mockLogger.Object, _mockMapper.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldAssignPermissionSuccessfully_WhenValidCommandProvided()
    {
        // Arrange
        var rolePermissionId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();

        var command = new AssignPermissionCommand
        {
            RoleId = roleId,
            PermissionId = permissionId
        };

        var rolePermission = new RolePermission
        {
            Id = rolePermissionId,
            RoleId = roleId,
            PermissionId = permissionId
        };

        _mockMapper.Setup(x => x.Map<RolePermission>(command)).Returns(rolePermission);
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(rolePermissionId);
        _mockRolePermissionRepository.Verify(x => x.Insert(rolePermission), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChanges(), Times.Once);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Permission assigned (Id: {rolePermissionId})")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldThrowException_WhenSaveChangesFails()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();

        var command = new AssignPermissionCommand
        {
            RoleId = roleId,
            PermissionId = permissionId
        };

        var rolePermission = new RolePermission
        {
            Id = Guid.NewGuid(),
            RoleId = roleId,
            PermissionId = permissionId
        };

        _mockMapper.Setup(x => x.Map<RolePermission>(command)).Returns(rolePermission);
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(0);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be("The permission has not been assigned");

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("The permission has not been assigned")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldMapCommandToRolePermission_WhenCalled()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();

        var command = new AssignPermissionCommand
        {
            RoleId = roleId,
            PermissionId = permissionId
        };

        var rolePermission = new RolePermission
        {
            Id = Guid.NewGuid(),
            RoleId = roleId,
            PermissionId = permissionId
        };

        _mockMapper.Setup(x => x.Map<RolePermission>(command)).Returns(rolePermission);
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockMapper.Verify(x => x.Map<RolePermission>(command), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldCallRepositoryMethods_InCorrectOrder()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();

        var command = new AssignPermissionCommand
        {
            RoleId = roleId,
            PermissionId = permissionId
        };

        var rolePermission = new RolePermission
        {
            Id = Guid.NewGuid(),
            RoleId = roleId,
            PermissionId = permissionId
        };

        var sequence = new MockSequence();

        _mockMapper.Setup(x => x.Map<RolePermission>(command)).Returns(rolePermission);
        _mockRolePermissionRepository.InSequence(sequence).Setup(x => x.Insert(rolePermission));
        _mockUnitOfWork.InSequence(sequence).Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRolePermissionRepository.Verify(x => x.Insert(rolePermission), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldReturnRolePermissionId_WhenPermissionAssignedSuccessfully()
    {
        // Arrange
        var rolePermissionId = Guid.NewGuid();
        var command = new AssignPermissionCommand
        {
            RoleId = Guid.NewGuid(),
            PermissionId = Guid.NewGuid()
        };

        var rolePermission = new RolePermission { Id = rolePermissionId };
        _mockMapper.Setup(x => x.Map<RolePermission>(command)).Returns(rolePermission);
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(rolePermissionId);
    }
}
