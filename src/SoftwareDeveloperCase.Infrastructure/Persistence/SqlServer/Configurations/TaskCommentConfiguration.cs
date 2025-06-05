using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoftwareDeveloperCase.Domain.Entities.Task;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Configurations;

/// <summary>
/// Entity Framework configuration for the TaskComment entity
/// </summary>
public class TaskCommentConfiguration : IEntityTypeConfiguration<TaskComment>
{
    public void Configure(EntityTypeBuilder<TaskComment> builder)
    {
        // Table name
        builder.ToTable("TaskComment");

        // Primary key
        builder.HasKey(tc => tc.Id);

        // Properties
        builder.Property(tc => tc.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(tc => tc.CreatedAt)
            .IsRequired();

        // Foreign key relationships
        builder.HasOne(tc => tc.Task)
            .WithMany(t => t.Comments)
            .HasForeignKey(tc => tc.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tc => tc.Author)
            .WithMany(u => u.TaskComments)
            .HasForeignKey(tc => tc.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(tc => tc.TaskId)
            .HasDatabaseName("IX_TaskComment_TaskId");

        builder.HasIndex(tc => tc.AuthorId)
            .HasDatabaseName("IX_TaskComment_AuthorId");

        builder.HasIndex(tc => tc.CreatedAt)
            .HasDatabaseName("IX_TaskComment_CreatedAt");

        // Composite indexes for common queries
        builder.HasIndex(tc => new { tc.TaskId, tc.CreatedAt })
            .HasDatabaseName("IX_TaskComment_TaskId_CreatedAt");

        builder.HasIndex(tc => new { tc.AuthorId, tc.CreatedAt })
            .HasDatabaseName("IX_TaskComment_AuthorId_CreatedAt");
    }
}
