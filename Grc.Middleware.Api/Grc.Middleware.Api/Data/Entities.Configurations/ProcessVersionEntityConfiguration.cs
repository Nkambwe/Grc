using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class ProcessVersionEntityConfiguration {

        public static void Configure(EntityTypeBuilder<ProcessVersion> builder) {
            builder.ToTable("TBL_GRC_PROCESS_VERSION");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id");
            builder.Property(p => p.VersionNumber).HasColumnName("version_number");
            builder.Property(p => p.Content).HasColumnName("content_details").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(p => p.ProcessId).HasColumnName("process_id");
            builder.Property(p => p.IsDeleted).HasColumnName("is_deleted");
            builder.Property(p => p.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(p => p.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(p => p.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(p => p.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(p => p.Process).WithMany(t => t.Versions).HasForeignKey(t => t.ProcessId);
        }

    }
}
