using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoftwareDeveloperCase.Domain.Entities.Project;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Configurations;

/// <summary>
/// Entity Framework configuration for the Project entity
/// </summary>
public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        // Table name
        builder.ToTable("Project");

        // Primary key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.Status)
            .IsRequired();

        builder.Property(p => p.Priority)
            .IsRequired();

        // Foreign key relationship
        builder.HasOne(p => p.Team)
            .WithMany(t => t.Projects)
            .HasForeignKey(p => p.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(p => p.Name)
            .HasDatabaseName("IX_Project_Name");

        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_Project_Status");

        builder.HasIndex(p => p.Priority)
            .HasDatabaseName("IX_Project_Priority");

        builder.HasIndex(p => p.TeamId)
            .HasDatabaseName("IX_Project_TeamId");

        // Composite indexes for common queries
        builder.HasIndex(p => new { p.TeamId, p.Status })
            .HasDatabaseName("IX_Project_TeamId_Status");

        builder.HasIndex(p => new { p.Status, p.Priority })
            .HasDatabaseName("IX_Project_Status_Priority");
    }
}
