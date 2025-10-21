namespace Grc.Middleware.Api.Data.Entities.Operations.Processes {
    public class ProcessActivity : BaseEntity {
        public string Activity { get; set; }
        public string Description { get; set; }
        public long ProcessId { get; set; }
        public virtual OperationProcess Process { get; set; }
    }
}
