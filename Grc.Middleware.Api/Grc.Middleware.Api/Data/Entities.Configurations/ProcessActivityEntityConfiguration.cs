using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class ProcessActivityEntityConfiguration {

        public static void Configure(EntityTypeBuilder<ProcessActivity> builder) {
            builder.ToTable("TBL_GRC_PROCESS_ACTIVITY");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Activity).HasColumnName("activity_name").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(a => a.Description).HasColumnName("activity_description").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(a => a.IsDeleted).HasColumnName("is_deleted");
            builder.Property(a => a.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(a => a.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(a => a.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(a => a.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.Property(a => a.ProcessId).HasColumnName("process_id");
            builder.HasOne(a => a.Process).WithMany(t => t.Activities).HasForeignKey(p => p.ProcessId);
        }
    }
}
