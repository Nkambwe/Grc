using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Grc.Middleware.Api.Data.Entities.Org;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class DepartmentUnitEntityConfiguration {

        public static void Configure(EntityTypeBuilder<DepartmentUnit> builder) {
            builder.ToTable("TBL_GRC_DEPARTMENT_UNIT");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.UnitCode).HasColumnName("Unit_code").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(u => u.UnitName).HasColumnName("Unit_name").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(u => u.DepartmentId).HasColumnName("Department_id");
            builder.Property(u => u.IsDeleted).HasColumnName("Is_deleted");
            builder.Property(u => u.CreatedOn).HasColumnName("Created_on").IsRequired();
            builder.Property(u => u.CreatedBy).HasColumnName("Created_by").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(u => u.LastModifiedOn).HasColumnName("Modified_on").IsRequired(false);
            builder.Property(u => u.LastModifiedBy).HasColumnName("Modified_by").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.HasOne(u => u.Department).WithMany(d => d.Units).HasForeignKey(d => d.DepartmentId);
            
        }

    }
}
