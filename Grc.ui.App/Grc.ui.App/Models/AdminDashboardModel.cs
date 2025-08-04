using Grc.ui.App.Defaults;

namespace Grc.ui.App.Models {
    public class AdminDashboardModel {
        public string Banner {get;} = CommonDefaults.AppVersion;
        public string WelcomeMessage { get; set; } = string.Empty;
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public DateTime LastLogin { get; set; }
        public List<QuickActionModel> QuickActions { get; set; } = new();
        public List<FavouriteModel> Favourites { get; set; } = new();
        public List<RecentModel> Recents { get; set; } = new();
    }
}
