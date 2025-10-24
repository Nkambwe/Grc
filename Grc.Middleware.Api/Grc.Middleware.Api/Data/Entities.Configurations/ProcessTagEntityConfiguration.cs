using Grc.Middleware.Api.Data.Entities.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class ProcessTagEntityConfiguration
    {

        public static void Configure(EntityTypeBuilder<ProcessTag> builder)
        {
            builder.ToTable("TBL_GRC_PROCESS_TAG");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.TagName).HasColumnName("tag_name").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(t => t.Description).HasColumnName("tag_description").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(t => t.IsDeleted).HasColumnName("is_deleted");
            builder.Property(t => t.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(t => t.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(t => t.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(t => t.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            //builder.HasMany(t => t.Processes).WithOne(t => t.Tag).HasForeignKey(p => p.TagId);
        }
    }
}
