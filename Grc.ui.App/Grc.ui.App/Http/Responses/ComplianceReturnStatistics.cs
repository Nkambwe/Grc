namespace Grc.ui.App.Http.Responses {
    public class ComplianceReturnStatistics {
        public ReturnStatistics Totals { get; set; } = new ReturnStatistics();
        public List<ReturnStatistics> ReturnsList { get; set; } = new List<ReturnStatistics>();
    }
}
