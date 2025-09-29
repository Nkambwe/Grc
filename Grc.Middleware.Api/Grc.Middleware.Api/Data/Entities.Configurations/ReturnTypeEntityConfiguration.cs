using Grc.Middleware.Api.Data.Entities.Compliance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations
{
    public class ReturnTypeEntityConfiguration
    {
        public static void Configure(EntityTypeBuilder<ReturnType> builder)
        {
            builder.ToTable("TBL_GRC_Return_Type");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.TypeName).HasColumnName("type_name").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(t => t.IsDeleted).HasColumnName("is_deleted");
            builder.Property(t => t.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(t => t.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(t => t.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(t => t.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasMany(t => t.Returns).WithOne(s => s.ReturnType).HasForeignKey(s => s.TypeId);
        }
    }
}
