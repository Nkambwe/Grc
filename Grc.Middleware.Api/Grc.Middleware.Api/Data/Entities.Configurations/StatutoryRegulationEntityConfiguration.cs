using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class StatutoryRegulationEntityConfiguration
{
    public static void Configure(EntityTypeBuilder<StatutoryRegulation> builder)
    {
        builder.ToTable("TBL_GRC_STATUTORY_REGULATION");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Code).HasColumnName("regulatory_code").HasColumnType("NVARCHAR(20)").IsRequired(true);
        builder.Property(s => s.RegulatoryName).HasColumnName("regulatory_name").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
        builder.Property(s => s.TypeId).HasColumnName("regulatory_type_id").IsRequired(true);
        builder.Property(s => s.AuthorityId).HasColumnName("regulatory_auth_id").IsRequired(true);
        builder.Property(s => s.CategoryId).HasColumnName("regulatory_cat_id").IsRequired(true); 
        builder.Property(s => s.IsDeleted).HasColumnName("is_deleted");
        builder.Property(s => s.CreatedOn).HasColumnName("created_on").IsRequired();
        builder.Property(s => s.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
        builder.Property(s => s.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
        builder.Property(s => s.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
        builder.HasOne(s => s.Authority).WithMany(u => u.Regulations).HasForeignKey(u => u.AuthorityId);
        builder.HasMany(s => s.Articles).WithOne(a => a.Statute).HasForeignKey(r => r.StatuteId);
    }
}
