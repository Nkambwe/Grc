using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class StatutoryArticleControlEntityConfiguration {
        public static void Configure(EntityTypeBuilder<StatutoryArticleControl> builder) {
            builder.ToTable("TBL_GRC_STATUTORY_ACT_CONTROL");
            builder.HasKey(pg => new { pg.StatutoryArticleId, pg.ControlItemId });
            builder.Property(pg => pg.StatutoryArticleId).HasColumnName("artical_id").IsRequired();
            builder.Property(pg => pg.ControlItemId).HasColumnName("item_id").IsRequired();
            builder.HasOne(pg => pg.StatutoryArticle).WithMany(p => p.StatutoryArticleControls).HasForeignKey(pg => pg.StatutoryArticleId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pg => pg.ControlItem).WithMany(g => g.StatutoryArticleControls).HasForeignKey(pg => pg.ControlItemId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
