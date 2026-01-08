using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations
{
    public class ReturnEntityConfiguration
    {
        public static void Configure(EntityTypeBuilder<ReturnReport> builder)
        {
            builder.ToTable("TBL_GRC_RETURNS");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasColumnName("id"); 
            builder.Property(r => r.ReturnName).HasColumnName("return_name").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(r => r.Risk).HasColumnName("return_risk").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(r => r.TypeId).HasColumnName("type_id");
            builder.Property(r => r.FrequencyId).HasColumnName("freq_id");
            builder.Property(r => r.ArticleId).HasColumnName("act_id");
            builder.Property(r => r.DepartmentId).HasColumnName("dept_id");
            builder.Property(r => r.AuthorityId).HasColumnName("auth_id");
            builder.Property(r => r.Comments).HasColumnName("comment").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(r => r.IsDeleted).HasColumnName("is_deleted");
            builder.Property(r => r.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(r => r.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(r => r.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(r => r.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(r => r.ReturnType).WithMany(t => t.Returns).HasForeignKey(r => r.TypeId);
            builder.HasOne(r => r.Article).WithMany(a => a.Returns).HasForeignKey(a => a.ArticleId);
            builder.HasOne(r => r.Authority).WithMany(a => a.Returns).HasForeignKey(a => a.AuthorityId);
            builder.HasOne(r => r.Department).WithMany(r => r.Returns).HasForeignKey(r => r.DepartmentId);
            builder.HasOne(r => r.Frequency).WithMany(f => f.Returns).HasForeignKey(r => r.FrequencyId);
        }
    }
}
