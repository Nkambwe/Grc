using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations
{
    public class AuditTaskEntityConfiguration {
        public static void Configure(EntityTypeBuilder<AuditTask> builder) {
            builder.ToTable("TBL_GRC_AUDITTASK");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.TaskName).HasColumnName("task_name").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(t => t.Status).HasColumnName("task_status").HasColumnType("NVARCHAR(50)").IsRequired(true);
            builder.Property(t => t.Description).HasColumnName("task_description").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(t => t.IsDeleted).HasColumnName("is_deleted");
            builder.Property(t => t.DueDate).HasColumnName("due_date").IsRequired(false);
            builder.Property(t => t.SendReminder).HasColumnName("send_reminder").IsRequired(true);
            builder.Property(t => t.Interval).HasColumnName("reminder_interval").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.Property(t => t.IntervalType).HasColumnName("interval_type").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.Property(t => t.Reminder).HasColumnName("reminder_message").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.Property(t => t.OwnerId).HasColumnName("owner_id").IsRequired();
            builder.Property(t => t.AuditId).HasColumnName("audit_id");
            builder.Property(t => t.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(t => t.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(t => t.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(t => t.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(t => t.Audit).WithMany(o => o.Tasks).HasForeignKey(t => t.AuditId);
            builder.HasOne(t => t.ActionOwner).WithMany(r => r.AuditTasks).HasForeignKey(t => t.AuditId);
            builder.HasMany(t => t.AuditExceptions).WithOne(x => x.AuditTask).HasForeignKey(x => x.AuditTaskId);
            
        }
    }
}
