using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Domain.Entities;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories;

internal class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(SoftwareDeveloperCaseDbContext context)
        : base(context)
    {

    }
}
