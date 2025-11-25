using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class ProcessTaskEntityConfiguration
    {

        public static void Configure(EntityTypeBuilder<ProcessTask> builder)
        {
            builder.ToTable("TBL_GRC_PROCESS_TASK");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.TaskName).HasColumnName("task_name").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(t => t.Description).HasColumnName("task_description").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(t => t.Status).HasColumnName("task_status").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(t => t.DueDate).HasColumnName("due_date").IsRequired();
            builder.Property(t => t.ProcessId).HasColumnName("process_id");
            builder.Property(t => t.OwnerId).HasColumnName("owner_id");
            builder.Property(t => t.IsDeleted).HasColumnName("is_deleted");
            builder.Property(t => t.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(t => t.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(t => t.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(t => t.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(t => t.TaskOwner).WithMany(t => t.ProcessTasks).HasForeignKey(p => p.ProcessId);
            builder.HasOne(t => t.Process).WithMany(p => p.Tasks).HasForeignKey(t => t.ProcessId);
        }

    }
}
