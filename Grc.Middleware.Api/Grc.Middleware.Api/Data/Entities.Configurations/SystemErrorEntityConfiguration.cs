using Grc.Middleware.Api.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class SystemErrorEntityConfiguration {
         public static void Configure(EntityTypeBuilder<SystemError> builder) {
            builder.ToTable("TBL_GRC_BUGS");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.ErrorMessage).HasColumnName("error_message").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(b => b.ErrorSource).HasColumnName("error_source").HasColumnType("NVARCHAR(MAX)");
            builder.Property(b => b.StackTrace).HasColumnName("stack_trace").HasColumnType("NVARCHAR(MAX)");
            builder.Property(b => b.Severity).HasColumnName("severity").HasColumnType("NVARCHAR(100)");
            builder.Property(b => b.IsUserReported).HasColumnName("is_userreported");
            builder.Property(b => b.Assigned).HasColumnName("is_assigned");
            builder.Property(b => b.AssignedTo).HasColumnName("assugned_to").HasColumnType("NVARCHAR(MAX)");
            builder.Property(b => b.FixStatus).HasColumnName("fix_status").HasColumnType("NVARCHAR(100)");
            builder.Property(b => b.CompanyId).HasColumnName("company_id");
            builder.Property(b => b.IsDeleted).HasColumnName("is_deleted");
            builder.Property(b => b.ReportedOn).HasColumnName("reported_on").IsRequired();
            builder.Property(b => b.ClosedOn).HasColumnName("closed_on").IsRequired(false);
            builder.Property(b => b.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(b => b.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(b => b.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(b => b.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.HasOne(b => b.Company).WithMany(c => c.SystemErrors).HasForeignKey(b => b.CompanyId);
        }
    }
}
