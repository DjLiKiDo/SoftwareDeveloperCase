using SoftwareDeveloperCase.Domain.Common;

namespace SoftwareDeveloperCase.Domain.Entities
{
    public class User : BaseEntity
    {
        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public Guid DepartmentId { get; set; }

        public virtual Department? Department { get; set; }

        public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    }
}
