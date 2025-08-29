using Grc.Middleware.Api.Data.Entities.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Grc.Middleware.Api.Data.Entities.Configurations {

    public class ActivityLogSettingEntityConfiguration {

        public static void Configure(EntityTypeBuilder<ActivityLogSetting> builder) {
            builder.ToTable("TBL_GRC_ACTIVITY_SETTING");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.ParameterName).HasColumnName("param_key").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(a => a.ParameterValue).HasColumnName("param_value").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(a => a.Description).HasColumnName("param_descr").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(a => a.CompanyId).HasColumnName("company_id").IsRequired();
            builder.Property(a => a.IsDeleted).HasColumnName("is_deleted");
            builder.Property(a => a.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(a => a.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(a => a.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(a => a.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(a => a.Company).WithMany(c => c.ActivitySettings).HasForeignKey(c => c.CompanyId);
        }
    }
}
