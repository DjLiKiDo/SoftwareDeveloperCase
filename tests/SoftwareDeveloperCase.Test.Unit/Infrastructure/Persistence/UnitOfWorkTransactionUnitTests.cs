using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Logging;
using Moq;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Core;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer;
using SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Infrastructure.Persistence;

/// <summary>
/// Unit tests for UnitOfWork transaction management functionality
/// </summary>
public class UnitOfWorkTransactionUnitTests : IDisposable
{
    private readonly SoftwareDeveloperCaseDbContext _context;
    private readonly UnitOfWork _unitOfWork;
    private readonly Mock<ILogger<UnitOfWork>> _mockLogger;

    public UnitOfWorkTransactionUnitTests()
    {
        var options = new DbContextOptionsBuilder<SoftwareDeveloperCaseDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings =>
            {
                warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning);
            })
            .Options;

        _context = new SoftwareDeveloperCaseDbContext(options);
        _mockLogger = new Mock<ILogger<UnitOfWork>>();

        // Create mock repositories
        var mockPermissionRepo = new Mock<IPermissionRepository>();
        var mockRolePermissionRepo = new Mock<IRolePermissionRepository>();
        var mockRoleRepo = new Mock<IRoleRepository>();
        var mockUserRepo = new Mock<IUserRepository>();
        var mockUserRoleRepo = new Mock<IUserRoleRepository>();
        var mockTeamRepo = new Mock<ITeamRepository>();
        var mockTeamMemberRepo = new Mock<ITeamMemberRepository>();
        var mockProjectRepo = new Mock<IProjectRepository>();
        var mockTaskRepo = new Mock<ITaskRepository>();
        var mockTaskCommentRepo = new Mock<ITaskCommentRepository>();

        _unitOfWork = new UnitOfWork(
            _context,
            _mockLogger.Object,
            mockPermissionRepo.Object,
            mockRolePermissionRepo.Object,
            mockRoleRepo.Object,
            mockUserRepo.Object,
            mockUserRoleRepo.Object,
            mockTeamRepo.Object,
            mockTeamMemberRepo.Object,
            mockProjectRepo.Object,
            mockTaskRepo.Object,
            mockTaskCommentRepo.Object);
    }

    [Fact]
    public void HasActiveTransaction_InitialState_ShouldBeFalse()
    {
        // Act & Assert
        _unitOfWork.HasActiveTransaction.Should().BeFalse();
        _unitOfWork.CurrentTransactionId.Should().BeNull();
    }

    [Fact]
    public async Task BeginTransactionAsync_ShouldSetActiveTransaction()
    {
        // Act
        await _unitOfWork.BeginTransactionAsync();

        // Assert
        _unitOfWork.HasActiveTransaction.Should().BeTrue();
        _unitOfWork.CurrentTransactionId.Should().NotBeNull();
    }

    [Fact]
    public async Task BeginTransactionAsync_WithActiveTransaction_ShouldThrowException()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();
        var firstTransactionId = _unitOfWork.CurrentTransactionId;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _unitOfWork.BeginTransactionAsync());

        exception.Message.Should().Be("A transaction is already active. Only one transaction is supported at a time.");
        _unitOfWork.HasActiveTransaction.Should().BeTrue();
        _unitOfWork.CurrentTransactionId.Should().Be(firstTransactionId);
    }

    [Fact]
    public async Task CommitTransactionAsync_WithoutActiveTransaction_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _unitOfWork.CommitTransactionAsync();
        });
    }

    [Fact]
    public async Task RollbackTransactionAsync_WithoutActiveTransaction_ShouldNotThrow()
    {
        // Act & Assert - Should not throw
        await _unitOfWork.RollbackTransactionAsync();

        _unitOfWork.HasActiveTransaction.Should().BeFalse();

        // Verify warning was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Attempted to rollback transaction but no active transaction found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WithFunction_ShouldReturnResult()
    {
        // Arrange
        var expectedResult = "test result";

        // Act
        var result = await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await Task.Delay(1); // Simulate async work
            return expectedResult;
        });

        // Assert
        result.Should().Be(expectedResult);
        _unitOfWork.HasActiveTransaction.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WithAction_ShouldComplete()
    {
        // Arrange
        var operationExecuted = false;

        // Act
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await Task.Delay(1); // Simulate async work
            operationExecuted = true;
        });

        // Assert
        operationExecuted.Should().BeTrue();
        _unitOfWork.HasActiveTransaction.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WithException_ShouldRollback()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await Task.Delay(1);
                throw new InvalidOperationException("Test exception");
            });
        });

        _unitOfWork.HasActiveTransaction.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WithNestedCall_ShouldReuseTransaction()
    {
        // Arrange
        Guid? outerTransactionId = null;
        Guid? innerTransactionId = null;

        // Act
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            outerTransactionId = _unitOfWork.CurrentTransactionId;

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                innerTransactionId = _unitOfWork.CurrentTransactionId;
                await Task.Delay(1);
            });
        });

        // Assert
        outerTransactionId.Should().NotBeNull();
        innerTransactionId.Should().Be(outerTransactionId);
        _unitOfWork.HasActiveTransaction.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WithNullOperation_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _unitOfWork.ExecuteInTransactionAsync((Func<Task<string>>)null!);
        });

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _unitOfWork.ExecuteInTransactionAsync((Func<Task>)null!);
        });
    }

    [Fact]
    public async Task Dispose_WithActiveTransaction_ShouldRollbackAndLog()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();

        // Act
        _unitOfWork.Dispose();

        // Assert - Verify warning was logged about disposing with active transaction
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Disposing UnitOfWork with active transaction")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TransactionOperations_ShouldLogAppropriately(bool withException)
    {
        // Act
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            if (withException)
            {
                throw new InvalidOperationException("Test exception");
            }

            await _unitOfWork.CommitTransactionAsync();
        }
        catch (InvalidOperationException)
        {
            await _unitOfWork.RollbackTransactionAsync();
        }

        // Assert - Verify appropriate logging occurred
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Beginning new database transaction")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        if (withException)
        {
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Rolling back transaction")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
        else
        {
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Committing transaction")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }

    public void Dispose()
    {
        _unitOfWork.Dispose();
        _context.Dispose();
    }
}
