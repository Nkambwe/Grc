namespace Grc.Middleware.Api.Data.Entities.Operations.Processes {
    public class ProcessWorkflowStep : BaseEntity {
        public string StepName { get; set; }
        public string Sequence { get; set; }
        public string RequiredRole { get; set; }
        public int SlaHours { get; set; }
        public virtual ICollection<ProcessWorkflow> Workflows { get; set; } = new List<ProcessWorkflow>();
    }
}
