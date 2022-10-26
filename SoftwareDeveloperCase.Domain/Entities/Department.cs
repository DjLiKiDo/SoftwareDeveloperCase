using SoftwareDeveloperCase.Domain.Common;

namespace SoftwareDeveloperCase.Domain.Entities
{
    public class Department : BaseEntity
    {
        public string? Name { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
