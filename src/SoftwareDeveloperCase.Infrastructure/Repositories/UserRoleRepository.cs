using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using SoftwareDeveloperCase.Infrastructure.Persistence;

namespace SoftwareDeveloperCase.Infrastructure.Repositories;

internal class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(SoftwareDeveloperCaseDbContext context)
        : base(context)
    {

    }
}
