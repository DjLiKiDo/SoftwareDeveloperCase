using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Entities.Core;
using SoftwareDeveloperCase.Domain.Entities.Lookups;

namespace SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;

/// <summary>
/// Repository interface for Department entity operations
/// </summary>
public interface IDepartmentRepository : IRepository<Department>
{
    /// <summary>
    /// Gets all managers for a specific department
    /// </summary>
    /// <param name="departmentId">The department identifier</param>
    /// <returns>A list of users who are managers in the department</returns>
    Task<List<User>> GetManagersAsync(Guid departmentId);
}
