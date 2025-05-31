using FluentAssertions;
using Moq;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.AssignRole;
using SoftwareDeveloperCase.Domain.Entities.Core;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using Task = System.Threading.Tasks.Task;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Validators;

public class AssignRoleCommandValidatorTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRoleRepository> _mockRoleRepository;
    private readonly AssignRoleCommandValidator _validator;

    public AssignRoleCommandValidatorTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockRoleRepository = new Mock<IRoleRepository>();

        _mockUnitOfWork.Setup(x => x.UserRepository).Returns(_mockUserRepository.Object);
        _mockUnitOfWork.Setup(x => x.RoleRepository).Returns(_mockRoleRepository.Object);

        _validator = new AssignRoleCommandValidator(_mockUnitOfWork.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task AssignRoleCommandValidator_ShouldBeValid_WhenBothUserAndRoleExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var command = new AssignRoleCommand
        {
            UserId = userId,
            RoleId = roleId
        };

        _mockUserRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { new User { Id = userId } });

        _mockRoleRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>()))
            .ReturnsAsync(new List<Role> { new Role { Id = roleId } });

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task AssignRoleCommandValidator_ShouldHaveError_WhenUserIdIsEmpty()
    {
        // Arrange
        var command = new AssignRoleCommand
        {
            UserId = Guid.Empty,
            RoleId = Guid.NewGuid()
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "UserId" && x.ErrorMessage.Contains("cannot be empty"));
    }

    [Fact]
    public async System.Threading.Tasks.Task AssignRoleCommandValidator_ShouldHaveError_WhenRoleIdIsEmpty()
    {
        // Arrange
        var command = new AssignRoleCommand
        {
            UserId = Guid.NewGuid(),
            RoleId = Guid.Empty
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "RoleId" && x.ErrorMessage.Contains("cannot be empty"));
    }

    [Fact]
    public async System.Threading.Tasks.Task AssignRoleCommandValidator_ShouldHaveError_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var command = new AssignRoleCommand
        {
            UserId = userId,
            RoleId = roleId
        };

        _mockUserRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User>());

        _mockRoleRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>()))
            .ReturnsAsync(new List<Role> { new Role { Id = roleId } });

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "UserId" && x.ErrorMessage.Contains("does not exist"));
    }

    [Fact]
    public async System.Threading.Tasks.Task AssignRoleCommandValidator_ShouldHaveError_WhenRoleDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var command = new AssignRoleCommand
        {
            UserId = userId,
            RoleId = roleId
        };

        _mockUserRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { new User { Id = userId } });

        _mockRoleRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>()))
            .ReturnsAsync(new List<Role>());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "RoleId" && x.ErrorMessage.Contains("does not exist"));
    }

    [Fact]
    public async System.Threading.Tasks.Task AssignRoleCommandValidator_ShouldHaveMultipleErrors_WhenBothIdsAreEmpty()
    {
        // Arrange
        var command = new AssignRoleCommand
        {
            UserId = Guid.Empty,
            RoleId = Guid.Empty
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
        result.Errors.Should().Contain(x => x.PropertyName == "UserId");
        result.Errors.Should().Contain(x => x.PropertyName == "RoleId");
    }
}
