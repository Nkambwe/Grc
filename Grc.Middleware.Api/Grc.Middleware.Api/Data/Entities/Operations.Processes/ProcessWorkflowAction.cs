namespace Grc.Middleware.Api.Data.Entities.Operations.Processes {
    public class ProcessWorkflowAction : BaseEntity {
        public long WorkflowId { get; set; }
        public string Comments { get; set; }
        public virtual ProcessWorkflow Workflow { get; set; }
    }
}
