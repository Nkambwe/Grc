using Grc.ui.App.Defaults;
using Grc.ui.App.Dtos;

namespace Grc.ui.App.Models {
    public class ReturnsMinDashboardModel {
        public string Banner { get; } = CommonDefaults.AppVersion;
        public string WelcomeMessage { get; set; } = string.Empty;
        public string Initials { get; set; }
        public DateTime LastLogin { get; set; }
        public WorkspaceModel Workspace { get; set; }
        public ReturnDashboardStatistic Returns { get; set; }
        public List<QuickActionModel> QuickActions { get; set; } = new();
    }
}
