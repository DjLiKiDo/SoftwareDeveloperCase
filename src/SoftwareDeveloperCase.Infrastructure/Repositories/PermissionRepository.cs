using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using SoftwareDeveloperCase.Infrastructure.Persistence;

namespace SoftwareDeveloperCase.Infrastructure.Repositories;

internal class PermissionRepository : Repository<Permission>, IPermissionRepository
{
    public PermissionRepository(SoftwareDeveloperCaseDbContext context)
        : base(context)
    {
    }
}
