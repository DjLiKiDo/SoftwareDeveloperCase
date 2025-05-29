using Microsoft.EntityFrameworkCore;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Infrastructure.Persistence.Extensions;

namespace SoftwareDeveloperCase.Infrastructure.Persistence
{
    public class SoftwareDeveloperCaseDbContext : DbContext
    {
        private readonly IDateTimeService _dateTimeService;
        private readonly EntitySaveChangesInterceptor _entitySaveChangesInterceptor;

        public DbSet<User>? Users { get; set; }

        public DbSet<Role>? Roles { get; set; }

        public DbSet<Permission>? Permissions { get; set; }

        public DbSet<Department>? Departments { get; set; }

        public DbSet<UserRole>? UserRoles { get; set; }

        public SoftwareDeveloperCaseDbContext(DbContextOptions<SoftwareDeveloperCaseDbContext> options, EntitySaveChangesInterceptor entitySaveChangesInterceptor, IDateTimeService dateTimeService)
            : base(options)
        {
            _dateTimeService = dateTimeService;
            _entitySaveChangesInterceptor = entitySaveChangesInterceptor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSingularTableNameConvention();

            modelBuilder.SeedDataBase(_dateTimeService);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_entitySaveChangesInterceptor);

            base.OnConfiguring(optionsBuilder);
        }
    }
}
