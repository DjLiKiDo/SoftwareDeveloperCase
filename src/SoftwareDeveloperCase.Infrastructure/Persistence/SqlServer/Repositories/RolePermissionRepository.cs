using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Domain.Entities.Identity;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories;

internal class RolePermissionRepository : Repository<RolePermission>, IRolePermissionRepository
{
    public RolePermissionRepository(SoftwareDeveloperCaseDbContext context)
        : base(context)
    {

    }
}
