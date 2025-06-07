using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using SoftwareDeveloperCase.Domain.ValueObjects;
using SoftwareDeveloperCase.Test.Integration.Common;
using Xunit;

namespace SoftwareDeveloperCase.Test.Integration.Persistence;

/// <summary>
/// Integration tests for Unit of Work transaction management functionality
/// </summary>
public class UnitOfWorkTransactionTests : DatabaseIntegrationTestBase
{
    public UnitOfWorkTransactionTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task BeginTransactionAsync_ShouldCreateNewTransaction()
    {
        // Arrange & Act
        await UnitOfWork.BeginTransactionAsync();

        // Assert
        UnitOfWork.HasActiveTransaction.Should().BeTrue();
        UnitOfWork.CurrentTransactionId.Should().NotBeNull();
    }

    [Fact]
    public async Task CommitTransactionAsync_WithValidChanges_ShouldPersistData()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = GenerateUniqueName("Test User"),
            Email = new Email(GenerateUniqueEmail()),
            Password = "password123",
            IsActive = true
        };

        // Act
        await UnitOfWork.BeginTransactionAsync();
        UnitOfWork.UserRepository.Insert(user);
        var result = await UnitOfWork.CommitTransactionAsync();

        // Assert
        result.Should().BeGreaterThan(0);
        UnitOfWork.HasActiveTransaction.Should().BeFalse();

        // Verify data persisted
        var persistedUser = await Context.Users!.FirstOrDefaultAsync(u => u.Id == user.Id);
        persistedUser.Should().NotBeNull();
        persistedUser!.Name.Should().Be(user.Name);
    }

    [Fact]
    public async Task RollbackTransactionAsync_ShouldDiscardChanges()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = GenerateUniqueName("Test User"),
            Email = new Email(GenerateUniqueEmail()),
            Password = "password123",
            IsActive = true
        };

        // Act
        await UnitOfWork.BeginTransactionAsync();
        UnitOfWork.UserRepository.Insert(user);
        await UnitOfWork.RollbackTransactionAsync();

        // Assert
        UnitOfWork.HasActiveTransaction.Should().BeFalse();

        // Verify data was not persisted
        var persistedUser = await Context.Users!.FirstOrDefaultAsync(u => u.Id == user.Id);
        persistedUser.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WithSuccess_ShouldCommitChanges()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = GenerateUniqueName("Test User"),
            Email = new Email(GenerateUniqueEmail()),
            Password = "password123",
            IsActive = true
        };

        // Act
        var result = await UnitOfWork.ExecuteInTransactionAsync(() =>
        {
            UnitOfWork.UserRepository.Insert(user);
            return Task.FromResult(user.Id);
        });

        // Assert
        result.Should().Be(user.Id);
        UnitOfWork.HasActiveTransaction.Should().BeFalse();

        // Verify data persisted
        var persistedUser = await Context.Users!.FirstOrDefaultAsync(u => u.Id == user.Id);
        persistedUser.Should().NotBeNull();
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WithException_ShouldRollbackChanges()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = GenerateUniqueName("Test User"),
            Email = new Email(GenerateUniqueEmail()),
            Password = "password123",
            IsActive = true
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await UnitOfWork.ExecuteInTransactionAsync(() =>
            {
                UnitOfWork.UserRepository.Insert(user);
                throw new InvalidOperationException("Test exception");
            });
        });

        UnitOfWork.HasActiveTransaction.Should().BeFalse();

        // Verify data was not persisted
        var persistedUser = await Context.Users!.FirstOrDefaultAsync(u => u.Id == user.Id);
        persistedUser.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WithAction_ShouldWork()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = GenerateUniqueName("Test User"),
            Email = new Email(GenerateUniqueEmail()),
            Password = "password123",
            IsActive = true
        };

        // Act
        await UnitOfWork.ExecuteInTransactionAsync(() =>
        {
            UnitOfWork.UserRepository.Insert(user);
            return Task.CompletedTask;
        });

        // Assert
        UnitOfWork.HasActiveTransaction.Should().BeFalse();

        // Verify data persisted
        var persistedUser = await Context.Users!.FirstOrDefaultAsync(u => u.Id == user.Id);
        persistedUser.Should().NotBeNull();
    }

    [Fact]
    public async Task BeginTransactionAsync_WhenTransactionAlreadyActive_ShouldThrowException()
    {
        // Arrange
        await UnitOfWork.BeginTransactionAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await UnitOfWork.BeginTransactionAsync();
        });

        // Cleanup
        await UnitOfWork.RollbackTransactionAsync();
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WithNestedCalls_ShouldReuseExistingTransaction()
    {
        // Arrange
        var user1 = new User
        {
            Id = Guid.NewGuid(),
            Name = GenerateUniqueName("User 1"),
            Email = new Email(GenerateUniqueEmail()),
            Password = "password123",
            IsActive = true
        };

        var user2 = new User
        {
            Id = Guid.NewGuid(),
            Name = GenerateUniqueName("User 2"),
            Email = new Email(GenerateUniqueEmail()),
            Password = "password123",
            IsActive = true
        };

        // Act - First ExecuteInTransactionAsync starts a transaction
        await UnitOfWork.ExecuteInTransactionAsync(() =>
        {
            UnitOfWork.UserRepository.Insert(user1);

            // Nested call should reuse existing transaction (not start a new one)
            return UnitOfWork.ExecuteInTransactionAsync(() =>
            {
                UnitOfWork.UserRepository.Insert(user2);
                return Task.CompletedTask;
            });
        });

        // Assert
        UnitOfWork.HasActiveTransaction.Should().BeFalse();

        var persistedUser1 = await Context.Users!.FirstOrDefaultAsync(u => u.Id == user1.Id);
        var persistedUser2 = await Context.Users!.FirstOrDefaultAsync(u => u.Id == user2.Id);

        persistedUser1.Should().NotBeNull();
        persistedUser2.Should().NotBeNull();
    }

    [Fact]
    public async Task CommitTransactionAsync_WithoutActiveTransaction_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await UnitOfWork.CommitTransactionAsync();
        });
    }

    [Fact]
    public async Task RollbackTransactionAsync_WithoutActiveTransaction_ShouldNotThrow()
    {
        // Act & Assert - Should not throw
        await UnitOfWork.RollbackTransactionAsync();

        UnitOfWork.HasActiveTransaction.Should().BeFalse();
    }

    [Fact]
    public async Task MultipleOperations_InSingleTransaction_ShouldAllCommitOrRollback()
    {
        // Arrange
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = GenerateUniqueName("Test Role")
        };

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = GenerateUniqueName("Test Permission"),
            Description = "Test Permission Description"
        };

        // Act
        await UnitOfWork.ExecuteInTransactionAsync(() =>
        {
            UnitOfWork.RoleRepository.Insert(role);
            UnitOfWork.PermissionRepository.Insert(permission);
            return Task.CompletedTask;
        });

        // Assert
        var persistedRole = await Context.Set<Role>().FirstOrDefaultAsync(r => r.Id == role.Id);
        var persistedPermission = await Context.Set<Permission>().FirstOrDefaultAsync(p => p.Id == permission.Id);

        persistedRole.Should().NotBeNull();
        persistedPermission.Should().NotBeNull();
    }
}
