using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoftwareDeveloperCase.Domain.Entities.Identity;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Configurations;

/// <summary>
/// Entity Framework configuration for the UserRole entity (User-Role many-to-many)
/// </summary>
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        // Table name
        builder.ToTable("UserRole");
        
        // Primary key
        builder.HasKey(ur => ur.Id);
        
        // Foreign key relationships
        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Indexes
        builder.HasIndex(ur => ur.UserId)
            .HasDatabaseName("IX_UserRole_UserId");
            
        builder.HasIndex(ur => ur.RoleId)
            .HasDatabaseName("IX_UserRole_RoleId");
            
        // Unique constraint to prevent duplicate user-role assignments
        builder.HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique()
            .HasDatabaseName("IX_UserRole_UserId_RoleId");
    }
}