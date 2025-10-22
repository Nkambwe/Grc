using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations
{
    public class ReturnSubmissionEntityConfiguration {
        public static void Configure(EntityTypeBuilder<ReturnSubmission> builder) {
            builder.ToTable("TBL_GRC_RETURN_SUBMISSION");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Description).HasColumnName("return_description").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(t => t.Deadline).HasColumnName("deadline").IsRequired(false);
            builder.Property(t => t.Status).HasColumnName("return_status").HasColumnType("NVARCHAR(50)").IsRequired(true);
            builder.Property(t => t.IsDeleted).HasColumnName("is_deleted");
            builder.Property(t => t.SubmissionDate).HasColumnName("submission_date").IsRequired();
            builder.Property(t => t.FilePath).HasColumnName("file_path").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(t => t.SubmittedBy).HasColumnName("submitted_by").HasColumnType("NVARCHAR(50)").IsRequired(true);
            builder.Property(t => t.Comments).HasColumnName("return_comments").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(t => t.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(t => t.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(t => t.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(t => t.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.Property(t => t.ReturnId).HasColumnName("return_id").IsRequired();
            builder.HasOne(t => t.RegulatoryReturn).WithMany(o => o.Submissions).HasForeignKey(s => s.ReturnId);
        }

    }
}
