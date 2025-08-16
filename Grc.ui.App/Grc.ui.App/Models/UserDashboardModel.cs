using Grc.ui.App.Defaults;

namespace Grc.ui.App.Models {
    public class UserDashboardModel {
        public string Banner {get;} = CommonDefaults.AppVersion;
        public string WelcomeMessage { get; set; } = string.Empty;
        public string Initials { get; set; }
        public DateTime LastLogin { get; set; }
        public List<QuickActionModel> QuickActions { get; set; } = new();
        public List<PinnedModel> PinnedItems { get; set; } = new();
        public List<RecentModel> Recents { get; set; } = new();
    }
}
