using Grc.Middleware.Api.Data.Entities.Compliance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations
{
    public class FrequencyEntityConfiguration
    {
        public static void Configure(EntityTypeBuilder<Frequency> builder)
        {
            builder.ToTable("TBL_GRC_RETURN_FREQ");
            builder.HasKey(f => f.Id);
            builder.Property(f => f.FrequencyName).HasColumnName("freq_name").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(f => f.IsDeleted).HasColumnName("is_deleted");
            builder.Property(f => f.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(f => f.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(f => f.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(f => f.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false); 
            builder.HasMany(f => f.Returns).WithOne(u => u.Frequency).HasForeignKey(u => u.FrequencyId);
        }
    }
}
