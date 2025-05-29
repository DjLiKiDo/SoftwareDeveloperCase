using FluentAssertions;
using Moq;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Features.Role.Commands.AssignPermission;
using SoftwareDeveloperCase.Domain.Entities;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Validators;

public class AssignPermissionCommandValidatorTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRoleRepository> _mockRoleRepository;
    private readonly Mock<IPermissionRepository> _mockPermissionRepository;
    private readonly AssignPermissionCommandValidator _validator;

    public AssignPermissionCommandValidatorTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockRoleRepository = new Mock<IRoleRepository>();
        _mockPermissionRepository = new Mock<IPermissionRepository>();

        _mockUnitOfWork.Setup(x => x.RoleRepository).Returns(_mockRoleRepository.Object);
        _mockUnitOfWork.Setup(x => x.PermissionRepository).Returns(_mockPermissionRepository.Object);

        _validator = new AssignPermissionCommandValidator(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task AssignPermissionCommandValidator_ShouldBeValid_WhenBothRoleAndPermissionExist()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();
        var command = new AssignPermissionCommand
        {
            RoleId = roleId,
            PermissionId = permissionId
        };

        _mockRoleRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>()))
            .ReturnsAsync(new List<Role> { new Role { Id = roleId } });

        _mockPermissionRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Permission, bool>>>()))
            .ReturnsAsync(new List<Permission> { new Permission { Id = permissionId } });

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task AssignPermissionCommandValidator_ShouldHaveError_WhenRoleIdIsEmpty()
    {
        // Arrange
        var command = new AssignPermissionCommand
        {
            RoleId = Guid.Empty,
            PermissionId = Guid.NewGuid()
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "RoleId" && x.ErrorMessage.Contains("cannot be empty"));
    }

    [Fact]
    public async Task AssignPermissionCommandValidator_ShouldHaveError_WhenPermissionIdIsEmpty()
    {
        // Arrange
        var command = new AssignPermissionCommand
        {
            RoleId = Guid.NewGuid(),
            PermissionId = Guid.Empty
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "PermissionId" && x.ErrorMessage.Contains("cannot be empty"));
    }

    [Fact]
    public async Task AssignPermissionCommandValidator_ShouldHaveError_WhenRoleDoesNotExist()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();
        var command = new AssignPermissionCommand
        {
            RoleId = roleId,
            PermissionId = permissionId
        };

        _mockRoleRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>()))
            .ReturnsAsync(new List<Role>());

        _mockPermissionRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Permission, bool>>>()))
            .ReturnsAsync(new List<Permission> { new Permission { Id = permissionId } });

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "RoleId" && x.ErrorMessage.Contains("does not exist"));
    }

    [Fact]
    public async Task AssignPermissionCommandValidator_ShouldHaveError_WhenPermissionDoesNotExist()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();
        var command = new AssignPermissionCommand
        {
            RoleId = roleId,
            PermissionId = permissionId
        };

        _mockRoleRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>()))
            .ReturnsAsync(new List<Role> { new Role { Id = roleId } });

        _mockPermissionRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Permission, bool>>>()))
            .ReturnsAsync(new List<Permission>());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "PermissionId" && x.ErrorMessage.Contains("does not exist"));
    }

    [Fact]
    public async Task AssignPermissionCommandValidator_ShouldHaveMultipleErrors_WhenBothIdsAreEmpty()
    {
        // Arrange
        var command = new AssignPermissionCommand
        {
            RoleId = Guid.Empty,
            PermissionId = Guid.Empty
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
        result.Errors.Should().Contain(x => x.PropertyName == "RoleId");
        result.Errors.Should().Contain(x => x.PropertyName == "PermissionId");
    }

    [Fact]
    public async Task AssignPermissionCommandValidator_ShouldHaveMultipleErrors_WhenBothEntitiesDoNotExist()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();
        var command = new AssignPermissionCommand
        {
            RoleId = roleId,
            PermissionId = permissionId
        };

        _mockRoleRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>()))
            .ReturnsAsync(new List<Role>());

        _mockPermissionRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Permission, bool>>>()))
            .ReturnsAsync(new List<Permission>());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain(x => x.PropertyName == "RoleId" && x.ErrorMessage.Contains("does not exist"));
        result.Errors.Should().Contain(x => x.PropertyName == "PermissionId" && x.ErrorMessage.Contains("does not exist"));
    }
}
