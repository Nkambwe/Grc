using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public static class ProcessProcessGroupEntityConfiguration
    {
        public static void Configure(EntityTypeBuilder<ProcessProcessGroup> builder)
        {
            builder.ToTable("TBL_GRC_PROCESS_PROCESS_GROUP");
            builder.HasKey(pg => new { pg.ProcessId, pg.GroupId });
            builder.Property(pg => pg.ProcessId).HasColumnName("process_id").IsRequired();
            builder.Property(pg => pg.GroupId).HasColumnName("group_id").IsRequired();
            builder.HasOne(pg => pg.Process).WithMany(p => p.Groups).HasForeignKey(pg => pg.ProcessId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pg => pg.Group).WithMany(g => g.Processes).HasForeignKey(pg => pg.GroupId).OnDelete(DeleteBehavior.Cascade);
        }
    }

}
