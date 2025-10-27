using Grc.Middleware.Api.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class SystemRolePermissionSetEntityConfiguration
    {
        public static void Configure(EntityTypeBuilder<SystemRolePermissionSet> builder)
        {
            builder.ToTable("TBL_GRC_ROLE_PERMISSION_SETS");
            builder.HasKey(pt => new { pt.RoleId, pt.PermissionSetId });
            builder.Property(pt => pt.RoleId).HasColumnName("Role_Id").IsRequired();
            builder.Property(pt => pt.PermissionSetId).HasColumnName("Permission_Set_Id").IsRequired();
            builder.HasOne(pt => pt.Role).WithMany(p => p.PermissionSets).HasForeignKey(pt => pt.RoleId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pt => pt.PermissionSet).WithMany(g => g.Roles).HasForeignKey(pt => pt.PermissionSetId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
