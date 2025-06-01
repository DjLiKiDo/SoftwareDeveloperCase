using SoftwareDeveloperCase.Domain.Entities;

namespace SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;

/// <summary>
/// Repository interface for User entity operations
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Gets a user by email address with roles included
    /// </summary>
    /// <param name="email">The email address</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetByEmailWithRolesAsync(string email, CancellationToken cancellationToken = default);
}
