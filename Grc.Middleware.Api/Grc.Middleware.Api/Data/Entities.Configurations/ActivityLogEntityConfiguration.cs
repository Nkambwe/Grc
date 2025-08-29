using Grc.Middleware.Api.Data.Entities.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {

    public class ActivityLogEntityConfiguration {
        public static void Configure(EntityTypeBuilder<ActivityLog> builder) {
            builder.ToTable("TBL_GRC_ACTIVITY_LOG");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.EntityId).HasColumnName("entity_id").IsRequired(false);
            builder.Property(a => a.TypeId).HasColumnName("type_id").IsRequired();
            builder.Property(a => a.EntityName).HasColumnName("entity_name").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(a => a.IpAddress).HasColumnName("ip_address").HasColumnType("NVARCHAR(80)").IsRequired(false);
            builder.Property(a => a.Comment).HasColumnName("log_comment").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(a => a.UserId).HasColumnName("user_id").IsRequired();
            builder.Property(a => a.IsDeleted).HasColumnName("is_deleted");
            builder.Property(a => a.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(a => a.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(a => a.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(a => a.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(a => a.ActivityType).WithMany(t => t.ActivityLogs).HasForeignKey(t => t.TypeId);
            builder.HasOne(a => a.User).WithMany(u => u.ActivityLogs).HasForeignKey(u => u.UserId);
        }
    }
}
