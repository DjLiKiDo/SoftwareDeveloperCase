using SoftwareDeveloperCase.Domain.Common;

namespace SoftwareDeveloperCase.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string? Name { get; set; }

        public Guid? ParentRoleId { get; set; }

        public virtual Role? ParentRole { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
