using Grc.ui.App.Defaults;

namespace Grc.ui.App.Models {
    public class ReturnsMinDashboardModel {
        public string Banner { get; } = CommonDefaults.AppVersion;
        public string WelcomeMessage { get; set; } = string.Empty;
        public string Initials { get; set; }
        public DateTime LastLogin { get; set; }
        public WorkspaceModel Workspace { get; set; }
        public Dictionary<string, int> Returns { get; set; }
        public List<QuickActionModel> QuickActions { get; set; } = new();
    }
}
