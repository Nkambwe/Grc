using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class SubmissionNotificationEntityConfiguration
    {

        public static void Configure(EntityTypeBuilder<SubmissionNotification> builder)
        {
            builder.ToTable("TBL_GRC_SUBMISSION_REMINDERS");
            builder.HasKey(n => n.Id);
            builder.Property(n => n.SentTo).HasColumnName("sent_to").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(n => n.CarbonCopy).HasColumnName("carbon_copy").HasColumnType("NVARCHAR(200)").IsRequired(false);
            builder.Property(n => n.BlindCopy).HasColumnName("blind_copy").HasColumnType("NVARCHAR(200)").IsRequired(false);
            builder.Property(n => n.Message).HasColumnName("notice_message").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(n => n.SubmissionId).HasColumnName("submission_id");
            builder.Property(n => n.IsDeleted).HasColumnName("is_deleted");
            builder.Property(n => n.SentOn).HasColumnName("sent_on").IsRequired();
            builder.Property(n => n.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(n => n.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(n => n.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(n => n.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(n => n.Submission).WithMany(s => s.Notifications).HasForeignKey(n => n.SubmissionId);
        }
    }
}
