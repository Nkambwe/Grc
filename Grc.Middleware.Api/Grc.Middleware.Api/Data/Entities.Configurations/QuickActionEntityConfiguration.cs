using Grc.Middleware.Api.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class QuickActionEntityConfiguration {
        public static void Configure(EntityTypeBuilder<UserQuickAction> builder) {
            builder.ToTable("TBL_GRC_QUICKACTION");
            builder.HasKey(q => q.Id);
            builder.Property(q => q.UserId).HasColumnName("user_id");
            builder.Property(q => q.Label).HasColumnName("menu_label").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(q => q.IconClass).HasColumnName("menu_icon").HasColumnType("NVARCHAR(180)");
            builder.Property(q => q.Controller).HasColumnName("controller_name").HasColumnType("NVARCHAR(200)").IsRequired();
            builder.Property(q => q.Action).HasColumnName("action_name").HasColumnType("NVARCHAR(200)").IsRequired();
            builder.Property(q => q.Area).HasColumnName("area_name").HasColumnType("NVARCHAR(200)");
            builder.Property(q => q.CssClass).HasColumnName("css_class").HasColumnType("NVARCHAR(200)");
            builder.Property(q => q.IsDeleted).HasColumnName("is_deleted");
            builder.Property(q => q.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(q => q.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(q => q.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(q => q.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.HasOne(q => q.User).WithMany(u => u.QuickActions).HasForeignKey(q => q.UserId);
        }
    }
}
