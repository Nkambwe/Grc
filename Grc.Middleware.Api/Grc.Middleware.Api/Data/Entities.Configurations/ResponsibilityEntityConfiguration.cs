using Grc.Middleware.Api.Data.Entities.Compliance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations
{
    public class ResponsibilityEntityConfiguration
    {
        public static void Configure(EntityTypeBuilder<Responsibility> builder)
        {
            builder.ToTable("TBL_GRC_RETURN_RESPONSE");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.ContactName).HasColumnName("resp_name").HasColumnType("NVARCHAR(200)").IsRequired(true);
            builder.Property(c => c.ContactPhone).HasColumnName("resp_phone").HasColumnType("NVARCHAR(12)").IsRequired(true);
            builder.Property(c => c.ContactEmail).HasColumnName("resp_email").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(c => c.ContactPosition).HasColumnName("resp_position").HasColumnType("NVARCHAR(180)").IsRequired(true);
            builder.Property(c => c.Description).HasColumnName("resp_comments").HasColumnType("NVARCHAR(180)").IsRequired(true);
            builder.Property(c => c.IsDeleted).HasColumnName("is_deleted");
            builder.Property(c => c.DepartmentId).HasColumnName("depart_id");
            builder.Property(c => c.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(c => c.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(c => c.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(c => c.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false); 
            builder.HasOne(c => c.Department).WithMany(d => d.Responsibilities).HasForeignKey(c => c.DepartmentId);
            builder.HasMany(f => f.Returns).WithOne(r => r.Responsibility).HasForeignKey(r => r.ResponsibilityId);
        }
    }
}
