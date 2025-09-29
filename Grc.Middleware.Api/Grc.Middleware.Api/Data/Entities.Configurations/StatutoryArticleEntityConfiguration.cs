using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations
{
    public class StatutoryArticleEntityConfiguration
    {
        public static void Configure(EntityTypeBuilder<StatutoryArticle> builder)
        {
            builder.ToTable("TBL_GRC_Statutory_Act");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Article).HasColumnName("article_descr").HasColumnType("NVARCHAR(10)").IsRequired(true);
            builder.Property(a => a.Summery).HasColumnName("act_summery").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(a => a.ObligationOrRequirement).HasColumnName("act_obligation").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(a => a.StatuteId).HasColumnName("statute_id").IsRequired();
            builder.Property(a => a.IsDeleted).HasColumnName("is_deleted");
            builder.Property(a => a.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(a => a.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(a => a.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(a => a.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false); 
            builder.HasOne(a => a.Statute).WithMany(r => r.Articles).HasForeignKey(a => a.StatuteId);
            builder.HasMany(a => a.Returns).WithOne(r => r.Article).HasForeignKey(a => a.ArticleId);
        }
    }
}
