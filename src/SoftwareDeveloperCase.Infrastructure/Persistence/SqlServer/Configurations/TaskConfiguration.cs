using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Configurations;

/// <summary>
/// Entity Framework configuration for the Task entity
/// </summary>
public class TaskConfiguration : IEntityTypeConfiguration<Domain.Entities.Task.Task>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Task.Task> builder)
    {
        // Table name
        builder.ToTable("Task");

        // Primary key
        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.Status)
            .IsRequired();

        builder.Property(t => t.Priority)
            .IsRequired();

        // Configure decimal properties with precision and scale
        builder.Property(t => t.EstimatedHours)
            .HasPrecision(18, 2);

        builder.Property(t => t.ActualHours)
            .HasPrecision(18, 2);

        // Configure TaskHierarchy value object as owned entity
        builder.OwnsOne(t => t.Hierarchy, hierarchy =>
        {
            hierarchy.Property(h => h.Level)
                .HasColumnName("HierarchyLevel")
                .IsRequired();

            hierarchy.Property(h => h.Path)
                .HasColumnName("HierarchyPath")
                .HasMaxLength(500)
                .IsRequired();

            hierarchy.Property(h => h.Order)
                .HasColumnName("HierarchyOrder")
                .IsRequired();
        });

        // Foreign key relationships
        builder.HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.AssignedTo)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssignedToId)
            .OnDelete(DeleteBehavior.SetNull);

        // Self-referencing relationship for task hierarchy
        builder.HasOne(t => t.ParentTask)
            .WithMany(t => t.SubTasks)
            .HasForeignKey(t => t.ParentTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(t => t.Title)
            .HasDatabaseName("IX_Task_Title");

        builder.HasIndex(t => t.Status)
            .HasDatabaseName("IX_Task_Status");

        builder.HasIndex(t => t.Priority)
            .HasDatabaseName("IX_Task_Priority");

        builder.HasIndex(t => t.ProjectId)
            .HasDatabaseName("IX_Task_ProjectId");

        builder.HasIndex(t => t.AssignedToId)
            .HasDatabaseName("IX_Task_AssignedToId");

        builder.HasIndex(t => t.ParentTaskId)
            .HasDatabaseName("IX_Task_ParentTaskId");

        builder.HasIndex(t => t.DueDate)
            .HasDatabaseName("IX_Task_DueDate");

        // Composite indexes for common queries as specified in requirements
        builder.HasIndex(t => new { t.ProjectId, t.Status })
            .HasDatabaseName("IX_Task_ProjectId_Status");

        builder.HasIndex(t => new { t.AssignedToId, t.Status })
            .HasDatabaseName("IX_Task_AssignedToId_Status");

        builder.HasIndex(t => new { t.Status, t.Priority })
            .HasDatabaseName("IX_Task_Status_Priority");

        builder.HasIndex(t => new { t.ProjectId, t.AssignedToId })
            .HasDatabaseName("IX_Task_ProjectId_AssignedToId");
    }
}
