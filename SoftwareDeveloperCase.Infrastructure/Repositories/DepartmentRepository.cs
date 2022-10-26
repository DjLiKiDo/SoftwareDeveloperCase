using Microsoft.EntityFrameworkCore;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Infrastructure.Persistence;

namespace SoftwareDeveloperCase.Infrastructure.Repositories
{
    internal class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(SoftwareDeveloperCaseDbContext context)
            : base(context)
        {

        }

        public async Task<List<User>> GetManagersAsync(Guid departmentId)
        {
            var managerIdList = await _context.UserRoles!
                .Where(ur => ur.RoleId.ToString().Equals("9ECA8D57-F7CA-4F8D-9C83-73B659225AE4"))
                .Select(ur => ur.UserId)
                .ToListAsync();

            var deparmentManagerList = await _context.Users!
                .Where(u => u.DepartmentId.Equals(departmentId))
                .Where(u => managerIdList.Contains(u.Id))
                .ToListAsync();

            return deparmentManagerList;
        }
    }
}
