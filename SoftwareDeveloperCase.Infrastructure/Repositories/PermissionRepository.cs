using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Infrastructure.Persistence;

namespace SoftwareDeveloperCase.Infrastructure.Repositories
{
    internal class PermissionRepository : Repository<Permission>, IPermissionRepository
    {
        public PermissionRepository(SoftwareDeveloperCaseDbContext context)
            : base(context)
        {
        }
    }
}
