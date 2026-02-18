using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {

    public class ProcessWorkflowEntityConfiguration {

        public static void Configure(EntityTypeBuilder<ProcessWorkflow> builder) {
            builder.ToTable("TBL_GRC_WORKFLOW");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id");
            builder.Property(p => p.Status).HasColumnName("workflow_status").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(p => p.ProcessId).HasColumnName("process_id");
            builder.Property(p => p.StepId).HasColumnName("step_id");
            builder.Property(p => p.IsDeleted).HasColumnName("is_deleted");
            builder.Property(p => p.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(p => p.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(p => p.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(p => p.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(p => p.Process).WithMany(t => t.Workflows).HasForeignKey(t => t.ProcessId);
            builder.HasOne(p => p.Step).WithMany(t => t.Workflows).HasForeignKey(t => t.StepId);
        }

    }
}
