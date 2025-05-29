using SoftwareDeveloperCase.Domain.Common;

namespace SoftwareDeveloperCase.Domain.Entities
{
    /// <summary>
    /// Represents the many-to-many relationship between roles and permissions.
    /// </summary>
    public class RolePermission : BaseEntity
    {
        /// <summary>
        /// Gets or sets the identifier of the role.
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// Gets or sets the role associated with this permission.
        /// </summary>
        public virtual Role? Role { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the permission.
        /// </summary>
        public Guid PermissionId { get; set; }

        /// <summary>
        /// Gets or sets the permission associated with this role.
        /// </summary>
        public virtual Permission? Permission { get; set; }
    }
}
