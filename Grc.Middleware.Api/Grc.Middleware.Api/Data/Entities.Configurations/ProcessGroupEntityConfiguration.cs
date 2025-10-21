using Grc.Middleware.Api.Data.Entities.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class ProcessGroupEntityConfiguration
    {

        public static void Configure(EntityTypeBuilder<ProcessGroup> builder)
        {
            builder.ToTable("TBL_GRC_PROCESS_GROUP");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.GroupName).HasColumnName("group_name").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(t => t.Description).HasColumnName("group_description").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(t => t.IsDeleted).HasColumnName("is_deleted");
            builder.Property(t => t.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(t => t.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(t => t.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(t => t.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasMany(t => t.Processes).WithOne(t => t.Group).HasForeignKey(p => p.GroupId);
        }
    }
}
