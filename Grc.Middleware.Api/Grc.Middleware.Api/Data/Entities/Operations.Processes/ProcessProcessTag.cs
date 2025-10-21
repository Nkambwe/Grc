using Grc.Middleware.Api.Data.Entities.Support;

namespace Grc.Middleware.Api.Data.Entities.Operations.Processes
{
    public class ProcessProcessTag
    {
        public long TagId { get; set; }
        public long ProcessId { get; set; }
        public virtual OperationProcess Process { get; set; }
        public virtual ProcessTag Tag { get; set; }
    }
}
