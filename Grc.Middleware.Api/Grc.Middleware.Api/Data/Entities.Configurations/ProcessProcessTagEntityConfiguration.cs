using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class ProcessProcessTagEntityConfiguration
    {
        public static void Configure(EntityTypeBuilder<ProcessProcessTag> builder)
        {
            builder.ToTable("TBL_GRC_PROCESS_PROCESS_TAG");
            builder.Property(t => t.TagId).HasColumnName("tag_id").IsRequired(false);
            builder.Property(p => p.ProcessId).HasColumnName("process_id").IsRequired();
        }
    }
}
