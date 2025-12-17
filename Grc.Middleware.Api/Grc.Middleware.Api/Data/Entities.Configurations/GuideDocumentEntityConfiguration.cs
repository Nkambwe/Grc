using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations
{
    public class GuideDocumentEntityConfiguration {
        public static void Configure(EntityTypeBuilder<RegulatoryDocument> builder) {
            builder.ToTable("TBL_GRC_GUIDE_DOCUMENT"); 
            builder.HasKey(t => t.Id);
            builder.Property(t => t.DocumentName).HasColumnName("doc_name").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(t => t.Status).HasColumnName("doc_status").HasColumnType("NVARCHAR(50)").IsRequired(true);
            builder.Property(t => t.ApprovedBy).HasColumnName("doc_approved_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.Property(t => t.PolicyAligned).HasColumnName("is_aligned").IsRequired(true);
            builder.Property(t => t.Comments).HasColumnName("doc_comments").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(t => t.IsDeleted).HasColumnName("is_deleted");
            builder.Property(t => t.IsLocked).HasColumnName("is_locked").IsRequired(false);
            builder.Property(t => t.LastRevisionDate).HasColumnName("last_revision_on").IsRequired();
            builder.Property(t => t.FrequencyId).HasColumnName("frequency_id").IsRequired();
            builder.Property(t => t.NextRevisionDate).HasColumnName("next_revision_on").IsRequired(false);
            builder.Property(t => t.DocumentTypeId).HasColumnName("doc_type_id");
            builder.Property(t => t.ResponsibilityId).HasColumnName("doc_owner_id"); 
            builder.Property(t => t.ApprovalDate).HasColumnName("approval_date").IsRequired(false);
            builder.Property(t => t.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(t => t.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(t => t.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(t => t.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(t => t.DocumentType).WithMany(o => o.Documents).HasForeignKey(s => s.DocumentTypeId);
            builder.HasOne(t => t.Owner).WithMany(o => o.ComplianceDocuments).HasForeignKey(s => s.ResponsibilityId);
            builder.HasOne(t => t.Frequency).WithMany(o => o.Documents).HasForeignKey(s => s.FrequencyId);
        }
    }
}
