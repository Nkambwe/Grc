using Grc.Middleware.Api.Data.Entities.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class ProcessTypeEntityConfiguration {

        public static void Configure(EntityTypeBuilder<ProcessType> builder)
        {
            builder.ToTable("TBL_GRC_PROCESS_TYPE");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.TypeName).HasColumnName("type_name").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(t => t.Description).HasColumnName("type_description").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(t => t.IsDeleted).HasColumnName("is_deleted");
            builder.Property(t => t.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(t => t.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(t => t.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(t => t.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasMany(t => t.Processes).WithOne(t => t.ProcessType).HasForeignKey(p => p.TypeId);
        }
    }
}
