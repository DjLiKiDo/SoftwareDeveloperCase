using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using SoftwareDeveloperCase.Infrastructure.Persistence;

namespace SoftwareDeveloperCase.Infrastructure.Repositories;

internal class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(SoftwareDeveloperCaseDbContext context)
        : base(context)
    {

    }
}
