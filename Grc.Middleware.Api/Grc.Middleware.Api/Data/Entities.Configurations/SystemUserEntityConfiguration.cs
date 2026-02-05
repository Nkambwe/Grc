using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Grc.Middleware.Api.Data.Entities.System;

namespace Grc.Middleware.Api.Data.Entities.Configurations {

    public class SystemUserEntityConfiguration { 
        
        public static void Configure(EntityTypeBuilder<SystemUser> builder) {
            builder.ToTable("TBL_GRC_SYSTEM_USERS");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.PFNumber).HasColumnName("Pf_No").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(u => u.FirstName).HasColumnName("FirstName").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(u => u.LastName).HasColumnName("LastName").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(u => u.MiddleName).HasColumnName("OtherName").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(u => u.PhoneNumber).HasColumnName("Phone").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(u => u.EmailAddress).HasColumnName("Email").HasColumnType("VARCHAR(MAX)").IsRequired();
            builder.Property(u => u.Username).HasColumnName("Username").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(u => u.PasswordHash).HasColumnName("PasswordHash").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(u => u.RoleId).HasColumnName("Role_id").IsRequired();
            builder.Property(u => u.DepartmentId).HasColumnName("Department_id").IsRequired();
            builder.Property(u => u.BranchSolId).HasColumnName("Sol_id").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(u => u.DepartmentUnit).HasColumnName("Unit_Code").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(u => u.IsActive).HasColumnName("Is_active");
            builder.Property(u => u.IsApproved).HasColumnName("Is_approved").IsRequired(false);
            builder.Property(u => u.IsVerified).HasColumnName("Is_verified").IsRequired(false);
            builder.Property(u => u.IsLoggedIn).HasColumnName("Is_loggedin");
            builder.Property(u => u.LastLoginDate).HasColumnName("Last_login_date").IsRequired(false);
            builder.Property(u => u.IsDeleted).HasColumnName("Is_deleted");
            builder.Property(u => u.CreatedOn).HasColumnName("Created_on").IsRequired();
            builder.Property(u => u.LastPasswordChange).HasColumnName("Last_Password_Change").IsRequired(false);
            builder.Property(u => u.CreatedBy).HasColumnName("Created_by").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(u => u.LastModifiedOn).HasColumnName("Modified_on").IsRequired(false);
            builder.Property(u => u.LastModifiedBy).HasColumnName("Modified_by").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.HasOne(u => u.Role).WithMany(r => r.Users).HasForeignKey(u => u.RoleId);
            builder.HasOne(u => u.Department).WithMany(d => d.Users).HasForeignKey(d => d.DepartmentId);
            builder.HasMany(u => u.ActivityLogs).WithOne(a => a.User).HasForeignKey(a => a.UserId);
        }
    }
}
