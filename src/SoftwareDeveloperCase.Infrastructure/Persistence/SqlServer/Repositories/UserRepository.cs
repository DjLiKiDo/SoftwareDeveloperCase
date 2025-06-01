using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories;

internal class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(SoftwareDeveloperCaseDbContext context)
        : base(context)
    {

    }
}
