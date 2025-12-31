using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class ComplianceIssueEntityConfiguration {
        public static void Configure(EntityTypeBuilder<ComplianceIssue> builder) {
            builder.ToTable("TBL_GRC_COMPLIANCE_ISSUE");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.Description).HasColumnName("issue_description").HasColumnType("NVARCHAR(250)").IsRequired(true);
            builder.Property(a => a.StatutoryArticleId).HasColumnName("artical_id");
            builder.Property(a => a.IsClosed).HasColumnName("is_closed");
            builder.Property(a => a.IsDeleted).HasColumnName("is_deleted");
            builder.Property(a => a.Notes).HasColumnName("notes").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(a => a.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(a => a.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(a => a.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(a => a.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(pg => pg.StatutoryArticle).WithMany(g => g.ComplianceIssues).HasForeignKey(pg => pg.StatutoryArticleId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
