using Grc.Middleware.Api.Data.Entities.Compliance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations
{
    public class RegulatoryCategoryEntityConfiguration
    {
        public static void Configure(EntityTypeBuilder<RegulatoryCategory> builder)
        {
            builder.ToTable("TBL_GRC_STATUTORY_CAT");
            builder.HasKey(f => f.Id);
            builder.Property(f => f.CategoryName).HasColumnName("category_name").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(f => f.IsDeleted).HasColumnName("is_deleted");
            builder.Property(f => f.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(f => f.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(f => f.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(f => f.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false); 
            builder.HasMany(f => f.Regulations).WithOne(u => u.Category).HasForeignKey(u => u.CategoryId);
        }
    }
}
