using Grc.Middleware.Api.Data.Entities.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AuthorityEntityConfiguration
{
    public static void Configure(EntityTypeBuilder<Authority> builder)
    {
        builder.ToTable("TBL_GRC_AUTHORITY");
        builder.HasKey(t => t.Id);
        builder.Property(a => a.AuthorityName).HasColumnName("auth_name").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
        builder.Property(a => a.AuthorityAlias).HasColumnName("auth_alias").HasColumnType("NVARCHAR(160)").IsRequired(true);
        builder.Property(a => a.IsDeleted).HasColumnName("is_deleted");
        builder.Property(a => a.CreatedOn).HasColumnName("created_on").IsRequired();
        builder.Property(a => a.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
        builder.Property(a => a.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
        builder.Property(a => a.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
        builder.HasMany(a => a.Regulations).WithOne(s => s.Authority).HasForeignKey(r => r.AuthorityId);
    }
}
