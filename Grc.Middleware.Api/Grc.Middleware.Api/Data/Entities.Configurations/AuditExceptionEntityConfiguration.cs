using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations
{
    public class AuditExceptionEntityConfiguration {
        public static void Configure(EntityTypeBuilder<AuditException> builder) {
            builder.ToTable("TBL_GRC_AUDIT_EXCEPTION");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.AuditFinding).HasColumnName("audit_finding").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(x => x.ProposedAction).HasColumnName("audit_proposed").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(x => x.CorrectiveAction).HasColumnName("audit_action").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(x => x.ExceptionNotes).HasColumnName("audit_notes").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(x => x.RemediationPlan).HasColumnName("audit_remediation").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(x => x.Executioner).HasColumnName("audit_exec").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(x => x.TargetDate).HasColumnName("target_date").IsRequired();
            builder.Property(x => x.RiskAssessment).HasColumnName("audit_risk").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(x => x.RiskRating).HasColumnName("risk_rating").IsRequired(true).HasDefaultValue(0.0M);
            builder.Property(x => x.Status).HasColumnName("audit_status").HasColumnType("NVARCHAR(50)").IsRequired(true);
            builder.Property(x => x.AuditReportId).HasColumnName("report_id");
            builder.Property(x => x.ResponsabilityId).HasColumnName("resp_id");
            builder.Property(x => x.IsDeleted).HasColumnName("is_deleted");
            builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(x => x.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(x => x.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(x => x.AuditReport).WithMany(r => r.AuditExceptions).HasForeignKey(s => s.AuditReportId);
            builder.HasOne(x => x.Responseability).WithMany(t => t.Findings).HasForeignKey(s => s.ResponsabilityId);
            builder.HasMany(x => x.AuditTasks).WithOne(t => t.AuditException).HasForeignKey(s => s.ExceptionId);
        }
    }
}
