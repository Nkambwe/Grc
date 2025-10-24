using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Grc.Middleware.Api.Data.Entities.Org;

namespace Grc.Middleware.Api.Data.Entities.Configurations {

    public class DepartmentEntityConfiguration {

        public static void Configure(EntityTypeBuilder<Department> builder) {
            builder.ToTable("TBL_GRC_DEPARTMENT");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.DepartmentName).HasColumnName("Department_name").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(d => d.DepartmentCode).HasColumnName("Department_code").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(d => d.Alias).HasColumnName("Department_alias").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(d => d.BranchId).HasColumnName("Branch_id");
            builder.Property(d => d.IsDeleted).HasColumnName("is_deleted");
            builder.Property(d => d.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(d => d.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(d => d.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(d => d.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.HasOne(d => d.Branch).WithMany(c => c.Departments).HasForeignKey(b => b.BranchId);
            
        }
    }
}
