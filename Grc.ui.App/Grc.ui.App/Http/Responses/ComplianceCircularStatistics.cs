namespace Grc.ui.App.Http.Responses {
    public class ComplianceCircularStatistics {
        public CircularStatistics Totals { get; set; } = new CircularStatistics();
        public List<CircularStatistics> CircularList { get; set; } = new List<CircularStatistics>();
    }
}
