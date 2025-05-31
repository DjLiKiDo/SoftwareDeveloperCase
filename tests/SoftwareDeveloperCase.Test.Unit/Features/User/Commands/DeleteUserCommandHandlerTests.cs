using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Exceptions;
using SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.DeleteUser;
using SoftwareDeveloperCase.Domain.Entities.Core;
using SoftwareDeveloperCase.Domain.ValueObjects;
using Xunit;
using Task = System.Threading.Tasks.Task;
using UserEntity = SoftwareDeveloperCase.Domain.Entities.Core.User;

namespace SoftwareDeveloperCase.Test.Unit.Features.User.Commands;

public class DeleteUserCommandHandlerTests
{
    private readonly Mock<ILogger<DeleteUserCommandHandler>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<DeleteUserCommandHandler>>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepository = new Mock<IUserRepository>();

        _mockUnitOfWork.Setup(x => x.UserRepository).Returns(_mockUserRepository.Object);

        _handler = new DeleteUserCommandHandler(_mockLogger.Object, _mockMapper.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldDeleteUserSuccessfully_WhenValidCommandProvided()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand { Id = userId };

        var existingUser = new UserEntity
        {
            Id = userId,
            Name = "Test User",
            Email = new Email("test@example.com")
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(userId);
        _mockUserRepository.Verify(x => x.Delete(existingUser), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChanges(), Times.Once);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Entity deleted successfully --> Id: {userId}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand { Id = userId };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((UserEntity?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be($"Entity \"User\" ({userId}) was not found.");

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Error retrieving entity from database. Entity not found --> Id: {userId}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockUserRepository.Verify(x => x.Delete(It.IsAny<UserEntity>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChanges(), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldCallRepositoryMethods_InCorrectOrder()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand { Id = userId };
        var existingUser = new UserEntity { Id = userId };
        var sequence = new MockSequence();

        _mockUserRepository.InSequence(sequence).Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);
        _mockUserRepository.InSequence(sequence).Setup(x => x.Delete(existingUser));
        _mockUnitOfWork.InSequence(sequence).Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _mockUserRepository.Verify(x => x.Delete(existingUser), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldReturnRequestId_WhenUserDeletedSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand { Id = userId };
        var existingUser = new UserEntity { Id = userId };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(command.Id);
    }
}
