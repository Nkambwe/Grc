using Grc.Middleware.Api.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class UserPrefferenceEntityConfiguration {
    public static void Configure(EntityTypeBuilder<UserPrefference> builder) {
            builder.ToTable("TBL_GRC_PREFFERENCE");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.UserId).HasColumnName("user_id");
            builder.Property(p => p.Theme).HasColumnName("user_theme").HasColumnType("NVARCHAR(100)");
            builder.Property(p => p.Language).HasColumnName("user_language").HasColumnType("NVARCHAR(100)");
            builder.Property(p => p.IsDeleted).HasColumnName("is_deleted");
            builder.Property(p => p.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(p => p.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(p => p.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(p => p.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.HasOne(p => p.User).WithMany(c => c.Prefferences).HasForeignKey(p => p.UserId);
         }
    }
}
