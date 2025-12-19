using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class ArticleRevisionEntityConfiguration {
        public static void Configure(EntityTypeBuilder<ArticleRevision> builder) {
            builder.ToTable("TBL_GRC_STATUTORY_ACT_REVISION");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Section).HasColumnName("section").HasColumnType("NVARCHAR(40)").IsRequired(true);
            builder.Property(a => a.Summery).HasColumnName("act_summery").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(a => a.Revision).HasColumnName("act_revision").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(a => a.ArticleId).HasColumnName("article_id");
            builder.Property(a => a.ReviewedOn).HasColumnName("review_date");
            builder.Property(a => a.IsDeleted).HasColumnName("is_deleted");
            builder.Property(a => a.Comments).HasColumnName("comments").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(a => a.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(a => a.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(a => a.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(a => a.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(a => a.Article).WithMany(r => r.ArticleRevisions).HasForeignKey(a => a.ArticleId);
        }
    }
}
