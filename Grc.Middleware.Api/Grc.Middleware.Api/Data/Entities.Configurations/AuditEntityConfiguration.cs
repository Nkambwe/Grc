using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations
{
    public class AuditEntityConfiguration
    {
        public static void Configure(EntityTypeBuilder<Audit> builder)
        {
            builder.ToTable("TBL_GRC_AUDITS");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.AuditName).HasColumnName("audit_name").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(a => a.Priority).HasColumnName("audit_priority").HasColumnType("NVARCHAR(50)").IsRequired(true);
            builder.Property(a => a.Category).HasColumnName("audit_category").HasColumnType("NVARCHAR(180)").IsRequired(false);
            builder.Property(a => a.SubCategory).HasColumnName("audit_sub_category").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.Property(a => a.NumberOfReports).HasColumnName("reports").HasColumnType("INT").IsRequired(true).HasDefaultValue(0);
            builder.Property(a => a.IsDeleted).HasColumnName("is_deleted");
            builder.Property(a => a.Attachement).HasColumnName("file_attachment").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(a => a.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(a => a.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(a => a.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(a => a.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.Property(a => a.AuthorityId).HasColumnName("issuer_id");
            builder.HasOne(a => a.Authority).WithMany(i => i.Audits).HasForeignKey(a => a.AuthorityId);
            builder.HasMany(a => a.Tasks).WithOne(t => t.Audit).HasForeignKey(a => a.AuditId);
            builder.HasMany(a => a.AuditReports).WithOne(r => r.Audit).HasForeignKey(a => a.AuditId);
        }
    }
}
