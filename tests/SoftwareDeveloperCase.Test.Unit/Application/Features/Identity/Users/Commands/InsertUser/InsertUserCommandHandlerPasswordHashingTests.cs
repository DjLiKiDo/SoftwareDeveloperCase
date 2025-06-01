using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.InsertUser;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Application.Contracts.Services;
using AutoMapper;
using SoftwareDeveloperCase.Infrastructure.Services;
using SoftwareDeveloperCase.Domain.Entities;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Application.Features.Identity.Users.Commands.InsertUser;

public class InsertUserCommandHandlerPasswordHashingTests
{
    private readonly Mock<ILogger<InsertUserCommandHandler>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly IPasswordService _passwordService;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRoleRepository> _mockRoleRepository;
    private readonly Mock<IUserRoleRepository> _mockUserRoleRepository;

    public InsertUserCommandHandlerPasswordHashingTests()
    {
        _mockLogger = new Mock<ILogger<InsertUserCommandHandler>>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockEmailService = new Mock<IEmailService>();
        _passwordService = new PasswordService(); // Use real password service
        _mockUserRepository = new Mock<IUserRepository>();
        _mockRoleRepository = new Mock<IRoleRepository>();
        _mockUserRoleRepository = new Mock<IUserRoleRepository>();

        // Setup UnitOfWork mock
        _mockUnitOfWork.Setup(x => x.UserRepository).Returns(_mockUserRepository.Object);
        _mockUnitOfWork.Setup(x => x.RoleRepository).Returns(_mockRoleRepository.Object);
        _mockUnitOfWork.Setup(x => x.UserRoleRepository).Returns(_mockUserRoleRepository.Object);
        _mockUnitOfWork.Setup(x => x.SaveChanges()).ReturnsAsync(1);
    }

    [Fact]
    public async Task Handle_ShouldHashPasswordBeforeSaving()
    {
        // Arrange
        const string plainTextPassword = "MySecurePassword123!";
        var command = new InsertUserCommand
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = plainTextPassword
        };

        var userEntity = new User
        {
            Name = command.Name,
            Email = new SoftwareDeveloperCase.Domain.ValueObjects.Email("test@example.com"),
            Password = plainTextPassword // Initially plain text
        };

        _mockMapper.Setup(m => m.Map<User>(command)).Returns(userEntity);

        // Setup role repository to return default role
        var roles = new List<Role>
        {
            new() { Id = Guid.NewGuid(), Name = "Employee" }
        };
        _mockRoleRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(roles);

        var handler = new InsertUserCommandHandler(
            _mockLogger.Object,
            _mockMapper.Object,
            _mockUnitOfWork.Object,
            _mockEmailService.Object,
            _passwordService);

        User? capturedUser = null;
        _mockUserRepository.Setup(r => r.Insert(It.IsAny<User>()))
            .Callback<User>(user => capturedUser = user);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        capturedUser.Should().NotBeNull();
        capturedUser!.Password.Should().NotBe(plainTextPassword, "password should be hashed");
        capturedUser.Password.Should().StartWith("$2a$12$", "should use BCrypt with work factor 12");
        
        // Verify the hash can be verified with the original password
        _passwordService.VerifyPassword(plainTextPassword, capturedUser.Password).Should().BeTrue();
        
        // Verify the hash cannot be verified with wrong password
        _passwordService.VerifyPassword("WrongPassword", capturedUser.Password).Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithEmptyPassword_ShouldNotHashEmptyString()
    {
        // Arrange
        var command = new InsertUserCommand
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "" // Empty password
        };

        var userEntity = new User
        {
            Name = command.Name,
            Email = new SoftwareDeveloperCase.Domain.ValueObjects.Email("test@example.com"),
            Password = ""
        };

        _mockMapper.Setup(m => m.Map<User>(command)).Returns(userEntity);

        // Setup role repository to return default role
        var roles = new List<Role>
        {
            new() { Id = Guid.NewGuid(), Name = "Employee" }
        };
        _mockRoleRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(roles);

        var handler = new InsertUserCommandHandler(
            _mockLogger.Object,
            _mockMapper.Object,
            _mockUnitOfWork.Object,
            _mockEmailService.Object,
            _passwordService);

        User? capturedUser = null;
        _mockUserRepository.Setup(r => r.Insert(It.IsAny<User>()))
            .Callback<User>(user => capturedUser = user);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        capturedUser.Should().NotBeNull();
        capturedUser!.Password.Should().Be("", "empty password should remain empty and not be hashed");
    }
}