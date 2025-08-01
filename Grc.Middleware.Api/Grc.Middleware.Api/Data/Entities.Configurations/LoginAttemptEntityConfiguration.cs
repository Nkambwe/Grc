using Grc.Middleware.Api.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class LoginAttemptEntityConfiguration {
         public static void Configure(EntityTypeBuilder<LoginAttempt> builder) {
            builder.ToTable("TBL_GRC_ATTEMPTS");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.UserId).HasColumnName("user_id");
            builder.Property(b => b.IpAddress).HasColumnName("ip_address").HasColumnType("NVARCHAR(100)").IsRequired();
            builder.Property(b => b.AttemptTime).HasColumnName("attempt_time").IsRequired();
            builder.Property(b => b.IsSuccessful).HasColumnName("is_success").IsRequired();
            builder.Property(b => b.IsDeleted).HasColumnName("is_deleted");
            builder.Property(b => b.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(b => b.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(b => b.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(b => b.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.HasOne(b => b.User).WithMany(c => c.Attempts).HasForeignKey(b => b.User);
         }
    }
}
