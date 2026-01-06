
namespace Grc.ui.App.Dtos {
    public class ComplianceGeneralStatistic {
        public Dictionary<string, int> Circulars { get; set; }
        public Dictionary<string, int> Returns { get; set; }
        public Dictionary<string, int> Tasks { get; set; }
        public Dictionary<string, int> Policies { get; set; }
    }
}
