using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public partial class CircularEntityConfiguration {
    public class CircularIssueEntityConfiguration {
        public static void Configure(EntityTypeBuilder<CircularIssue> builder) {
            builder.ToTable("TBL_GRC_CIRCULAR_ISSUE");
            builder.HasKey(t => t.Id);
            builder.Property(a => a.IssueDescription).HasColumnName("issue_description").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(a => a.Resolution).HasColumnName("resolution").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
            builder.Property(a => a.RecievedOn).HasColumnName("received_on").IsRequired(true);
            builder.Property(a => a.Status).HasColumnName("status").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(a => a.ResolvedOn).HasColumnName("resolved_on").IsRequired(false);
            builder.Property(a => a.CircularId).HasColumnName("circular_id");
            builder.Property(a => a.IsDeleted).HasColumnName("is_deleted");
            builder.Property(a => a.CreatedOn).HasColumnName("created_on").IsRequired();
            builder.Property(a => a.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(a => a.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
            builder.Property(a => a.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
            builder.HasOne(a => a.Circular).WithMany(s => s.Issues).HasForeignKey(r => r.CircularId);
        }
    }
}

