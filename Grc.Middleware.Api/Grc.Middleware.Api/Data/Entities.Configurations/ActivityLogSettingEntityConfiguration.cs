using Grc.Middleware.Api.Data.Entities.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Grc.Middleware.Api.Data.Entities.Configurations {

    public class ActivityLogSettingEntityConfiguration {

        public static void Configure(EntityTypeBuilder<ActivityLogSetting> builder) {
            builder.ToTable("TBL_GRC_ACTIVITY_SETTING");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.EnableLogging).HasColumnName("enable_logging").IsRequired();
            builder.Property(a => a.LogSupportActivities).HasColumnName("log_support").IsRequired();
            builder.Property(a => a.LogUserActivities).HasColumnName("log_user").IsRequired();
            builder.Property(a => a.AutoDeleteActivities).HasColumnName("auto_delete").IsRequired();
            builder.Property(a => a.AutoDeleteDays).HasColumnName("auto_delete_days").IsRequired();
            builder.Property(a => a.CompanyId).HasColumnName("company_id").IsRequired();
            builder.Property(a => a.DisabledActivityTypes)
            .HasColumnName("disabled_activites")
            .HasColumnType("NVARCHAR(MAX)")
            .HasConversion(
                l => JsonSerializer.Serialize(l, (JsonSerializerOptions)null),  
                l => JsonSerializer.Deserialize<List<string>>(l, (JsonSerializerOptions)null)
            );
            builder.Property(a => a.LogIpAddress).HasColumnName("log_ip");
            builder.Property(a => a.IsDeleted).HasColumnName("is_deleted");
            builder.Property(a => a.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(a => a.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(a => a.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(a => a.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(a => a.Company).WithMany(c => c.ActivitySettings).HasForeignKey(c => c.CompanyId);
        }
    }
}
