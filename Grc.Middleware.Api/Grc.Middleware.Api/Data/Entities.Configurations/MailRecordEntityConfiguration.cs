using Grc.Middleware.Api.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class MailRecordEntityConfiguration {
        public static void Configure(EntityTypeBuilder<MailRecord> builder) {
            builder.ToTable("TBL_GRC_MAILRECORD");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).HasColumnName("id");
            builder.Property(m => m.DocumentId).HasColumnName("document_id").IsRequired(false);
            builder.Property(m => m.ReturnId).HasColumnName("return_id").IsRequired(false);
            builder.Property(m => m.SubmissionId).HasColumnName("submission_id").IsRequired(false);
            builder.Property(m => m.CircularId).HasColumnName("circular_id").IsRequired(false);
            builder.Property(m => m.SentToEmail).HasColumnName("sent_to").HasColumnType("NVARCHAR(200)").IsRequired();
            builder.Property(m => m.CCMail).HasColumnName("cc_mails").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(m => m.Subject).HasColumnName("mail_subject").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(m => m.Mail).HasColumnName("mail_body").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(m => m.ApprovalId).HasColumnName("approval_id").IsRequired(false);
            builder.Property(m => m.IsDeleted).HasColumnName("is_deleted");
            builder.Property(m => m.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(m => m.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(m => m.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(m => m.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(m => m.Approval).WithMany(a => a.MailRecords).HasForeignKey(m  => m.ApprovalId);
        }
    }
}
