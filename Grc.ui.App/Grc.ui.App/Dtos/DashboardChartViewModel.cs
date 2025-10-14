using Grc.ui.App.Enums;

namespace Grc.ui.App.Dtos {
    public class DashboardChartViewModel {
        public List<StatCardViewModel> ProcessCards { get; set; } = new();
        public List<StatCardViewModel> UnitCards { get; set; } = new();
        public Dictionary<OperationUnit, int> CategoryTotals { get; set; } = new();
        public List<ProcessSummary> StatusSummaries { get; set; } = new();
    }
}
