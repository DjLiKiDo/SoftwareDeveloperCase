using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Exceptions;
using SoftwareDeveloperCase.Application.Features.User.Commands.UpdateUser;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Features.User.Commands;

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<ILogger<UpdateUserCommandHandler>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<UpdateUserCommandHandler>>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepository = new Mock<IUserRepository>();

        _mockUnitOfWork.Setup(x => x.UserRepository).Returns(_mockUserRepository.Object);

        _handler = new UpdateUserCommandHandler(_mockLogger.Object, _mockMapper.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateUserSuccessfully_WhenValidCommandProvided()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateUserCommand
        {
            Id = userId,
            Name = "Updated Name",
            Email = "updated@example.com",
            Password = "newpassword123",
            DepartmentId = Guid.NewGuid()
        };

        var existingUser = new Domain.Entities.User
        {
            Id = userId,
            Name = "Original Name",
            Email = "original@example.com"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(userId);
        _mockMapper.Verify(x => x.Map(command, existingUser, typeof(UpdateUserCommand), typeof(Domain.Entities.User)), Times.Once);
        _mockUserRepository.Verify(x => x.Update(existingUser), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChanges(), Times.Once);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Entity updated successfully --> Id: {userId}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateUserCommand
        {
            Id = userId,
            Name = "Updated Name",
            Email = "updated@example.com",
            Password = "newpassword123",
            DepartmentId = Guid.NewGuid()
        };

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

        _mockUserRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.User>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldMapCommandToUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var command = new UpdateUserCommand
        {
            Id = userId,
            Name = "Updated Name",
            Email = "updated@example.com",
            Password = "newpassword123",
            DepartmentId = departmentId
        };

        var existingUser = new Domain.Entities.User
        {
            Id = userId,
            Name = "Original Name",
            Email = "original@example.com",
            DepartmentId = Guid.NewGuid()
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockMapper.Verify(x => x.Map(command, existingUser, typeof(UpdateUserCommand), typeof(Domain.Entities.User)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryMethods_InCorrectOrder()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateUserCommand
        {
            Id = userId,
            Name = "Updated Name",
            Email = "updated@example.com",
            Password = "newpassword123",
            DepartmentId = Guid.NewGuid()
        };

        var existingUser = new Domain.Entities.User { Id = userId };
        var sequence = new MockSequence();

        _mockUserRepository.InSequence(sequence).Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);
        _mockUserRepository.InSequence(sequence).Setup(x => x.Update(existingUser));
        _mockUnitOfWork.InSequence(sequence).Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _mockUserRepository.Verify(x => x.Update(existingUser), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChanges(), Times.Once);
    }
}
