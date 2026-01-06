namespace Grc.ui.App.Dtos {
    public class ComplianceTaskViewModel {
        public Dictionary<string, int> Total { get; set; }
        public Dictionary<string, int> Open { get; set; }
        public Dictionary<string, int> Closed { get; set; }
        public Dictionary<string, int> Breached { get; set; }
    }
}
