using Grc.Middleware.Api.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class MailSettingEntityConfiguration {
        public static void Configure(EntityTypeBuilder<MailSettings> builder) {
            builder.ToTable("TBL_GRC_MAILSETTINGS");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).HasColumnName("id");
            builder.Property(s => s.SystemEmail).HasColumnName("system_mail").HasColumnType("NVARCHAR(200)").IsRequired();
            builder.Property(s => s.SystemPassword).HasColumnName("cc_mails").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(s => s.NetworkPort).HasColumnName("nwt_port").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(s => s.Sender).HasColumnName("mail_sender").HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(s => s.CompanyId).HasColumnName("company_id").IsRequired();
            builder.Property(s => s.IsDeleted).HasColumnName("is_deleted");
            builder.Property(s => s.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(s => s.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(s => s.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(s => s.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(s => s.Company).WithMany(m => m.MailSettings).HasForeignKey(m => m.CompanyId);
        }
    }
}
