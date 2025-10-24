using Grc.Middleware.Api.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class SystemConfigurationEntityConfiguration
    {
        public static void Configure(EntityTypeBuilder<SystemConfiguration> builder)
        {
            builder.ToTable("TBL_GRC_SYSTEM_CONFIG");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.ParameterName).HasColumnName("param_name").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(b => b.ParameterValue).HasColumnName("param_value").HasColumnType("NVARCHAR(MAX)");
            builder.Property(b => b.ParameterType).HasColumnName("param_type").HasColumnType("NVARCHAR(50)");
            builder.Property(b => b.CompanyId).HasColumnName("company_id");
            builder.Property(b => b.IsDeleted).HasColumnName("is_deleted");
            builder.Property(b => b.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(b => b.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(b => b.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(b => b.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.HasOne(b => b.Company).WithMany(c => c.SystemConfigurations).HasForeignKey(b => b.CompanyId);
        }
    }
}
