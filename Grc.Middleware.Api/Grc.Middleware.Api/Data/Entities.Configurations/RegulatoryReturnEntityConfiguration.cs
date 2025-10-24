using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations
{
    public class RegulatoryReturnEntityConfiguration
    {
        public static void Configure(EntityTypeBuilder<RegulatoryReturn> builder)
        {
            builder.ToTable("TBL_GRC_RETURNS");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.ReturnName).HasColumnName("return_name").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(r => r.Description).HasColumnName("return_descr").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(r => r.RequirementDetails).HasColumnName("return_details").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(r => r.TypeId).HasColumnName("type_id");
            builder.Property(r => r.FrequencyId).HasColumnName("freq_id");
            builder.Property(r => r.FrequencyInfo).HasColumnName("freq_descr").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(r => r.ArticleId).HasColumnName("act_id");
            builder.Property(r => r.ResponsibilityId).HasColumnName("resp_id");
            builder.Property(r => r.ComplianceGap).HasColumnName("comp_gap").HasColumnType("NVARCHAR(10)").IsRequired(false);
            builder.Property(r => r.Comments).HasColumnName("return_comment").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(r => r.IsDeleted).HasColumnName("is_deleted");
            builder.Property(r => r.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(r => r.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(r => r.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(r => r.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(r => r.ReturnType).WithMany(t => t.Returns).HasForeignKey(r => r.TypeId);
            builder.HasOne(r => r.Article).WithMany(a => a.Returns).HasForeignKey(a => a.ArticleId);
            builder.HasOne(r => r.Responsibility).WithMany(r => r.Returns).HasForeignKey(r => r.ResponsibilityId);
            builder.HasOne(r => r.Frequency).WithMany(f => f.Returns).HasForeignKey(r => r.FrequencyId);
        }
    }
}
