using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Grc.Middleware.Api.Data.Entities.System;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class SystemUserEntityConfiguration { 
        
        public static void Configure(EntityTypeBuilder<SystemUser> builder) {
            builder.ToTable("TBL_GRC_SYSTEM_USERS");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.PersonalFileNumber).HasColumnName("Pf_No").HasColumnType("VARCHAR(MAX)").IsFixedLength().IsRequired();
            builder.Property(u => u.FirstName).HasColumnName("FirstName").HasColumnType("VARCHAR(MAX)").IsRequired();
            builder.Property(u => u.LastName).HasColumnName("LastName").HasColumnType("VARCHAR(MAX)").IsRequired();
            builder.Property(u => u.OtherName).HasColumnName("OtherName").HasColumnType("VARCHAR(MAX)").IsRequired(false);
            builder.Property(u => u.PhoneNumber).HasColumnName("Phone").HasColumnType("VARCHAR(MAX)").IsFixedLength().IsRequired(false);
            builder.Property(u => u.EmailAddress).HasColumnName("Email").HasColumnType("VARCHAR(MAX)").IsRequired();
            builder.Property(u => u.Username).HasColumnName("Username").HasColumnType("VARCHAR(MAX)").IsRequired();
            builder.Property(u => u.PasswordHash).HasColumnName("PasswordHash").HasColumnType("VARCHAR(MAX)").IsRequired();
            builder.Property(u => u.RoleId).HasColumnName("Role_id").IsRequired();
            builder.Property(u => u.DepartmentId).HasColumnName("Department_id").IsRequired();
            builder.Property(u => u.BranchSolId).HasColumnName("Sol_id").HasMaxLength(10).IsFixedLength().IsRequired();
            builder.Property(u => u.DepartmentUnit).HasColumnName("Unit_Code").HasMaxLength(10).IsFixedLength().IsRequired();
            builder.Property(u => u.IsActive).HasColumnName("Is_active");
            builder.Property(u => u.IsApproved).HasColumnName("Is_approved").IsRequired(false);
            builder.Property(u => u.IsVerified).HasColumnName("Is_verified").IsRequired(false);
            builder.Property(u => u.IsLoggedIn).HasColumnName("Is_loggedin");
            builder.Property(u => u.LastLoginDate).HasColumnName("Last_login_date").IsRequired(false);
            builder.Property(u => u.IsDeleted).HasColumnName("Is_deleted");
            builder.Property(u => u.CreatedOn).HasColumnName("Created_on").IsRequired();
            builder.Property(u => u.LastPasswordChange).HasColumnName("Last_Password_Change").IsRequired(false);
            builder.Property(u => u.CreatedBy).HasColumnName("Created_by").HasMaxLength(10).IsFixedLength().IsRequired();
            builder.Property(u => u.LastModifiedOn).HasColumnName("Modified_on").IsRequired(false);
            builder.Property(u => u.LastModifiedBy).HasColumnName("Modified_by").HasMaxLength(10).IsFixedLength().IsRequired(false);
            builder.HasOne(u => u.Role).WithMany(r => r.Users).HasForeignKey(u => u.RoleId);
            builder.HasOne(u => u.Department).WithMany(d => d.Users).HasForeignKey(u => u.DepartmentId);
        }
    }
}
