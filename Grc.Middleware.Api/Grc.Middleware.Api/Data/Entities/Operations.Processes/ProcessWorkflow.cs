
namespace Grc.Middleware.Api.Data.Entities.Operations.Processes {

    public class ProcessWorkflow : BaseEntity {
        public long ProcessId { get; set; }
        public long StepId { get; set; }
        public string Status { get; set; }
        public virtual OperationProcess Process { get; set; }
        public virtual ProcessWorkflowStep Step { get; set; }
        public virtual ICollection<ProcessWorkflowAction> Actions { get; set; } = new List<ProcessWorkflowAction>();
    }
}
