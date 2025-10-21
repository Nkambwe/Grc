using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations
{
    public class AuditExceptionEntityConfiguration {
        public static void Configure(EntityTypeBuilder<AuditException> builder) {
            builder.ToTable("TBL_GRC_AUDIT_EXCEPTION");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Obligation).HasColumnName("audit_obligation").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(x => x.CorrectiveAction).HasColumnName("audit_action").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(x => x.ExceptionNoted).HasColumnName("audit_notes").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(x => x.RemediationPlan).HasColumnName("audit_remediation").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(x => x.TargetDate).HasColumnName("target_date").IsRequired();
            builder.Property(x => x.RiskAssessment).HasColumnName("audit_risk").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(x => x.RiskRating).HasColumnName("risk_rating").IsRequired(true).HasDefaultValue(0.0M);
            builder.Property(x => x.Status).HasColumnName("audit_status").HasColumnType("NVARCHAR(50)").IsRequired(true);
            builder.Property(x => x.LastUpdated).HasColumnName("last_updated").IsRequired(false);
            builder.Property(x => x.AuditReportId).HasColumnName("report_id");
            builder.Property(x => x.AuditTaskId).HasColumnName("task_id");
            builder.Property(x => x.IsDeleted).HasColumnName("is_deleted");
            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(x => x.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(x => x.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(x => x.AuditReport).WithMany(r => r.AuditExceptions).HasForeignKey(s => s.AuditReportId);
            builder.HasOne(x => x.AuditTask).WithMany(t => t.AuditExceptions).HasForeignKey(s => s.AuditTaskId);
        }
    }
}
