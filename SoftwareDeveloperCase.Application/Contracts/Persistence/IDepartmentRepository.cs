using SoftwareDeveloperCase.Domain.Entities;

namespace SoftwareDeveloperCase.Application.Contracts.Persistence
{
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
}
