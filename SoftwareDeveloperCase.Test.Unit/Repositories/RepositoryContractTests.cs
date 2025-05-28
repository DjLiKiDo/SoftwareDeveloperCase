using FluentAssertions;
using Moq;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Entities;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Repositories;

public class RepositoryContractTests
{
    [Fact]
    public void IUserRepository_ShouldInheritFromIRepository()
    {
        // Arrange & Act
        var userRepoType = typeof(IUserRepository);
        var baseRepoType = typeof(IRepository<User>);

        // Assert
        baseRepoType.IsAssignableFrom(userRepoType).Should().BeTrue();
    }

    [Fact]
    public void IRoleRepository_ShouldInheritFromIRepository()
    {
        // Arrange & Act
        var roleRepoType = typeof(IRoleRepository);
        var baseRepoType = typeof(IRepository<Role>);

        // Assert
        baseRepoType.IsAssignableFrom(roleRepoType).Should().BeTrue();
    }

    [Fact]
    public void IPermissionRepository_ShouldInheritFromIRepository()
    {
        // Arrange & Act
        var permissionRepoType = typeof(IPermissionRepository);
        var baseRepoType = typeof(IRepository<Permission>);

        // Assert
        baseRepoType.IsAssignableFrom(permissionRepoType).Should().BeTrue();
    }

    [Fact]
    public void IRolePermissionRepository_ShouldInheritFromIRepository()
    {
        // Arrange & Act
        var rolePermissionRepoType = typeof(IRolePermissionRepository);
        var baseRepoType = typeof(IRepository<RolePermission>);

        // Assert
        baseRepoType.IsAssignableFrom(rolePermissionRepoType).Should().BeTrue();
    }

    [Fact]
    public void IUserRoleRepository_ShouldInheritFromIRepository()
    {
        // Arrange & Act
        var userRoleRepoType = typeof(IUserRoleRepository);
        var baseRepoType = typeof(IRepository<UserRole>);

        // Assert
        baseRepoType.IsAssignableFrom(userRoleRepoType).Should().BeTrue();
    }

    [Fact]
    public async Task IRepository_ShouldHaveAllRequiredMethods()
    {
        // Arrange
        var mock = new Mock<IRepository<User>>();
        var testUser = new User { Id = Guid.NewGuid(), Name = "Test" };
        
        mock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(testUser);
        mock.Setup(x => x.GetAllAsync()).ReturnsAsync(new[] { testUser });
        mock.Setup(x => x.InsertAsync(It.IsAny<User>())).ReturnsAsync(testUser);

        var repository = mock.Object;

        // Act & Assert - Verify all required methods exist
        var getByIdResult = await repository.GetByIdAsync(Guid.NewGuid());
        getByIdResult.Should().NotBeNull();

        var getAllResult = await repository.GetAllAsync();
        getAllResult.Should().NotBeNull();

        var insertResult = await repository.InsertAsync(testUser);
        insertResult.Should().NotBeNull();

        // Verify synchronous methods exist
        repository.Insert(testUser);
        repository.Update(testUser);
        repository.Delete(testUser);

        // Verify async CUD methods exist
        await repository.UpdateAsync(testUser);
        await repository.DeleteAsync(testUser);
    }

    [Fact]
    public void IUnitOfWork_ShouldExposeAllRepositories()
    {
        // Arrange
        var mock = new Mock<IUnitOfWork>();
        var unitOfWork = mock.Object;

        // Act & Assert - Verify all repository properties exist
        var userRepoProperty = typeof(IUnitOfWork).GetProperty(nameof(IUnitOfWork.UserRepository));
        var roleRepoProperty = typeof(IUnitOfWork).GetProperty(nameof(IUnitOfWork.RoleRepository));
        var permissionRepoProperty = typeof(IUnitOfWork).GetProperty(nameof(IUnitOfWork.PermissionRepository));
        var rolePermissionRepoProperty = typeof(IUnitOfWork).GetProperty(nameof(IUnitOfWork.RolePermissionRepository));
        var userRoleRepoProperty = typeof(IUnitOfWork).GetProperty(nameof(IUnitOfWork.UserRoleRepository));
        var departmentRepoProperty = typeof(IUnitOfWork).GetProperty(nameof(IUnitOfWork.DepartmentRepository));

        userRepoProperty.Should().NotBeNull();
        roleRepoProperty.Should().NotBeNull();
        permissionRepoProperty.Should().NotBeNull();
        rolePermissionRepoProperty.Should().NotBeNull();
        userRoleRepoProperty.Should().NotBeNull();
        departmentRepoProperty.Should().NotBeNull();

        userRepoProperty!.PropertyType.Should().Be<IUserRepository>();
        roleRepoProperty!.PropertyType.Should().Be<IRoleRepository>();
        permissionRepoProperty!.PropertyType.Should().Be<IPermissionRepository>();
        rolePermissionRepoProperty!.PropertyType.Should().Be<IRolePermissionRepository>();
        userRoleRepoProperty!.PropertyType.Should().Be<IUserRoleRepository>();
        departmentRepoProperty!.PropertyType.Should().Be<IDepartmentRepository>();
    }
}