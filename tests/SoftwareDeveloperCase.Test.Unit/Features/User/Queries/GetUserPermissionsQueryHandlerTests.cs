using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Features.Identity.Users.Queries.GetUserPermissions;
using SoftwareDeveloperCase.Domain.Entities.Core;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using SoftwareDeveloperCase.Domain.ValueObjects;
using Task = System.Threading.Tasks.Task;
using Xunit;
using UserEntity = SoftwareDeveloperCase.Domain.Entities.Core.User;

namespace SoftwareDeveloperCase.Test.Unit.Features.User.Queries;

public class GetUserPermissionsQueryHandlerTests
{
    private readonly Mock<ILogger<GetUserPermissionsQueryHandler>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUserRoleRepository> _mockUserRoleRepository;
    private readonly Mock<IRolePermissionRepository> _mockRolePermissionRepository;
    private readonly Mock<IPermissionRepository> _mockPermissionRepository;
    private readonly GetUserPermissionsQueryHandler _handler;

    public GetUserPermissionsQueryHandlerTests()
    {
        _mockLogger = new Mock<ILogger<GetUserPermissionsQueryHandler>>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockUserRoleRepository = new Mock<IUserRoleRepository>();
        _mockRolePermissionRepository = new Mock<IRolePermissionRepository>();
        _mockPermissionRepository = new Mock<IPermissionRepository>();

        _mockUnitOfWork.Setup(x => x.UserRepository).Returns(_mockUserRepository.Object);
        _mockUnitOfWork.Setup(x => x.UserRoleRepository).Returns(_mockUserRoleRepository.Object);
        _mockUnitOfWork.Setup(x => x.RolePermissionRepository).Returns(_mockRolePermissionRepository.Object);
        _mockUnitOfWork.Setup(x => x.PermissionRepository).Returns(_mockPermissionRepository.Object);

        _handler = new GetUserPermissionsQueryHandler(_mockLogger.Object, _mockMapper.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldReturnUserPermissions_WhenUserHasRolesAndPermissions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId1 = Guid.NewGuid();
        var roleId2 = Guid.NewGuid();
        var permissionId1 = Guid.NewGuid();
        var permissionId2 = Guid.NewGuid();
        var permissionId3 = Guid.NewGuid();

        var query = new GetUserPermissionsQuery(userId);

        var user = new UserEntity { Id = userId };

        var userRoles = new List<UserRole>
        {
            new() { UserId = userId, RoleId = roleId1 },
            new() { UserId = userId, RoleId = roleId2 }
        };

        var rolePermissions = new List<RolePermission>
        {
            new() { RoleId = roleId1, PermissionId = permissionId1 },
            new() { RoleId = roleId1, PermissionId = permissionId2 },
            new() { RoleId = roleId2, PermissionId = permissionId2 }, // Duplicate permission
            new() { RoleId = roleId2, PermissionId = permissionId3 }
        };

        var permissions = new List<Permission>
        {
            new() { Id = permissionId1, Name = "Read" },
            new() { Id = permissionId2, Name = "Write" },
            new() { Id = permissionId3, Name = "Delete" }
        };

        var permissionDtos = new List<PermissionDto>
        {
            new() { Id = permissionId1, Name = "Read" },
            new() { Id = permissionId2, Name = "Write" },
            new() { Id = permissionId3, Name = "Delete" }
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserRoleRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserRole, bool>>>()))
            .ReturnsAsync(userRoles);
        _mockRolePermissionRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RolePermission, bool>>>()))
            .ReturnsAsync(rolePermissions);
        _mockPermissionRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Permission, bool>>>()))
            .ReturnsAsync(permissions);
        _mockMapper.Setup(x => x.Map<List<PermissionDto>>(permissions))
            .Returns(permissionDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(p => p.Name == "Read");
        result.Should().Contain(p => p.Name == "Write");
        result.Should().Contain(p => p.Name == "Delete");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldReturnEmptyList_WhenUserHasNoRoles()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserPermissionsQuery(userId);

        var user = new UserEntity { Id = userId };
        var emptyUserRoles = new List<UserRole>();
        var emptyPermissions = new List<Permission>();
        var emptyPermissionDtos = new List<PermissionDto>();

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserRoleRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserRole, bool>>>()))
            .ReturnsAsync(emptyUserRoles);
        _mockRolePermissionRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RolePermission, bool>>>()))
            .ReturnsAsync(new List<RolePermission>());
        _mockPermissionRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Permission, bool>>>()))
            .ReturnsAsync(emptyPermissions);
        _mockMapper.Setup(x => x.Map<List<PermissionDto>>(emptyPermissions))
            .Returns(emptyPermissionDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldReturnEmptyList_WhenUserRolesHaveNoPermissions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var query = new GetUserPermissionsQuery(userId);

        var user = new UserEntity { Id = userId };
        var userRoles = new List<UserRole>
        {
            new() { UserId = userId, RoleId = roleId }
        };
        var emptyRolePermissions = new List<RolePermission>();
        var emptyPermissions = new List<Permission>();
        var emptyPermissionDtos = new List<PermissionDto>();

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserRoleRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserRole, bool>>>()))
            .ReturnsAsync(userRoles);
        _mockRolePermissionRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RolePermission, bool>>>()))
            .ReturnsAsync(emptyRolePermissions);
        _mockPermissionRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Permission, bool>>>()))
            .ReturnsAsync(emptyPermissions);
        _mockMapper.Setup(x => x.Map<List<PermissionDto>>(emptyPermissions))
            .Returns(emptyPermissionDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldCallRepositoriesInCorrectOrder()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserPermissionsQuery(userId);

        var user = new UserEntity { Id = userId };
        var userRoles = new List<UserRole>();
        var rolePermissions = new List<RolePermission>();
        var permissions = new List<Permission>();
        var permissionDtos = new List<PermissionDto>();

        var sequence = new MockSequence();

        _mockUserRepository.InSequence(sequence).Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserRoleRepository.InSequence(sequence).Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserRole, bool>>>()))
            .ReturnsAsync(userRoles);
        _mockRolePermissionRepository.InSequence(sequence).Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RolePermission, bool>>>()))
            .ReturnsAsync(rolePermissions);
        _mockPermissionRepository.InSequence(sequence).Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Permission, bool>>>()))
            .ReturnsAsync(permissions);
        _mockMapper.Setup(x => x.Map<List<PermissionDto>>(permissions))
            .Returns(permissionDtos);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _mockUserRoleRepository.Verify(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserRole, bool>>>()), Times.Once);
        _mockRolePermissionRepository.Verify(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RolePermission, bool>>>()), Times.Once);
        _mockPermissionRepository.Verify(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Permission, bool>>>()), Times.Once);
        _mockMapper.Verify(x => x.Map<List<PermissionDto>>(permissions), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldDeduplicatePermissions_WhenMultipleRolesHaveSamePermission()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId1 = Guid.NewGuid();
        var roleId2 = Guid.NewGuid();
        var permissionId = Guid.NewGuid();
        var query = new GetUserPermissionsQuery(userId);

        var user = new UserEntity { Id = userId };
        var userRoles = new List<UserRole>
        {
            new() { UserId = userId, RoleId = roleId1 },
            new() { UserId = userId, RoleId = roleId2 }
        };

        var rolePermissions = new List<RolePermission>
        {
            new() { RoleId = roleId1, PermissionId = permissionId },
            new() { RoleId = roleId2, PermissionId = permissionId } // Same permission from different roles
        };

        var permissions = new List<Permission>
        {
            new() { Id = permissionId, Name = "Read" }
        };

        var permissionDtos = new List<PermissionDto>
        {
            new() { Id = permissionId, Name = "Read" }
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserRoleRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserRole, bool>>>()))
            .ReturnsAsync(userRoles);
        _mockRolePermissionRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RolePermission, bool>>>()))
            .ReturnsAsync(rolePermissions);
        _mockPermissionRepository.Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Permission, bool>>>()))
            .ReturnsAsync(permissions);
        _mockMapper.Setup(x => x.Map<List<PermissionDto>>(permissions))
            .Returns(permissionDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Read");
    }
}
