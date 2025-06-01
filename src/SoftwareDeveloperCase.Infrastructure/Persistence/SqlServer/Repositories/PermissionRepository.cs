using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Domain.Entities.Identity;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories;

internal class PermissionRepository : Repository<Permission>, IPermissionRepository
{
    public PermissionRepository(SoftwareDeveloperCaseDbContext context)
        : base(context)
    {
    }
}
