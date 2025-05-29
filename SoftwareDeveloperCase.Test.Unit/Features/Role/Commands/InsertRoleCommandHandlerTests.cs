using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Features.Role.Commands.InsertRole;
using SoftwareDeveloperCase.Domain.Entities;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Features.Role.Commands;

public class InsertRoleCommandHandlerTests
{
    private readonly Mock<ILogger<InsertRoleCommandHandler>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRoleRepository> _mockRoleRepository;
    private readonly InsertRoleCommandHandler _handler;

    public InsertRoleCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<InsertRoleCommandHandler>>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockRoleRepository = new Mock<IRoleRepository>();

        _mockUnitOfWork.Setup(x => x.RoleRepository).Returns(_mockRoleRepository.Object);

        _handler = new InsertRoleCommandHandler(_mockLogger.Object, _mockMapper.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ShouldInsertRoleSuccessfully_WhenValidCommandProvided()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var command = new InsertRoleCommand
        {
            Name = "Manager",
            ParentRoleId = null
        };

        var role = new Domain.Entities.Role
        {
            Id = roleId,
            Name = command.Name,
            ParentRoleId = command.ParentRoleId
        };

        _mockMapper.Setup(x => x.Map<Domain.Entities.Role>(command)).Returns(role);
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(roleId);
        _mockRoleRepository.Verify(x => x.Insert(role), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChanges(), Times.Once);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"New role registered (Id: {roleId})")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenSaveChangesFails()
    {
        // Arrange
        var command = new InsertRoleCommand
        {
            Name = "Manager",
            ParentRoleId = null
        };

        var role = new Domain.Entities.Role { Id = Guid.NewGuid() };
        _mockMapper.Setup(x => x.Map<Domain.Entities.Role>(command)).Returns(role);
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(0);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be("The role was not inserted");

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("The role was not inserted")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapCommandToRole_WhenCalled()
    {
        // Arrange
        var command = new InsertRoleCommand
        {
            Name = "Manager",
            ParentRoleId = Guid.NewGuid()
        };

        var role = new Domain.Entities.Role
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            ParentRoleId = command.ParentRoleId
        };

        _mockMapper.Setup(x => x.Map<Domain.Entities.Role>(command)).Returns(role);
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockMapper.Verify(x => x.Map<Domain.Entities.Role>(command), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryMethods_InCorrectOrder()
    {
        // Arrange
        var command = new InsertRoleCommand
        {
            Name = "Manager",
            ParentRoleId = null
        };

        var role = new Domain.Entities.Role { Id = Guid.NewGuid() };
        var sequence = new MockSequence();

        _mockMapper.Setup(x => x.Map<Domain.Entities.Role>(command)).Returns(role);
        _mockRoleRepository.InSequence(sequence).Setup(x => x.Insert(role));
        _mockUnitOfWork.InSequence(sequence).Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRoleRepository.Verify(x => x.Insert(role), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnRoleId_WhenRoleInsertedSuccessfully()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var command = new InsertRoleCommand { Name = "Manager" };
        var role = new Domain.Entities.Role { Id = roleId };

        _mockMapper.Setup(x => x.Map<Domain.Entities.Role>(command)).Returns(role);
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(roleId);
    }

    [Fact]
    public async Task Handle_ShouldHandleHierarchicalRole_WhenParentRoleIdProvided()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var parentRoleId = Guid.NewGuid();
        var command = new InsertRoleCommand
        {
            Name = "Senior Manager",
            ParentRoleId = parentRoleId
        };

        var role = new Domain.Entities.Role
        {
            Id = roleId,
            Name = command.Name,
            ParentRoleId = parentRoleId
        };

        _mockMapper.Setup(x => x.Map<Domain.Entities.Role>(command)).Returns(role);
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(roleId);
        _mockRoleRepository.Verify(x => x.Insert(It.Is<Domain.Entities.Role>(r => r.ParentRoleId == parentRoleId)), Times.Once);
    }
}
