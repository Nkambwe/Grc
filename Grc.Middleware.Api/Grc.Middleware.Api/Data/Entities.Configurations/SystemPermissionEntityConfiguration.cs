using Grc.Middleware.Api.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations
{
    public class SystemPermissionEntityConfiguration
    {

        public static void Configure(EntityTypeBuilder<SystemPermission> builder)
        {
            builder.ToTable("TBL_GRC_PERMISSION");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.PermissionName).HasColumnName("Permission_Name").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(p => p.Description).HasColumnName("Permission_Description").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(p => p.IsDeleted).HasColumnName("Is_Deleted");
            builder.Property(p => p.CreatedOn).HasColumnName("Created_On").IsRequired();
            builder.Property(p => p.CreatedBy).HasColumnName("Created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(p => p.LastModifiedOn).HasColumnName("Modified_On").IsRequired(false);
            builder.Property(p => p.LastModifiedBy).HasColumnName("Modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
        }
    }


    public class SystemPermissionSetEntityConfiguration
    {

        public static void Configure(EntityTypeBuilder<SystemPermissionSet> builder)
        {
            builder.ToTable("TBL_GRC_PERMISSION_SET");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.SetName).HasColumnName("Set_Name").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(p => p.Description).HasColumnName("Set_Description").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(p => p.IsDeleted).HasColumnName("Is_Deleted");
            builder.Property(p => p.CreatedOn).HasColumnName("Created_On").IsRequired();
            builder.Property(p => p.CreatedBy).HasColumnName("Created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(p => p.LastModifiedOn).HasColumnName("Modified_On").IsRequired(false);
            builder.Property(p => p.LastModifiedBy).HasColumnName("Modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
        }
    }
}
