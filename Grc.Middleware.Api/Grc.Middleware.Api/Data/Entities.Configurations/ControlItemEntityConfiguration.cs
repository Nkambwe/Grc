using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class ControlItemEntityConfiguration {
        public static void Configure(EntityTypeBuilder<ControlItem> builder) {
            builder.ToTable("TBL_GRC_COMPLIANCE_CONTROLITEM");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.ItemName).HasColumnName("item_name").HasColumnType("NVARCHAR(250)").IsRequired(true);
            builder.Property(a => a.Exclude).HasColumnName("exclude");
            builder.Property(a => a.ControlCategoryId).HasColumnName("category_id");
            builder.Property(a => a.IsDeleted).HasColumnName("is_deleted");
            builder.Property(a => a.Notes).HasColumnName("notes").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(a => a.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(a => a.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(a => a.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(a => a.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(pg => pg.ControlCategory).WithMany(g => g.ControlItems).HasForeignKey(pg => pg.ControlCategoryId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(pg => pg.StatutoryArticleControls).WithOne(p => p.ControlItem).HasForeignKey(pg => pg.ControlItemId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
