using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Infrastructure.Persistence;

namespace SoftwareDeveloperCase.Infrastructure.Repositories
{
    internal class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(SoftwareDeveloperCaseDbContext context)
            : base(context)
        {

        }
    }
}
