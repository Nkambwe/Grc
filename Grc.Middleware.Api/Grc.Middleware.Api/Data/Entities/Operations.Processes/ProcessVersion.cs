namespace Grc.Middleware.Api.Data.Entities.Operations.Processes {
    public class ProcessVersion : BaseEntity {
        public long ProcessId { get; set; }
        public int VersionNumber { get; set; }
        public string Content { get; set; }
        public virtual OperationProcess Process { get; set; }
    }
}
