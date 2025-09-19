using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Grc.Middleware.Api.Data.Entities.System;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class SystemRoleGroupEntityConfiguration {

        public static void Configure(EntityTypeBuilder<SystemRoleGroup> builder) {
            builder.ToTable("TBL_GRC_ROLE_GROUP");
            builder.HasKey(g => g.Id);
            builder.Property(g => g.GroupName).HasColumnName("Group_name").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(g => g.Description).HasColumnName("Group_description").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(g => g.Scope).HasColumnName("Group_scope").IsRequired();
            builder.Property(g => g.Department).HasColumnName("Department").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(g => g.Type).HasColumnName("Group_type").IsRequired();
            builder.Property(g => g.IsApproved).HasColumnName("Is_approved").IsRequired(false);
            builder.Property(g => g.IsVerified).HasColumnName("Is_verified").IsRequired(false);
            builder.Property(g => g.IsDeleted).HasColumnName("Is_deleted");
            builder.Property(g => g.CreatedOn).HasColumnName("Created_on").IsRequired();
            builder.Property(g => g.CreatedBy).HasColumnName("Created_by").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(g => g.LastModifiedOn).HasColumnName("Modified_on").IsRequired(false);
            builder.Property(g => g.LastModifiedBy).HasColumnName("Modified_by").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.HasMany(g => g.Roles).WithOne(r => r.Group).HasForeignKey(r => r.GroupId);
            
        }
    }
}
