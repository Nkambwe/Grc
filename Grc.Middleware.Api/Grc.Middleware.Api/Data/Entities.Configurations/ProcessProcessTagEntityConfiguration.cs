using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class ProcessProcessTagEntityConfiguration
    {
        public static void Configure(EntityTypeBuilder<ProcessProcessTag> builder)
        {
            builder.ToTable("TBL_GRC_PROCESS_PROCESS_TAG");
            builder.HasKey(pt => new { pt.ProcessId, pt.TagId });
            builder.Property(pt => pt.ProcessId).HasColumnName("process_id").IsRequired();
            builder.Property(pt => pt.TagId).HasColumnName("tag_id").IsRequired();
            builder.HasOne(pt => pt.Process).WithMany(p => p.Tags).HasForeignKey(pt => pt.ProcessId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pt => pt.Tag).WithMany(g => g.Processes).HasForeignKey(pt => pt.TagId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
