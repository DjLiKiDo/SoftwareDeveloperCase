using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Exceptions;
using SoftwareDeveloperCase.Application.Features.User.Commands.DeleteUser;
using Xunit;

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
    public async Task Handle_ShouldDeleteUserSuccessfully_WhenValidCommandProvided()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand { Id = userId };

        var existingUser = new Domain.Entities.User
        {
            Id = userId,
            Name = "Test User",
            Email = "test@example.com"
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
    public async Task Handle_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand { Id = userId };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((Domain.Entities.User?)null);

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
        
        _mockUserRepository.Verify(x => x.Delete(It.IsAny<Domain.Entities.User>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryMethods_InCorrectOrder()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand { Id = userId };
        var existingUser = new Domain.Entities.User { Id = userId };
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
    public async Task Handle_ShouldReturnRequestId_WhenUserDeletedSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand { Id = userId };
        var existingUser = new Domain.Entities.User { Id = userId };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(command.Id);
    }
}