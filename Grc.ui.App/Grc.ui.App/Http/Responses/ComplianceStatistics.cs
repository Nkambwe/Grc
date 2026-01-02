namespace Grc.ui.App.Http.Responses {
    public class ComplianceStatistics {   
        public CircularTotalsStatistics CircularTotals { get; set; } = new CircularTotalsStatistics();
        public ReturnTotalsStatistics ReturnTotals { get; set; } = new ReturnTotalsStatistics();
        public TaskTotalsStatistics TaskTotals { get; set; } = new TaskTotalsStatistics();
        public List<CircularAuthorityStatistics> Circulars { get; set; } = new List<CircularAuthorityStatistics>();
        public List<TaskTotalsStatistics> Tasks { get; set; } = new List<TaskTotalsStatistics>();
        public List<RegulationStatistics> Regulations { get; set; } = new List<RegulationStatistics>();
    }

}
