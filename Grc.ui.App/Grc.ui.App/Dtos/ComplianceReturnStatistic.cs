
namespace Grc.ui.App.Dtos {
    public class ComplianceReturnStatistic {
        public Dictionary<string, int> Periods { get; set; }
        public Dictionary<string, Dictionary<string, int>> Statuses { get; set; } = new();
    }
}
