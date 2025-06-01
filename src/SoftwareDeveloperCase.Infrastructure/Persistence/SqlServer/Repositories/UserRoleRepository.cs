using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Domain.Entities.Identity;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories;

internal class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(SoftwareDeveloperCaseDbContext context)
        : base(context)
    {

    }
}
