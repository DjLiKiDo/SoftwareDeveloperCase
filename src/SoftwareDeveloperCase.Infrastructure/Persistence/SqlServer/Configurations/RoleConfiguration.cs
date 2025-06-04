using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoftwareDeveloperCase.Domain.Entities;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Configurations;

/// <summary>
/// Entity Framework configuration for the Role entity
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Table name
        builder.ToTable("Role");
        
        // Primary key
        builder.HasKey(r => r.Id);
        
        // Properties
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        // Self-referencing relationship for hierarchical roles
        builder.HasOne(r => r.ParentRole)
            .WithMany()
            .HasForeignKey(r => r.ParentRoleId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Indexes
        builder.HasIndex(r => r.Name)
            .IsUnique()
            .HasDatabaseName("IX_Role_Name");
            
        builder.HasIndex(r => r.ParentRoleId)
            .HasDatabaseName("IX_Role_ParentRoleId");
    }
}