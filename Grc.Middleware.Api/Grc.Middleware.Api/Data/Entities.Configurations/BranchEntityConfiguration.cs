using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Grc.Middleware.Api.Data.Entities.Org;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class BranchEntityConfiguration {

        public static void Configure(EntityTypeBuilder<Branch> builder) {
            builder.ToTable("TBL_GRC_BRANCH");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.BranchName).HasColumnName("branch_name").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(b => b.SolId).HasColumnName("Sol_id").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(b => b.CompanyId).HasColumnName("company_id");
            builder.Property(b => b.IsDeleted).HasColumnName("is_deleted");
            builder.Property(b => b.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(b => b.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(b => b.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(b => b.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.HasOne(b => b.Company).WithMany(c => c.Branches).HasForeignKey(b => b.CompanyId);
            builder.HasMany(b => b.Departments).WithOne(d => d.Branch).HasForeignKey(d => d.BranchId);
        }
    }
}
