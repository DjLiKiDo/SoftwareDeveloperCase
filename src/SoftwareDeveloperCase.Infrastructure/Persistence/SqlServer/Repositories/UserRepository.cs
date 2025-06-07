using Microsoft.EntityFrameworkCore;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Domain.ValueObjects;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories;

internal class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(SoftwareDeveloperCaseDbContext context)
        : base(context)
    {

    }

    /// <summary>
    /// Gets a user by email address with roles included
    /// </summary>
    public async Task<User?> GetByEmailWithRolesAsync(string email, CancellationToken cancellationToken = default)
    {
        // Create Email value object for comparison
        var emailValueObject = new Email(email);

        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == emailValueObject, cancellationToken);
    }
}
