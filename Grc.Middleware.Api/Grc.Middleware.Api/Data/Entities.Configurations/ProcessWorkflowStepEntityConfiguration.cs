using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class ProcessWorkflowStepEntityConfiguration {

        public static void Configure(EntityTypeBuilder<ProcessWorkflowStep> builder) {
            builder.ToTable("TBL_GRC_WORKFLOW_STEP");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id");
            builder.Property(p => p.StepName).HasColumnName("step_name").HasColumnType("NVARCHAR(200)").IsRequired();
            builder.Property(p => p.Sequence).HasColumnName("step_squence");
            builder.Property(p => p.RequiredRole).HasColumnName("role_required").HasColumnType("NVARCHAR(100)").IsRequired();
            builder.Property(p => p.SlaHours).HasColumnName("sla_hours");
            builder.Property(p => p.IsDeleted).HasColumnName("is_deleted");
            builder.Property(p => p.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(p => p.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(p => p.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(p => p.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasMany(p => p.Workflows).WithOne(t => t.Step).HasForeignKey(t => t.StepId);
        }

    }
}
