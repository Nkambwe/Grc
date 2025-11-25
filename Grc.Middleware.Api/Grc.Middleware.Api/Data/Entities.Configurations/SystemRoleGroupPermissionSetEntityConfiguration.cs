using Grc.Middleware.Api.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class SystemRoleGroupPermissionSetEntityConfiguration
    {
        public static void Configure(EntityTypeBuilder<SystemRoleGroupPermissionSet> builder)
        {
            builder.ToTable("TBL_GRC_ROLE_GROUP_PERMISSION_SETS");
            builder.HasKey(pt => new { pt.RoleGroupId, pt.PermissionSetId });
            builder.Property(pt => pt.RoleGroupId).HasColumnName("Role_Group_Id").IsRequired();
            builder.Property(pt => pt.PermissionSetId).HasColumnName("Permission_Set_Id").IsRequired();
            builder.HasOne(pt => pt.RoleGroup).WithMany(p => p.PermissionSets).HasForeignKey(pt => pt.RoleGroupId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pt => pt.PermissionSet).WithMany(g => g.RoleGroups).HasForeignKey(pt => pt.PermissionSetId).OnDelete(DeleteBehavior.Cascade);
        }
    }

}
