namespace Grc.ui.App.Http.Responses {
    public class GeneralComplianceReturnStatistics {
        public CircularStatistics TotalCircular { get; set; } = new CircularStatistics();
        public ReturnStatistics TotalReturns { get; set; } = new ReturnStatistics();
        public TaskStatistics TotalTasks { get; set; } = new TaskStatistics();
        public List<CircularStatistics> CircularList { get; set; } = new List<CircularStatistics>();
        public List<ReturnStatistics> ReturnsList { get; set; } = new List<ReturnStatistics>();
        public List<TaskStatistics> TaskLists { get; set; } = new List<TaskStatistics>();
    }
}
