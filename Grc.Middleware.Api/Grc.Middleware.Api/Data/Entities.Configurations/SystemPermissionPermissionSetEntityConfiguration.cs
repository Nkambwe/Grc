using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class SystemPermissionPermissionSetEntityConfiguration
    {
        public static void Configure(EntityTypeBuilder<SystemPermissionPermissionSet> builder)
        {
            builder.ToTable("TBL_GRC_PERMISSION_PERMISSION_SET");
            builder.HasKey(pt => new { pt.PermissionId, pt.PermissionSetId });
            builder.Property(pt => pt.PermissionId).HasColumnName("Permission_Id").IsRequired();
            builder.Property(pt => pt.PermissionSetId).HasColumnName("Permission_Set_Id").IsRequired();
            builder.HasOne(pt => pt.Permission).WithMany(p => p.PermissionSets).HasForeignKey(pt => pt.PermissionId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pt => pt.PermissionSet).WithMany(g => g.Permissions).HasForeignKey(pt => pt.PermissionSetId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
