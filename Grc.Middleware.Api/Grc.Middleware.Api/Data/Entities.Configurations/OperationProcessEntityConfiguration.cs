using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations
{
    public class OperationProcessEntityConfiguration
    {

        public static void Configure(EntityTypeBuilder<OperationProcess> builder)
        {
            builder.ToTable("TBL_GRC_OPERATION_PROCESS");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.ProcessName).HasColumnName("process_name").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(p => p.Description).HasColumnName("process_description").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(p => p.CurrentVersion).HasColumnName("current_version").HasColumnType("NVARCHAR(100)").IsRequired();
            builder.Property(p => p.EffectiveDate).HasColumnName("effective_date").IsRequired(false);
            builder.Property(p => p.LastUpdated).HasColumnName("last_updated").IsRequired(false);
            builder.Property(p => p.OriginalOnFile).HasColumnName("on_file");
            builder.Property(p => p.FileName).HasColumnName("file_path").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(p => p.ProcessStatus).HasColumnName("process_status").HasColumnType("NVARCHAR(50)").IsRequired().HasDefaultValue("Draft");
            builder.Property(p => p.ApprovalStatus).HasColumnName("approval_status").HasColumnType("NVARCHAR(50)").IsRequired().HasDefaultValue("Draft");
            builder.Property(p => p.ApprovalComment).HasColumnName("approval_comment").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(p => p.Comments).HasColumnName("process_comment").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(p => p.ReasonOnhold).HasColumnName("onhold_reason").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(p => p.UnitId).HasColumnName("unit_id");
            builder.Property(p => p.OwnerId).HasColumnName("owner_id");
            builder.Property(p => p.ResponsibilityId).HasColumnName("response_id");
            builder.Property(p => p.TypeId).HasColumnName("type_id");
            builder.Property(p => p.IsDeleted).HasColumnName("is_deleted");
            builder.Property(p => p.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(p => p.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(p => p.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(p => p.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(p => p.ProcessType).WithMany(t => t.Processes).HasForeignKey(t => t.TypeId);
            builder.HasMany(p => p.Tasks).WithOne(t => t.Process).HasForeignKey(t => t.ProcessId);
            builder.HasOne(p => p.Owner).WithMany(p => p.OwnerProcesses).HasForeignKey(p => p.OwnerId);
            builder.HasOne(p => p.Responsible).WithMany(p => p.AssigneeProcesses).HasForeignKey(p => p.ResponsibilityId);
            builder.HasMany(p => p.Groups).WithOne(g => g.Process).HasForeignKey(a => a.ProcessId);
            builder.HasMany(p => p.Activities).WithOne(a => a.Process).HasForeignKey(a => a.ProcessId);
        }

    }
}
