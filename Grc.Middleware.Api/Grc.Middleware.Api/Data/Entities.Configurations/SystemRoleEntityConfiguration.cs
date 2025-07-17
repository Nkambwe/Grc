using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Grc.Middleware.Api.Data.Entities.System;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class SystemRoleEntityConfiguration { 
        
        public static void Configure(EntityTypeBuilder<SystemRole> builder) {
            builder.ToTable("TBL_GRC_SYSTEM_ROLE");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.RoleName).HasColumnName("Role_name").HasMaxLength(250).IsRequired();
            builder.Property(r => r.Description).HasColumnName("Role_description").HasMaxLength(255).IsRequired();
            builder.Property(r => r.GroupId).HasColumnName("Group_Id").IsRequired();
            builder.Property(r => r.IsApproved).HasColumnName("Is_approved").IsRequired(false);
            builder.Property(r => r.IsVerified).HasColumnName("Is_verified").IsRequired(false);
            builder.Property(r => r.IsDeleted).HasColumnName("Is_deleted");
            builder.Property(r => r.CreatedOn).HasColumnName("Created_on").IsRequired();
            builder.Property(r => r.CreatedBy).HasColumnName("Created_by").HasMaxLength(10).IsFixedLength().IsRequired();
            builder.Property(r => r.LastModifiedOn).HasColumnName("Modified_on").IsRequired(false);
            builder.Property(r => r.LastModifiedBy).HasColumnName("Modified_by").HasMaxLength(10).IsFixedLength().IsRequired(false);
            builder.HasOne(r => r.Group).WithMany(g => g.Roles).HasForeignKey(r => r.GroupId);
            builder.HasMany(r => r.Users).WithOne(u => u.Role).HasForeignKey(u => u.RoleId);
        }
    }
}
