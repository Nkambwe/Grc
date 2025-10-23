using Grc.Middleware.Api.Data.Entities.Support;

namespace Grc.Middleware.Api.Data.Entities.Operations.Processes {
    public class ProcessTask : BaseEntity {
        public string TaskName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public long ProcessId { get; set; }
        public long OwnerId { get; set; }
        public virtual Responsebility TaskOwner { get; set; }
        public virtual OperationProcess Process { get; set; }
    }
}
