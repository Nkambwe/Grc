namespace Grc.ui.App.Models {
    public class AdminDashboardModel {
        public string WelcomeMessage { get; set; } = string.Empty;
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public DateTime LastLogin { get; set; }
    }
}
