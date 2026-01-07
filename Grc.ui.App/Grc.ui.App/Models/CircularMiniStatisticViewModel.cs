using Grc.ui.App.Dtos;

namespace Grc.ui.App.Models {
    public class CircularMiniStatisticViewModel {
        public string Banner { get; set; }
        public string WelcomeMessage { get; set; } = string.Empty;
        public string Initials { get; set; }
        public WorkspaceModel Workspace { get; set; }
        public CircularDashboardStatistic Circulars { get; set; } = new();
        public List<QuickActionModel> QuickActions { get; set; } = new();

    }
}
