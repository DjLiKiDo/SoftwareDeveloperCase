using SoftwareDeveloperCase.Domain.Entities;

namespace SoftwareDeveloperCase.Application.Contracts.Persistence
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<List<User>> GetManagersAsync(Guid departmentId);
    }
}
