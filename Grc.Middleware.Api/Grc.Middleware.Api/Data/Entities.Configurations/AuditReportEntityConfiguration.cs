using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations
{
    public class AuditReportEntityConfiguration
    {
        public static void Configure(EntityTypeBuilder<AuditReport> builder)
        {
            builder.ToTable("TBL_GRC_AUDIT_REPORT");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.ReportName).HasColumnName("report_name").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(r => r.Subject).HasColumnName("report_Subject").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(r => r.AuditedOn).HasColumnName("audited_on").IsRequired();
            builder.Property(r => r.Status).HasColumnName("doc_status").HasColumnType("NVARCHAR(50)").IsRequired(true);
            builder.Property(r => r.RespondedOn).HasColumnName("responded_on").IsRequired(false);
            builder.Property(r => r.ManagementComment).HasColumnName("mgt_comments").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(r => r.AdditionalNotes).HasColumnName("more_notes").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(r => r.AuditId).HasColumnName("audit_id");
            builder.Property(r => r.IsDeleted).HasColumnName("is_deleted");
            builder.Property(r => r.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(r => r.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(r => r.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(r => r.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(r => r.Audit).WithMany(a => a.AuditReports).HasForeignKey(r => r.AuditId);
            builder.HasMany(r => r.AuditExceptions).WithOne(x => x.AuditReport).HasForeignKey(s => s.AuditReportId);
        }
    }
}
