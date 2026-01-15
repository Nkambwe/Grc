using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class AuditUpdateEntityConfiguration {

        public static void Configure(EntityTypeBuilder<AuditUpdate> builder) {
            builder.ToTable("TBL_GRC_AUDIT_UPDATE");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.Notes).HasColumnName("audit_notes").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(a => a.NoteDate).HasColumnName("added_on").IsRequired(true);
            builder.Property(a => a.SendReminder).HasColumnName("send_reminder");
            builder.Property(a => a.SendReminderOn).HasColumnName("send_on").IsRequired(false);
            builder.Property(a => a.ReminderMessage).HasColumnName("reminder_msg").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(a => a.SendTo).HasColumnName("send_to").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(a => a.AddedBy).HasColumnName("added_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(a => a.IsDeleted).HasColumnName("is_deleted");
            builder.Property(a => a.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(a => a.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(a => a.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(a => a.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.Property(a => a.ReportId).HasColumnName("report_id");
            builder.HasOne(a => a.Report).WithMany(r => r.AuditUpdates).HasForeignKey(a => a.ReportId);
        }
    }

}
