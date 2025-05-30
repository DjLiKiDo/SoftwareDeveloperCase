using FluentAssertions;
using Moq;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Entities;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Repositories;

public class DepartmentRepositoryTests
{
    private readonly Guid _managerRoleId = Guid.Parse("9ECA8D57-F7CA-4F8D-9C83-73B659225AE4");

    [Fact]
    public void IDepartmentRepository_ShouldInheritFromIRepository()
    {
        // Arrange & Act
        var departmentRepoType = typeof(IDepartmentRepository);
        var baseRepoType = typeof(IRepository<Department>);

        // Assert
        baseRepoType.IsAssignableFrom(departmentRepoType).Should().BeTrue();
    }

    [Fact]
    public void IDepartmentRepository_ShouldHaveGetManagersAsyncMethod()
    {
        // Arrange
        var departmentRepoType = typeof(IDepartmentRepository);

        // Act
        var getManagersMethod = departmentRepoType.GetMethod("GetManagersAsync");

        // Assert
        getManagersMethod.Should().NotBeNull();
        getManagersMethod!.ReturnType.Should().Be(typeof(Task<List<User>>));
        getManagersMethod.GetParameters().Should().HaveCount(1);
        getManagersMethod.GetParameters()[0].ParameterType.Should().Be(typeof(Guid));
    }

    [Fact]
    public async Task GetManagersAsync_MockImplementation_ShouldReturnExpectedResult()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var expectedManagers = new List<User>
        {
            new User { Id = Guid.NewGuid(), Name = "Manager1", DepartmentId = departmentId },
            new User { Id = Guid.NewGuid(), Name = "Manager2", DepartmentId = departmentId }
        };

        var mockRepository = new Mock<IDepartmentRepository>();
        mockRepository.Setup(x => x.GetManagersAsync(departmentId))
                     .ReturnsAsync(expectedManagers);

        // Act
        var result = await mockRepository.Object.GetManagersAsync(departmentId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().OnlyContain(u => u.DepartmentId == departmentId);
    }
}
