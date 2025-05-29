using Microsoft.EntityFrameworkCore;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Infrastructure.Persistence;

namespace SoftwareDeveloperCase.Infrastructure.Repositories;

internal class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public DepartmentRepository(SoftwareDeveloperCaseDbContext context)
        : base(context)
    {

    }

    private static readonly Guid ManagerRoleId = Guid.Parse("9ECA8D57-F7CA-4F8D-9C83-73B659225AE4");

    public async Task<List<User>> GetManagersAsync(Guid departmentId)
    {
        var departmentManagers = await _context.Users!
            .Where(u => u.DepartmentId == departmentId)
            .Where(u => u.UserRoles.Any(ur => ur.RoleId == ManagerRoleId))
            .ToListAsync();

        return departmentManagers;
    }
}
