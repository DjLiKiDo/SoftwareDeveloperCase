using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories;

internal class PermissionRepository : Repository<Permission>, IPermissionRepository
{
    public PermissionRepository(SoftwareDeveloperCaseDbContext context)
        : base(context)
    {
    }
}
