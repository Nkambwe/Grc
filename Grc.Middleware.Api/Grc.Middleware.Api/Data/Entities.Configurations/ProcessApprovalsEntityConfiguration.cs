using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations
{
    public class ProcessApprovalsEntityConfiguration
    {
        public static void Configure(EntityTypeBuilder<ProcessApproval> builder)
        {
            builder.ToTable("TBL_GRC_PROCESS_APPROVAL");
            builder.HasKey(p => p.Id).HasName("id");
            builder.Property(p => p.HeadOfDepartmentStart).HasColumnName("hod_startdate").IsRequired(false);
            builder.Property(p => p.HeadOfDepartmentEnd).HasColumnName("hod_enddate").IsRequired(false);
            builder.Property(p => p.HeadOfDepartmentStatus).HasColumnName("hod_status").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.Property(p => p.HeadOfDepartmentComment).HasColumnName("hod_comment").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(p => p.RiskStart).HasColumnName("risk_startdate").IsRequired(false);
            builder.Property(p => p.RiskEnd).HasColumnName("risk_enddate").IsRequired(false);
            builder.Property(p => p.RiskStatus).HasColumnName("risk_status").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.Property(p => p.RiskComment).HasColumnName("risk_comment").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(p => p.ComplianceStart).HasColumnName("compliance_startdate").IsRequired(false);
            builder.Property(p => p.ComplianceEnd).HasColumnName("compliance_enddate").IsRequired(false);
            builder.Property(p => p.ComplianceStatus).HasColumnName("compliance_status").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.Property(p => p.ComplianceComment).HasColumnName("compliance_comment").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(p => p.BranchOperationsStatusStart).HasColumnName("brops_startdate").IsRequired(false);
            builder.Property(p => p.BranchOperationsStatusEnd).HasColumnName("brops_enddate").IsRequired(false);
            builder.Property(p => p.BranchOperationsStatus).HasColumnName("brops_status").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.Property(p => p.BranchManagerComment).HasColumnName("brops_comment").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(p => p.CreditStart).HasColumnName("credit_startdate").IsRequired(false);
            builder.Property(p => p.CreditEnd).HasColumnName("credit_enddate").IsRequired(false);
            builder.Property(p => p.CreditStatus).HasColumnName("credit_status").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.Property(p => p.CreditComment).HasColumnName("credit_comment").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(p => p.TreasuryStart).HasColumnName("treasury_startdate").IsRequired(false);
            builder.Property(p => p.TreasuryEnd).HasColumnName("treasury_enddate").IsRequired(false);
            builder.Property(p => p.TreasuryStatus).HasColumnName("treasury_status").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.Property(p => p.TreasuryComment).HasColumnName("treasury_comment").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(p => p.FintechStart).HasColumnName("fintech_startdate").IsRequired(false);
            builder.Property(p => p.FintechEnd).HasColumnName("fintech_enddate").IsRequired(false);
            builder.Property(p => p.FintechStatus).HasColumnName("fintech_status").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.Property(p => p.FintechComment).HasColumnName("fintech_comment").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(p => p.IsDeleted).HasColumnName("is_deleted");
            builder.Property(p => p.ProcessId).HasColumnName("process_id");
            builder.Property(p => p.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(p => p.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(p => p.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(p => p.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(p => p.Process).WithMany(p => p.Approvals).HasForeignKey(p => p.ProcessId);
        }
    }
}
