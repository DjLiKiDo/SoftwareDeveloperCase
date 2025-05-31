using Microsoft.EntityFrameworkCore;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Domain.Entities.Core;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using SoftwareDeveloperCase.Domain.Entities.Lookups;
using SoftwareDeveloperCase.Domain.ValueObjects;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.Extensions;

internal static class ModelBuilderExtensions
{
    internal static void UseSingularTableNameConvention(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.DisplayName());
        }
    }

    internal static void SeedDataBase(this ModelBuilder modelBuilder, IDateTimeService datetimeService)
    {
        modelBuilder.Entity<Department>()
            .HasData(
               new Department { Id = Guid.Parse("7E1ECEDD-D9A5-4C81-8D2D-0FFD332F29C0"), Name = "HR", CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now },
               new Department { Id = Guid.Parse("0EDED24E-F07E-434C-AF1D-B97D638564C9"), Name = "IT", CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now }
            );

        modelBuilder.Entity<Permission>()
            .HasData(
                new Permission { Id = Guid.Parse("9A6AE1D8-0688-43D4-B1CE-2A13608FA68C"), Name = "Read", CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now },
                new Permission { Id = Guid.Parse("D2E69E18-E1A5-48C2-B5B5-EB888C13D46B"), Name = "Add", CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now },
                new Permission { Id = Guid.Parse("5162E7DA-6B87-424A-A08D-FDD6E3C6B4B2"), Name = "Update", CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now },
                new Permission { Id = Guid.Parse("4A03F568-69E4-4548-85B6-8100CAD15631"), Name = "Delete", CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now }
            );

        modelBuilder.Entity<Role>()
            .HasData(
                new Role { Id = Guid.Parse("2D7AA3B0-F221-4753-B77F-FF261858A13A"), Name = "Employee", ParentRoleId = null, CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now },
                new Role { Id = Guid.Parse("9ECA8D57-F7CA-4F8D-9C83-73B659225AE4"), Name = "Manager", ParentRoleId = Guid.Parse("2D7AA3B0-F221-4753-B77F-FF261858A13A"), CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now }

            );

        modelBuilder.Entity<RolePermission>()
            .HasData(
                new RolePermission { Id = Guid.Parse("9E89D8F2-C8DD-474C-B7FA-267A9570488A"), RoleId = Guid.Parse("2D7AA3B0-F221-4753-B77F-FF261858A13A"), PermissionId = Guid.Parse("9A6AE1D8-0688-43D4-B1CE-2A13608FA68C"), CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now },
                new RolePermission { Id = Guid.Parse("00AEB653-9BFD-46BD-98E8-631C080A7CD3"), RoleId = Guid.Parse("9ECA8D57-F7CA-4F8D-9C83-73B659225AE4"), PermissionId = Guid.Parse("9A6AE1D8-0688-43D4-B1CE-2A13608FA68C"), CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now },
                new RolePermission { Id = Guid.Parse("265E22E5-2252-432E-B6D2-E994740F6F08"), RoleId = Guid.Parse("9ECA8D57-F7CA-4F8D-9C83-73B659225AE4"), PermissionId = Guid.Parse("D2E69E18-E1A5-48C2-B5B5-EB888C13D46B"), CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now },
                new RolePermission { Id = Guid.Parse("5B953AA8-6BDE-4F47-B984-D3622A8E0550"), RoleId = Guid.Parse("9ECA8D57-F7CA-4F8D-9C83-73B659225AE4"), PermissionId = Guid.Parse("5162E7DA-6B87-424A-A08D-FDD6E3C6B4B2"), CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now },
                new RolePermission { Id = Guid.Parse("0754E6C4-4E12-42B1-845B-21AD1FC2F6B0"), RoleId = Guid.Parse("9ECA8D57-F7CA-4F8D-9C83-73B659225AE4"), PermissionId = Guid.Parse("4A03F568-69E4-4548-85B6-8100CAD15631"), CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now }

            );

        modelBuilder.Entity<User>()
            .HasData(
                new User { Id = Guid.Parse("2D7AA3B0-F221-4753-B77F-FF261858A13A"), Name = "HR Employee", Email = (Email)"hremployee@sdc.com", Password = "sdc", DepartmentId = Guid.Parse("7E1ECEDD-D9A5-4C81-8D2D-0FFD332F29C0"), CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now },
                new User { Id = Guid.Parse("64A19C7D-A7A9-4481-A498-7DF87F341DA4"), Name = "HR Manager", Email = (Email)"hrmanager@sdc.com", Password = "sdc", DepartmentId = Guid.Parse("7E1ECEDD-D9A5-4C81-8D2D-0FFD332F29C0"), CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now },
                new User { Id = Guid.Parse("29D6CF7D-2335-4329-91AD-4A7EC437D73C"), Name = "IT Employee", Email = (Email)"itemployee@sdc.com", Password = "sdc", DepartmentId = Guid.Parse("0EDED24E-F07E-434C-AF1D-B97D638564C9"), CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now },
                new User { Id = Guid.Parse("94651E82-5FC3-43D0-9C64-5A16AC517D43"), Name = "IT Manager", Email = (Email)"itmanager@sdc.com", Password = "sdc", DepartmentId = Guid.Parse("0EDED24E-F07E-434C-AF1D-B97D638564C9"), CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now }
            );

        modelBuilder.Entity<UserRole>()
            .HasData(
                new UserRole { Id = Guid.Parse("47175080-A8C0-4D4F-BF88-85CD5CBFF45B"), UserId = Guid.Parse("2D7AA3B0-F221-4753-B77F-FF261858A13A"), RoleId = Guid.Parse("2D7AA3B0-F221-4753-B77F-FF261858A13A"), CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now },
                new UserRole { Id = Guid.Parse("CF61C58C-E416-4B84-A195-DD7B77EF96DD"), UserId = Guid.Parse("64A19C7D-A7A9-4481-A498-7DF87F341DA4"), RoleId = Guid.Parse("2D7AA3B0-F221-4753-B77F-FF261858A13A"), CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now },
                new UserRole { Id = Guid.Parse("8E49F3BA-11F1-4256-A195-91F9A8F3A41B"), UserId = Guid.Parse("64A19C7D-A7A9-4481-A498-7DF87F341DA4"), RoleId = Guid.Parse("9ECA8D57-F7CA-4F8D-9C83-73B659225AE4"), CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now },
                new UserRole { Id = Guid.Parse("5562714C-A186-439F-A0DF-25321703CF7E"), UserId = Guid.Parse("29D6CF7D-2335-4329-91AD-4A7EC437D73C"), RoleId = Guid.Parse("2D7AA3B0-F221-4753-B77F-FF261858A13A"), CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now },
                new UserRole { Id = Guid.Parse("4DC7A2EA-5E37-4038-8810-79FD9DF7AC0A"), UserId = Guid.Parse("94651E82-5FC3-43D0-9C64-5A16AC517D43"), RoleId = Guid.Parse("2D7AA3B0-F221-4753-B77F-FF261858A13A"), CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now },
                new UserRole { Id = Guid.Parse("07A59E4E-EAD7-4597-BB36-96A06EE3A847"), UserId = Guid.Parse("94651E82-5FC3-43D0-9C64-5A16AC517D43"), RoleId = Guid.Parse("9ECA8D57-F7CA-4F8D-9C83-73B659225AE4"), CreatedBy = "InitialSeed", CreatedOn = datetimeService.Now }
            );
    }
}
