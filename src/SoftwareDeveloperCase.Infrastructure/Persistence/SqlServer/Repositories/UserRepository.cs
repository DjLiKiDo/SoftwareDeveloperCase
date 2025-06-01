using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Domain.Entities;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories;

internal class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(SoftwareDeveloperCaseDbContext context)
        : base(context)
    {

    }
}
