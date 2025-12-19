using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {

    public class StatutoryArticleEntityConfiguration {
        public static void Configure(EntityTypeBuilder<StatutoryArticle> builder) {
            builder.ToTable("TBL_GRC_STATUTORY_ACT");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Article).HasColumnName("section").HasColumnType("NVARCHAR(40)").IsRequired();
            builder.Property(a => a.Summery).HasColumnName("act_summery").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(a => a.ObligationOrRequirement).HasColumnName("act_obligation").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(s => s.IsMandatory).HasColumnName("is_mandatory");
            builder.Property(s => s.ExcludeFromCompliance).HasColumnName("is_excluded");
            builder.Property(s => s.Coverage).HasColumnName("coverage");
            builder.Property(s => s.IsCovered).HasColumnName("is_covered");
            builder.Property(s => s.ComplianceAssurance).HasColumnName("assurance");
            builder.Property(a => a.StatuteId).HasColumnName("statute_id").IsRequired();
            builder.Property(a => a.FrequencyId).HasColumnName("frequency_id");
            builder.Property(a => a.Comments).HasColumnName("comments").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(a => a.IsDeleted).HasColumnName("is_deleted");
            builder.Property(a => a.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(a => a.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(a => a.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(a => a.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(a => a.Frequency).WithMany(r => r.Articles).HasForeignKey(a => a.FrequencyId);
            builder.HasOne(a => a.Statute).WithMany(r => r.Articles).HasForeignKey(a => a.StatuteId);
            builder.HasMany(a => a.Returns).WithOne(r => r.Article).HasForeignKey(a => a.ArticleId);
        }
    }
}
