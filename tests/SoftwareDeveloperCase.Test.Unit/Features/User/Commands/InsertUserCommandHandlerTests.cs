using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.InsertUser;
using SoftwareDeveloperCase.Application.Models;
using SoftwareDeveloperCase.Domain.Entities.Core;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using Task = System.Threading.Tasks.Task;
using Xunit;
using UserEntity = SoftwareDeveloperCase.Domain.Entities.Core.User;
using RoleEntity = SoftwareDeveloperCase.Domain.Entities.Identity.Role;
using DomainEmail = SoftwareDeveloperCase.Domain.ValueObjects.Email;
using ApplicationEmail = SoftwareDeveloperCase.Application.Models.Email;

namespace SoftwareDeveloperCase.Test.Unit.Features.User.Commands;

public class InsertUserCommandHandlerTests
{
    private readonly Mock<ILogger<InsertUserCommandHandler>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRoleRepository> _mockRoleRepository;
    private readonly Mock<IUserRoleRepository> _mockUserRoleRepository;
    private readonly Mock<IDepartmentRepository> _mockDepartmentRepository;
    private readonly InsertUserCommandHandler _handler;

    public InsertUserCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<InsertUserCommandHandler>>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockEmailService = new Mock<IEmailService>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockRoleRepository = new Mock<IRoleRepository>();
        _mockUserRoleRepository = new Mock<IUserRoleRepository>();
        _mockDepartmentRepository = new Mock<IDepartmentRepository>();

        _mockUnitOfWork.Setup(x => x.UserRepository).Returns(_mockUserRepository.Object);
        _mockUnitOfWork.Setup(x => x.RoleRepository).Returns(_mockRoleRepository.Object);
        _mockUnitOfWork.Setup(x => x.UserRoleRepository).Returns(_mockUserRoleRepository.Object);
        _mockUnitOfWork.Setup(x => x.DepartmentRepository).Returns(_mockDepartmentRepository.Object);

        _handler = new InsertUserCommandHandler(_mockLogger.Object, _mockMapper.Object, _mockUnitOfWork.Object, _mockEmailService.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldInsertUserSuccessfully_WhenValidCommandProvided()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();

        var command = new InsertUserCommand
        {
            Name = "John Doe",
            Email = new DomainEmail("john.doe@example.com"),
            Password = "password123",
            DepartmentId = departmentId
        };

        var user = new UserEntity
        {
            Id = userId,
            Name = command.Name,
            Email = new DomainEmail(command.Email),
            Password = command.Password,
            DepartmentId = command.DepartmentId
        };

        var employeeRole = new RoleEntity { Id = roleId, Name = "Employee" };
        var departmentManagers = new List<UserEntity>
        {
            new() { Email = new DomainEmail("manager@example.com") }
        };

        _mockMapper.Setup(x => x.Map<UserEntity>(command)).Returns(user);
        _mockRoleRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RoleEntity, bool>>>()))
            .ReturnsAsync(new List<RoleEntity> { employeeRole });
        _mockDepartmentRepository.Setup(x => x.GetManagersAsync(departmentId))
            .ReturnsAsync(departmentManagers);
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(userId);
        _mockUserRepository.Verify(x => x.Insert(user), Times.Once);
        _mockUserRoleRepository.Verify(x => x.Insert(It.Is<UserRole>(ur => ur.UserId == userId && ur.RoleId == roleId)), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChanges(), Times.Once);
        _mockEmailService.Verify(x => x.SendEmail(It.IsAny<ApplicationEmail>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldThrowException_WhenSaveChangesFails()
    {
        // Arrange
        var command = new InsertUserCommand
        {
            Name = "John Doe",
            Email = new DomainEmail("john.doe@example.com"),
            Password = "password123",
            DepartmentId = Guid.NewGuid()
        };

        var user = new UserEntity { Id = Guid.NewGuid() };
        _mockMapper.Setup(x => x.Map<UserEntity>(command)).Returns(user);
        _mockRoleRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RoleEntity, bool>>>()))
            .ReturnsAsync(new List<RoleEntity>());
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(0);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be("The user was not inserted");
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("The user was not inserted")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldNotAssignRole_WhenEmployeeRoleDoesNotExist()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var command = new InsertUserCommand
        {
            Name = "John Doe",
            Email = new DomainEmail("john.doe@example.com"),
            Password = "password123",
            DepartmentId = departmentId
        };

        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            DepartmentId = departmentId
        };
        _mockMapper.Setup(x => x.Map<UserEntity>(command)).Returns(user);
        _mockRoleRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RoleEntity, bool>>>()))
            .ReturnsAsync(new List<RoleEntity>());
        _mockDepartmentRepository.Setup(x => x.GetManagersAsync(departmentId))
            .ReturnsAsync(new List<UserEntity>());
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(user.Id);
        _mockUserRoleRepository.Verify(x => x.Insert(It.IsAny<UserRole>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldContinueWhenEmailFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var command = new InsertUserCommand
        {
            Name = "John Doe",
            Email = new DomainEmail("john.doe@example.com"),
            Password = "password123",
            DepartmentId = departmentId
        };

        var user = new UserEntity
        {
            Id = userId,
            DepartmentId = departmentId
        };
        var departmentManagers = new List<UserEntity>
        {
            new() { Email = new DomainEmail("manager@example.com") }
        };

        _mockMapper.Setup(x => x.Map<UserEntity>(command)).Returns(user);
        _mockRoleRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RoleEntity, bool>>>()))
            .ReturnsAsync(new List<RoleEntity>());
        _mockDepartmentRepository.Setup(x => x.GetManagersAsync(departmentId))
            .ReturnsAsync(departmentManagers);
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(1);
        _mockEmailService.Setup(x => x.SendEmail(It.IsAny<ApplicationEmail>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Email service error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(userId);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error trying to send new user registration notification")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
