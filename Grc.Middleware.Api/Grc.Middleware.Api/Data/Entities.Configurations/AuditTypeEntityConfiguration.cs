using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {

    public class AuditTypeEntityConfiguration {

        public static void Configure(EntityTypeBuilder<AuditType> builder) {
            builder.ToTable("TBL_GRC_AUDIT_TYPE");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.TypeCode).HasColumnName("type_code").HasColumnType("NVARCHAR(10)").IsRequired(true);
            builder.Property(a => a.TypeName).HasColumnName("type_name").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(a => a.Description).HasColumnName("description").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(a => a.IsDeleted).HasColumnName("is_deleted");
            builder.Property(a => a.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(a => a.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(a => a.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(a => a.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasMany(a => a.Audits).WithOne(r => r.AuditType).HasForeignKey(a => a.AuditTypeId);
        }
    }

}
