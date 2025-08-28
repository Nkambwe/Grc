using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Grc.Middleware.Api.Data.Entities.Org;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class CompanyEntityConfiguration {

        public static void Configure(EntityTypeBuilder<Company> builder) {
            builder.ToTable("TBL_GRC_COMPANY");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CompanyName).HasColumnName("Company_name").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(c => c.ShortName).HasColumnName("Alias").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(c => c.RegistrationNumber).HasColumnName("Reg_number").HasColumnType("NVARCHAR(MAX)");
            builder.Property(c => c.SystemLanguage).HasColumnName("Language").HasColumnType("NVARCHAR(MAX)");
            builder.Property(c => c.IsDeleted).HasColumnName("Is_Deleted");
            builder.Property(c => c.CreatedOn).HasColumnName("Created_on").IsRequired();
            builder.Property(c => c.CreatedBy).HasColumnName("Created_by").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(c => c.LastModifiedOn).HasColumnName("Modefied_on").IsRequired(false);
            builder.Property(c => c.LastModifiedBy).HasColumnName("Modified_by").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.HasMany(c => c.ActivitySettings).WithOne(a => a.Company).HasForeignKey(a => a.CompanyId);
            builder.HasMany(c => c.Branches).WithOne(b => b.Company).HasForeignKey(b => b.CompanyId);
        }
    }
}
