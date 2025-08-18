using Grc.Middleware.Api.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class PinnedItemEntityConfiguration {
        public static void Configure(EntityTypeBuilder<UserPinnedItem> builder) {
            builder.ToTable("TBL_GRC_PINNEDITEM");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.UserId).HasColumnName("user_id");
            builder.Property(p => p.Label).HasColumnName("menu_label").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(p => p.IconClass).HasColumnName("menu_icon").HasColumnType("NVARCHAR(180)");
            builder.Property(p => p.Controller).HasColumnName("controller_name").HasColumnType("NVARCHAR(200)").IsRequired();
            builder.Property(p => p.Action).HasColumnName("action_name").HasColumnType("NVARCHAR(200)").IsRequired();
            builder.Property(q => q.Area).HasColumnName("area_name").HasColumnType("NVARCHAR(200)");
            builder.Property(p => p.CssClass).HasColumnName("css_class").HasColumnType("NVARCHAR(200)");
            builder.Property(p => p.IsDeleted).HasColumnName("is_deleted");
            builder.Property(p => p.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(p => p.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(p => p.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(p => p.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.HasOne(p => p.User).WithMany(u => u.PinnedItems).HasForeignKey(p => p.UserId);
        }
    }
}
