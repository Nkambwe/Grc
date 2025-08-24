using Grc.Middleware.Api.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class UserViewEntityConfiguration {
    public static void Configure(EntityTypeBuilder<UserView> builder) {
            builder.ToTable("TBL_GRC_VIEW");
            builder.HasKey(v => v.Id);
            builder.Property(v => v.UserId).HasColumnName("user_id");
            builder.Property(v => v.Name).HasColumnName("view_name").HasColumnType("NVARCHAR(MAX)");
            builder.Property(v => v.View).HasColumnName("view_info").HasColumnType("NVARCHAR(MAX)");
            builder.Property(v => v.IsDeleted).HasColumnName("is_deleted");
            builder.Property(v => v.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(v => v.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(v => v.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(v => v.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.HasOne(v => v.User).WithMany(s => s.Views).HasForeignKey(v => v.UserId);
         }
    }
}
