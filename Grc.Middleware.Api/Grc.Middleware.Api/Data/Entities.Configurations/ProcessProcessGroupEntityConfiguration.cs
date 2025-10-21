using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class ProcessProcessGroupEntityConfiguration
    {
        public static void Configure(EntityTypeBuilder<ProcessProcessGroup> builder)
        {
            builder.ToTable("TBL_GRC_PROCESS_PROCESS_GROUP");
            builder.Property(t => t.GroupId).HasColumnName("group_id").IsRequired(false);
            builder.Property(p => p.ProcessId).HasColumnName("process_id").IsRequired();
        }
    }
}
