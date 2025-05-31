using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Core;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Infrastructure.Persistence;
using SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Infrastructure;

public class UnitOfWorkTests
{
    [Fact]
    public void Constructor_InitializesRepositories()
    {
        // Arrange
        var dbContextMock = new Mock<SoftwareDeveloperCaseDbContext>();
        var loggerMock = new Mock<ILogger<UnitOfWork>>();
        var permissionRepoMock = new Mock<IPermissionRepository>();
        var rolePermissionRepoMock = new Mock<IRolePermissionRepository>();
        var roleRepoMock = new Mock<IRoleRepository>();
        var userRepoMock = new Mock<IUserRepository>();
        var userRoleRepoMock = new Mock<IUserRoleRepository>();
        var teamRepoMock = new Mock<ITeamRepository>();
        var teamMemberRepoMock = new Mock<ITeamMemberRepository>();
        var projectRepoMock = new Mock<IProjectRepository>();
        var taskRepoMock = new Mock<ITaskRepository>();
        var taskCommentRepoMock = new Mock<ITaskCommentRepository>();

        // Act
        var unitOfWork = new UnitOfWork(
            dbContextMock.Object,
            loggerMock.Object,
            permissionRepoMock.Object,
            rolePermissionRepoMock.Object,
            roleRepoMock.Object,
            userRepoMock.Object,
            userRoleRepoMock.Object,
            teamRepoMock.Object,
            teamMemberRepoMock.Object,
            projectRepoMock.Object,
            taskRepoMock.Object,
            taskCommentRepoMock.Object
        );

        // Assert
        Assert.NotNull(unitOfWork.PermissionRepository);
        Assert.NotNull(unitOfWork.RolePermissionRepository);
        Assert.NotNull(unitOfWork.RoleRepository);
        Assert.NotNull(unitOfWork.UserRepository);
        Assert.NotNull(unitOfWork.UserRoleRepository);
        Assert.NotNull(unitOfWork.TeamRepository);
        Assert.NotNull(unitOfWork.TeamMemberRepository);
        Assert.NotNull(unitOfWork.ProjectRepository);
        Assert.NotNull(unitOfWork.TaskRepository);
        Assert.NotNull(unitOfWork.TaskCommentRepository);
        
        Assert.Equal(permissionRepoMock.Object, unitOfWork.PermissionRepository);
        Assert.Equal(rolePermissionRepoMock.Object, unitOfWork.RolePermissionRepository);
        Assert.Equal(roleRepoMock.Object, unitOfWork.RoleRepository);
        Assert.Equal(userRepoMock.Object, unitOfWork.UserRepository);
        Assert.Equal(userRoleRepoMock.Object, unitOfWork.UserRoleRepository);
        Assert.Equal(teamRepoMock.Object, unitOfWork.TeamRepository);
        Assert.Equal(teamMemberRepoMock.Object, unitOfWork.TeamMemberRepository);
        Assert.Equal(projectRepoMock.Object, unitOfWork.ProjectRepository);
        Assert.Equal(taskRepoMock.Object, unitOfWork.TaskRepository);
        Assert.Equal(taskCommentRepoMock.Object, unitOfWork.TaskCommentRepository);
    }

    [Fact]
    public async Task SaveChangesAsync_DelegatesToDbContext()
    {
        // Arrange
        var dbContextMock = new Mock<SoftwareDeveloperCaseDbContext>();
        dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(5)
            .Verifiable();

        var unitOfWork = new UnitOfWork(
            dbContextMock.Object,
            NullLogger<UnitOfWork>.Instance,
            Mock.Of<IPermissionRepository>(),
            Mock.Of<IRolePermissionRepository>(),
            Mock.Of<IRoleRepository>(),
            Mock.Of<IUserRepository>(),
            Mock.Of<IUserRoleRepository>(),
            Mock.Of<ITeamRepository>(),
            Mock.Of<ITeamMemberRepository>(),
            Mock.Of<IProjectRepository>(),
            Mock.Of<ITaskRepository>(),
            Mock.Of<ITaskCommentRepository>()
        );

        // Act
        var result = await unitOfWork.SaveChangesAsync();

        // Assert
        Assert.Equal(5, result);
        dbContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
