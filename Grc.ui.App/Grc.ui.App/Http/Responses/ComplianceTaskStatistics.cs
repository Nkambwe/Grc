namespace Grc.ui.App.Http.Responses {
    public class ComplianceTaskStatistics {
        public TaskStatistics Totals { get; set; } = new TaskStatistics();
        public List<TaskStatistics> TaskLists { get; set; } = new List<TaskStatistics>();
    }
}
