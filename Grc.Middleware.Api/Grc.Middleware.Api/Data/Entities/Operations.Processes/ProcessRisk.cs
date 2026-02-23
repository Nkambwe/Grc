namespace Grc.Middleware.Api.Data.Entities.Operations.Processes {
    public class ProcessRisk: BaseEntity {
        public long ProcessId { get; set; }
        public string RiskLevel { get; set; }
        public int Impact { get; set; }
        public int Liklyhood { get; set; }
        public int Score { get; set; }
        public bool IsCurrent { get; set; }
        public virtual OperationProcess Process { get; set; }
    }
}
