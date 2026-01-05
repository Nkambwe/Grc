using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public partial class CircularEntityConfiguration {
    public static void Configure(EntityTypeBuilder<Circular> builder) {
        builder.ToTable("TBL_GRC_CIRCULAR");
        builder.HasKey(t => t.Id);
        builder.Property(a => a.Id).HasColumnName("id");
        builder.Property(a => a.CircularTitle).HasColumnName("circular_title").HasColumnType("NVARCHAR(MAX)").IsRequired(true);
        builder.Property(a => a.RecievedOn).HasColumnName("received_on");
        builder.Property(a => a.DeadlineOn).HasColumnName("deadline_on");
        builder.Property(a => a.Status).HasColumnName("status").HasColumnType("NVARCHAR(20)").IsRequired();
        builder.Property(a => a.SubmissionDate).HasColumnName("submission_on").IsRequired(false);
        builder.Property(a => a.FilePath).HasColumnName("file_path").HasColumnType("NVARCHAR(MAX)").IsRequired(false);
        builder.Property(a => a.SubmittedBy).HasColumnName("submitted_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
        builder.Property(a => a.RefNumber).HasColumnName("ref_number").HasColumnType("NVARCHAR(20)").IsRequired(false);
        builder.Property(a => a.Comments).HasColumnName("comments").HasColumnType("NVARCHAR(50)").IsRequired(false);
        builder.Property(a => a.AuthorityId).HasColumnName("auth_id");
        builder.Property(a => a.FrequencyId).HasColumnName("freq_id");
        builder.Property(a => a.DepartmentId).HasColumnName("dept_id");
        builder.Property(a => a.IsDeleted).HasColumnName("is_deleted");
        builder.Property(a => a.CreatedOn).HasColumnName("created_on").IsRequired();
        builder.Property(a => a.CreatedBy).HasColumnName("created_by").HasColumnType("NVARCHAR(50)").IsRequired();
        builder.Property(a => a.LastModifiedOn).HasColumnName("modified_on").IsRequired(false);
        builder.Property(a => a.LastModifiedBy).HasColumnName("modified_by").HasColumnType("NVARCHAR(50)").IsRequired(false);
        builder.HasOne(a => a.Authority).WithMany(s => s.Circulars).HasForeignKey(r => r.AuthorityId);
        builder.HasOne(a => a.Frequency).WithMany(s => s.Circulars).HasForeignKey(r => r.FrequencyId);
        builder.HasOne(a => a.Department).WithMany(s => s.Circulars).HasForeignKey(r => r.DepartmentId);
        builder.HasMany(a => a.Issues).WithOne(s => s.Circular).HasForeignKey(r => r.CircularId);
    }
}

