using Grc.Middleware.Api.Data.Entities.Support;

namespace Grc.Middleware.Api.Data.Entities.Operations.Processes
{
    public class ProcessProcessGroup
    {
        public long GroupId { get; set; }
        public long ProcessId { get; set; }
        public virtual OperationProcess Process { get; set; }
        public virtual ProcessGroup Group { get; set; }
    }
}
