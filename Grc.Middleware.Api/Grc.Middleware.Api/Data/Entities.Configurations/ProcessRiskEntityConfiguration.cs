using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {

    public class ProcessRiskEntityConfiguration {

        public static void Configure(EntityTypeBuilder<ProcessRisk> builder) {
            builder.ToTable("TBL_GRC_PROCESS_RISK");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id");
            builder.Property(p => p.Description).HasColumnName("risk_description").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(p => p.Control).HasColumnName("control").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(p => p.Impact).HasColumnName("risk_impact");
            builder.Property(p => p.Liklyhood).HasColumnName("liklyhood");
            builder.Property(p => p.Score).HasColumnName("risk_score");
            builder.Property(p => p.ProcessId).HasColumnName("process_id");
            builder.Property(p => p.IsDeleted).HasColumnName("is_deleted");
            builder.Property(p => p.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(p => p.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(p => p.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(p => p.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(p => p.Process).WithMany(t => t.RiskAssessments).HasForeignKey(t => t.ProcessId);

        }
    }
}
