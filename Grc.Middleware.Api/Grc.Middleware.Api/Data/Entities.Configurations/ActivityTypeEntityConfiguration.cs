using Grc.Middleware.Api.Data.Entities.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {

    public class ActivityTypeEntityConfiguration {

        public static void Configure(EntityTypeBuilder<ActivityType> builder) {
            builder.ToTable("TBL_GRC_ACTIVITY_TYPE");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Name).HasColumnName("type_name").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(t => t.SystemKeyword).HasColumnName("key_word").HasColumnType("NVARCHAR(200)").IsRequired();
            builder.Property(t => t.Description).HasColumnName("type_description").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(t => t.Enabled).HasColumnName("Is_enabled");
            builder.Property(t => t.Category).HasColumnName("type_category").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(t => t.IsSupportActivity).HasColumnName("Is_support");
            builder.Property(t => t.IsDeleted).HasColumnName("Is_deleted");
            builder.Property(t => t.CreatedOn).HasColumnName("Created_on").IsRequired();
            builder.Property(t => t.CreatedBy).HasColumnName("Created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(t => t.LastModifiedOn).HasColumnName("Modified_on").IsRequired(false);
            builder.Property(t => t.LastModifiedBy).HasColumnName("Modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasMany(t => t.ActivityLogs).WithOne(a => a.ActivityType).HasForeignKey(a => a.EntityId);
        }
    }
}
