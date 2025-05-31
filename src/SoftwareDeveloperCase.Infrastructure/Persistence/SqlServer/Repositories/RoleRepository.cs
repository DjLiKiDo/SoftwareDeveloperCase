using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories;

internal class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(SoftwareDeveloperCaseDbContext context)
        : base(context)
    {

    }
}
