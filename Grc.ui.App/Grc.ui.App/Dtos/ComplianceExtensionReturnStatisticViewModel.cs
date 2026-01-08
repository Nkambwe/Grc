using Grc.ui.App.Models;

namespace Grc.ui.App.Dtos {
    public class ComplianceExtensionReturnStatisticViewModel {
        public string Banner { get; set; }
        public string WelcomeMessage { get; set; } = string.Empty;
        public string Initials { get; set; }
        public ComplianceExtensionReturnStatistic Returns { get; set; }
        public WorkspaceModel Workspace { get; set; }
        public List<QuickActionModel> QuickActions { get; set; } = new();
    }

}
