using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Infrastructure.Persistence;

namespace SoftwareDeveloperCase.Infrastructure.Repositories;

internal class RolePermissionRepository : Repository<RolePermission>, IRolePermissionRepository
{
    public RolePermissionRepository(SoftwareDeveloperCaseDbContext context)
        : base(context)
    {

    }
}
